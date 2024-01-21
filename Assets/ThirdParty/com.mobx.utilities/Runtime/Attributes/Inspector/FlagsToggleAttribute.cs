using System;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class FlagsToggleAttribute : PropertyAttribute
    {
    }
}