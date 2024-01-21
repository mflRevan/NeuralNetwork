using FMOD;
using FMOD.Studio;
using FMODUnity;
using MobX.Utilities.Types;
using UnityEngine;

namespace MobX.Utilities.Fmod
{
    public class AudioAsset : InlinedAsset
    {
        [SerializeField] private EventReference audioEvent;
        [SerializeField] private Optional<float> volume = new(1f, false);

        public EventReference EventReference => audioEvent;

        // ReSharper disable Unity.PerformanceAnalysis
        public void Play()
        {
            audioEvent.PlayOneShot();
        }

        public void Play(Transform target)
        {
            audioEvent.PlayOneShotAttached(target, volume.ValueOrDefault(1f));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void Play(Vector3 position)
        {
            audioEvent.PlayOneShot(position);
        }

        public EventInstance CreateInstance()
        {
            return RuntimeManager.CreateInstance(audioEvent);
        }

        public void StartInstance(out EventInstance audioInstance)
        {
            audioInstance = RuntimeManager.CreateInstance(audioEvent);
            audioInstance.start();
            if (volume.Enabled)
            {
                audioInstance.setVolume(volume.Value);
            }
        }

        public void StartInstance(out EventInstance audioInstance, ATTRIBUTES_3D attributes3D)
        {
            audioInstance = RuntimeManager.CreateInstance(audioEvent);
            audioInstance.set3DAttributes(attributes3D);
            audioInstance.start();
            if (volume.Enabled)
            {
                audioInstance.setVolume(volume.Value);
            }
        }
    }
}
