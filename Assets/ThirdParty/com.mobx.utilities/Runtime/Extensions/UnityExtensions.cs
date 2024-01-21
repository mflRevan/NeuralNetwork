using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace MobX.Utilities
{
    public static class UnityExtensions
    {
        #region Transform

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform[] GetChildren(this Transform transform)
        {
            var buffer = ListPool<Transform>.Get();
            foreach (Transform child in transform)
            {
                buffer.Add(child);
            }
            var result = buffer.ToArray();
            ListPool<Transform>.Release(buffer);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AnchorX(this RectTransform rectTransform)
        {
            return rectTransform.anchoredPosition.x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AnchorY(this RectTransform rectTransform)
        {
            return rectTransform.anchoredPosition.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void KillChildObjects(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPositionAndRotation(this Transform transform, Transform target)
        {
            transform.SetPositionAndRotation(target.position, target.rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetLocalPositionAndRotation(this Transform transform, Transform target)
        {
            transform.localPosition = target.localPosition;
            transform.localRotation = target.localRotation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPositionAndViewDirection(this Transform transform, Vector3 position, Vector3 viewDirection)
        {
            if (viewDirection == Vector3.zero)
            {
                transform.position = position;
            }
            else
            {
                transform.SetPositionAndRotation(position, Quaternion.LookRotation(viewDirection));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveTowards(this Transform transform, Transform target, float maxDistanceDelta, float maxDegreesDelta)
        {
            var position = Vector3.MoveTowards(transform.position, target.position, maxDistanceDelta);
            var rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, maxDegreesDelta);
            transform.SetPositionAndRotation(position, rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetScale(this Transform transform, float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetScale(this Transform transform, Vector3 scale)
        {
            transform.localScale = scale;
        }

        #endregion


        #region Component

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPosition<TComponent>(this TComponent component, Vector3 position) where TComponent : Component
        {
            component.transform.position = position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPositionAndRotation<TComponent>(this TComponent component, Vector3 position, Quaternion rotation) where TComponent : Component
        {
            component.transform.SetPositionAndRotation(position, rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPositionAndRotation<TComponent>(this TComponent component, Transform target) where TComponent : Component
        {
            component.transform.SetPositionAndRotation(target.position, target.rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPositionAndRotation(this GameObject gameObject, Transform target)
        {
            gameObject.transform.SetPositionAndRotation(target.position, target.rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetScale<TComponent>(this TComponent component, float scale) where TComponent : Component
        {
            component.transform.localScale = Vector3.one * scale;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetScale<TComponent>(this TComponent component, Vector3 scale) where TComponent : Component
        {
            component.transform.localScale = scale;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetActive<TComponent>(this TComponent component, bool active) where TComponent : Component
        {
            component.gameObject.SetActive(active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParent<TComponent>(this TComponent component, Transform parent)
            where TComponent : Component
        {
            component.transform.SetParent(parent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DontDestroyOnLoad<TComponent>(this TComponent component)
            where TComponent : Component
        {
            component.SetParent(null);
            Object.DontDestroyOnLoad(component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActive<TComponent>(this TComponent component) where TComponent : Component
        {
            return component.gameObject.activeSelf;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActiveInHierarchy<TComponent>(this TComponent component) where TComponent : Component
        {
            return component.gameObject.activeInHierarchy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotActive<TComponent>(this TComponent component) where TComponent : Component
        {
            return !component.gameObject.activeSelf;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotActiveInHierarchy<TComponent>(this TComponent component) where TComponent : Component
        {
            return !component.gameObject.activeInHierarchy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyGameObject<TComponent>(this TComponent component) where TComponent : Component
        {
            Object.Destroy(component.gameObject);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyGameObject<TComponent>(this TComponent component, float secondsDelay) where TComponent : Component
        {
            Object.Destroy(component.gameObject, secondsDelay);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponents<TComponent>(this GameObject gameObject, out TComponent[] components) where TComponent : Component
        {
            components = gameObject.GetComponents<TComponent>();
            return components.IsNotNullOrEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponents(this GameObject gameObject, Type componentType, out Component[] components)
        {
            components = gameObject.GetComponents(componentType);
            return components.IsNotNullOrEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponentInChildren<T>(this Component target, out T component, bool includeInactive = false)
        {
            component = target.GetComponentInChildren<T>(includeInactive);
            return component != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponentInParent<T>(this Component target, out T component, bool includeInactive = false)
        {
            component = target.GetComponentInChildren<T>(includeInactive);
            return component != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrCreateComponent<T>(this Component target) where T : Component
        {
            return target.gameObject.GetOrCreateComponent<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform GetParent<TComponent>(this TComponent component) where TComponent : Component
        {
            return component.transform.parent;
        }

        #endregion


        #region GameObject

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DontDestroyOnLoad(this GameObject gameObject, bool setParent = true)
        {
            if (setParent)
            {
                gameObject.transform.SetParent(null);
            }

            Object.DontDestroyOnLoad(gameObject);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrefab(this GameObject gameObject)
        {
            return gameObject.scene.rootCount == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrCreateComponent<T>(this GameObject target) where T : Component
        {
            if (!target.TryGetComponent(out T component))
            {
                component = target.AddComponent<T>();
            }

            return component;
        }

        #endregion


        #region Object

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSelected(this Object obj)
        {
#if UNITY_EDITOR
            return UnityEditor.Selection.activeObject == obj;
#else
            return false;
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void SetObjectDirty(this Object target)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(target);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetPosition(this Object obj, Vector3 position)
        {
            switch (obj)
            {
                case Component component:
                    component.transform.position = position;
                    return true;
                case GameObject gameObject:
                    gameObject.transform.position = position;
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPosition(this GameObject gameObject, Vector3 position)
        {
            gameObject.transform.position = position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrefab(this Object obj)
        {
#if UNITY_EDITOR
            return UnityEditor.PrefabUtility.GetPrefabInstanceStatus(obj) != UnityEditor.PrefabInstanceStatus.NotAPrefab;
#else
        return false;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGameObjectInScene(this Object obj)
        {
            if (obj is GameObject gameObject && gameObject != null)
            {
                return gameObject.scene.name != null;
            }

            return false;
        }

        #endregion
    }
}
