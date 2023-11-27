using UnityEngine;
using UnityEngine.AI;

namespace Default
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class TargetDirectionAgent : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;

        public Vector3 TargetDirection { get; private set; }


        public void SetTarget(Vector3 pos)
        {
            agent.SetDestination(pos);
        }

        public void Reset()
        {
            agent.ResetPath();
            TargetDirection = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (agent.hasPath)
                TargetDirection = agent.steeringTarget - transform.position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + TargetDirection);
        }
    }
}