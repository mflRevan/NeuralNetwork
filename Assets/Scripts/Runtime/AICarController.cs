using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DavidJalbert;
using TMPro;
using UnityEngine;


namespace Default
{
    public class AICarController : MonoBehaviour
    {
        [SerializeField] public bool isPlayerController;
        [SerializeField] public SensorFeed Feed;
        [SerializeField] public TinyCarController tinyCarController;
        [SerializeField] public TargetDirectionAgent targetDirectionAgent;
        [SerializeField] public WallHitDetector wallHitDetector;
        [SerializeField] public TMP_Text uiHeader;

        [Header("Config")]
        [SerializeField] public float boostMultiplier = 2f;

        public NeuralNetwork AI { get; private set; }
        public bool AIDrivingEnabled { get; private set; }

        private float[] NNInputBuffer;

        private float motorInput;
        private float steeringInput;
        private float boostConfidenceInput;

        private float initialDistance = 0f;
        private float furthestDistancePassed;
        private float stuckTimer;

        // temp vars in order to execute in main thread when SetPositionAndRotation() is called
        private bool setPosAndRot;
        private Vector3 positionToSet;
        private Quaternion rotationToSet;

        public const int INPUT_NEURONS = 5;
        public const int OUTPUT_NEURONS = 3;
        public const float BOOST_CONFIDENCE_THRESHHOLD = 0.8f;
        public const float STUCK_MAX_TIMER = 12f;
        public const float TURN_INDICATOR_PROCESSING_THRESHHOLD_MAX_ANGLE = 100f; // the max angle (right and left) from the targetDirection to be processed by the AI for decision making


        private void Awake()
        {
            NNInputBuffer = new float[INPUT_NEURONS];

            wallHitDetector.WallHit += OnWallHit;
        }

        private void OnDestroy()
        {
            wallHitDetector.WallHit -= OnWallHit;
        }

        private void FixedUpdate()
        {
            if (isPlayerController)
            {
                var dir = EncodeDirectionIndicator(transform.forward, targetDirectionAgent.TargetDirection);
                print($"Turn {dir}");
            }

            if (setPosAndRot)
            {
                setPosAndRot = false;
                transform.SetPositionAndRotation(positionToSet, rotationToSet);

                targetDirectionAgent.enabled = true;
            }

            if (!AIDrivingEnabled || AI == null || !targetDirectionAgent.HasTarget()) { return; }

            InputInference();

            // if car is "stuck" somewhere, reset
            if (stuckTimer >= STUCK_MAX_TIMER)
            {
                Reset();
            }

            var currentDistancePassed = initialDistance - targetDirectionAgent.GetCurrentDistanceToTarget();

            if (currentDistancePassed < furthestDistancePassed + 6f)
            {
                stuckTimer += Time.fixedDeltaTime;
            }
            else
            {
                furthestDistancePassed = currentDistancePassed;
                stuckTimer = 0;
            }
        }

        private void OnWallHit()
        {
            Reset();
            Debug.Log("Hit something!");
        }

        private void Reset()
        {
            // SetPositionAndRotation(GameManager.Instance.spawn.position, GameManager.Instance.spawn.rotation);
            targetDirectionAgent.Reset();
            EnableDrivingAI(false);
        }

        public async UniTask SetTarget(Vector3 pos)
        {
            stuckTimer = 0f;
            furthestDistancePassed = 0f;

            targetDirectionAgent.SetTarget(pos);
            await targetDirectionAgent.WaitUntilHasPath();
            initialDistance = targetDirectionAgent.GetCurrentDistanceToTarget();
        }

        /// <returns>The completion percentage of the current path</returns>
        public float GetCompletionPercentage()
        {
            var currentDistance = targetDirectionAgent.GetCurrentDistanceToTarget();
            // print(1f - Mathf.Clamp01(currentDistance / initialDistance));
            return 1f - Mathf.Clamp01(currentDistance / initialDistance);
        }

        public void SetPositionAndRotation(Vector3 pos, Quaternion rot)
        {
            targetDirectionAgent.enabled = false;
            positionToSet = pos;
            rotationToSet = rot;
            setPosAndRot = true;
        }

        public void SetUIHeader(string text)
        {
            if (uiHeader != null) uiHeader.text = text;
        }

        public void SetAI(string data)
        {
            // init network from data
            AI = new NeuralNetwork(data);
        }

