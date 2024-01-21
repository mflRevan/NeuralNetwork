using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Platforms.Editor
{
    public class PlatformVariableTest : ScriptableObject
    {
        [SerializeField] private PlatformVariable<string> platformVariable;
        [SerializeField] private HardwareVariable<string> hardwareVariable;

        [Button]
        private void Log()
        {
            Debug.Log($"Platform: {platformVariable.ToString()}");
            Debug.Log($"Hardware: {hardwareVariable.ToString()}");
        }
    }
}
