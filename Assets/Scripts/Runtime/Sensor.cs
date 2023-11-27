using Unity.Barracuda;
using UnityEngine;

namespace Default
{
    public enum SensorDirection
    {
        FORWARD,
        SIDE,
        BACKWARD
    }

    public class Sensor : MonoBehaviour
    {
        [SerializeField] private SensorDirection sensorDirection;
        public SensorDirection Direction => sensorDirection;

        /// <summary>
        /// Get the sensordata
        /// </summary>
        /// <returns>Returns the distance as float percentage (0 - 1), (1) being the max distance</returns>
        public float GetFeed()
        {
            LayerMask mask = GameManager.Instance != null ? GameManager.Instance.WallMask : 0;
            var range = 0f;

            switch (Direction)
            {
                case SensorDirection.FORWARD:
                    range = SensorFeed.RANGE_FORWARD;
                    break;
                case SensorDirection.SIDE:
                    range = SensorFeed.RANGE_SIDE;
                    break;
                case SensorDirection.BACKWARD:
                    range = SensorFeed.RANGE_BACKWARD;
                    break;
            }

            Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range, mask, QueryTriggerInteraction.Collide);
            var rangePercentage = hit.collider != null ? Vector3.Distance(transform.position, hit.point) / range : 1f;

            return rangePercentage;
        }

        private void OnDrawGizmos()
        {
            float range = GetFeed();

            switch (Direction)
            {
                case SensorDirection.FORWARD:
                    range *= SensorFeed.RANGE_FORWARD;
                    Gizmos.color = Color.green;
                    break;
                case SensorDirection.SIDE:
                    range *= SensorFeed.RANGE_SIDE;
                    Gizmos.color = Color.yellow;
                    break;
                case SensorDirection.BACKWARD:
                    range *= SensorFeed.RANGE_BACKWARD;
                    Gizmos.color = Color.red;
                    break;
            }

            Gizmos.DrawLine(transform.position, transform.position + (transform.forward * range));
        }
    }
}