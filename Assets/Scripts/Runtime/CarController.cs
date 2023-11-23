using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

namespace Default
{
    public class CarController : MonoBehaviour
    {
        [SerializeField] private Rigidbody carBody;
        [SerializeField] private InputActionReference driveInput;
        [SerializeField] private InputActionReference brakeInput;

        [Header("Wheels (Colliders)")]
        [SerializeField] private WheelCollider frontLeftWheelCollider;
        [SerializeField] private WheelCollider frontRightWheelCollider;
        [SerializeField] private WheelCollider rearLeftWheelCollider;
        [SerializeField] private WheelCollider rearRightWheelCollider;

        [Header("Wheels (Transforms)")]
        [SerializeField] private Transform frontLeftWheelTransform;
        [SerializeField] private Transform frontRightWheelTransform;
        [SerializeField] private Transform rearLeftWheelTransform;
        [SerializeField] private Transform rearRightWheelTransform;

        [SerializeField] private bool inputEnabledFromStart = true;
        [SerializeField] private float maxMotorForce = 1000f;
        [SerializeField] private float maxBrakeForce = 100f;
        [SerializeField] private float maxSteerAngle = 80f;

        private float currentBrakeForce;
        private float currentSteerAngle;

        public bool InputEnabled { get; private set; }
        public float Forward { get; private set; } // -1 to 1
        public float Turn { get; private set; } // -1 to 1
        public float Brake { get; private set; } // 0 to 1


        private void Start()
        {
            // driveInput.action.performed += OnDriveInput;
            // brakeInput.action.performed += OnBrakeInput;

            InputEnabled = inputEnabledFromStart;
        }

        private void OnDestroy()
        {
            // driveInput.action.performed -= OnDriveInput;
            // brakeInput.action.performed -= OnBrakeInput;
        }

        private void UpdateInputs()
        {
            Forward = driveInput.action.ReadValue<Vector2>().y;
            Turn = driveInput.action.ReadValue<Vector2>().x;
            Brake = brakeInput.action.ReadValue<float>();
        }

        private void FixedUpdate()
        {
            if (!InputEnabled)
            {
                return;
            }

            UpdateInputs();

            HandleMotor();
            HandleSteering();
            UpdateWheels();

            Forward = 0f;
            Turn = 0f;
            Brake = 0f;
        }

        private void HandleMotor()
        {
            rearLeftWheelCollider.motorTorque = Forward * maxMotorForce;
            rearRightWheelCollider.motorTorque = Forward * maxMotorForce;
            currentBrakeForce = Brake;

            ApplyBreaking();
        }

        private void ApplyBreaking()
        {
            frontRightWheelCollider.brakeTorque = currentBrakeForce;
            frontLeftWheelCollider.brakeTorque = currentBrakeForce;
            rearLeftWheelCollider.brakeTorque = currentBrakeForce;
            rearRightWheelCollider.brakeTorque = currentBrakeForce;
        }

        private void HandleSteering()
        {
            currentSteerAngle = maxSteerAngle * Turn;
            frontLeftWheelCollider.steerAngle = currentSteerAngle;
            frontRightWheelCollider.steerAngle = currentSteerAngle;
        }

        private void UpdateWheels() // just for visuals
        {
            UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
            UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
            UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
            UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        }

        private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
        {
            wheelCollider.GetWorldPose(out var pos, out var rot);
            wheelTransform.SetPositionAndRotation(pos, rot);
        }
    }
}