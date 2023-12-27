using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Default
{
    public class RaceTrack : MonoBehaviour
    {
        public Transform spawn;
        public List<FinishTrigger> targets;

        public Transform GetRandomTargetAndActivateIt()
        {
            var rndm = Random.Range(0, targets.Count - 1);

            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];

                targets[i].IsFinishTrigger = (i == rndm);
            }

            return targets[rndm].transform;
        }
    }
}