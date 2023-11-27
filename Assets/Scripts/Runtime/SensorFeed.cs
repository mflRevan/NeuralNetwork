using System.Collections.Generic;
using UnityEngine;

namespace Default
{
    public class SensorFeed : MonoBehaviour
    {
        public const float RANGE_FORWARD = 60f;
        public const float RANGE_SIDE = 20f;
        public const float RANGE_BACKWARD = 40f;

        [SerializeField] private List<Sensor> sensors;
        public List<Sensor> Sensors => sensors;


        public float[] GetSensorData()
        {
            var output = new float[Sensors.Count];

            for (int i = 0; i < Sensors.Count; i++)
            {
                var s = Sensors[i];
                output[i] = s.GetFeed();
            }

            return output;
        }
    }
}