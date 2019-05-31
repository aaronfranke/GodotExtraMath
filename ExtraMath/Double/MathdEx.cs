using System;

namespace ExtraMath
{
    public static partial class Mathd
    {
        // Define constants with Decimal precision and cast down to double.

        public const double E = (double) 2.7182818284590452353602874714M; // 2.718281828459045
        public const double Sqrt2 = (double) 1.4142135623730950488016887242M; // 1.414213562373095

        // If Godot is using single-precision, then we should be lenient with Epsilon comparisons.
        // Any Godot types imported into here would still be limited by the precision of real_t.
#if GODOT_REAL_T_IS_DOUBLE
        public const double Epsilon = 1e-14;
#else
        public const double Epsilon = 1e-06f;
#endif

        public static int DecimalCount(double s)
        {
            return DecimalCount((decimal)s);
        }

        public static int DecimalCount(decimal s)
        {
            return BitConverter.GetBytes(decimal.GetBits(s)[3])[2];
        }

        public static int CeilToInt(double s)
        {
            return (int)Math.Ceiling(s);
        }

        public static int FloorToInt(double s)
        {
            return (int)Math.Floor(s);
        }

        public static int RoundToInt(double s)
        {
            return (int)Math.Round(s);
        }

        public static bool IsEqualApprox(double a, double b, double tolerance)
        {
            return Abs(a - b) < tolerance;
        }
    }
}