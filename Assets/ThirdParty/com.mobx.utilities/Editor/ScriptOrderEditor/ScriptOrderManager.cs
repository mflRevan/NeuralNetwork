using MobX.Utilities.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobX.Utilities.Editor.ScriptOrderEditor
{
    /// <summary>
    ///     Dynamically manages script execution order for Mono scripts based on these attributes:<br />
    ///     <see cref="ExecuteAfterAttribute" />, <see cref="ExecuteAfterAttribute" /> or
    ///     <see cref="ExecutionOrderAttribute" />
    /// </summary>
    public static class ExecutionOrderAttributeEditor
    {
        #region Nested Types

        private static class Graph
        {
            public struct Edge
            {
                public UnityEditor.MonoScript Node;
                public int Weight;
            }

            public static Dictionary<UnityEditor.MonoScript, List<Edge>> Create(List<OrderDefinition> definitions,
                List<OrderDependency> dependencies)
            {
                var graph = new Dictionary<UnityEditor.MonoScript, List<Edge>>();

                for (var i = 0; i < dependencies.Count; i++)
                {
                    OrderDependency dependency = dependencies[i];
                    UnityEditor.MonoScript source = dependency.FirstScript;
                    UnityEditor.MonoScript dest = dependency.SecondScript;

                    if (!graph.TryGetValue(source, out List<Edge> edges))
                    {
                        edges = new List<Edge>();
                        graph.Add(source, edges);
                    }

                    edges.Add(new Edge
                    {
                        Node = dest,
                        Weight = dependency.OrderDelta
                    });
                    if (!graph.ContainsKey(dest))
                    {
                        graph.Add(dest, new List<Edge>());
                    }
                }

                for (var i = 0; i < definitions.Count; i++)
                {
                    OrderDefinition definition = definitions[i];
                    UnityEditor.MonoScript node = definition.Script;
                    if (!graph.ContainsKey(node))
                    {
                        graph.Add(node, new List<Edge>());
                    }
                }

                return graph;
            }

            private static bool IsCyclicRecursion(
                Dictionary<UnityEditor.MonoScript, List<Edge>> graph,
                UnityEditor.MonoScript node,
                Dictionary<UnityEditor.MonoScript, bool> visited,
                Dictionary<UnityEditor.MonoScript, bool> inPath)
            {
                if (visited[node])
                {
                    return inPath[node];
                }

                visited[node] = true;
                inPath[node] = true;

                for (var i = 0; i < graph[node].Count; i++)
                {
                    Edge edge = graph[node][i];
                    UnityEditor.MonoScript succeeding = edge.Node;
                    if (!IsCyclicRecursion(graph, succeeding, visited, inPath))
                    {
                        continue;
                    }

                    inPath[node] = false;
                    return true;
                }

                inPath[node] = false;
                return false;
            }

            public static bool IsCyclic(Dictionary<UnityEditor.MonoScript, List<Edge>> graph)
            {
                var visited = new Dictionary<UnityEditor.MonoScript, bool>();
                var inPath = new Dictionary<UnityEditor.MonoScript, bool>();

                Dictionary<UnityEditor.MonoScript, List<Edge>>.KeyCollection keys = graph.Keys;

                foreach (UnityEditor.MonoScript node in keys)
                {
                    visited.Add(node, false);
                    inPath.Add(node, false);
                }

                foreach (UnityEditor.MonoScript node in keys)
                {
                    if (IsCyclicRecursion(graph, node, visited, inPath))
                    {
                        return true;
                    }
                }

                return false;
            }

            public static List<UnityEditor.MonoScript> GetRoots(Dictionary<UnityEditor.MonoScript, List<Edge>> graph)
            {
                var degrees = new Dictionary<UnityEditor.MonoScript, int>();

                foreach (UnityEditor.MonoScript node in graph.Keys)
                {
                    degrees.Add(node, 0);
                }

                foreach ((var _, List<Edge> edges) in graph)
                {
                    foreach (Edge edge in edges)
                    {
                        UnityEditor.MonoScript succeeding = edge.Node;
                        degrees[succeeding]++;
                    }
                }

                var roots = new List<UnityEditor.MonoScript>();
                foreach ((UnityEditor.MonoScript node, var degree) in degrees)
                {
                    if (degree == 0)
                    {
                        roots.Add(node);
                    }
                }

                return roots;
            }

            public static void PropagateValues(Dictionary<UnityEditor.MonoScript, List<Edge>> graph,
                Dictionary<UnityEditor.MonoScript, int> values)
            {
                var queue = new Queue<UnityEditor.MonoScript>();

                foreach (UnityEditor.MonoScript node in values.Keys)
                {
                    queue.Enqueue(node);
                }

                while (queue.Count > 0)
                {
                    UnityEditor.MonoScript node = queue.Dequeue();
                    var currentValue = values[node];

                    foreach (Edge edge in graph[node])
                    {
                        UnityEditor.MonoScript succeeding = edge.Node;
                        var newValue = currentValue + edge.Weight;
                        var hasPrevValue = values.TryGetValue(succeeding, out var previousValue);
                        var newValueBeyond =
                            edge.Weight > 0 ? newValue > previousValue : newValue < previousValue;
                        if (hasPrevValue && !newValueBeyond)
                        {
                            continue;
                        }

                        values[succeeding] = newValue;
                        queue.Enqueue(succeeding);
                    }
                }
            }
        }

        private struct OrderDefinition
        {
            public UnityEditor.MonoScript Script { get; set; }
            public int Order { get; set; }
        }

        private struct OrderDependency
        {
            public UnityEditor.MonoScript FirstScript { get; set; }
            public UnityEditor.MonoScript SecondScript { get; set; }
            public int OrderDelta { get; set; }
        }

        #endregion


        #region Setup

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnDidReloadScripts()
        {
            Dictionary<Type, UnityEditor.MonoScript> types = GetTypeDictionary();
            List<OrderDefinition> definitions = GetExecutionOrderDefinitions(types);
            List<OrderDependency> dependencies = GetExecutionOrderDependencies(types);
            Dictionary<UnityEditor.MonoScript, List<Graph.Edge>> graph = Graph.Create(definitions, dependencies);

            if (Graph.IsCyclic(graph))
            {
                UnityEngine.Debug.LogError("Circular script execution order definitions");
                return;
            }

            List<UnityEditor.MonoScript> roots = Graph.GetRoots(graph);
            Dictionary<UnityEditor.MonoScript, int> orders = GetInitialExecutionOrder(definitions, roots);
            Graph.PropagateValues(graph, orders);

            UpdateExecutionOrder(orders);
        }

        #endregion


        #region Logic

        private static List<OrderDependency> GetExecutionOrderDependencies(Dictionary<Type, UnityEditor.MonoScript> types)
        {
            var list = new List<OrderDependency>();

            foreach ((Type type, UnityEditor.MonoScript script) in types)
            {
                var hasExecutionOrderAttribute = Attribute.IsDefined(type, typeof(ExecutionOrderAttribute), true);
                var hasExecuteAfterAttribute = Attribute.IsDefined(type, typeof(ExecuteAfterAttribute), true);
                var hasExecuteBeforeAttribute = Attribute.IsDefined(type, typeof(ExecuteBeforeAttribute), true);

                if (hasExecuteAfterAttribute &&
                    GetExecuteAfterDependencies(types, hasExecutionOrderAttribute, script, type, list))
                {
                    continue;
                }

                if (hasExecuteBeforeAttribute)
                {
                    GetExecuteBeforeDependencies(types, hasExecutionOrderAttribute, script, hasExecuteAfterAttribute,
                        type, list);
                }
            }

            return list;
        }

        private static void GetExecuteBeforeDependencies(Dictionary<Type, UnityEditor.MonoScript> types,
            bool hasExecutionOrderAttribute, UnityEditor.MonoScript script,
            bool hasExecuteAfterAttribute, Type type, List<OrderDependency> list)
        {
            if (hasExecutionOrderAttribute)
            {
                UnityEngine.Debug.LogError(
                    $"Script {script.name} has both [ExecutionOrder] and [ExecuteBefore] attributes. Ignoring the [ExecuteBefore] attribute.",
                    script);
                return;
            }

            if (hasExecuteAfterAttribute)
            {
                UnityEngine.Debug.LogError(
                    $"Script {script.name} has both [ExecuteAfter] and [ExecuteBefore] attributes. Ignoring the [ExecuteBefore] attribute.",
                    script);
                return;
            }

            var attributes =
                (ExecuteBeforeAttribute[]) Attribute.GetCustomAttributes(type, typeof(ExecuteBeforeAttribute), true);
            for (var i = 0; i < attributes.Length; i++)
            {
                ExecuteBeforeAttribute attribute = attributes[i];
                if (attribute.OrderDecrease < 0)
                {
                    UnityEngine.Debug.LogError(
                        $"Script {script.name} has an [ExecuteBefore] attribute with a negative orderDecrease. Use the [ExecuteAfter] attribute instead. Ignoring this [ExecuteBefore] attribute.",
                        script);
                    continue;
                }

                if (!attribute.TargetType.IsSubclassOf(typeof(MonoBehaviour)) &&
                    !attribute.TargetType.IsSubclassOf(typeof(ScriptableObject)))
                {
                    UnityEngine.Debug.LogError(
                        $"Script {script.name} has an [ExecuteBefore] attribute with targetScript={attribute.TargetType.Name} which is not a MonoBehaviour nor a ScriptableObject. Ignoring this [ExecuteBefore] attribute.",
                        script);
                    continue;
                }

                UnityEditor.MonoScript targetScript = types[attribute.TargetType];
                var dependency = new OrderDependency
                {
                    FirstScript = targetScript,
                    SecondScript = script,
                    OrderDelta = -attribute.OrderDecrease
                };
                list.Add(dependency);
            }
        }

        private static bool GetExecuteAfterDependencies(Dictionary<Type, UnityEditor.MonoScript> types,
            bool hasExecutionOrderAttribute, UnityEditor.MonoScript script, Type type, List<OrderDependency> list)
        {
            if (hasExecutionOrderAttribute)
            {
                UnityEngine.Debug.LogError(
                    $"Script {script.name} has both [ExecutionOrder] and [ExecuteAfter] attributes. Ignoring the [ExecuteAfter] attribute.",
                    script);
                return true;
            }

            var attributes =
                (ExecuteAfterAttribute[]) Attribute.GetCustomAttributes(type, typeof(ExecuteAfterAttribute), true);
            foreach (ExecuteAfterAttribute attribute in attributes)
            {
                if (!attribute.TargetType.IsSubclassOf(typeof(MonoBehaviour)) &&
                    !attribute.TargetType.IsSubclassOf(typeof(ScriptableObject)))
                {
                    UnityEngine.Debug.LogError(
                        $"Script {script.name} has an [ExecuteAfter] attribute with targetScript={attribute.TargetType.Name} which is not a MonoBehaviour nor a ScriptableObject. Ignoring this [ExecuteAfter] attribute.",
                        script);
                    continue;
                }

                UnityEditor.MonoScript targetScript = types[attribute.TargetType];
                var dependency = new OrderDependency
                {
                    FirstScript = targetScript,
                    SecondScript = script,
                    OrderDelta = (int) attribute.OrderIncrease
                };
                list.Add(dependency);
            }

            return false;
        }

        private static List<OrderDefinition> GetExecutionOrderDefinitions(Dictionary<Type, UnityEditor.MonoScript> types)
        {
            var list = new List<OrderDefinition>();

            foreach ((Type type, UnityEditor.MonoScript script) in types)
            {
                if (!Attribute.IsDefined(type, typeof(ExecutionOrderAttribute), true))
                {
                    continue;
                }

                var attribute =
                    (ExecutionOrderAttribute) Attribute.GetCustomAttribute(type, typeof(ExecutionOrderAttribute), true);
                var definition = new OrderDefinition
                {
                    Script = script,
                    Order = attribute.Order
                };
                list.Add(definition);
            }

            return list;
        }

        private static Dictionary<UnityEditor.MonoScript, int> GetInitialExecutionOrder(List<OrderDefinition> definitions,
            List<UnityEditor.MonoScript> graphRoots)
        {
            var orders = new Dictionary<UnityEditor.MonoScript, int>();
            for (var i = 0; i < definitions.Count; i++)
            {
                OrderDefinition definition = definitions[i];
                UnityEditor.MonoScript script = definition.Script;
                var order = definition.Order;
                orders.Add(script, order);
            }

            for (var i = 0; i < graphRoots.Count; i++)
            {
                UnityEditor.MonoScript script = graphRoots[i];
                if (orders.ContainsKey(script))
                {
                    continue;
                }

                var order = UnityEditor.MonoImporter.GetExecutionOrder(script);
                orders.Add(script, order);
            }

            return orders;
        }

        private static void UpdateExecutionOrder(Dictionary<UnityEditor.MonoScript, int> orders)
        {
            var startedEdit = false;

            foreach ((UnityEditor.MonoScript script, var order) in orders)
            {
                if (UnityEditor.MonoImporter.GetExecutionOrder(script) == order)
                {
                    continue;
                }

                if (!startedEdit)
                {
                    UnityEditor.AssetDatabase.StartAssetEditing();
                    startedEdit = true;
                }

                UnityEditor.MonoImporter.SetExecutionOrder(script, order);
            }

            if (startedEdit)
            {
                UnityEditor.AssetDatabase.StopAssetEditing();
            }
        }

        #endregion


        #region Helper

        private static Dictionary<Type, UnityEditor.MonoScript> GetTypeDictionary()
        {
            var types = new Dictionary<Type, UnityEditor.MonoScript>();

            UnityEditor.MonoScript[] scripts = UnityEditor.MonoImporter.GetAllRuntimeMonoScripts();
            foreach (UnityEditor.MonoScript script in scripts)
            {
                Type type = script.GetClass();

                if (!IsTypeValid(type))
                {
                    continue;
                }

                if (!types.ContainsKey(type))
                {
                    types.Add(type, script);
                }
            }

            return types;
        }

        private static bool IsTypeValid(Type type)
        {
            if (type != null)
            {
                return type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsSubclassOf(typeof(ScriptableObject));
            }

            return false;
        }

        #endregion
    }
}
