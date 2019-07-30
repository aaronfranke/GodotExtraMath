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
    /// 3-element structure that can be used to represent positions in 3D space or any other pair of numeric values.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3d : IEquatable<Vector3d>
    {
        public enum Axis
        {
            X = 0,
            Y,
            Z
        }

        public double x;
        public double y;
        public double z;

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
                    case 2:
                        return z;
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
                    case 2:
                        z = value;
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
                x = y = z = 0f;
            }
            else
            {
                double length = Mathd.Sqrt(lengthsq);
                x /= length;
                y /= length;
                z /= length;
            }
        }

        public Vector3d Abs()
        {
            return new Vector3d(Mathd.Abs(x), Mathd.Abs(y), Mathd.Abs(z));
        }

        public double AngleTo(Vector3d to)
        {
            return Mathd.Atan2(Cross(to).Length(), Dot(to));
        }

        public Vector3d Bounce(Vector3d n)
        {
            return -Reflect(n);
        }

        public Vector3d Ceil()
        {
            return new Vector3d(Mathd.Ceil(x), Mathd.Ceil(y), Mathd.Ceil(z));
        }

        public Vector3d Cross(Vector3d b)
        {
            return new Vector3d
            (
                y * b.z - z * b.y,
                z * b.x - x * b.z,
                x * b.y - y * b.x
            );
        }

        public Vector3d CubicInterpolate(Vector3d b, Vector3d preA, Vector3d postB, double t)
        {
            var p0 = preA;
            var p1 = this;
            var p2 = b;
            var p3 = postB;

            double t2 = t * t;
            double t3 = t2 * t;

            return 0.5f * (
                        p1 * 2.0f + (-p0 + p2) * t +
                        (2.0f * p0 - 5.0f * p1 + 4f * p2 - p3) * t2 +
                        (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3
                    );
        }

        public Vector3d DirectionTo(Vector3d b)
        {
            return new Vector3d(b.x - x, b.y - y, b.z - z).Normalized();
        }

        public double DistanceSquaredTo(Vector3d b)
        {
            return (b - this).LengthSquared();
        }

        public double DistanceTo(Vector3d b)
        {
            return (b - this).Length();
        }

        public double Dot(Vector3d b)
        {
            return x * b.x + y * b.y + z * b.z;
        }

        public Vector3d Floor()
        {
            return new Vector3d(Mathd.Floor(x), Mathd.Floor(y), Mathd.Floor(z));
        }

        public Vector3d Inverse()
        {
            return new Vector3d(1.0f / x, 1.0f / y, 1.0f / z);
        }

        public bool IsNormalized()
        {
            return Mathd.Abs(LengthSquared() - 1.0f) < Mathd.Epsilon;
        }

        public double Length()
        {
            double x2 = x * x;
            double y2 = y * y;
            double z2 = z * z;

            return Mathd.Sqrt(x2 + y2 + z2);
        }

        public double LengthSquared()
        {
            double x2 = x * x;
            double y2 = y * y;
            double z2 = z * z;

            return x2 + y2 + z2;
        }

        public Vector3d LinearInterpolate(Vector3d b, double t)
        {
            return new Vector3d
            (
                x + t * (b.x - x),
                y + t * (b.y - y),
                z + t * (b.z - z)
            );
        }

        public Axis MaxAxis()
        {
            return x < y ? (y < z ? Axis.Z : Axis.Y) : (x < z ? Axis.Z : Axis.X);
        }

        public Axis MinAxis()
        {
            return x < y ? (x < z ? Axis.X : Axis.Z) : (y < z ? Axis.Y : Axis.Z);
        }

        public Vector3d Normalized()
        {
            var v = this;
            v.Normalize();
            return v;
        }

        public Basisd Outer(Vector3d b)
        {
            return new Basisd(
                x * b.x, x * b.y, x * b.z,
                y * b.x, y * b.y, y * b.z,
                z * b.x, z * b.y, z * b.z
            );
        }

        public Vector3d PosMod(double mod)
        {
            Vector3d v = this;
            v.x = Mathd.PosMod(v.x, mod);
            v.y = Mathd.PosMod(v.y, mod);
            v.z = Mathd.PosMod(v.z, mod);
            return v;
        }

        public Vector3d PosMod(Vector3d modv)
        {
            Vector3d v = this;
            v.x = Mathd.PosMod(v.x, modv.x);
            v.y = Mathd.PosMod(v.y, modv.y);
            v.z = Mathd.PosMod(v.z, modv.z);
            return v;
        }

        public Vector3d Project(Vector3d onNormal)
        {
            return onNormal * (Dot(onNormal) / onNormal.LengthSquared());
        }

        public Vector3d Reflect(Vector3d n)
        {
#if DEBUG
            if (!n.IsNormalized())
                throw new ArgumentException(String.Format("{0} is not normalized", n), nameof(n));
#endif
            return 2.0f * n * Dot(n) - this;
        }

        public Vector3d Round()
        {
            return new Vector3d(Mathd.Round(x), Mathd.Round(y), Mathd.Round(z));
        }

        public Vector3d Rotated(Vector3d axis, double phi)
        {
            return new Basisd(axis, phi).Xform(this);
        }

        public Vector3d Sign()
        {
            Vector3d v = this;
            v.x = Mathd.Sign(v.x);
            v.y = Mathd.Sign(v.y);
            v.z = Mathd.Sign(v.z);
            return v;
        }

        public Vector3d Slerp(Vector3d b, double t)
        {
            double theta = AngleTo(b);
            return Rotated(Cross(b), theta * t);
        }

        public Vector3d Slide(Vector3d n)
        {
            return this - n * Dot(n);
        }

        public Vector3d Snapped(Vector3d by)
        {
            return new Vector3d
            (
                Mathd.Stepify(x, by.x),
                Mathd.Stepify(y, by.y),
                Mathd.Stepify(z, by.z)
            );
        }

        public Basisd ToDiagonalMatrix()
        {
            return new Basisd(
                x, 0f, 0f,
                0f, y, 0f,
                0f, 0f, z
            );
        }

        // Constants
        private static readonly Vector3d _zero = new Vector3d(0, 0, 0);
        private static readonly Vector3d _one = new Vector3d(1, 1, 1);
        private static readonly Vector3d _negOne = new Vector3d(-1, -1, -1);
        private static readonly Vector3d _inf = new Vector3d(Mathd.Inf, Mathd.Inf, Mathd.Inf);

        private static readonly Vector3d _up = new Vector3d(0, 1, 0);
        private static readonly Vector3d _down = new Vector3d(0, -1, 0);
        private static readonly Vector3d _right = new Vector3d(1, 0, 0);
        private static readonly Vector3d _left = new Vector3d(-1, 0, 0);
        private static readonly Vector3d _forward = new Vector3d(0, 0, -1);
        private static readonly Vector3d _back = new Vector3d(0, 0, 1);

        public static Vector3d Zero { get { return _zero; } }
        public static Vector3d One { get { return _one; } }
        public static Vector3d NegOne { get { return _negOne; } }
        public static Vector3d Inf { get { return _inf; } }

        public static Vector3d Up { get { return _up; } }
        public static Vector3d Down { get { return _down; } }
        public static Vector3d Right { get { return _right; } }
        public static Vector3d Left { get { return _left; } }
        public static Vector3d Forward { get { return _forward; } }
        public static Vector3d Back { get { return _back; } }

        // Constructors
        public Vector3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3d(Vector3d v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public static explicit operator Godot.Vector3(Vector3d value)
        {
            return new Godot.Vector3((real_t)value.x, (real_t)value.y, (real_t)value.z);
        }

        public static implicit operator Vector3d(Godot.Vector3 value)
        {
            return new Vector3d(value.x, value.y, value.z);
        }

        public static explicit operator Vector3i(Vector3d value)
        {
            return new Vector3i((int)value.x, (int)value.y, (int)value.z);
        }

        public static implicit operator Vector3d(Vector3i value)
        {
            return new Vector3d(value.x, value.y, value.z);
        }

        public static Vector3d operator +(Vector3d left, Vector3d right)
        {
            left.x += right.x;
            left.y += right.y;
            left.z += right.z;
            return left;
        }

        public static Vector3d operator -(Vector3d left, Vector3d right)
        {
            left.x -= right.x;
            left.y -= right.y;
            left.z -= right.z;
            return left;
        }

        public static Vector3d operator -(Vector3d vec)
        {
            vec.x = -vec.x;
            vec.y = -vec.y;
            vec.z = -vec.z;
            return vec;
        }

        public static Vector3d operator *(Vector3d vec, double scale)
        {
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            return vec;
        }

        public static Vector3d operator *(double scale, Vector3d vec)
        {
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            return vec;
        }

        public static Vector3d operator *(Vector3d left, Vector3d right)
        {
            left.x *= right.x;
            left.y *= right.y;
            left.z *= right.z;
            return left;
        }

        public static Vector3d operator /(Vector3d vec, double scale)
        {
            vec.x /= scale;
            vec.y /= scale;
            vec.z /= scale;
            return vec;
        }

        public static Vector3d operator /(Vector3d left, Vector3d right)
        {
            left.x /= right.x;
            left.y /= right.y;
            left.z /= right.z;
            return left;
        }

        public static Vector3d operator %(Vector3d vec, double divisor)
        {
            vec.x %= divisor;
            vec.y %= divisor;
            vec.z %= divisor;
            return vec;
        }

        public static Vector3d operator %(Vector3d vec, Vector3d divisorv)
        {
            vec.x %= divisorv.x;
            vec.y %= divisorv.y;
            vec.z %= divisorv.z;
            return vec;
        }

        public static bool operator ==(Vector3d left, Vector3d right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3d left, Vector3d right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(Vector3d left, Vector3d right)
        {
            if (Mathd.IsEqualApprox(left.x, right.x))
            {
                if (Mathd.IsEqualApprox(left.y, right.y))
                    return left.z < right.z;
                return left.y < right.y;
            }

            return left.x < right.x;
        }

        public static bool operator >(Vector3d left, Vector3d right)
        {
            if (Mathd.IsEqualApprox(left.x, right.x))
            {
                if (Mathd.IsEqualApprox(left.y, right.y))
                    return left.z > right.z;
                return left.y > right.y;
            }

            return left.x > right.x;
        }

        public static bool operator <=(Vector3d left, Vector3d right)
        {
            if (Mathd.IsEqualApprox(left.x, right.x))
            {
                if (Mathd.IsEqualApprox(left.y, right.y))
                    return left.z <= right.z;
                return left.y < right.y;
            }

            return left.x < right.x;
        }

        public static bool operator >=(Vector3d left, Vector3d right)
        {
            if (Mathd.IsEqualApprox(left.x, right.x))
            {
                if (Mathd.IsEqualApprox(left.y, right.y))
                    return left.z >= right.z;
                return left.y > right.y;
            }

            return left.x > right.x;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3d)
            {
                return Equals((Vector3d)obj);
            }

            return false;
        }

        public bool Equals(Vector3d other)
        {
            return Mathd.IsEqualApprox(x, other.x) && Mathd.IsEqualApprox(y, other.y) && Mathd.IsEqualApprox(z, other.z);
        }

        public override int GetHashCode()
        {
            return y.GetHashCode() ^ x.GetHashCode() ^ z.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", new object[]
            {
                x.ToString(),
                y.ToString(),
                z.ToString()
            });
        }

        public string ToString(string format)
        {
            return String.Format("({0}, {1}, {2})", new object[]
            {
                x.ToString(format),
                y.ToString(format),
                z.ToString(format)
            });
        }
    }
}
