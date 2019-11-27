using System;

namespace ExtraMath
{
    public static partial class Mathd
    {
        // Define constants with Decimal precision and cast down to double.

        public const double Tau = (double) 6.2831853071795864769252867666M; // 6.28318530717959
        public const double Pi = (double) 3.1415926535897932384626433833M; // 3.14159265358979
        public const double Inf = double.PositiveInfinity;
        public const double NaN = double.NaN;

        private const double Deg2RadConst = (double) 0.0174532925199432957692369077M; // 0.0174532925199433
        private const double Rad2DegConst = (double) 57.295779513082320876798154814M; // 57.2957795130823

        // Yes, I know many of these seem pointless, but it's for consistency with Mathf.
        public static double Abs(double s)
        {
            return Math.Abs(s);
        }

        public static int Abs(int s)
        {
            return Math.Abs(s);
        }

        public static double Acos(double s)
        {
            return Math.Acos(s);
        }

        public static double Asin(double s)
        {
            return Math.Asin(s);
        }

        public static double Atan(double s)
        {
            return Math.Atan(s);
        }

        public static double Atan2(double y, double x)
        {
            return Math.Atan2(y, x);
        }

        public static Vector2d Cartesian2Polar(double x, double y)
        {
            return new Vector2d(Sqrt(x * x + y * y), Atan2(y, x));
        }

        public static double Ceil(double s)
        {
            return Math.Ceiling(s);
        }

        public static int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static double Clamp(double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static double Cos(double s)
        {
            return Math.Cos(s);
        }

        public static double Cosh(double s)
        {
            return Math.Cosh(s);
        }

        public static double Deg2Rad(double deg)
        {
            return deg * Deg2RadConst;
        }

        public static double Ease(double s, double curve)
        {
            if (s < 0f)
            {
                s = 0f;
            }
            else if (s > 1.0f)
            {
                s = 1.0f;
            }

            if (curve > 0f)
            {
                if (curve < 1.0f)
                {
                    return 1.0f - Pow(1.0f - s, 1.0f / curve);
                }

                return Pow(s, curve);
            }

            if (curve < 0f)
            {
                if (s < 0.5f)
                {
                    return Pow(s * 2.0f, -curve) * 0.5f;
                }

                return (1.0f - Pow(1.0f - (s - 0.5f) * 2.0f, -curve)) * 0.5f + 0.5f;
            }

            return 0f;
        }

        public static double Exp(double s)
        {
            return Math.Exp(s);
        }

        public static double Floor(double s)
        {
            return Math.Floor(s);
        }

        public static double InverseLerp(double from, double to, double weight)
        {
            return (weight - from) / (to - from);
        }

        public static bool IsEqualApprox(double a, double b)
        {
            // Check for exact equality first, required to handle "infinity" values.
            if (a == b)
            {
                return true;
            }
            // Then check for approximate equality.
            double tolerance = Epsilon * Abs(a);
            if (tolerance < Epsilon)
            {
                tolerance = Epsilon;
            }
            return Abs(a - b) < tolerance;
        }

        public static bool IsInf(double s)
        {
            return double.IsInfinity(s);
        }

        public static bool IsNaN(double s)
        {
            return double.IsNaN(s);
        }

        public static bool IsZeroApprox(double s)
        {
            return Abs(s) < Epsilon;
        }

        public static double Lerp(double from, double to, double weight)
        {
            return from + (to - from) * weight;
        }

        public static double LerpAngle(double from, double to, double weight)
        {
            double difference = (to - from) % Mathd.Tau;
            double distance = ((2 * difference) % Mathd.Tau) - difference;
            return from + distance * weight;
        }

        public static double Log(double s)
        {
            return Math.Log(s);
        }

        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        public static double Max(double a, double b)
        {
            return a > b ? a : b;
        }

        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        public static double Min(double a, double b)
        {
            return a < b ? a : b;
        }

        public static double MoveToward(double from, double to, double delta)
        {
            return Abs(to - from) <= delta ? to : from + Sign(to - from) * delta;
        }

        public static int NearestPo2(int value)
        {
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;
            return value;
        }

        public static Vector2d Polar2Cartesian(double r, double th)
        {
            return new Vector2d(r * Cos(th), r * Sin(th));
        }

        /// <summary>
        /// Performs a canonical Modulus operation, where the output is on the range [0, b).
        /// </summary>
        public static double PosMod(double a, double b)
        {
            double c = a % b;
            if ((c < 0 && b > 0) || (c > 0 && b < 0))
            {
                c += b;
            }
            return c;
        }

        /// <summary>
        /// Performs a canonical Modulus operation, where the output is on the range [0, b).
        /// </summary>
        public static int PosMod(int a, int b)
        {
            int c = a % b;
            if ((c < 0 && b > 0) || (c > 0 && b < 0))
            {
                c += b;
            }
            return c;
        }

        public static double Pow(double x, double y)
        {
            return Math.Pow(x, y);
        }

        public static double Rad2Deg(double rad)
        {
            return rad * Rad2DegConst;
        }

        public static double Round(double s)
        {
            return Math.Round(s);
        }

        public static int Sign(int s)
        {
            return s < 0 ? -1 : 1;
        }

        public static double Sign(double s)
        {
            return s < 0f ? -1f : 1f;
        }

        public static double Sin(double s)
        {
            return Math.Sin(s);
        }

        public static double Sinh(double s)
        {
            return Math.Sinh(s);
        }

        public static double SmoothStep(double from, double to, double weight)
        {
            if (IsEqualApprox(from, to))
            {
                return from;
            }
            double x = Clamp((weight - from) / (to - from), 0.0, 1.0);
            return x * x * (3 - 2 * x);
        }

        public static int StepDecimals(double step)
        {
            double[] sd = new double[] {
                0.9999,
                0.09999,
                0.009999,
                0.0009999,
                0.00009999,
                0.000009999,
                0.0000009999,
                0.00000009999,
                0.000000009999,
                0.0000000009999,
                0.00000000009999,
                0.000000000009999,
                0.0000000000009999,
                0.00000000000009999,
                0.000000000000009999,
            };
            double abs = Mathd.Abs(step);
            double decs = abs - (int)abs; // Strip away integer part
            for (int i = 0; i < sd.Length; i++)
            {
                if (decs >= sd[i])
                {
                    return i;
                }
            }
            return 0;
        }

        public static double Sqrt(double s)
        {
            return Math.Sqrt(s);
        }

        public static double Stepify(double s, double step)
        {
            if (step != 0f)
            {
                s = Floor(s / step + 0.5f) * step;
            }

            return s;
        }

        public static double Tan(double s)
        {
            return Math.Tan(s);
        }

        public static double Tanh(double s)
        {
            return Math.Tanh(s);
        }

        public static int Wrap(int value, int min, int max)
        {
            int rng = max - min;
            return rng != 0 ? min + ((value - min) % rng + rng) % rng : min;
        }

        public static double Wrap(double value, double min, double max)
        {
            double rng = max - min;
            return !IsEqualApprox(rng, default(double)) ? min + ((value - min) % rng + rng) % rng : min;
        }
    }
}
