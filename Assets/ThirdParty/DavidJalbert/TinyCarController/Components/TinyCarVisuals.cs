using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DavidJalbert
{
    public class TinyCarVisuals : MonoBehaviour
    {
        public TinyCarController carController;
        [Tooltip("Scales the mass, acceleration, and max speed according to the GameObject's scale.")]
        public bool adjustToScale = false;

        [Header("Visuals")]
        [Tooltip("Object on which to apply the controller's position and rotation.")]
        public Transform vehicleContainer;
        [Tooltip("How fast the wheels will spin relative to the car's forward velocity.")]
        public float wheelsSpinForce = 100;
        [Tooltip("Maximum angle at which to turn the wheels when steering.")]
        public float wheelsMaxTurnAngle = 45;
        [Tooltip("How much to smooth the rotation when wheels are turning.")]
        public float wheelsTurnSmoothing = 10;
        [Tooltip("Body object of the vehicle.")]
        public Transform vehicleBody;
        [Tooltip("Wheel objects that will turn and spin with steering and acceleration.")]
        public Transform[] wheelsFront;
        [Tooltip("Wheel objects that will spin with acceleration.")]
        public Transform[] wheelsBack;
        [Tooltip("Whether to rotate the vehicle forward and back on slopes.")]
        public bool rotatePitch = true;
        [Tooltip("Whether to rotate the vehicle left and right on slopes.")]
        public bool rotateRoll = true;
        [Tooltip("The target angle that the car will lean towards when in the air.")]
        public float airPitchMax = 45f;
        [Tooltip("The speed at which the car will change its pitch in the air.")]
        public float airPitchSpeed = 1f;
        [Tooltip("The speed at which the car will change its pitch when it's back on ground.")]
        public float airPitchSpeedGrounded = 10f;

        [Header("Particles")]

        [Tooltip("Minimum velocity when scraping against a wall to play the particle system.")]
        public float minSideFrictionVelocity = 1;
        [Tooltip("Particle system to play when scraping against a wall.")]
        public ParticleSystem particlesSideFriction;

        [Tooltip("Minimum force when hitting a wall to play the particle system.")]
        public float minSideCollisionForce = 10;
        [Tooltip("Particle system to play when hitting a wall.")]
        public ParticleSystem particlesSideCollision;

        [Tooltip("Minimum force when landing on the ground to play the particle system.")]
        public float minLandingForce = 20;
        [Tooltip("Particle system to play when landing on the ground.")]
        public ParticleSystem particlesLanding;

        [Tooltip("Minimum lateral velocity when drifting to play the particle system.")]
        public float minDriftingSpeed = 10;
        [Tooltip("Particle system to play when drifting on the ground.")]
        public ParticleSystem particlesDrifting;
        [Tooltip("Particle system to play when using the boost.")]
        public ParticleSystem particlesBoost;

        [Tooltip("How fast the car should align to the ground.")]
        public float rotationSmoothingOnGround = 10f;
        [Tooltip("How fast the car should align in the air.")]
        public float rotationSmoothingInAir = 5f;

        private float wheelRotation = 0;
        private float wheelSpin = 0;
        private float pitchModifier = 0;
        private Quaternion groundRotationSmooth;

        void Start()
        {
            stopAllParticles();
        }

        void FixedUpdate()
        {
            if (!carController.gameObject.activeInHierarchy)
            {
                return;
            }

            float averageScale = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3f;

            float deltaTime = Time.fixedDeltaTime;

            // visuals

            if (vehicleContainer != null)
            {
                Quaternion groundRotationPlane = Quaternion.Euler(groundRotationSmooth.eulerAngles.x, carController.getGroundRotation().eulerAngles.y, groundRotationSmooth.eulerAngles.z);
                float rotationLerpValue = carController.isGrounded() ? rotationSmoothingOnGround : rotationSmoothingInAir;
                groundRotationSmooth = Quaternion.Slerp(groundRotationPlane, carController.getGroundRotation(), rotationLerpValue <= 0 ? 1 : deltaTime * rotationLerpValue);

                Vector3 smoothRotation = groundRotationSmooth.eulerAngles;
                if (!rotatePitch)
                {
                    smoothRotation.x = 0;
                }
                if (!rotateRoll)
                {
                    smoothRotation.z = 0;
                }

                if (!carController.isGrounded())
                {
                    pitchModifier += (airPitchMax - pitchModifier) * Mathf.Clamp01(deltaTime * airPitchSpeed);
                }
                else
                {
                    pitchModifier += -pitchModifier * Mathf.Clamp01(deltaTime * airPitchSpeedGrounded);
                }

                smoothRotation.x += pitchModifier;

                vehicleContainer.rotation = Quaternion.Euler(smoothRotation);
                vehicleContainer.position = carController.getBodyPosition();
            }

            wheelSpin += carController.getForwardVelocity() * deltaTime * wheelsSpinForce * (adjustToScale ? averageScale : 1);
            wheelRotation = Mathf.Lerp(wheelRotation, carController.getSteering() * wheelsMaxTurnAngle, wheelsTurnSmoothing <= 0 ? 1 : Mathf.Clamp01(wheelsTurnSmoothing * deltaTime));

            foreach (Transform t in wheelsFront)
            {
                t.transform.localRotation = Quaternion.Euler(wheelSpin, wheelRotation, 0);
            }
            foreach (Transform t in wheelsBack)
            {
                t.transform.localRotation = Quaternion.Euler(wheelSpin, 0, 0);
            }

            // particles

            // drifting smoke

            if (particlesDrifting != null)
            {
                if (Mathf.Abs(carController.getLateralVelocity()) > minDriftingSpeed * (adjustToScale ? averageScale : 1) && carController.isGrounded())
                {
                    if (!particlesDrifting.isPlaying)
                    {
                        particlesDrifting.Play();
                    }
                }
                else
                {
                    if (particlesDrifting.isPlaying)
                    {
                        particlesDrifting.Stop();
                    }
                }
            }

            // collision sparks

            if (particlesSideCollision != null)
            {
                if (carController.hasHitSide(minSideCollisionForce * (adjustToScale ? averageScale : 1)))
                {
                    particlesSideCollision.transform.position = carController.getSideHitPosition();
                    particlesSideCollision.Play();
                }
            }

            if (particlesLanding != null)
            {
                if (carController.hasHitGround(minLandingForce * (adjustToScale ? averageScale : 1)))
                {
                    particlesLanding.Play();
                }
            }

            if (particlesSideFriction != null)
            {
                if (carController.isHittingSide() && carController.getGroundVelocity() > minSideFrictionVelocity * (adjustToScale ? averageScale : 1))
                {
                    particlesSideFriction.transform.position = carController.getSideHitPosition();
                    if (!particlesSideFriction.isPlaying) particlesSideFriction.Play();
                }
                else
                {
                    if (particlesSideFriction.isPlaying) particlesSideFriction.Stop();
                }
            }

            if (particlesBoost != null)
            {
                if (carController.getBoostMultiplier() > 1f)
                {
                    particlesBoost.Play();
                }
                else
                {
                    particlesBoost.Stop();
                }
            }
        }

        public void stopAllParticles()
        {
            if (particlesSideFriction != null) particlesSideFriction.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (particlesSideCollision != null) particlesSideCollision.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (particlesLanding != null) particlesLanding.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (particlesDrifting != null) particlesDrifting.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (particlesBoost != null) particlesBoost.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}