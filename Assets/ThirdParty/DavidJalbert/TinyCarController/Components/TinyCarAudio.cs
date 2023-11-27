using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DavidJalbert
{
    [RequireComponent(typeof(AudioSource))]
    public class TinyCarAudio : MonoBehaviour
    {
        public TinyCarController carController;
        [Header("Engine audio")]
        [Tooltip("Looping engine sound clip.")]
        public AudioClip engineSoundClip;
        [Tooltip("The pitch of the engine relative to the speed delta of the car. The speed delta is calculated by dividing the current speed by the max speed.")]
        public AnimationCurve enginePitchOverSpeed = new AnimationCurve(new Keyframe(0, 0.25f), new Keyframe(1, 1));
        [Header("Brakes and drifting audio")]
        [Tooltip("Looping braking sound clip.")]
        public AudioClip brakeSoundClip;
        [Tooltip("The volume of the brake sound relative to the speed delta of the car. The speed delta is calculated by dividing the current speed by the max speed.")]
        public AnimationCurve driftOverSpeed = new AnimationCurve(new Keyframe(0.25f, 0), new Keyframe(0.75f, 1));
        [Header("Bumping audio")]
        [Tooltip("Collision (bumping) sound clip.")]
        public AudioClip bumpSound;
        [Tooltip("The volume multiplier of the collision sound relative to the mass of the object the car collided with.")]
        public AnimationCurve bumpOverMass = new AnimationCurve(new Keyframe(0, 0), new Keyframe(2, 1));
        [Tooltip("The minimum relative velocity at which to play the collision sound.")]
        public float bumpMinForce = 2;
        [Tooltip("The maximum relative velocity at which to play the collision sound. Velocities over this number will still play the sound but clamp the volume to 1.")]
        public float bumpMaxForce = 10;
        [Header("Grinding audio")]
        [Tooltip("Looping wall grinding sound clip.")]
        public AudioClip grindingSound;
        [Tooltip("Whether to only play the grinding sound on static objects or on all objects, static or dynamic.")]
        public bool onlyGrindOnStatic = true;
        [Tooltip("The minimum relative velocity at which to play the grinding sound.")]
        public float grindMinForce = 0f;
        [Tooltip("The maximum relative velocity at which to play the grinding sound. Velocities over this number will still play the sound but clamp the volume to 1.")]
        public float grindMaxForce = 2f;
        [Tooltip("How much smoothing to apply to the change in volume of the grinding sound.")]
        public float grindSmoothing = 5f;
        [Header("Landing audio")]
        [Tooltip("Car landing sound clip.")]
        public AudioClip landingSound;
        [Tooltip("The minimum relative velocity at which to play the landing sound.")]
        public float landingMinForce = 1f;
        [Tooltip("The maximum relative velocity at which to play the landing sound. Velocities over this number will still play the sound but clamp the volume to 1.")]
        public float landingMaxForce = 2f;

        private AudioSource audioSourceTemplate;
        private AudioSource sourceEngine;
        private AudioSource sourceBrake;
        private AudioSource sourceGrinding;
        private AudioSource sourceBump;
        private AudioSource sourceLanding;

        void Start()
        {
            audioSourceTemplate = GetComponent<AudioSource>();

            sourceEngine = carController.gameObject.AddComponent<AudioSource>();
            setAudioSourceFromTemplate(ref sourceEngine, audioSourceTemplate);
            sourceEngine.playOnAwake = false;
            sourceEngine.loop = true;
            sourceEngine.clip = engineSoundClip;
            sourceEngine.volume = 1;

            sourceEngine.Play();

            sourceBrake = carController.gameObject.AddComponent<AudioSource>();
            setAudioSourceFromTemplate(ref sourceBrake, audioSourceTemplate);
            sourceBrake.playOnAwake = false;
            sourceBrake.loop = true;
            sourceBrake.clip = brakeSoundClip;
            sourceBrake.volume = 0;

            sourceBrake.Play();

            sourceGrinding = carController.gameObject.AddComponent<AudioSource>();
            setAudioSourceFromTemplate(ref sourceGrinding, audioSourceTemplate);
            sourceGrinding.playOnAwake = false;
            sourceGrinding.loop = true;
            sourceGrinding.clip = grindingSound;
            sourceGrinding.volume = 0;

            sourceGrinding.Play();

            sourceBump = carController.gameObject.AddComponent<AudioSource>();
            setAudioSourceFromTemplate(ref sourceBump, audioSourceTemplate);
            sourceBump.playOnAwake = false;
            sourceBump.loop = false;
            sourceBump.clip = bumpSound;

            sourceLanding = carController.gameObject.AddComponent<AudioSource>();
            setAudioSourceFromTemplate(ref sourceLanding, audioSourceTemplate);
            sourceLanding.playOnAwake = false;
            sourceLanding.loop = false;
            sourceLanding.clip = bumpSound;
        }

        void Update()
        {
            setAudioSourceFromTemplate(ref sourceEngine, audioSourceTemplate);
            setAudioSourceFromTemplate(ref sourceBrake, audioSourceTemplate);
            setAudioSourceFromTemplate(ref sourceGrinding, audioSourceTemplate);
            setAudioSourceFromTemplate(ref sourceBump, audioSourceTemplate);
            setAudioSourceFromTemplate(ref sourceLanding, audioSourceTemplate);

            if (!sourceEngine.isPlaying && sourceEngine.isActiveAndEnabled) sourceEngine.Play();
            if (!sourceBrake.isPlaying && sourceBrake.isActiveAndEnabled) sourceBrake.Play();
            if (!sourceGrinding.isPlaying && sourceGrinding.isActiveAndEnabled) sourceGrinding.Play();

            float speedDelta = Mathf.Clamp01(Mathf.Abs(carController.getForwardVelocity() / carController.getMaxSpeed()));
            sourceEngine.pitch = enginePitchOverSpeed.Evaluate(speedDelta);

            float driftDelta = Mathf.Clamp01(Mathf.Abs(carController.getLateralVelocity() / carController.getMaxSpeed()));
            float brakeDelta = (carController.isBraking() ? 1 : 0) * speedDelta;
            sourceBrake.volume = Mathf.Clamp01(getDriftVolume(brakeDelta) + getDriftVolume(driftDelta));

            if (carController.hasHitSide())
            {
                float bumpMassValue = bumpOverMass.Evaluate(carController.getSideHitMass());
                float bumpForceValue = Mathf.Lerp(bumpMinForce, bumpMaxForce, carController.getSideHitForce());
                float bumpVolume = bumpMassValue * bumpForceValue;
                if (bumpVolume > 0)
                {
                    sourceBump.volume = bumpVolume;
                    sourceBump.Play();
                }
            }

            float grindVolume = 0;
            if (carController.isHittingSide(onlyGrindOnStatic))
            {
                grindVolume = Mathf.Clamp01((carController.getSideHitForce() - grindMinForce) / (grindMaxForce - grindMinForce));
            }
            sourceGrinding.volume = Mathf.Lerp(sourceGrinding.volume, grindVolume, Time.deltaTime * grindSmoothing);

            if (carController.hasHitGround(landingMinForce))
            {
                float landingVolume = Mathf.Clamp01((carController.getGroundHitForce() - landingMinForce) / (landingMaxForce - landingMinForce));
                sourceLanding.volume = landingVolume;
                sourceLanding.Play();
            }
        }

        private float getDriftVolume(float driftDelta)
        {
            return driftOverSpeed.Evaluate(driftDelta);
        }

        private void setAudioSourceFromTemplate(ref AudioSource source, AudioSource template)
        {
            source.bypassEffects = template.bypassEffects;
            source.bypassListenerEffects = template.bypassListenerEffects;
            source.bypassReverbZones = template.bypassReverbZones;
            source.dopplerLevel = template.dopplerLevel;
            source.maxDistance = template.maxDistance;
            source.minDistance = template.minDistance;
            source.outputAudioMixerGroup = template.outputAudioMixerGroup;
            source.panStereo = template.panStereo;
            source.pitch = template.pitch;
            source.priority = template.priority;
            source.reverbZoneMix = template.reverbZoneMix;
            source.rolloffMode = template.rolloffMode;
            source.spatialBlend = template.spatialBlend;
            source.spread = template.spread;
            source.velocityUpdateMode = template.velocityUpdateMode;
        }
    }
}