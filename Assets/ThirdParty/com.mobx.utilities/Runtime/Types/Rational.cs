using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace MobX.Utilities.Types
{
    public readonly struct Rational : IEquatable<Rational>, IComparable<Rational>, IEqualityComparer<Rational>
    {
        #region Fields

        public readonly int Numerator;

        public readonly int Denominator;

        public Rational(int num, int den)
        {
            Numerator = num;
            Denominator = den;
        }

        #endregion


        #region Operator (Convertion)

        public static implicit operator float(Rational rational)
        {
            return (float) rational.Numerator / rational.Denominator;
        }

        public static implicit operator double(Rational rational)
        {
            return (double) rational.Numerator / rational.Denominator;
        }

        #endregion


        #region Operator (Calculation)

        // Additions
        public static Rational operator +(Rational a, Rational b) => checked(new(a.Numerator * b.Denominator + a.Denominator * b.Numerator, a.Denominator * b.Denominator));

        public static Rational operator +(Rational a, int b) => checked(new(a.Numerator + a.Denominator * b, a.Denominator));

        public static Rational operator +(int a, Rational b) => checked(new(a * b.Denominator + b.Numerator, b.Denominator));

        // Subtraction
        public static Rational operator -(Rational a, Rational b) => checked(new(a.Numerator * b.Denominator - a.Denominator * b.Numerator, a.Denominator * b.Denominator));

        public static Rational operator -(Rational a, int b) => checked(new(a.Numerator - a.Denominator * b, a.Denominator));

        public static Rational operator -(int a, Rational b) => checked(new(a * b.Denominator - b.Numerator, b.Denominator));

        // Multiplication
        public static Rational operator *(Rational a, Rational b) => checked(new(a.Numerator * b.Numerator, a.Denominator * b.Denominator));

        public static Rational operator *(Rational a, int b) => checked(new(a.Numerator * b, a.Denominator));

        public static Rational operator *(int a, Rational b) => checked(new(b.Numerator * a, b.Denominator));

        // Division
        public static Rational operator /(Rational a, Rational b) =>
            checked(new(a.Numerator * b.Denominator, a.Denominator * b.Numerator));

        public static Rational operator /(Rational a, int b) => checked(new(a.Numerator, a.Denominator * b));

        public static Rational operator /(int a, Rational b) => checked(new(a * b.Denominator, b.Numerator));

        #endregion


        #region String Representation

        [Pure]
        public Rational Simplify()
        {
            var sign = MathF.Sign(Denominator);
            var num = sign * Numerator;
            var den = sign * Denominator;
            var gcd = Gcd(Numerator, Denominator);
            num /= gcd;
            den /= gcd;
            return new Rational(num, den);
        }

        [Pure]
        public (int num, int den) SimplifyDeconstruct()
        {
            var sign = MathF.Sign(Denominator);
            var num = sign * Numerator;
            var den = sign * Denominator;
            var gcd = Gcd(Numerator, Denominator);
            num /= gcd;
            den /= gcd;
            return (num, den);
        }

        private static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                var t = b;
                b = a % b;
                a = t;
            }

            return a;
        }

        public override string ToString()
        {
            var (num, den) = SimplifyDeconstruct();
            return $"{num.ToString()} / {den.ToString()}";
        }

        #endregion


        #region IEquatable IEqualityComparer

        public bool Equals(Rational other)
        {
            return (Numerator, Denominator) == (other.Numerator, other.Denominator);
        }

        public override bool Equals(object obj)
        {
            return obj is Rational other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Numerator, Denominator);
        }

        public bool Equals(Rational x, Rational y)
        {
            return x.Numerator == y.Numerator && x.Denominator == y.Denominator;
        }

        public int GetHashCode(Rational rational)
        {
            return rational.GetHashCode();
        }

        #endregion


        #region IComparable

        public int CompareTo(Rational other)
        {
            var numeratorComparison = Numerator.CompareTo(other.Numerator);
            return numeratorComparison != 0 ? numeratorComparison : Denominator.CompareTo(other.Denominator);
        }

        #endregion
    }
}