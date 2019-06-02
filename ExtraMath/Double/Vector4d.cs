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
    /// 4-element structure that can be used to represent positions in 4D space or any other pair of numeric values.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4d : IEquatable<Vector4d>
    {
        public enum Axis
        {
            X = 0,
            Y,
            Z,
            W
        }

        public double x;
        public double y;
        public double z;
        public double w;

        /// <summary>
        /// Useful for storing and retrieving Direction in DirMag Vector4s and Axis in AxisAngle Vector4s.
        /// </summary>
        public Vector3d XYZ
        {
            get
            {
                return new Vector3d(x, y, z);
            }
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }

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
                    case 3:
                        return w;
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
                    case 3:
                        w = value;
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
                x = y = z = w = 0f;
            }
            else
            {
                double length = Mathd.Sqrt(lengthsq);
                x /= length;
                y /= length;
                z /= length;
                w /= length;
            }
        }

        public Vector4d Abs()
        {
            return new Vector4d(Mathd.Abs(x), Mathd.Abs(y), Mathd.Abs(z), Mathd.Abs(w));
        }

        public Basisd AxisAngleBasis()
        {
            return new Basisd(XYZ, w);
        }

        public Quatd AxisAngleQuat()
        {
            return new Quatd(XYZ, w);
        }

        public static Vector4d AxisAngle(Quatd q)
        {
            double angle = 2 * Mathd.Acos(q.w);
            double den = Mathd.Sqrt(1 - q.w * q.w);
            if (den == 0)
            {
                return new Vector4d(0, 0, -1, angle);
            }
            return new Vector4d(q.x / den, q.y / den, q.z / den, angle);
        }

        public static Vector4d AxisAngle(Basisd b)
        {
            return AxisAngle(b.Quat()); // Might be a more efficient way to do this.
        }

        public Vector4d Bounce(Vector4d n)
        {
            return -Reflect(n);
        }

        public Vector4d Ceil()
        {
            return new Vector4d(Mathd.Ceil(x), Mathd.Ceil(y), Mathd.Ceil(z), Mathd.Ceil(w));
        }

        public Vector4d CubicInterpolate(Vector4d b, Vector4d preA, Vector4d postB, double t)
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

        public Vector4d DirectionTo(Vector4d b)
        {
            return new Vector4d(b.x - x, b.y - y, b.z - z, b.w - w).Normalized();
        }

        public static Vector4d DirMag(Vector3d v)
        {
            return new Vector4d(v.Normalized(), v.Length());
        }

        public Vector3d DirMag()
        {
            return w * XYZ;
        }

        public double DistanceSquaredTo(Vector4d b)
        {
            return (b - this).LengthSquared();
        }

        public double DistanceTo(Vector4d b)
        {
            return (b - this).Length();
        }

        public double Dot(Vector4d b)
        {
            return x * b.x + y * b.y + z * b.z + w * b.w;
        }

        public Vector4d Floor()
        {
            return new Vector4d(Mathd.Floor(x), Mathd.Floor(y), Mathd.Floor(z), Mathd.Floor(w));
        }

        public Vector4d Inverse()
        {
            return new Vector4d(1.0f / x, 1.0f / y, 1.0f / z, 1.0f / w);
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
            double w2 = w * w;

            return Mathd.Sqrt(x2 + y2 + z2 + w2);
        }

        public double LengthSquared()
        {
            double x2 = x * x;
            double y2 = y * y;
            double z2 = z * z;
            double w2 = w * w;

            return x2 + y2 + z2 + w2;
        }

        public Vector4d LinearInterpolate(Vector4d b, double t)
        {
            return new Vector4d
            (
                x + t * (b.x - x),
                y + t * (b.y - y),
                z + t * (b.z - z),
                z + t * (b.z - z)
            );
        }

        public Axis MaxAxis()
        {
            byte index = 0;
            for (byte i = 1; i < 4; i++)
            {
                if (this[i] > this[index])
                {
                    index = i;
                }
            }
            return (Axis)index;
        }

        public Axis MinAxis()
        {
            byte index = 0;
            for (byte i = 1; i < 4; i++)
            {
                if (this[i] < this[index])
                {
                    index = i;
                }
            }
            return (Axis)index;
        }

        public Vector4d Normalized()
        {
            var v = this;
            v.Normalize();
            return v;
        }

        public Vector4d PosMod(double mod)
        {
            Vector4d v = this;
            v.x = Mathd.PosMod(v.x, mod);
            v.y = Mathd.PosMod(v.y, mod);
            v.z = Mathd.PosMod(v.z, mod);
            v.w = Mathd.PosMod(v.w, mod);
            return v;
        }

        public Vector4d PosMod(Vector4d modv)
        {
            Vector4d v = this;
            v.x = Mathd.PosMod(v.x, modv.x);
            v.y = Mathd.PosMod(v.y, modv.y);
            v.z = Mathd.PosMod(v.z, modv.z);
            v.w = Mathd.PosMod(v.w, modv.w);
            return v;
        }

        public Vector4d Project(Vector4d onNormal)
        {
            return onNormal * (Dot(onNormal) / onNormal.LengthSquared());
        }

        public Vector4d Reflect(Vector4d n)
        {
#if DEBUG
            if (!n.IsNormalized())
                throw new ArgumentException(String.Format("{0} is not normalized", n), nameof(n));
#endif
            return 2.0f * n * Dot(n) - this;
        }

        /// <summary>
        /// Rotates the given Basis, interpreting this Vector4 as AxisAngle.
        /// </summary>
        public Basisd Rotated(Basisd b)
        {
            return b * new Basisd(XYZ, w);
        }

        public Vector4d Round()
        {
            return new Vector4d(Mathd.Round(x), Mathd.Round(y), Mathd.Round(z), Mathd.Round(w));
        }

        public Vector4d Sign()
        {
            Vector4d v = this;
            v.x = Mathd.Sign(v.x);
            v.y = Mathd.Sign(v.y);
            v.z = Mathd.Sign(v.z);
            v.w = Mathd.Sign(v.w);
            return v;
        }

        public Vector4d Slide(Vector4d n)
        {
            return this - n * Dot(n);
        }

        public Vector4d Snapped(Vector4d by)
        {
            return new Vector4d
            (
                Mathd.Stepify(x, by.x),
                Mathd.Stepify(y, by.y),
                Mathd.Stepify(z, by.z),
                Mathd.Stepify(w, by.w)
            );
        }

        public Vector2d[] UnpackVector2()
        {
            Vector2d[] arr = new Vector2d[2];
            arr[0] = new Vector2d(x, y);
            arr[1] = new Vector2d(z, w);
            return arr;
        }

        public void UnpackVector2(out Vector2d xy, out Vector2d zw)
        {
            xy = new Vector2d(x, y);
            zw = new Vector2d(z, w);
        }

        // Constants
        private static readonly Vector4d _zero = new Vector4d(0, 0, 0, 0);
        private static readonly Vector4d _one = new Vector4d(1, 1, 1, 1);
        private static readonly Vector4d _negOne = new Vector4d(-1, -1, -1, -1);
        private static readonly Vector4d _inf = new Vector4d(Mathd.Inf, Mathd.Inf, Mathd.Inf, Mathd.Inf);

        private static readonly Vector4d _unitX = new Vector4d(1, 0, 0, 0);
        private static readonly Vector4d _unitY = new Vector4d(0, 1, 0, 0);
        private static readonly Vector4d _unitZ = new Vector4d(0, 0, 1, 0);
        private static readonly Vector4d _unitW = new Vector4d(0, 0, 0, 1);

        public static Vector4d Zero { get { return _zero; } }
        public static Vector4d One { get { return _one; } }
        public static Vector4d NegOne { get { return _negOne; } }
        public static Vector4d Inf { get { return _inf; } }

        public static Vector4d UnitX { get { return _unitX; } }
        public static Vector4d UnitY { get { return _unitY; } }
        public static Vector4d UnitZ { get { return _unitZ; } }
        public static Vector4d UnitW { get { return _unitW; } }

        // Constructors
        public Vector4d(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public Vector4d(Vector4d v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = v.w;
        }
        public Vector4d(Vector3d xyz, double w)
        {
            x = xyz.x;
            y = xyz.y;
            z = xyz.z;
            this.w = w;
        }
        public Vector4d(Vector2d xy, Vector2d zw)
        {
            x = xy.x;
            y = xy.y;
            z = zw.x;
            w = zw.y;
        }

        public static explicit operator Vector4(Vector4d value)
        {
            return new Vector4((real_t)value.x, (real_t)value.y, (real_t)value.z, (real_t)value.w);
        }

        public static implicit operator Vector4d(Vector4 value)
        {
            return new Vector4d(value.x, value.y, value.z, value.w);
        }

        public static explicit operator Vector4i(Vector4d value)
        {
            return new Vector4i((int)value.x, (int)value.y, (int)value.z, (int)value.w);
        }

        public static implicit operator Vector4d(Vector4i value)
        {
            return new Vector4d(value.x, value.y, value.z, value.w);
        }

#if UNITY_5_3_OR_NEWER
        public static explicit operator UnityEngine.Vector4(Vector4d value)
        {
            return new Vector4((real_t)value.x, (real_t)value.y, (real_t)value.z, (real_t)value.w);
        }

        public static implicit operator Vector4d(UnityEngine.Vector4 value)
        {
            return new Vector4d(value.x, value.y, value.z, value.w);
        }
#endif

        public static Vector4d operator +(Vector4d left, Vector4d right)
        {
            left.x += right.x;
            left.y += right.y;
            left.z += right.z;
            left.w += right.w;
            return left;
        }

        public static Vector4d operator -(Vector4d left, Vector4d right)
        {
            left.x -= right.x;
            left.y -= right.y;
            left.z -= right.z;
            left.w -= right.w;
            return left;
        }

        public static Vector4d operator -(Vector4d vec)
        {
            vec.x = -vec.x;
            vec.y = -vec.y;
            vec.z = -vec.z;
            vec.w = -vec.w;
            return vec;
        }

        public static Vector4d operator *(Vector4d vec, double scale)
        {
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            vec.w *= scale;
            return vec;
        }

        public static Vector4d operator *(double scale, Vector4d vec)
        {
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            vec.w *= scale;
            return vec;
        }

        public static Vector4d operator *(Vector4d left, Vector4d right)
        {
            left.x *= right.x;
            left.y *= right.y;
            left.z *= right.z;
            left.w *= right.w;
            return left;
        }

        public static Vector4d operator /(Vector4d vec, double scale)
        {
            vec.x /= scale;
            vec.y /= scale;
            vec.z /= scale;
            vec.w /= scale;
            return vec;
        }

        public static Vector4d operator /(Vector4d left, Vector4d right)
        {
            left.x /= right.x;
            left.y /= right.y;
            left.z /= right.z;
            left.w /= right.w;
            return left;
        }

        public static Vector4d operator %(Vector4d vec, double divisor)
        {
            vec.x %= divisor;
            vec.y %= divisor;
            vec.z %= divisor;
            vec.w %= divisor;
            return vec;
        }

        public static Vector4d operator %(Vector4d vec, Vector4d divisorv)
        {
            vec.x %= divisorv.x;
            vec.y %= divisorv.y;
            vec.z %= divisorv.z;
            vec.w %= divisorv.w;
            return vec;
        }

        public static bool operator ==(Vector4d left, Vector4d right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector4d left, Vector4d right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(Vector4d left, Vector4d right)
        {
            if (Mathd.IsEqualApprox(left.x, right.x))
            {
                if (Mathd.IsEqualApprox(left.y, right.y))
                {
                    if (Mathd.IsEqualApprox(left.z, right.z))
                    {
                        return left.w < right.w;
                    }
                    return left.z < right.z;
                }
                return left.y < right.y;
            }
            return left.x < right.x;
        }

        public static bool operator >(Vector4d left, Vector4d right)
        {
            if (Mathd.IsEqualApprox(left.x, right.x))
            {
                if (Mathd.IsEqualApprox(left.y, right.y))
                {
                    if (Mathd.IsEqualApprox(left.z, right.z))
                    {
                        return left.w > right.w;
                    }
                    return left.z > right.z;
                }
                return left.y > right.y;
            }
            return left.x > right.x;
        }

        public static bool operator <=(Vector4d left, Vector4d right)
        {
            if (Mathd.IsEqualApprox(left.x, right.x))
            {
                if (Mathd.IsEqualApprox(left.y, right.y))
                {
                    if (Mathd.IsEqualApprox(left.z, right.z))
                    {
                        return left.w <= right.w;
                    }
                    return left.z < right.z;
                }
                return left.y < right.y;
            }
            return left.x < right.x;
        }

        public static bool operator >=(Vector4d left, Vector4d right)
        {
            if (Mathd.IsEqualApprox(left.x, right.x))
            {
                if (Mathd.IsEqualApprox(left.y, right.y))
                {
                    if (Mathd.IsEqualApprox(left.z, right.z))
                    {
                        return left.w >= right.w;
                    }
                    return left.z > right.z;
                }
                return left.y > right.y;
            }
            return left.x > right.x;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector4d)
            {
                return Equals((Vector4d)obj);
            }

            return false;
        }

        public bool Equals(Vector4d other)
        {
            return Mathd.IsEqualApprox(x, other.x) && Mathd.IsEqualApprox(y, other.y) && Mathd.IsEqualApprox(z, other.z) && Mathd.IsEqualApprox(w, other.w);
        }

        public override int GetHashCode()
        {
            return y.GetHashCode() ^ x.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2}, {3})", new object[]
            {
                x.ToString(),
                y.ToString(),
                z.ToString(),
                w.ToString()
            });
        }

        public string ToString(string format)
        {
            return String.Format("({0}, {1}, {2}, {3})", new object[]
            {
                x.ToString(format),
                y.ToString(format),
                z.ToString(format),
                w.ToString(format)
            });
        }
    }
}
