using MobX.Utilities;
using MobX.Utilities.Callbacks;
using MobX.Utilities.Inspector;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Mediator.Collections
{
    public abstract class RuntimeCollectionAsset<T> : MediatorAsset, IOnEnterEditMode
    {
        [Foldout("Options")]
        [Tooltip("When enabled, leaks that occur when exiting playmode will logged to the console")]
        [SerializeField] private bool logLeaks = true;
        [Tooltip("When enabled, leaks that occur when exiting playmode will be cleared automatically")]
        [SerializeField] private bool clearLeaks = true;
        [Tooltip("When enabled, changes to the collection will trigger an immediate repaint in the inspector")]
        [SerializeField] private bool allowRepaint = true;

        protected RuntimeCollectionAsset()
        {
            EngineCallbacks.AddEnterEditModeListener(this);
        }

        private protected abstract int CountInternal { get; }
        private protected abstract IEnumerable<T> CollectionInternal { get; }

        public void OnEnterEditMode()
        {
            if (logLeaks && CountInternal > 0)
            {
                Debug.LogWarning("Collection",
                    $"Leak detected in runtime collection: {name}\n{CollectionInternal.ToCollectionString()}", this);
            }

            if (clearLeaks && CountInternal > 0)
            {
                ClearInternal();
            }
        }

        [Conditional("UNITY_EDITOR")]
        protected new void Repaint()
        {
            if (!allowRepaint)
            {
                return;
            }
#if UNITY_EDITOR
            base.Repaint();
#endif
        }

        [Button("Clear")]
        [Foldout("Options")]
        private protected abstract void ClearInternal();
    }
}