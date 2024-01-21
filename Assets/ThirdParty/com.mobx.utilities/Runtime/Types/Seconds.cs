using System;
using UnityEngine;

namespace MobX.Utilities.Types
{
    [Serializable]
    public struct Seconds : IEquatable<Seconds>, IComparable<Seconds>, IComparable, IEquatable<float>, IEquatable<int>
    {
        [SerializeField] private float value;

        public Seconds(float seconds)
        {
            value = seconds;
        }

        public Seconds(int seconds)
        {
            value = seconds;
        }

        public Seconds(TimeSpan seconds)
        {
            value = seconds.Milliseconds * .001f;
        }

        public float Value => value;

        // Implicit conversion to float
        public static implicit operator float(Seconds seconds)
        {
            return seconds.value;
        }

        public static explicit operator int(Seconds seconds)
        {
            return Mathf.RoundToInt(seconds.value);
        }

        public static implicit operator TimeSpan(Seconds seconds)
        {
            return TimeSpan.FromSeconds(seconds.value);
        }

        public static bool operator ==(Seconds lhs, Seconds rhs)
        {
            return Mathf.Approximately(lhs.value, rhs.value);
        }

        public static bool operator !=(Seconds lhs, Seconds rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Seconds))
            {
                return false;
            }

            return this == (Seconds) obj;
        }

        public bool Equals(Seconds other)
        {
            return this == other;
        }

        public bool Equals(float other)
        {
            return Mathf.Approximately(value, other);
        }

        public bool Equals(int other)
        {
            return Mathf.RoundToInt(value) == other;
        }

        public int CompareTo(Seconds other)
        {
            return value.CompareTo(other.value);
        }

        public int CompareTo(object obj)
        {
            if (obj == null || !(obj is Seconds))
            {
                throw new ArgumentException("Object is not a Seconds");
            }

            return CompareTo((Seconds) obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}
