using System;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class RuntimeReadonlyAttribute : PropertyAttribute
    {
    }
}