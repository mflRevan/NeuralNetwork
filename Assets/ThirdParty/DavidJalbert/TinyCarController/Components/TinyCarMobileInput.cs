using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DavidJalbert
{
    public class TinyCarMobileInput : MonoBehaviour
    {
        public TinyCarController carController;

        [Tooltip("Whether the mouse can be used as input for the on-screen controls.")]
        public bool simulateTouchWithMouse = true;
        [Tooltip("For how long the boost should last in seconds.")]
        public float boostDuration = 1;
        [Tooltip("How long to wait after a boost has been used before it can be used again, in seconds.")]
        public float boostCoolOff = 0;
        [Tooltip("The value by which to multiply the speed and acceleration of the car when a boost is used.")]
        public float boostMultiplier = 2;
        [Tooltip("The color of the UI element when idle.")]
        public Color colorIdle = new Color(1f, 1f, 1f, 0.5f);
        [Tooltip("The color of the UI element when touched.")]
        public Color colorTouched = new Color(1f, 1f, 1f, 1f);

        [Tooltip("The UI graphic container for the steering wheel.")]
        public RectTransform steeringWheel;
        [Tooltip("The UI area of the steering wheel that will be checked for touches.")]
        public RectTransform steeringWheelTouchArea;
        [Tooltip("The value by which to multiply the value of the steering. Useful if you want to clamp the steering to its min/max value.")]
        public float steeringWheelMultiplier = 2f;

        [Tooltip("The UI graphic container and touch area for the gas pedal.")]
        public RectTransform gasPedal;
        [Tooltip("The UI graphic container and touch area for the brake pedal.")]
        public RectTransform brakePedal;
        [Tooltip("The UI graphic container and touch area for the boost button.")]
        public RectTransform boostButton;

        private GraphicRaycaster raycaster;
        private Graphic steeringWheelGraphic;
        private Graphic gasPedalGraphic;
        private Graphic brakePedalGraphic;
        private Graphic boostButtonGraphic;
        private float boostTimer = 0;

        void Start()
        {
            raycaster = GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = gameObject.AddComponent<GraphicRaycaster>();
            }

            steeringWheelGraphic = steeringWheel.GetComponentInChildren<Graphic>();
            gasPedalGraphic = gasPedal.GetComponentInChildren<Graphic>();
            brakePedalGraphic = brakePedal.GetComponentInChildren<Graphic>();
            boostButtonGraphic = boostButton.GetComponentInChildren<Graphic>();
        }

        void Update()
        {
            bool steeringWheelTouched = false;
            float steeringWheelDelta = 0;
            bool gasPedalTouched = false;
            bool brakePedalTouched = false;
            bool boostButtonTouched = false;

            List<PointerEventData> pointers = new List<PointerEventData>();

            foreach (Touch touch in Input.touches)
            {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = touch.position;
                pointers.Add(pointer);
            }

            if (simulateTouchWithMouse && Input.GetMouseButton(0))
            {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;
                pointers.Add(pointer);
            }

            foreach (PointerEventData pointer in pointers)
            {
                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(pointer, results);
                foreach (RaycastResult result in results)
                {
                    Graphic graphic = result.gameObject.GetComponent<Graphic>();
                    if (graphic != null)
                    {
                        Vector2 uiScreenPosition = RectTransformUtility.PixelAdjustPoint(graphic.transform.position, graphic.transform, graphic.canvas);
                        Vector2 rayScreenPosition = result.screenPosition;
                        Vector2 relativePosition = rayScreenPosition - uiScreenPosition;
                        Vector2 positionDelta = new Vector2(relativePosition.x / (graphic.rectTransform.rect.width * graphic.rectTransform.lossyScale.x), relativePosition.y / (graphic.rectTransform.rect.height * graphic.rectTransform.lossyScale.y)) * 2f;
                    
                        if (steeringWheelTouchArea != null && result.gameObject == steeringWheelTouchArea.gameObject)
                        {
                            steeringWheelTouched = true;
                            steeringWheelDelta = Mathf.Clamp(positionDelta.x * steeringWheelMultiplier, -1, 1);
                        }
                        else if (gasPedal != null && result.gameObject == gasPedal.gameObject)
                        {
                            gasPedalTouched = true;
                        }
                        else if (brakePedal != null && result.gameObject == brakePedal.gameObject)
                        {
                            brakePedalTouched = true;
                        }
                        else if (boostButton != null && result.gameObject == boostButton.gameObject)
                        {
                            boostButtonTouched = true;
                        }
                    }
                }
            }

            if (steeringWheelTouched)
            {
                if (steeringWheelGraphic != null) steeringWheelGraphic.color = colorTouched;
                steeringWheel.localRotation = Quaternion.Euler(0, 0, -steeringWheelDelta * 90);
                carController.setSteering(steeringWheelDelta);
            }
            else
            {
                if (steeringWheelGraphic != null) steeringWheelGraphic.color = colorIdle;
                steeringWheel.localRotation = Quaternion.identity;
            }

            if (gasPedalTouched)
            {
                if (gasPedalGraphic != null) gasPedalGraphic.color = colorTouched;
                carController.setMotor(1);
            }
            else
            {
                if (gasPedalGraphic != null) gasPedalGraphic.color = carController.getMotor() > 0 ? colorTouched : colorIdle;
            }

            if (brakePedalTouched)
            {
                if (brakePedalGraphic != null) brakePedalGraphic.color = colorTouched;
                carController.setMotor(-1);
            }
            else
            {
                if (brakePedalGraphic != null) brakePedalGraphic.color = carController.getMotor() < 0 ? colorTouched : colorIdle;
            }

            if (boostButtonTouched)
            {
                if (boostButtonGraphic != null) boostButtonGraphic.color = colorTouched;
                if (boostTimer == 0)
                {
                    boostTimer = boostDuration + boostCoolOff;
                }
            }
            else
            {
                if (boostButtonGraphic != null) boostButtonGraphic.color = colorIdle;
            }

            if (boostTimer > 0)
            {
                boostTimer = Mathf.Max(boostTimer - Time.deltaTime, 0);
                carController.setBoostMultiplier(boostTimer > boostCoolOff ? boostMultiplier : 1);
            }
        }
    }
}