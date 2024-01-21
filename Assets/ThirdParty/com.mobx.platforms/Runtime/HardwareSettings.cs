using MobX.Utilities.Inspector;
using MobX.Utilities.Types;
using UnityEngine;

namespace MobX.Platforms
{
    public class HardwareSettings : ScriptableObject
    {
        #region Inspector

        [SerializeField] private string hardwareName;
        [Foldout("Settings")]
        [SerializeField] private Optional<bool> runInBackground;
        [SerializeField] private Optional<bool> resetInputDevicesOnReconnect;
        [Foldout("Prefabs")]
        [SerializeField] private Prefab controllerLeft;
        [SerializeField] private Prefab controllerRight;

        #endregion


        #region Properties

        public string Name => hardwareName;
        public Prefab ControllerLeft => controllerLeft;
        public Prefab ControllerRight => controllerRight;
        public Optional<bool> RunInBackground => runInBackground;
        public Optional<bool> ResetInputDevicesOnReconnect => resetInputDevicesOnReconnect;

        #endregion
    }
}
