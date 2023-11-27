using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DavidJalbert
{
    public class ExampleGui : MonoBehaviour
    {
        public Text textDebug;
        public Text textDescription;
        public TinyCarController carController;
        public TinyCarCamera carCamera;
        public TinyCarMobileInput mobileInput;

        void Update()
        {
            if (textDebug == null) return;

            textDebug.text = "";

            if (carController != null)
            {
                textDebug.text += "Speed : " + (int)carController.getForwardVelocity() + " m/s\n";
                textDebug.text += "Drift speed : " + (int)carController.getLateralVelocity() + " m/s\n";
                textDebug.text += "Is grounded : " + carController.isGrounded() + "\n";
                textDebug.text += "Ground type : " + carController.getSurfaceParameters()?.getName() + "\n";
                textDebug.text += "Is braking : " + carController.isBraking() + "\n";
                textDebug.text += "Side hit force : " + carController.getSideHitForce() + "\n";
            }
        }

        public void onClickMobileInput()
        {
            mobileInput.gameObject.SetActive(!mobileInput.gameObject.activeSelf);
        }

        public void onClickCameraAngle()
        {
            if (carCamera != null)
            {
                switch (carCamera.viewMode)
                {
                    case TinyCarCamera.CAMERA_MODE.TopDown:
                        carCamera.viewMode = TinyCarCamera.CAMERA_MODE.ThirdPerson;
                        break;
                    case TinyCarCamera.CAMERA_MODE.ThirdPerson:
                        carCamera.viewMode = TinyCarCamera.CAMERA_MODE.TopDown;
                        break;
                }
            }
        }

        public void onClickDescriptionText()
        {
            textDebug.enabled = !textDebug.enabled;
            textDescription.enabled = !textDescription.enabled;
        }
    }
}