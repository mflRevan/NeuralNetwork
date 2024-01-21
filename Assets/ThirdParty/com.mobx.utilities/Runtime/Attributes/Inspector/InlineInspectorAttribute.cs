using System;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InlineInspectorAttribute : PropertyAttribute
    {
        public bool Required { get; set; } = true;
    }
}
