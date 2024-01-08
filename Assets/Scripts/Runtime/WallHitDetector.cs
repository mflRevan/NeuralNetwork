using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Default
{
    [RequireComponent(typeof(BoxCollider))]
    public class WallHitDetector : MonoBehaviour
    {
        public Action WallHit;

        private void Start()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            WallHit?.Invoke();
        }
    }
}