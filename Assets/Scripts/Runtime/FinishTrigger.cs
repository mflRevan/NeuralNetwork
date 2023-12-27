using UnityEngine;

namespace Default
{
    public class FinishTrigger : MonoBehaviour
    {
        public bool IsFinishTrigger { get; set; }

        private void OnTriggerEnter(Collider c)
        {
            if (c.TryGetComponent<AICarController>(out var car))
            {
                if (IsFinishTrigger)
                    car.OnFinish();
                else
                    car.OnFail();
            }
        }
    }
}