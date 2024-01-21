using JetBrains.Annotations;
using System;
using System.Diagnostics;

namespace MobX.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    [MeansImplicitUse]
    public class ShowInInspectorAttribute : Attribute
    {
    }
}