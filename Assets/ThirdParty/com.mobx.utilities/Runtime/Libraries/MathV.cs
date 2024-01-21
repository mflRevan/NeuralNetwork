using UnityEngine;

namespace MobX.Utilities
{
    /// <summary>
    ///     Class containing the vector equivalent operations of <see cref="Mathf" />
    /// </summary>
    public struct MathV
    {
        public static Vector3 Abs(Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        public static Vector3 Sqrt(Vector3 v)
        {
            return new Vector3(Mathf.Sqrt(v.x), Mathf.Sqrt(v.y), Mathf.Sqrt(v.z));
        }

        public static Vector3 Sqr(Vector3 v)
        {
            return new Vector3(v.x * v.x, v.y * v.y, v.z * v.z);
        }

        public static Vector3 Round(Vector3 v)
        {
            return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
        }

        public static Vector3Int RoundToInt(Vector3 v)
        {
            return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }

        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            return new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
        }

        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
        }

        public static Vector3 Max(Vector3 a, float b)
        {
            return new Vector3(Mathf.Max(a.x, b), Mathf.Max(a.y, b), Mathf.Max(a.z, b));
        }

        public static float Max(Vector3 v)
        {
            return Mathf.Max(v.x, Mathf.Max(v.y, v.z));
        }

        public static Vector2 Clamp(Vector2 v, Vector2 min, Vector2 max)
        {
            return new Vector2(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));
        }

        public static Vector3 Clamp(Vector3 v, Vector3 min, Vector3 max)
        {
            return new Vector3(Mathf.Clamp(v.x, min.x, max.x),
                Mathf.Clamp(v.y, min.y, max.y), Mathf.Clamp(v.z, min.z, max.z));
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, Vector3 t)
        {
            return new Vector3(Mathf.Lerp(a.x, b.x, t.x), Mathf.Lerp(a.y, b.y, t.y), Mathf.Lerp(a.z, b.z, t.z));
        }

        public static Vector3 Random(Vector3 range)
        {
            var randomX = RNG.Float(0, range.x);
            var randomY = RNG.Float(0, range.y);
            var randomZ = RNG.Float(0, range.z);
            return new Vector3(randomX, randomY, randomZ);
        }

        public static (float, float) MinMax(float a, float b)
        {
            return a <= b ? (a, b) : (b, a);
        }

        public static float Sqr(float v)
        {
            return v * v;
        }

        /// <summary>
        ///     Quaternion equivalent for Vector3.SmoothDamp
        /// </summary>
        public static Quaternion QuaternionSmoothDamp(Quaternion current, Quaternion target, ref float currentAngularSpeed, float smoothTime)
        {
            var delta = Quaternion.Angle(current, target);
            if (delta > 0.0f)
            {
                var t = Mathf.SmoothDampAngle(delta, 0.0f, ref currentAngularSpeed, smoothTime);
                t = 1.0f - t / delta;
                return Quaternion.Slerp(current, target, t);
            }

            return target;
        }

        /// <summary>
        ///     Decomposes a vector into a normalized direction and its magnitude
        /// </summary>
        /// <param name="vector">The vector to decompose</param>
        /// <returns>(vector.normalized, vector.magnitude) equivalent</returns>
        public static (Vector3, float) Decompose(Vector3 vector)
        {
            var mag = vector.magnitude;
            return mag > Vector3.kEpsilon ? (vector / mag, mag) : (Vector3.zero, 0f);
        }

        /// <summary>
        ///     Clamps a quaternion to specific min max euler angles in ranges of [-180, 180]
        /// </summary>
        /// <param name="val">The rotation to clamp</param>
        /// <param name="minAngles">Values in Range [-180, 180]</param>
        /// <param name="maxAngles">Values in Range [-180, 180]</param>
        /// <returns>The clamped rotation</returns>
        public static Quaternion ClampAngles(Quaternion val, Vector3 minAngles, Vector3 maxAngles)
        {
            Vector3 eulers = val.eulerAngles;
            for (var i = 0; i < 3; i++)
            {
                if (eulers[i] > 180f)
                {
                    eulers[i] -= 360f;
                }
            }
            return Quaternion.Euler(Clamp(eulers, minAngles, maxAngles));
        }

        public static Vector3 GetClosestPointOnLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            // calculate coordinates of the point on the line segment
            Vector3 dir = lineEnd - lineStart;
            var t = Vector3.Dot(point - lineStart, dir) / dir.sqrMagnitude;
            // clamp to segment
            t = Mathf.Clamp01(t);
            // return point on segment
            return lineStart + t * dir;
        }

        public static float GetClosestSqrDistanceToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd, out Vector3 projectedPoint)
        {
            projectedPoint = GetClosestPointOnLineSegment(point, lineStart, lineEnd);
            var result = projectedPoint.SqrDist(point);
            if (float.IsNaN(result))
            {
                Debug.LogError($"result of Distance to Line was NaN with p:{point} start:{lineStart} end:{lineEnd}");
            }
            return result;
        }
    }
}
