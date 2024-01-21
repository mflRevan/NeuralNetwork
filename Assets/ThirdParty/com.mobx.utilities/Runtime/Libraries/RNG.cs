using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;
using static UnityEngine.Random;

namespace MobX.Utilities
{
    public static class RNG
    {
        #region Methods

        public static bool Bool()
        {
            return value > .5f;
        }

        public static bool Bool(float percentage)
        {
            return value < percentage * .01f;
        }

        public static int Int(int min = int.MinValue, int max = int.MaxValue)
        {
            return Range(min, max);
        }

        public static long Int64()
        {
            return (long) Range(int.MinValue, int.MaxValue) + Range(int.MinValue, int.MaxValue);
        }

        public static int Int(Range range)
        {
            return Range(range.Start.Value, range.End.Value);
        }

        public static float Float(float min = float.MinValue, float max = float.MaxValue)
        {
            return Range(min, max);
        }

        #endregion


        #region Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomItem<T>(this IList<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source.Count == 1)
            {
                return source[0];
            }

            var randomIndex = Range(0, source.Count);
            return source[randomIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomItemRemove<T>(this IList<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var randomIndex = Range(0, source.Count);
            var item = source[randomIndex];
            source.RemoveAt(randomIndex);
            return item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] RandomItems<T>(this IList<T> source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source.Count < count)
            {
                throw new InvalidOperationException(
                    $"Cannot select {count} random items from a collection with {source.Count} items!");
            }

            var buffer = HashSetPool<T>.Get();

            while (buffer.Count < count)
            {
                buffer.Add(source[Range(0, source.Count)]);
            }

            var selection = buffer.ToArray();
            HashSetPool<T>.Release(buffer);
            return selection;
        }

        #endregion
    }
}