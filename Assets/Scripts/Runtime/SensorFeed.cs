using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Default
{
    public class SensorFeed : MonoBehaviour
    {
        public const float RANGE_FORWARD = 80f;
        public const float RANGE_SIDE = 40f;
        public const float RANGE_BACKWARD = 20f;
        public const float RANGE_CURVE = 100f;
        public const float AGENT_RADIUS = 1f;


        [SerializeField] private List<Sensor> sensors;
        public List<Sensor> Sensors => sensors;


        public float[] GetSensorData()
        {
            var output = new List<float>();

            foreach (var sensor in Sensors)
            {
                output.AddRange(sensor.GetFeed());
            }

            return output.ToArray();
        }
    }
}