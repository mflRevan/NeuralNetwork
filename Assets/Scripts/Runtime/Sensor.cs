using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

namespace Default
{
    public enum SensorDirection
    {
        FORWARD,
        SIDE,
        BACKWARD,
        CURVE_DETECTOR
    }

    public class Sensor : MonoBehaviour
    {
        [SerializeField] private SensorDirection sensorDirection;
        public SensorDirection Direction => sensorDirection;

        /// <summary>
        /// Get the sensordata
        /// </summary>
        /// <returns>Returns the either the distance as float percentage (0 - 1), (1) being the max distance, or if the sensor is a curve detector it returns the curve direction encoded into 2 floats</returns>
        public float[] GetFeed()
        {
            LayerMask mask = GameManager.Instance != null ? GameManager.Instance.WallMask : 0;

            var hit = new RaycastHit();
            List<float> results = new();

            switch (Direction)
            {
                case SensorDirection.FORWARD:
                    Physics.SphereCast(transform.position, SensorFeed.AGENT_RADIUS, transform.forward, out hit, SensorFeed.RANGE_FORWARD, mask, QueryTriggerInteraction.Ignore);
                    results.Add(hit.collider != null ? Vector3.Distance(transform.position, hit.point) / SensorFeed.RANGE_FORWARD : 1f);
                    break;

                case SensorDirection.SIDE:
                    Physics.Raycast(transform.position, transform.forward, out hit, SensorFeed.RANGE_SIDE, mask, QueryTriggerInteraction.Ignore);
                    results.Add(hit.collider != null ? Vector3.Distance(transform.position, hit.point) / SensorFeed.RANGE_SIDE : 1f);
                    break;

                case SensorDirection.CURVE_DETECTOR:
                    Physics.Raycast(transform.position, transform.forward, out hit, SensorFeed.RANGE_CURVE, mask, QueryTriggerInteraction.Ignore);
                    var crossY = Vector3.Cross(transform.forward, hit.normal).y;
                    results.Add(crossY <= 0f ? 1f : Mathf.Abs(crossY));
                    results.Add(crossY >= 0f ? 1f : Mathf.Abs(crossY));
                    break;
            }

            return results.ToArray();
        }

        private void OnDrawGizmos()
        {
            var range = 1f;

            switch (Direction)
            {
                case SensorDirection.FORWARD:
                    range = GetFeed()[0];
                    range *= SensorFeed.RANGE_FORWARD;
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transform.position, transform.position + (transform.forward * range));
                    break;
                case SensorDirection.SIDE:
                    range = GetFeed()[0];
                    range *= SensorFeed.RANGE_SIDE;
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, transform.position + (transform.forward * range));
                    break;
                case SensorDirection.BACKWARD:
                    range = GetFeed()[0];
                    range *= SensorFeed.RANGE_BACKWARD;
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, transform.position + (transform.forward * range));
                    break;
                case SensorDirection.CURVE_DETECTOR:
                    var feed = GetFeed();
                    range = feed[1] - feed[0];
                    range *= SensorFeed.RANGE_CURVE;
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(transform.position, transform.position + (transform.right * range));
                    Gizmos.DrawLine(transform.position, transform.position + (transform.right * range));
                    break;
            }
        }
    }
}