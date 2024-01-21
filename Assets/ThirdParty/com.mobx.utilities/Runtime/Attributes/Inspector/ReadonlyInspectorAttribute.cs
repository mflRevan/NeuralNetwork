using JetBrains.Annotations;
using System;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    [MeansImplicitUse]
    public class ReadonlyInspectorAttribute : PropertyAttribute
    {
    }
}