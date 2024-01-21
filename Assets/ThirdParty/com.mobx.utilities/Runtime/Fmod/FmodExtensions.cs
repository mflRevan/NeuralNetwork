using FMOD.Studio;
using FMODUnity;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace MobX.Utilities.Fmod
{
    public static class FmodExtensions
    {
        /// <summary>
        ///     Starts playback if not already playing
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Play(this EventInstance eventInstance)
        {
            eventInstance.getPlaybackState(out var state);
            if (state != PLAYBACK_STATE.PLAYING)
            {
                eventInstance.start();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShotAttached(this EventReference eventReference, Transform target, float volume = 1f)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            var eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstance.set3DAttributes(target.To3DAttributes());
            eventInstance.setVolume(volume);
            eventInstance.start();
            eventInstance.release();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StopAndRelease(this EventInstance eventInstance, STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
        {
            eventInstance.stop(stopMode);
            eventInstance.release();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShot(this EventReference eventReference, Vector3 position = default)
        {
            RuntimeManager.PlayOneShot(eventReference, position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShot<TComponent>(this EventReference eventReference, TComponent component) where TComponent : Component
        {
            RuntimeManager.PlayOneShot(eventReference, component.transform.position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShotIf(this EventReference eventReference, bool condition, Vector3 position = default)
        {
            if (condition)
            {
                RuntimeManager.PlayOneShot(eventReference, position);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShotIf(this EventReference eventReference, Func<bool> condition, Vector3 position = default)
        {
            if (condition())
            {
                RuntimeManager.PlayOneShot(eventReference, position);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShotIfNotNull(this EventReference eventReference)
        {
            if (eventReference.IsNull)
            {
                return;
            }
            RuntimeManager.PlayOneShot(eventReference);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShotAttachedIfNotNull(this EventReference eventReference, Transform target)
        {
            if (eventReference.IsNull)
            {
                return;
            }
            RuntimeManager.PlayOneShotAttached(eventReference.Guid, target.gameObject);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShotAttachedIfNotNull(this EventReference eventReference, Vector3 pos)
        {
            if (eventReference.IsNull)
            {
                return;
            }
            RuntimeManager.PlayOneShot(eventReference.Guid, pos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EventInstance PlayAt(this EventReference eventReference, Transform target)
        {
            if (eventReference.IsNull)
            {
                Debug.LogError("Provided Event Instance for Target " + target + " was null");
                return new EventInstance();
            }
            var eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstance.set3DAttributes(target.To3DAttributes());
            eventInstance.start();
            return eventInstance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EventInstance PlayAttached(this EventReference eventReference, Transform target)
        {
            if (eventReference.IsNull)
            {
                Debug.LogError("Provided Event Instance for Target " + target + " was null");
                return new EventInstance();
            }
            var eventInstance = RuntimeManager.CreateInstance(eventReference);
            RuntimeManager.AttachInstanceToGameObject(eventInstance, target);
            eventInstance.start();
            return eventInstance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EventInstance CreateAttached(this EventReference eventReference, Transform target)
        {
            if (eventReference.IsNull)
            {
                Debug.LogError("Provided Event Instance for Target " + target + " was null");
                return new EventInstance();
            }
            var eventInstance = RuntimeManager.CreateInstance(eventReference);
            RuntimeManager.AttachInstanceToGameObject(eventInstance, target);
            return eventInstance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StopInstance(this EventInstance eventInstance, STOP_MODE stopMode = STOP_MODE.IMMEDIATE)
        {
            if (eventInstance.isValid())
            {
                eventInstance.stop(stopMode);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EventInstance PlayGlobal(this EventReference eventReference)
        {
            var eventInstance = eventReference.CreateGlobalInstance();
            if (eventInstance.isValid())
            {
                eventInstance.start();
            }
            return eventInstance;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EventInstance CreateGlobalInstance(this EventReference eventReference)
        {
            if (eventReference.IsNull)
            {
                Debug.LogError("Provided Event Instance for Target was null");
                return new EventInstance();
            }
            var eventInstance = RuntimeManager.CreateInstance(eventReference);
            return eventInstance;
        }
    }
}
