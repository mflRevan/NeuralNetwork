using MobX.Utilities.Reflection;
using System.Reflection;
using UnityEngine;

namespace MobX.Utilities.Editor.ScriptOrderEditor
{
    public static class HideFlagsUtility
    {
        public static void ShowAllHiddenObjects()
        {
            GameObject[] allGameObjects = Object.FindObjectsOfType<GameObject>();
            foreach (GameObject gameObject in allGameObjects)
            {
                switch (gameObject.hideFlags)
                {
                    case HideFlags.HideAndDontSave:
                        gameObject.hideFlags = HideFlags.DontSave;
                        break;
                    case HideFlags.HideInHierarchy:
                    case HideFlags.HideInInspector:
                        gameObject.hideFlags = HideFlags.None;
                        break;
                }
            }

            UnityEditor.EditorApplication.RepaintHierarchyWindow();
        }

        public static void ShowAllHiddenInspector()
        {
            GameObject[] allGameObjects = Object.FindObjectsOfType<GameObject>();
            foreach (GameObject gameObject in allGameObjects)
            {
                foreach (Component component in gameObject.GetComponents<Component>())
                {
                    switch (component.hideFlags)
                    {
                        case HideFlags.HideAndDontSave:
                            component.hideFlags = HideFlags.DontSave;
                            break;
                        case HideFlags.HideInHierarchy:
                        case HideFlags.HideInInspector:
                            component.hideFlags = HideFlags.None;
                            break;
                    }
                }
            }

            UnityEditor.EditorApplication.RepaintHierarchyWindow();
        }

        [UnityEditor.InitializeOnLoadMethodAttribute]
        public static void ValidateAllObjectsHideFlags()
        {
            MonoBehaviour[] monoBehaviours = Object.FindObjectsOfType<MonoBehaviour>(true);

            foreach (MonoBehaviour monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour.GetType().GetCustomAttribute<HideFlagsAttribute>() is { } attribute)
                {
                    GameObject gameObject = monoBehaviour.gameObject;

                    if (attribute.InternalObjectFlags.HasValue)
                    {
                        gameObject.hideFlags = attribute.ObjectFlags;
                        UnityEditor.EditorUtility.SetDirty(gameObject);
                    }

                    if (attribute.InternalScriptFlags.HasValue)
                    {
                        monoBehaviour.hideFlags = attribute.ScriptFlags;
                        UnityEditor.EditorUtility.SetDirty(monoBehaviour);
                    }
                }
            }

            UnityEditor.EditorApplication.DirtyHierarchyWindowSorting();
            UnityEditor.EditorApplication.RepaintHierarchyWindow();
        }

        [UnityEditor.MenuItem("GameObject/Hide GameObject")]
        public static void HideSelectedGameObject(UnityEditor.MenuCommand command)
        {
            if (command.context != null)
            {
                command.context.hideFlags |= HideFlags.HideInHierarchy;
            }
        }
    }
}
