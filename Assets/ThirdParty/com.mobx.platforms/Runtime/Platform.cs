using MobX.Utilities.Collections;
using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Platforms
{
    public abstract partial class Platform : ScriptableObject
    {
        #region Fields

        [Foldout("Settings")]
        [InlineInspector]
        [SerializeField] private HardwareSettings hardware;
        [SerializeField] private bool allowOtherHardware;
        [ConditionalShow(nameof(allowOtherHardware))]
        [Tooltip("List of available hardware devices for the platform")]
        [SerializeField] private Map<HardwareType, HardwareSettings> availableHardware;

        [Space]
        [SerializeField] private PlatformType buildPlatform;
        [SerializeField] [Required] private string platformDefine;
        [InlineInspector]
        [SerializeField] private SceneBuildSettings sceneBuildSettings;

        [Space]
        [InlineInspector]
        [SerializeField] private CreditsConfiguration credits;

        #endregion


        #region Properties

        public static Platform Current => PlatformManager.Singleton.Platform;
        public HardwareType HardwareType { get; protected set; }
        public PlatformType PlatformType => buildPlatform;

        public string PlatformDefine => platformDefine;
        public CreditsConfiguration Credits => credits;

        [Foldout("Activation")]
        [ReadonlyInspector]
        public bool IsActive => PlatformManager.Singleton.Platform == this;

        #endregion


        #region Device Handling

        public HardwareSettings Hardware
        {
            get
            {
                if (allowOtherHardware && availableHardware.TryGetValue(HardwareType, out var hardwareSettings))
                {
                    return hardwareSettings;
                }
                return hardware;
            }
        }

        #endregion


        #region Initialization

        public void Initialize()
        {
            InitializePlatform();
        }

        protected abstract void InitializePlatform();

        #endregion


        #region Validation

        protected virtual void OnValidate()
        {
            if (allowOtherHardware is false)
            {
                // Clear the dictionary to prevent the hardware configurations from being included in a build.
                availableHardware.Clear();
            }
        }

        #endregion
    }
}
