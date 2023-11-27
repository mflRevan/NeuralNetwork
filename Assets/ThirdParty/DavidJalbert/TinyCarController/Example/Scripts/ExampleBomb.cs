using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DavidJalbert
{
    public class ExampleBomb : MonoBehaviour
    {
        public AudioClip explosionSoundClip;

        private AudioSource sourceExplosion;

        private void Start()
        {
            sourceExplosion = gameObject.AddComponent<AudioSource>();
            sourceExplosion.playOnAwake = false;
            sourceExplosion.loop = false;
            sourceExplosion.clip = explosionSoundClip;
        }

        private void OnTriggerEnter(Collider collider)
        {
            TinyCarExplosiveBody car = collider.GetComponentInParent<TinyCarExplosiveBody>();
            if (car != null && !car.hasExploded())
            {
                car.explode();
                StartCoroutine(resetCar(car));
            }
        }

        private IEnumerator resetCar(TinyCarExplosiveBody car)
        {
            sourceExplosion.Play();
            yield return new WaitForSeconds(2);
            car.restore();
            yield return null;
        }
    }
}