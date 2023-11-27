using DavidJalbert;
using UnityEngine;


namespace Default
{
    public class AICarController : MonoBehaviour
    {
        [SerializeField] public SensorFeed Feed;
        [SerializeField] public TinyCarController tinyCarController;
        [SerializeField] public TargetDirectionAgent targetDirectionAgent;
        [SerializeField] public NeuralNetworkData data;
        [SerializeField] public Transform target;

        [Header("Config")]
        [SerializeField] public float boostMultiplier = 2f;

        public NeuralNetwork AI { get; private set; }
        public bool AIDrivingEnabled { get; private set; }

        private float[] NNInputBuffer;

        private float forwardInput;
        private float backwardInput;
        private float rightInput;
        private float leftInput;
        private float boostConfidenceInput;

        public const int INPUT_NEURONS = 14;
        public const int OUTPUT_NEURONS = 5;
        public const float BOOST_CONFIDENCE_THRESHHOLD = 0.7f;
        public const float TURN_INDICATOR_PROCESSING_THRESHHOLD_MAX_ANGLE = 100f; // the max angle (right and left) from the targetDirection to be processed by the AI for decision making


        private void Awake()
        {
            TickSystem.OnTick += InputInference;

            NNInputBuffer = new float[INPUT_NEURONS];
        }

        private void Start()
        {
            // debug
            targetDirectionAgent.SetTarget(target.position);

            AI = new NeuralNetwork(data.FittestNetwork);


            // EnableDrivingAI(true);
        }

        private void OnDestroy()
        {
            TickSystem.OnTick -= InputInference;
        }

        public void EnableDrivingAI(bool enable)
        {
            AIDrivingEnabled = enable;
        }

        public float[] ProcessAndUpdateEnvironmentData()
        {
            var sensorData = Feed.GetSensorData();


            // add sensordata to input neurons
            for (int i = 0; i < sensorData.Length; i++)
            {
                NNInputBuffer[i] = sensorData[i];
            }


            // add normalized car speed
            var direction = tinyCarController.getVelocityDirection();
            var maxSpeed = tinyCarController.getMaxSpeed();
            var currentSpeed = Mathf.Abs(tinyCarController.getForwardVelocity());

            var normalizedForwardSpeed = 0f;
            var normalizedBackwardSpeed = 0f;

            if (direction > 0) // driving forward
            {
                normalizedForwardSpeed = 1f - (currentSpeed / maxSpeed); // inverted for improved processing (due to sigmoid) of higher speeds
            }
            else // driving backward
            {
                normalizedBackwardSpeed = 1f - (currentSpeed / maxSpeed); // inverted for improved processing (due to sigmoid) of higher speeds
            }

            NNInputBuffer[sensorData.Length] = normalizedForwardSpeed;
            NNInputBuffer[sensorData.Length + 1] = normalizedBackwardSpeed;


            // add direction indicators for right and left
            (float rightIndicator, float leftIndicator) = EncodeDirectionIndicator(transform.forward, targetDirectionAgent.TargetDirection);

            NNInputBuffer[sensorData.Length + 2] = rightIndicator;
            NNInputBuffer[sensorData.Length + 3] = leftIndicator;

            return NNInputBuffer;
        }

        private void InputInference()
        {
            if (!AIDrivingEnabled || AI == null) { return; }

            ProcessAndUpdateEnvironmentData();


            var input = AI.FeedForward(NNInputBuffer);

            forwardInput = input[0];
            backwardInput = input[1];
            rightInput = input[2];
            leftInput = input[3];
            boostConfidenceInput = input[4];


            tinyCarController.setMotor(forwardInput - backwardInput);
            // print($"Forward: {forwardInput}, Backward {backwardInput}");
            tinyCarController.setSteering(rightInput - leftInput);
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