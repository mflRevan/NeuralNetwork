using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class LockableFieldAttribute : PropertyAttribute
    {
    }
}