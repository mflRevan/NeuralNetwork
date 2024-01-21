using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using InputDevice = UnityEngine.InputSystem.InputDevice;
#if PLATFORM_PICO
using Unity.XR.PXR;
#endif

namespace MobX.Platforms
{
    public class XRPlatform : Platform
    {
        private const string QuestDeviceName = "OCULUS";
        private const string PicoDeviceName = "PICO";
        private const string ViveDeviceName = "VIVE";
        private const string PSVR2DeviceName = "PSVR2";

        protected override void InitializePlatform()
        {
            XRDevice.deviceLoaded += UpdateXRHardware;
            UpdateXRHardware(XRSettings.loadedDeviceName);
        }

        private void UpdateXRHardware(string deviceName)
        {
            var hardwareType = GetHardwareTypeByName(deviceName);
            HardwareType = hardwareType;
            UpdateHardwareSettings();
        }

        private static HardwareType GetHardwareTypeByName(string deviceName)
        {
            const StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;

            if (deviceName.Contains(QuestDeviceName, Comparison))
            {
                return HardwareType.Quest2;
            }
            if (deviceName.Contains(PicoDeviceName, Comparison))
            {
#if PLATFORM_PICO
                return PXR_Input.GetControllerDeviceType() == PXR_Input.ControllerDevice.Neo3
                    ? HardwareType.PicoNeo3
                    : HardwareType.Pico;
#else
                return HardwareType.Pico;
#endif
            }
            if (deviceName.Contains(ViveDeviceName, Comparison))
            {
                return HardwareType.ViveXRElite;
            }
            if (deviceName.Contains(PSVR2DeviceName, Comparison))
            {
                return HardwareType.PSVR2;
            }
            return HardwareType.Undefined;
        }

        private void UpdateHardwareSettings()
        {
            Application.runInBackground = Hardware.RunInBackground.ValueOrDefault(Application.runInBackground);

            InputSystem.onDeviceChange -= OnDeviceChange;
            if (Hardware.ResetInputDevicesOnReconnect.ValueOrDefault())
            {
                InputSystem.onDeviceChange += OnDeviceChange;
            }

            Debug.Log("Platform", $"Loaded platform device: {Hardware.name}");
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Reconnected)
            {
                InputSystem.ResetDevice(device);
            }
            InputSystem.FlushDisconnectedDevices();
        }
    }
}
