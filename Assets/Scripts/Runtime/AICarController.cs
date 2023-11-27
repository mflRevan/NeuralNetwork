using UnityEngine;

namespace Default
{
    public class AICarController : MonoBehaviour
    {
        [SerializeField] public SensorFeed Feed;

        public NeuralNetwork AI { get; private set; }
        public bool InputEnabled { get; private set; }


        private void Start()
        {

        }

        private void OnDestroy()
        {

        }

        private void UpdateInputs()
        {

        }

        private void FixedUpdate()
        {

        }
    }
}