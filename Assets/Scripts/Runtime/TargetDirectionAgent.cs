using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Default
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class TargetDirectionAgent : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;

        public NavMeshAgent Agent => agent;
        public Vector3 TargetDirection { get; private set; }

        private float lastDistance;

        private void Start()
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            agent.updatePosition = false;
        }

        public void SetTarget(Vector3 pos)
        {
            agent.isStopped = false;
            agent.SetDestination(pos);
        }

        public bool HasTarget()
        {
            return agent.hasPath;
        }

        public float GetCurrentDistanceToTarget()
        {
            if (!agent.hasPath)
            {
                return 0f;
            }

            var corners = agent.path.corners;
            var fullDistance = 0f;

            for (int i = 1; i < corners.Length; i++)
            {
                fullDistance += Vector3.Distance(corners[i - 1], corners[i]);
            }

            return fullDistance;
        }

        public async UniTask WaitUntilHasPath()
        {
            while (!agent.hasPath)
            {
                await UniTask.Yield();
            }
        }

        public void Reset()
        {
            // agent.ResetPath();
            agent.isStopped = true;
        }

        private void FixedUpdate()
        {
            if (agent.hasPath)
            {
                agent.nextPosition = transform.position;
                TargetDirection = agent.steeringTarget - transform.position;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + TargetDirection);
        }
    }
}