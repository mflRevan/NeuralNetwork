using System;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Callbacks
{
    public abstract class RuntimeAsset : ScriptableObject, IDisposable
    {

        protected virtual void OnDisable()
        {
            EngineCallbacks.RemoveCallbacks(this);
        }
        [Conditional("UNITY_EDITOR")]
        protected void Repaint()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        protected virtual void OnEnable()
        {
            EngineCallbacks.AddCallbacks(this);
        }

        public virtual void Dispose()
        {
            EngineCallbacks.RemoveCallbacks(this);
        }
    }
}
