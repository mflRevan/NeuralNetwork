using MobX.Utilities.Inspector;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.InspectorFields
{
    public static class InspectorFieldUtils
    {
        public static InspectorMember[] GetInspectorMembers(UnityEditor.SerializedObject target)
        {
            var type = target.targetObject.GetType();
            var list = new List<InspectorMember>();

            const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic |
                                       BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static;

            var fieldInfos = type.GetFieldsIncludeBaseTypes(Flags);
            for (var i = 0; i < fieldInfos.Length; i++)
            {
                HandleFieldInfo(target, fieldInfos[i], ref list);
            }

            var propertyInfos = type.GetPropertiesIncludeBaseTypes(Flags);
            for (var i = 0; i < propertyInfos.Length; i++)
            {
                HandlePropertyInfo(target, propertyInfos[i], ref list);
            }

            var methodInfos = type.GetMethodsIncludeBaseTypes(Flags);
            for (var i = 0; i < methodInfos.Length; i++)
            {
                HandleMethodInfo(target, methodInfos[i], ref list);
            }

            return list.ToArray();
        }


        #region Methods

        private static void HandleMethodInfo(UnityEditor.SerializedObject target, MethodInfo methodInfo,
            ref List<InspectorMember> list)
        {
            if (methodInfo.TryGetCustomAttribute<ButtonAttribute>(out var buttonAttribute))
            {
                try
                {
                    list.Add(new MethodButtonInspectorMember(methodInfo, buttonAttribute, target.targetObject));
                }
                catch (Exception exception)
                {
                    list.Add(new ExceptionInspectorMember(
                        exception,
                        methodInfo,
                        target.targetObject));
                }
            }
        }

        #endregion


        #region Properties

        private static void HandlePropertyInfo(UnityEditor.SerializedObject target, PropertyInfo propertyInfo,
            ref List<InspectorMember> list)
        {
            try
            {
                var hasSetAccess = propertyInfo.SetMethod != null;

                if (propertyInfo.HasAttribute<ReadonlyInspectorAttribute>())
                {
                    list.Add(new ReadonlyPropertyInspector(propertyInfo, target.targetObject));
                    return;
                }

                if (propertyInfo.HasAttribute<ConditionalDrawerAttribute>())
                {
                    InspectorMember inspector = hasSetAccess
                        ? new PropertyInspector(propertyInfo, target.targetObject)
                        : new ReadonlyPropertyInspector(propertyInfo, target.targetObject);

                    list.Add(inspector);
                    return;
                }

                if (propertyInfo.HasAttribute<ShowInInspectorAttribute>())
                {
                    InspectorMember inspector = hasSetAccess
                        ? new PropertyInspector(propertyInfo, target.targetObject)
                        : new ReadonlyPropertyInspector(propertyInfo, target.targetObject);

                    list.Add(inspector);
                    return;
                }

                if (propertyInfo.HasAttribute<InlineInspectorAttribute>())
                {
                    InspectorMember inspector = hasSetAccess
                        ? new PropertyInspector(propertyInfo, target.targetObject)
                        : new ReadonlyPropertyInspector(propertyInfo, target.targetObject);

                    list.Add(inspector);
                }
            }
            catch (Exception exception)
            {
                list.Add(new ExceptionInspectorMember(
                    exception,
                    propertyInfo,
                    target.targetObject));
            }
        }

        #endregion


        #region Fields

        private static void HandleFieldInfo(UnityEditor.SerializedObject target, FieldInfo fieldInfo,
            ref List<InspectorMember> list)
        {
            try
            {
                var isStatic = fieldInfo.IsStatic;
                var hideInInspector = fieldInfo.HasAttribute<HideInInspector>();
                var hasSerializeField = fieldInfo.HasAttribute<SerializeField>();
                var hasSerializeReference = fieldInfo.HasAttribute<SerializeField>();
                var isPublicField = fieldInfo.IsPublic && !fieldInfo.IsInitOnly;

                if (!hideInInspector && !isStatic && (hasSerializeField || hasSerializeReference || isPublicField))
                {
                    HandleSerializedField(target, fieldInfo, ref list);
                    return;
                }

                HandleNonSerializedField(target, fieldInfo, ref list);
            }
            catch (Exception exception)
            {
                list.Add(new ExceptionInspectorMember(
                    exception,
                    fieldInfo,
                    target.targetObject));
            }
        }

        private static void HandleSerializedField(UnityEditor.SerializedObject target, FieldInfo fieldInfo,
            ref List<InspectorMember> list)
        {
            var serializedProperty = target.FindProperty(fieldInfo.Name);
            if (serializedProperty.IsNull())
            {
                return;
            }

            list.Add(new SerializedPropertyInspectorMember(serializedProperty, fieldInfo, target.targetObject));
        }

        private static void HandleNonSerializedField(UnityEditor.SerializedObject target, FieldInfo fieldInfo,
            ref List<InspectorMember> list)
        {
            var showInInspector = fieldInfo.HasAttribute<ShowInInspectorAttribute>();
            var isReadonly = fieldInfo.HasAttribute<ReadonlyInspectorAttribute>();
            var conditionalDraw = fieldInfo.HasAttribute<ConditionalDrawerAttribute>();
            var isCollection = fieldInfo.FieldType.IsDictionary() || fieldInfo.FieldType.IsHashSet() ||
                               fieldInfo.FieldType.IsStack() || fieldInfo.FieldType.IsQueue();

            if (!showInInspector && !isReadonly && !conditionalDraw)
            {
                return;
            }

            if (isReadonly || isCollection)
            {
                list.Add(new NonSerializedReadonlyFieldInspector(fieldInfo, target.targetObject));
                return;
            }

            list.Add(new NonSerializedFieldInspector(fieldInfo, target.targetObject));
        }

        #endregion
    }
}