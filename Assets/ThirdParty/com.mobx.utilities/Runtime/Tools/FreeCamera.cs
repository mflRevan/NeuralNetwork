using MobX.Utilities.Inspector;
using MobX.Utilities.Types;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MobX.Utilities.Tools
{
    public class FreeCamera : MonoBehaviour
    {
        #region Fields

        [SerializeField] [Range(0f, 2f)] private float mouseSensitivity = 1f;
        [SerializeField] private float accelerationSharpness = 5f;
        [SerializeField] [Range(0f, 90f)] private float maxXAngle = 90f;

        [Header("Speed")]
        [SerializeField] private float startSpeed = 10f;
        [SerializeField] private float minSpeed = .1f;
        [SerializeField] private float maxSpeed = 100f;
        [SerializeField] [Range(0f, 1f)] private float breakSpeedMultiplier = .35f;
        [SerializeField] [Range(1f, 10f)] private float sprintSpeedMultiplier = 2f;
        [SerializeField] private AnimationCurve speedCurve = AnimationCurve.EaseInOut(0, 1, 1, 2);

        [Header("Input Maps")]
        [SerializeField] private bool enableOnLoad;
        [SerializeField] private ActionMapName actionMap;
        [SerializeField] [Required] private InputActionAsset inputActionAsset;
        [Header("Input")]
        [SerializeField] [Required] private InputActionReference movementInput;
        [SerializeField] [Required] private InputActionReference lookInput;
        [SerializeField] [Required] private InputActionReference speedInput;
        [SerializeField] [Required] private InputActionReference breakInput;
        [SerializeField] [Required] private InputActionReference sprintInput;

        [Header("Cursor")]
        [SerializeField] private Optional<bool> cursorVisible;
        [SerializeField] private Optional<CursorLockMode> cursorLockMode;

        private float _speed;
        private float _rotationX;
        private int _frameCount;
        private Vector3 _targetDirection;

        #endregion


        #region Public

        public float Speed { get; private set; }

        public void Activate()
        {
            inputActionAsset.Enable();
            inputActionAsset.FindActionMap(actionMap).Enable();
        }

        public void Deactivate()
        {
            inputActionAsset.Disable();
            inputActionAsset.FindActionMap(actionMap).Disable();
        }

        #endregion


        #region Logic

        private void Awake()
        {
            Cursor.visible = cursorVisible.ValueOrDefault(Cursor.visible);
            Cursor.lockState = cursorLockMode.ValueOrDefault(Cursor.lockState);

            if (enableOnLoad)
            {
                Activate();
            }

            _speed = startSpeed;
        }

        private void LateUpdate()
        {
            HandleMouseRotation();

            var direction = CalculateDirectionVector();
            _targetDirection = Vector3.Lerp(_targetDirection, direction, accelerationSharpness * Time.deltaTime);
            transform.Translate(_targetDirection);
            _frameCount++;
        }

        private Vector3 CalculateDirectionVector()
        {
            var normalizedSpeed = Mathf.InverseLerp(minSpeed, maxSpeed, _speed);
            var speedMultiplier = speedCurve.Evaluate(normalizedSpeed);
            _speed += speedInput.action.ReadValue<float>() * speedMultiplier * Time.deltaTime;
            _speed = Mathf.Clamp(_speed, minSpeed, maxSpeed);

            var acceleration = movementInput.action.ReadValue<Vector3>();
            var breakValue = breakInput.action.IsPressed() ? breakSpeedMultiplier : 1f;
            var sprintValue = sprintInput.action.IsPressed() ? sprintSpeedMultiplier : 1f;
            Speed = _speed * breakValue * sprintValue;
            return acceleration.normalized * (Speed * Time.deltaTime);
        }

        private void HandleMouseRotation()
        {
            var rotationInput = _frameCount < 2 ? Vector2.zero : lookInput.action.ReadValue<Vector2>();
            var self = transform;

            var rotationHorizontal = mouseSensitivity * rotationInput.x;
            var rotationVertical = mouseSensitivity * rotationInput.y;

            self.Rotate(Vector3.up * rotationHorizontal, Space.World);

            var rotationY = self.localEulerAngles.y;

            _rotationX += rotationVertical;
            _rotationX = Mathf.Clamp(_rotationX, -maxXAngle, maxXAngle);

            self.localEulerAngles = new Vector3(-_rotationX, rotationY, 0);
        }

        #endregion
    }
}