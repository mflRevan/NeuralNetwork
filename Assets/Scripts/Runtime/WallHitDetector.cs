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
            var colliderMask = LayerMask.GetMask(LayerMask.LayerToName(other.gameObject.layer));

            if (GameManager.Instance.WallMask == colliderMask)
            {
                WallHit?.Invoke();
            }
        }
    }
}