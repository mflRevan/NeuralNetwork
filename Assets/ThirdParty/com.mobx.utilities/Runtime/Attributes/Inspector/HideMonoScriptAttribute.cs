using System;
using System.Diagnostics;

namespace MobX.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("UNITY_EDITOR")]
    public class HideMonoScriptAttribute : Attribute
    {
    }
}