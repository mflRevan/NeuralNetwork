using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DavidJalbert
{
    public class TinyCarExplosiveBody : MonoBehaviour
    {
        public class OriginalPosition
        {
            public Transform transform;
            public Vector3 position;
            public Quaternion rotation;

            public OriginalPosition(Transform t)
            {
                transform = t;
                position = t.localPosition;
                rotation = t.localRotation;
            }

            public void restore()
            {
                transform.localRotation = rotation;
                transform.localPosition = position;
            }
        }

        [Tooltip("The car controller. Will be disabled when the explosion occurs.")]
        public TinyCarController carController;
        [Tooltip("The car visuals. Will be disabled when the explosion occurs.")]
        public GameObject visuals;
        [Tooltip("Object that contains the car parts that will fly off when exploding.")]
        public GameObject partsContainer;
        [Tooltip("The transform at which the car will respawn.")]
        public Transform spawnPoint;

        [Tooltip("The speed at which the parts of the car will fly off when exploding.")]
        public float explosionForce = 30f;
        [Tooltip("The angular speed at which the parts of the car will fly off when exploding.")]
        public float explosionTorque = 30f;

        private List<OriginalPosition> originalPositions;
        private List<Rigidbody> carParts;
        private bool exploded = false;

        void Start()
        {
            originalPositions = new List<OriginalPosition>();
            carParts = new List<Rigidbody>(partsContainer.GetComponentsInChildren<Rigidbody>());
            foreach (Rigidbody b in carParts)
            {
                originalPositions.Add(new OriginalPosition(b.transform));
            }
            partsContainer.SetActive(false);
        }

        void Update()
        {
        
        }

        public bool hasExploded()
        {
            return exploded;
        }

        public void explode()
        {
            exploded = true;

            carController.clearVelocity();

            visuals.SetActive(false);
            partsContainer.SetActive(true);
            partsContainer.transform.position = carController.transform.position;
            partsContainer.transform.rotation = carController.transform.rotation;
            carController.gameObject.SetActive(false);
            
            foreach (Rigidbody body in carParts)
            {
                Vector3 forceVector = Random.onUnitSphere;
                forceVector.y = Mathf.Abs(forceVector.y) * 2;
                forceVector.Normalize();
                Vector3 torqueVector = Random.onUnitSphere;
                body.AddForce(forceVector * explosionForce, ForceMode.Impulse);
                body.AddTorque(torqueVector * explosionTorque);
            }
        }

        public void restore()
        {
            foreach (OriginalPosition op in originalPositions)
            {
                op.restore();
            }

            visuals.SetActive(true);
            partsContainer.SetActive(false);

            carController.transform.position = spawnPoint.position;
            carController.transform.rotation = spawnPoint.rotation;
            carController.gameObject.SetActive(true);

            exploded = false;
        }
    }
}