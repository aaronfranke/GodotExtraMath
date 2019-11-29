using System;
using System.Runtime.InteropServices;

#if GODOT_REAL_T_IS_DOUBLE
using real_t = System.Double;
#else
using real_t = System.Single;
#endif

namespace ExtraMath
{
    /// <summary>
    /// 2-element structure that can be used to represent positions in 2D space or any other pair of numeric values.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2d : IEquatable<Vector2d>
    {
        public enum Axis
        {
            X = 0,
            Y
        }

        public double x;
        public double y;

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        return;
                    case 1:
                        y = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        internal void Normalize()
        {
            double lengthsq = LengthSquared();

            if (lengthsq == 0)
            {
                x = y = 0f;
            }
            else
            {
                double length = Mathd.Sqrt(lengthsq);
                x /= length;
                y /= length;
            }
        }

        public double Cross(Vector2d b)
        {
            return x * b.y - y * b.x;
        }

        public Vector2d Abs()
        {
            return new Vector2d(Mathd.Abs(x), Mathd.Abs(y));
        }

        public double Angle()
        {
            return Mathd.Atan2(y, x);
        }

        public double AngleTo(Vector2d to)
        {
            return Mathd.Atan2(Cross(to), Dot(to));
        }

        public double AngleToPoint(Vector2d to)
        {
            return Mathd.Atan2(y - to.y, x - to.x);
        }

        public double Aspect()
        {
            return x / y;
        }

        public Vector2d Bounce(Vector2d n)
        {
            return -Reflect(n);
        }

        public Vector2d Ceil()
        {
            return new Vector2d(Mathd.Ceil(x), Mathd.Ceil(y));
        }

        public Vector2d Clamped(double length)
        {
            var v = this;
            double l = Length();

            if (l > 0 && length < l)
            {
                v /= l;
                v *= length;
            }

            return v;
        }

        public Vector2d CubicInterpolate(Vector2d b, Vector2d preA, Vector2d postB, double t)
        {
            var p0 = preA;
            var p1 = this;
            var p2 = b;
            var p3 = postB;

            double t2 = t * t;
            double t3 = t2 * t;

            return 0.5f * (p1 * 2.0f +
                                (-p0 + p2) * t +
                                (2.0f * p0 - 5.0f * p1 + 4 * p2 - p3) * t2 +
                                (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3);
        }

        public Vector2d DirectionTo(Vector2d b)
        {
            return new Vector2d(b.x - x, b.y - y).Normalized();
        }

        public double DistanceSquaredTo(Vector2d to)
        {
            return (x - to.x) * (x - to.x) + (y - to.y) * (y - to.y);
        }

        public double DistanceTo(Vector2d to)
        {
            return Mathd.Sqrt((x - to.x) * (x - to.x) + (y - to.y) * (y - to.y));
        }

        public double Dot(Vector2d with)
        {
            return x * with.x + y * with.y;
        }

        public Vector2d Floor()
        {
            return new Vector2d(Mathd.Floor(x), Mathd.Floor(y));
        }

        public bool IsNormalized()
        {
            return Mathd.Abs(LengthSquared() - 1.0f) < Mathd.Epsilon;
        }

        public double Length()
        {
            return Mathd.Sqrt(x * x + y * y);
        }

        public double LengthSquared()
        {
            return x * x + y * y;
        }

        public Vector2d LinearInterpolate(Vector2d b, double t)
        {
            var res = this;

            res.x += t * (b.x - x);
            res.y += t * (b.y - y);

            return res;
        }

        public Vector2d Normalized()
        {
            var v = this;
            v.Normalize();
            return v;
        }

        public Vector2d PosMod(double mod)
        {
            Vector2d v = this;
            v.x = Mathd.PosMod(v.x, mod);
            v.y = Mathd.PosMod(v.y, mod);
            return v;
        }

        public Vector2d PosMod(Vector2d modv)
        {
            Vector2d v = this;
            v.x = Mathd.PosMod(v.x, modv.x);
            v.y = Mathd.PosMod(v.y, modv.y);
            return v;
        }

        public Vector2d Project(Vector2d onNormal)
        {
            return onNormal * (Dot(onNormal) / onNormal.LengthSquared());
        }

        public Vector2d Reflect(Vector2d n)
        {
            return 2.0f * n * Dot(n) - this;
        }

        public Vector2d Rotated(double phi)
        {
            double rads = Angle() + phi;
            return new Vector2d(Mathd.Cos(rads), Mathd.Sin(rads)) * Length();
        }

        public Vector2d Round()
        {
            return new Vector2d(Mathd.Round(x), Mathd.Round(y));
        }

        public Vector2d Sign()
        {
            Vector2d v = this;
            v.x = Mathd.Sign(v.x);
            v.y = Mathd.Sign(v.y);
            return v;
        }

        public Vector2d Slerp(Vector2d b, double t)
        {
            double theta = AngleTo(b);
            return Rotated(theta * t);
        }

        public Vector2d Slide(Vector2d n)
        {
            return this - n * Dot(n);
        }

        public Vector2d Snapped(Vector2d by)
        {
            return new Vector2d(Mathd.Stepify(x, by.x), Mathd.Stepify(y, by.y));
        }

        public Vector2d Tangent()
        {
            return new Vector2d(y, -x);
        }

        // Constants
        private static readonly Vector2d _zero = new Vector2d(0, 0);
        private static readonly Vector2d _one = new Vector2d(1, 1);
        private static readonly Vector2d _negOne = new Vector2d(-1, -1);
        private static readonly Vector2d _inf = new Vector2d(Mathd.Inf, Mathd.Inf);

        private static readonly Vector2d _up = new Vector2d(0, -1);
        private static readonly Vector2d _down = new Vector2d(0, 1);
        private static readonly Vector2d _right = new Vector2d(1, 0);
        private static readonly Vector2d _left = new Vector2d(-1, 0);

        public static Vector2d Zero { get { return _zero; } }
        public static Vector2d NegOne { get { return _negOne; } }
        public static Vector2d One { get { return _one; } }
        public static Vector2d Inf { get { return _inf; } }

        public static Vector2d Up { get { return _up; } }
        public static Vector2d Down { get { return _down; } }
        public static Vector2d Right { get { return _right; } }
        public static Vector2d Left { get { return _left; } }

        // Constructors
        public Vector2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2d(Vector2d v)
        {
            x = v.x;
            y = v.y;
        }

        public static explicit operator Godot.Vector2(Vector2d value)
        {
            return new Godot.Vector2((real_t)value.x, (real_t)value.y);
        }

        public static implicit operator Vector2d(Godot.Vector2 value)
        {
            return new Vector2d(value.x, value.y);
        }

        public static explicit operator Vector2i(Vector2d value)
        {
            return new Vector2i((int)value.x, (int)value.y);
        }

        public static implicit operator Vector2d(Vector2i value)
        {
            return new Vector2d(value.x, value.y);
        }

        public static Vector2d operator +(Vector2d left, Vector2d right)
        {
            left.x += right.x;
            left.y += right.y;
            return left;
        }

        public static Vector2d operator -(Vector2d left, Vector2d right)
        {
            left.x -= right.x;
            left.y -= right.y;
            return left;
        }

        public static Vector2d operator -(Vector2d vec)
        {
            vec.x = -vec.x;
            vec.y = -vec.y;
            return vec;
        }

        public static Vector2d operator *(Vector2d vec, double scale)
        {
            vec.x *= scale;
            vec.y *= scale;
            return vec;
        }

        public static Vector2d operator *(double scale, Vector2d vec)
        {
            vec.x *= scale;
            vec.y *= scale;
            return vec;
        }

        public static Vector2d operator *(Vector2d left, Vector2d right)
        {
            left.x *= right.x;
            left.y *= right.y;
            return left;
        }

        public static Vector2d operator /(Vector2d vec, double scale)
        {
            vec.x /= scale;
            vec.y /= scale;
            return vec;
        }

        public static Vector2d operator /(Vector2d left, Vector2d right)
        {
            left.x /= right.x;
            left.y /= right.y;
            return left;
        }

        public static Vector2d operator %(Vector2d vec, double divisor)
        {
            vec.x %= divisor;
            vec.y %= divisor;
            return vec;
        }

        public static Vector2d operator %(Vector2d vec, Vector2d divisorv)
        {
            vec.x %= divisorv.x;
            vec.y %= divisorv.y;
            return vec;
        }

        public static bool operator ==(Vector2d left, Vector2d right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2d left, Vector2d right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(Vector2d left, Vector2d right)
        {
            if (Mathd.IsEqualApprox(left.x, right.x))
            {
                return left.y < right.y;
            }

            return left.x < right.x;
        }

        public static bool operator >(Vector2d left, Vector2d right)
        {
            if (Mathd.IsEqualApprox(left.x, right.x))
            {
                return left.y > right.y;
            }

            return left.x > right.x;
        }

        public static bool operator <=(Vector2d left, Vector2d right)
        {
            if (Mathd.IsEqualApprox(left.x, right.x))
            {
                return left.y <= right.y;
            }

            return left.x <= right.x;
        }

        public static bool operator >=(Vector2d left, Vector2d right)
        {
            if (Mathd.IsEqualApprox(left.x, right.x))
            {
                return left.y >= right.y;
            }

            return left.x >= right.x;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2d)
            {
                return Equals((Vector2d)obj);
            }

            return false;
        }

        public bool Equals(Vector2d other)
        {
            return Mathd.IsEqualApprox(x, other.x) && Mathd.IsEqualApprox(y, other.y);
        }

        public override int GetHashCode()
        {
            return y.GetHashCode() ^ x.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", new object[]
            {
                x.ToString(),
                y.ToString()
            });
        }

        public string ToString(string format)
        {
            return String.Format("({0}, {1})", new object[]
            {
                x.ToString(format),
                y.ToString(format)
            });
        }
    }
}