        public void SetAI(int[] hiddenLayerStructure)
        {
            var structure = new List<int>();

            structure.Add(INPUT_NEURONS);
            structure.AddRange(hiddenLayerStructure);
            structure.Add(OUTPUT_NEURONS);

            // init random network with set architecture
            AI = new NeuralNetwork(structure.ToArray());
        }

        public void SetAI(NeuralNetwork copyNetwork)
        {
            if (copyNetwork == null)
            {
                AI = null;
                return;
            };

            // deepcopy
            AI = new NeuralNetwork(copyNetwork);
        }

        public void EnableDrivingAI(bool enable)
        {
            AIDrivingEnabled = enable;
            tinyCarController.setMotor(0f);
            tinyCarController.setSteering(0f);
            tinyCarController.setBoostMultiplier(1f);
            tinyCarController.clearVelocity();

            if (uiHeader != null)
                uiHeader.color = enable ? Color.green : Color.red;
        }

        public float[] ProcessAndUpdateEnvironmentData()
        {
            var sensorData = Feed.GetSensorData();

            // add sensordata to input neurons
            for (int i = 0; i < sensorData.Length; i++)
            {
                NNInputBuffer[i] = sensorData[i];
            }

            // // add normalized car speed
            // var direction = tinyCarController.getVelocityDirection();
            // var maxSpeed = tinyCarController.getMaxSpeed();
            // var currentSpeed = Mathf.Abs(tinyCarController.getForwardVelocity());

            // var normalizedForwardSpeed = 0f;
            // var normalizedBackwardSpeed = 0f;

            // if (direction > 0) // driving forward
            // {
            //     normalizedForwardSpeed = 1f - (currentSpeed / maxSpeed); // inverted for improved processing (due to sigmoid) of higher speeds
            // }
            // else // driving backward
            // {
            //     normalizedBackwardSpeed = 1f - (currentSpeed / maxSpeed); // inverted for improved processing (due to sigmoid) of higher speeds
            // }

            // NNInputBuffer[sensorData.Length] = normalizedForwardSpeed;
            // NNInputBuffer[sensorData.Length + 1] = normalizedBackwardSpeed;

            // add direction indicators for right and left
            (float rightIndicator, float leftIndicator) = EncodeDirectionIndicator(transform.forward, targetDirectionAgent.TargetDirection);

            NNInputBuffer[sensorData.Length] = rightIndicator;
            NNInputBuffer[sensorData.Length + 1] = leftIndicator;

            return NNInputBuffer;
        }

        private void InputInference()
        {
            ProcessAndUpdateEnvironmentData();

            var input = AI.FeedForward(NNInputBuffer);

            motorInput = input[0];
            steeringInput = (input[1] * 2f) - 1f;
            boostConfidenceInput = input[2];

            tinyCarController.setMotor(motorInput);
            // print($"Forward: {forwardInput}, Backward {backwardInput}");
            tinyCarController.setSteering(steeringInput);
            // print($"Right: {rightInput}, Left {leftInput}");
            tinyCarController.setBoostMultiplier(boostConfidenceInput > BOOST_CONFIDENCE_THRESHHOLD ? boostMultiplier : 1f);
            // print($"Boost Confidence: {boostConfidenceInput}");
        }

        private (float, float) EncodeDirectionIndicator(Vector3 carForward, Vector3 directionIndicator)
        {
            // Flatten and normalize the input vectors 
            carForward.y = 0;
            carForward.Normalize();
            directionIndicator.y = 0;
            directionIndicator.Normalize();

            // Calculate the angle between the two vectors
            float angle = Vector3.Angle(carForward, directionIndicator);

            // Determine the direction of the turn using cross product
            float turnDirection = Vector3.Cross(carForward, directionIndicator).y;

            // Calculate the encoded outputs, inverted for better processing by the AI (due to sigmoid)
            float rightIndicator = turnDirection > 0 ? 1 - (angle / TURN_INDICATOR_PROCESSING_THRESHHOLD_MAX_ANGLE) : 1f;
            float leftIndicator = turnDirection < 0 ? 1 - (angle / TURN_INDICATOR_PROCESSING_THRESHHOLD_MAX_ANGLE) : 1f;

            // Ensure the indicators are within the range [0, 1]
            rightIndicator = Mathf.Clamp(rightIndicator, 0f, 1f);
            leftIndicator = Mathf.Clamp(leftIndicator, 0f, 1f);

            return (rightIndicator, leftIndicator);
        }
    }
}