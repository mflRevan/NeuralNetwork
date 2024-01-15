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
        public float InitialDistance => initialDistance;

        public float[] NNInputBuffer { get; private set; }

        private float motorInput;
        private float steeringInput;
        private float boostConfidenceInput;

        private bool hasCrashedLastRun;
        private float initialDistance = 0f;
        private float lastFinishTime;
        private float furthestDistancePassed;
        private float stuckTimer;
        private float driveTimer;
        private int skippedFrames;

        public const int INFERENCE_FRAMES_TO_SKIP = 2;
        public const int INPUT_NEURONS = 8;
        public const int OUTPUT_NEURONS = 3;
        public const float BOOST_CONFIDENCE_THRESHHOLD = 0.7f;
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
            // if (isPlayerController)
            // {
            //     var sensorData = Feed.GetSensorData();
            //     print($"To the left: {sensorData[sensorData.Length - 1]}\nTo the right: {sensorData[sensorData.Length - 2]}");
            // }

            // if (isPlayerController) print(GetCurrentSpeedNormalized());

            if (!AIDrivingEnabled || AI == null || !targetDirectionAgent.HasTarget()) { return; }

            if (skippedFrames >= INFERENCE_FRAMES_TO_SKIP)
            {
                InputInference();
                skippedFrames = 0;
            }
            else
            {
                skippedFrames++;
            }

            driveTimer += Time.fixedDeltaTime;

            // if car is "stuck" somewhere, reset
            if (stuckTimer >= STUCK_MAX_TIMER)
            {
                Reset();
            }

            var currentDistancePassed = initialDistance - targetDirectionAgent.GetCurrentDistanceToTarget();

            if (currentDistancePassed < furthestDistancePassed + 10f)
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
            hasCrashedLastRun = true;
        }

        private void Reset()
        {
            targetDirectionAgent.Reset();
            EnableDrivingAI(false);
        }

        public void OnFinish()
        {
            lastFinishTime = driveTimer;
            Reset();
        }

        public void OnFail()
        {
            Reset();
        }

        public async UniTask SetTarget(Vector3 pos)
        {
            stuckTimer = 0f;
            furthestDistancePassed = 0f;

            targetDirectionAgent.SetTarget(pos);
            await targetDirectionAgent.WaitUntilHasPath();
            initialDistance = targetDirectionAgent.GetCurrentDistanceToTarget();
        }

        public float GetCurrentSpeedNormalized()
        {
            return tinyCarController.getForwardVelocityDelta() / boostMultiplier;
        }

        /// <returns>The completion percentage of the current path</returns>
        public float GetCompletionPercentage()
        {
            if (lastFinishTime >= 0.5f) // if the car has reached the finish line => return 100%
            {
                return 1f;
            }
            else // return the actual distance
            {
                var currentDistance = targetDirectionAgent.GetCurrentDistanceToTarget();

                return 1f - Mathf.Clamp01(currentDistance / initialDistance);
            }
        }

        /// <summary>
        /// If the car crashed, finish time will be 0 seconds
        /// </summary>
        public float GetLastFinishTime()
        {
            return lastFinishTime;
        }

        public bool HasCrashedLastRun()
        {
            return hasCrashedLastRun;
        }

        public async UniTask SetPositionAndRotation(Vector3 pos, Quaternion rot)
        {
            await UniTask.WaitForFixedUpdate(gameObject.GetCancellationTokenOnDestroy());

            transform.SetPositionAndRotation(pos, rot);
            targetDirectionAgent.Warp(pos);
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
            driveTimer = 0f;
            lastFinishTime = enable ? 0f : lastFinishTime; // reset the finishtime upon enabling the car
            hasCrashedLastRun = enable ? false : hasCrashedLastRun; // reset the crash indicator upon enabling the car

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

            // add normalized & inverted car speed
            NNInputBuffer[sensorData.Length] = GetCurrentSpeedNormalized();

            // add direction indicators for right and left
            (float rightIndicator, float leftIndicator) = EncodeDirectionIndicator(transform.forward, targetDirectionAgent.TargetDirection);
            NNInputBuffer[sensorData.Length + 1] = rightIndicator;
            NNInputBuffer[sensorData.Length + 2] = leftIndicator;

            return NNInputBuffer;
        }

        private void InputInference()
        {
            ProcessAndUpdateEnvironmentData();

            var input = AI.FeedForward(NNInputBuffer);

            motorInput = (input[0] * 2f) - 1f;
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

            // Calculate the encoded outputs, inverted for better processing by the AI 
            float rightIndicator = turnDirection > 0 ? 1f - (angle / TURN_INDICATOR_PROCESSING_THRESHHOLD_MAX_ANGLE) : 1f;
            float leftIndicator = turnDirection < 0 ? 1f - (angle / TURN_INDICATOR_PROCESSING_THRESHHOLD_MAX_ANGLE) : 1f;

            // Ensure the indicators are within the range [0, 1]
            rightIndicator = Mathf.Clamp(rightIndicator, 0f, 1f);
            leftIndicator = Mathf.Clamp(leftIndicator, 0f, 1f);

            return (rightIndicator, leftIndicator);
        }
    }
}