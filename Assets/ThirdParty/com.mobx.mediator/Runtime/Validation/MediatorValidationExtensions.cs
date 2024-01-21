using System.Diagnostics;
using UnityEngine;

namespace MobX.Mediator
{
    public static class MediatorValidationExtensions
    {
        [Conditional("UNITY_ASSERTIONS")]
        public static void AssertMediatorIsAssigned(this ScriptableObject mediator, string memberName, Object target)
        {
            if (mediator == null)
            {
                Debug.LogError("Mediator", $"{memberName} is not assigned!", target);
            }
        }
    }
}
