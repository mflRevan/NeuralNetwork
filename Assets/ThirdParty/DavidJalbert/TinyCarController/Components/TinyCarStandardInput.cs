using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DavidJalbert
{
    public class TinyCarStandardInput : MonoBehaviour
    {
        public TinyCarController carController;

        public enum InputType
        {
            None, Axis, RawAxis, Key, Button
        }

        [System.Serializable]
        public struct InputValue
        {
            [Tooltip("Type of input.")]
            public InputType type;
            [Tooltip("Name of the input entry.")]
            public string name;
            [Tooltip("Returns the negative value when using an axis type.")]
            public bool invert;
        }

        public bool InputEnabled { get; private set; }

        [Header("Input")]
        [Tooltip("Input type to check to make the vehicle move forward.")]
        public bool inputEnabledFromStart;
        [Tooltip("Input type to check to make the vehicle move forward.")]
        public InputValue forwardInput = new InputValue() { type = InputType.RawAxis, name = "Vertical", invert = false };
        [Tooltip("Input type to check to make the vehicle move backward.")]
        public InputValue reverseInput = new InputValue() { type = InputType.RawAxis, name = "Vertical", invert = true };
        [Tooltip("Input type to check to make the vehicle turn right.")]
        public InputValue steerRightInput = new InputValue() { type = InputType.RawAxis, name = "Horizontal", invert = false };
        [Tooltip("Input type to check to make the vehicle turn left.")]
        public InputValue steerLeftInput = new InputValue() { type = InputType.RawAxis, name = "Horizontal", invert = true };
        [Tooltip("Input type to check to give the vehicle a speed boost.")]
        public InputValue boostInput = new InputValue() { type = InputType.Key, name = ((int)KeyCode.LeftShift).ToString(), invert = false };
        [Tooltip("For how long the boost should last in seconds.")]
        public float boostDuration = 1;
        [Tooltip("How long to wait after a boost has been used before it can be used again, in seconds.")]
        public float boostCoolOff = 0;
        [Tooltip("The value by which to multiply the speed and acceleration of the car when a boost is used.")]
        public float boostMultiplier = 2;

        private float boostTimer = 0;

        void Start()
        {
            InputEnabled = inputEnabledFromStart;
        }

        void Update()
        {
            if (!InputEnabled)
            {
                return;
            }

            float motorDelta = getInput(forwardInput) - getInput(reverseInput);
            float steeringDelta = getInput(steerRightInput) - getInput(steerLeftInput);

            if (getInput(boostInput) == 1 && boostTimer == 0)
            {
                boostTimer = boostCoolOff + boostDuration;
            }
            else if (boostTimer > 0)
            {
                boostTimer = Mathf.Max(boostTimer - Time.deltaTime, 0);
                carController.setBoostMultiplier(boostTimer > boostCoolOff ? boostMultiplier : 1);
            }

            carController.setSteering(steeringDelta);
            carController.setMotor(motorDelta);
        }

        public float getInput(InputValue v)
        {
            float value = 0;
            switch (v.type)
            {
                case InputType.Axis: value = Input.GetAxis(v.name); break;
                case InputType.RawAxis: value = Input.GetAxisRaw(v.name); break;
                case InputType.Key: value = Input.GetKey((KeyCode)int.Parse(v.name)) ? 1 : 0; break;
                case InputType.Button: value = Input.GetButton(v.name) ? 1 : 0; break;
            }
            if (v.invert) value *= -1;
            return Mathf.Clamp01(value);
        }
    }
}