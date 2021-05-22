using System;

namespace ExtraMath
{
    public static partial class Mathd
    {
        // Define constants with Decimal precision and cast down to double.

        /// <summary>
        /// The natural number `e`.
        /// </summary>
        public const double E = (double) 2.7182818284590452353602874714M; // 2.718281828459045

        /// <summary>
        /// The square root of 2.
        /// </summary>
        public const double Sqrt2 = (double) 1.4142135623730950488016887242M; // 1.414213562373095

        // If Godot is using single-precision, then we should be lenient with Epsilon comparisons.
        // Any Godot types imported into here would still be limited by the precision of real_t.
        /// <summary>
        /// A very small number used for float comparison with error tolerance.
        /// 1e-06 with single-precision floats, but 1e-14 if `REAL_T_IS_DOUBLE`.
        /// </summary>
#if GODOT_REAL_T_IS_DOUBLE
        public const double Epsilon = 1e-14;
#else
        public const double Epsilon = 1e-06f;
#endif

        public static double Acosh(double x)
        {
#if DEBUG
            if (x < 1.0) throw new ArgumentOutOfRangeException("Acosh failure: " + x + " is less than one.");
#endif
            return Math.Log(x + Math.Sqrt(x * x - 1.0));
        }

        /// <summary>
        /// Returns the amount of digits after the decimal place.
        /// </summary>
        /// <param name="s">The input value.</param>
        /// <returns>The amount of digits.</returns>
        public static int DecimalCount(double s)
        {
            return DecimalCount((decimal)s);
        }

        /// <summary>
        /// Returns the amount of digits after the decimal place.
        /// </summary>
        /// <param name="s">The input <see cref="System.Decimal"/> value.</param>
        /// <returns>The amount of digits.</returns>
        public static int DecimalCount(decimal s)
        {
            return BitConverter.GetBytes(decimal.GetBits(s)[3])[2];
        }

        /// <summary>
        /// Rounds `s` upward (towards positive infinity).
        ///
        /// This is the same as <see cref="Ceil(double)"/>, but returns an `int`.
        /// </summary>
        /// <param name="s">The number to ceil.</param>
        /// <returns>The smallest whole number that is not less than `s`.</returns>
        public static int CeilToInt(double s)
        {
            return (int)Math.Ceiling(s);
        }

        /// <summary>
        /// Rounds `s` downward (towards negative infinity).
        ///
        /// This is the same as <see cref="Floor(double)"/>, but returns an `int`.
        /// </summary>
        /// <param name="s">The number to floor.</param>
        /// <returns>The largest whole number that is not more than `s`.</returns>
        public static int FloorToInt(double s)
        {
            return (int)Math.Floor(s);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int RoundToInt(double s)
        {
            return (int)Math.Round(s);
        }

        /// <summary>
        /// Returns true if `a` and `b` are approximately equal to each other.
        /// The comparison is done using the provided tolerance value.
        /// If you want the tolerance to be calculated for you, use <see cref="IsEqualApprox(double, double)"/>.
        /// </summary>
        /// <param name="a">One of the values.</param>
        /// <param name="b">The other value.</param>
        /// <param name="tolerance">The pre-calculated tolerance value.</param>
        /// <returns>A bool for whether or not the two values are equal.</returns>
        public static bool IsEqualApprox(double a, double b, double tolerance)
        {
            // Check for exact equality first, required to handle "infinity" values.
            if (a == b)
            {
                return true;
            }
            // Then check for approximate equality.
            return Abs(a - b) < tolerance;
        }
    }
}
