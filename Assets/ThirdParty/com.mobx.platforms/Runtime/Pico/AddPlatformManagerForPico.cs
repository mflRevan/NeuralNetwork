using MobX.Utilities.Reflection;
using UnityEngine;
#if PLATFORM_PICO
using Unity.XR.PXR;
#endif

namespace MobX.Platforms.Pico
{
    [Description("Adds the PXR Manager component during runtime if the current platform is PLATFORM_PICO")]
    public class AddPlatformManagerForPico : MonoBehaviour
    {
        private void Awake()
        {
#if PLATFORM_PICO
            gameObject.AddComponent<PXR_Manager>();
#endif
        }
    }
}
