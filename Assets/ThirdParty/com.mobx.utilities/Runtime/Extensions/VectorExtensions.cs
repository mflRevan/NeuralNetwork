using JetBrains.Annotations;
using UnityEngine;

namespace MobX.Utilities
{
    /// <summary>
    /// Functionality extensions on the Vector class
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// Returns a copy of this color with modified values for the given components
        /// </summary>
        /// <param name="original">this specifies the class to apply the extension method to</param>
        /// <param name="r">The new r value</param>
        /// <param name="g">The new g value</param>
        /// <param name="b">The new b value</param>
        /// <param name="a">The new a value</param>
        /// <returns>The modified color</returns>
        [MustUseReturnValue("A modified copy of the vector is returned, you need to assign it.")]
        public static Color With(this Color original, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            return new Color(r ?? original.r, g ?? original.g, b ?? original.b, a ?? original.a);
        }

        /// <summary>
        /// Returns a copy of this vector with modified values for the given components
        /// </summary>
        /// <param name="original">this specifies the class to apply the extension method to</param>
        /// <param name="x">The new x value</param>
        /// <param name="y">The new y value</param>
        /// <param name="z">The new z value</param>
        /// <returns>The modified vector</returns>
        [MustUseReturnValue("A modified copy of the vector is returned, you need to assign it.")]
        public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? original.x, y ?? original.y, z ?? original.z);
        }

        /// <summary>
        /// Adds the given components to this vector
        /// </summary>
        /// <param name="v">this specifies the class to apply the extension method to</param>
        /// <param name="x">The x value to add</param>
        /// <param name="y">The y value to add</param>
        /// <param name="z">The z value to add</param>
        /// <returns>Self for chaining</returns>
        [MustUseReturnValue("A modified copy of the vector is returned, you need to assign it.")]
        public static Vector3 Add(this Vector3 v, float? x = null, float? y = null, float? z = null)
        {
            if (x.HasValue)
            {
                v.x += x.Value;
            }

            if (y.HasValue)
            {
                v.y += y.Value;
            }

            if (z.HasValue)
            {
                v.z += z.Value;
            }

            return v;
        }

        /// <summary>
        /// Returns the the given vector <paramref name="vector2"/> rotated by the given amount of <paramref name="degrees"/>
        /// </summary>
        public static Vector2 Rotate(this Vector2 vector2, float degrees)
        {
            var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            var tx = vector2.x;
            var ty = vector2.y;
            vector2.x = cos * tx - sin * ty;
            vector2.y = sin * tx + cos * ty;
            return vector2;
        }

        public static Vector3 Div(this Vector3 a, Vector3 b)
        {
            var res = new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
            if (float.IsNaN(res.x))
            {
                res.x = 0f;
            }

            if (float.IsNaN(res.y))
            {
                res.y = 0f;
            }

            if (float.IsNaN(res.z))
            {
                res.z = 0f;
            }

            return res;
        }

        public static Vector3 Mul(this Vector3 vectorLeft, Vector3 vectorRight)
        {
            return new Vector3(vectorLeft.x * vectorRight.x, vectorLeft.y * vectorRight.y, vectorLeft.z * vectorRight.z);
        }

        /// <summary>
        /// Returns the squared distance between this and the other vector
        /// </summary>
        public static float SqrDist(this Vector3 vectorLeft, Vector3 vectorRight)
        {
            return (vectorLeft - vectorRight).sqrMagnitude;
        }

        /// <summary>
        /// Returns the x and y coordinates as <see cref="Vector2"/>
        /// </summary>
        public static Vector2 XY(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        /// <summary>
        /// Returns the x and z coordinates as <see cref="Vector2"/>
        /// </summary>
        public static Vector2 XZ(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        /// <summary>
        /// Returns the x and y coordinates as x and z coordinates of a <see cref="Vector3"/>, with a new y defaulting to 0
        /// </summary>
        public static Vector3 X0Y(this Vector2 vector, float newY = 0f)
        {
            return new Vector3(vector.x, newY, vector.y);
        }

        public static Vector3 XYX(this Vector2 v)
        {
            return new Vector3(v.x, v.y, v.x);
        }

        public static bool Approximately(this Vector3 vectorLeft, Vector3 vectorRight)
        {
            return Mathf.Approximately(vectorLeft.x, vectorRight.x) &&
                   Mathf.Approximately(vectorLeft.y, vectorRight.y) &&
                   Mathf.Approximately(vectorLeft.z, vectorRight.z);
        }
    }
}