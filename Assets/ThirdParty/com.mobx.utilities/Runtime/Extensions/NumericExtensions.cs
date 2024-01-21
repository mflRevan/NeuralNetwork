using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace MobX.Utilities
{
    public static class NumberExtensions
    {
        #region Math

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(this int origin, int min, int max)
        {
            return Mathf.Clamp(origin, min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(this int origin, int min)
        {
            return origin < min ? min : origin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(this int origin, int max)
        {
            return origin > max ? max : origin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEven(this int n)
        {
            return (n ^ 1) == n + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(this int n)
        {
            return (n ^ 1) != n + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBinarySequenceExcludeZero(this int n)
        {
            return n > 0 && (n & n - 1) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBinarySequence(this int n)
        {
            return (n & n - 1) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CheckNan(this float value, float fallback)
        {
            return float.IsNaN(value) ? fallback : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WithMaxLimit(this int value, int limit)
        {
            return Mathf.Min(value, limit);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WithMaxLimit(this float value, float limit)
        {
            return Mathf.Min(value, limit);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WithMinLimit(this int value, int min)
        {
            return Mathf.Max(value, min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WithMinLimit(this float value, float min)
        {
            return Mathf.Max(value, min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(this float value)
        {
            return Mathf.CeilToInt(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(this float value)
        {
            return Mathf.FloorToInt(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfinity(this float value)
        {
            return float.IsInfinity(value);
        }

        public static bool IsInRange(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        #endregion


        #region Approximately Equals

        public static bool ApproximatelyEquals(this double rhs, double lhs, double acceptableDifference)
        {
            return Math.Abs(rhs - lhs) <= acceptableDifference;
        }

        public static bool ApproximatelyEquals(this float rhs, float lhs, float acceptableDifference)
        {
            return Math.Abs(rhs - lhs) <= acceptableDifference;
        }

        public static bool ApproximatelyEquals(this int rhs, int lhs, int acceptableDifference)
        {
            return Math.Abs(rhs - lhs) <= acceptableDifference;
        }

        #endregion


        #region Vector

        public static Vector2 Clamp(this Vector2 target, Vector2 min, Vector2 max)
        {
            return new Vector2(
                Mathf.Clamp(target.x, min.x, max.x),
                Mathf.Clamp(target.y, min.y, max.y));
        }

        public static Vector2 Min(this Vector2 target, Vector2 other)
        {
            return Vector2.Min(target, other);
        }

        public static Vector2 Max(this Vector2 target, Vector2 other)
        {
            return Vector2.Max(target, other);
        }

        public static bool HasNan(this Vector3 vector3)
        {
            return float.IsNaN(vector3.x) || float.IsNaN(vector3.y) || float.IsNaN(vector3.z);
        }

        #endregion


        #region Quaternion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Quaternion quaternion)
        {
            return new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        #endregion


        #region Timer

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MillisecondsToSeconds(this int milliseconds)
        {
            return milliseconds * 0.001f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SecondsToMilliseconds(this float seconds)
        {
            return (int)(seconds * 1000);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SecondsToMilliseconds(this int seconds)
        {
            return seconds * 1000;
        }

        #endregion


        #region Conversion

        public static float3 ToFloat3(this Vector3 vector3)
        {
            return new float3(vector3.x, vector3.y, vector3.z);
        }

        #endregion
    }
}
