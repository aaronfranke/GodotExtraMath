#if GODOT
using Godot;
#elif UNITY_5_3_OR_NEWER
using UnityEngine;
#endif
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
    public struct Vector4 : IEquatable<Vector4>
    {
        public enum Axis
        {
            X = 0,
            Y,
            Z,
            W
        }

        public real_t x;
        public real_t y;
        public real_t z;
        public real_t w;

        /// <summary>
        /// Useful for storing and retrieving Direction in DirMag Vector4s and Axis in AxisAngle Vector4s.
        /// </summary>
        public Vector3 XYZ
        {
            get
            {
                return new Vector3(x, y, z);
            }
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }

        public real_t this[int index]
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
            real_t lengthsq = LengthSquared();

            if (lengthsq == 0)
            {
                x = y = z = w = 0f;
            }
            else
            {
                real_t length = Mathf.Sqrt(lengthsq);
                x /= length;
                y /= length;
                z /= length;
                w /= length;
            }
        }

        public Vector4 Abs()
        {
            return new Vector4(Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z), Mathf.Abs(w));
        }

#if GODOT
        public Basis AxisAngleBasis()
        {
            return new Basis(XYZ, w);
        }

        public Quat AxisAngleQuat()
        {
            return new Quat(XYZ, w);
        }

        public static Vector4 AxisAngle(Quat q)
        {
            real_t angle = 2 * Mathf.Acos(q.w);
            real_t den = Mathf.Sqrt(1 - q.w * q.w);
            if (den == 0)
            {
                return new Vector4(0, 0, -1, angle);
            }
            return new Vector4(q.x / den, q.y / den, q.z / den, angle);
        }

        public static Vector4 AxisAngle(Basis b)
        {
            return AxisAngle(b.Quat()); // Might be a more efficient way to do this.
        }
#endif

        public Vector4 Bounce(Vector4 n)
        {
            return -Reflect(n);
        }

        public Vector4 Ceil()
        {
            return new Vector4(Mathf.Ceil(x), Mathf.Ceil(y), Mathf.Ceil(z), Mathf.Ceil(w));
        }

        public Vector4 CubicInterpolate(Vector4 b, Vector4 preA, Vector4 postB, real_t t)
        {
            var p0 = preA;
            var p1 = this;
            var p2 = b;
            var p3 = postB;

            real_t t2 = t * t;
            real_t t3 = t2 * t;

            return 0.5f * (
                        p1 * 2.0f + (-p0 + p2) * t +
                        (2.0f * p0 - 5.0f * p1 + 4f * p2 - p3) * t2 +
                        (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3
                    );
        }

        public Vector4 DirectionTo(Vector4 b)
        {
            return new Vector4(b.x - x, b.y - y, b.z - z, b.w - w).Normalized();
        }

        /// <summary>
        /// Creates a Vector4 that represents the Direction and Magnitude of a Vector3.
        /// </summary>
        public static Vector4 DirMag(Vector3 v)
        {
#if GODOT
            return new Vector4(v.Normalized(), v.Length());
#elif UNITY_5_3_OR_NEWER
            return new Vector4(v.normalized, v.magnitude);
#endif
        }

        /// <summary>
        /// Creates a Vector3 interpreting this Vector4's components as Direction and Magnitude.
        /// </summary>
        public Vector3 DirMag()
        {
            return w * XYZ;
        }

        public real_t DistanceSquaredTo(Vector4 b)
        {
            return (b - this).LengthSquared();
        }

        public real_t DistanceTo(Vector4 b)
        {
            return (b - this).Length();
        }

        public real_t Dot(Vector4 b)
        {
            return x * b.x + y * b.y + z * b.z + w * b.w;
        }

        public Vector4 Floor()
        {
            return new Vector4(Mathf.Floor(x), Mathf.Floor(y), Mathf.Floor(z), Mathf.Floor(w));
        }

        public Vector4 Inverse()
        {
            return new Vector4(1.0f / x, 1.0f / y, 1.0f / z, 1.0f / w);
        }

        public bool IsNormalized()
        {
            return Mathf.Abs(LengthSquared() - 1.0f) < Mathf.Epsilon;
        }

        public real_t Length()
        {
            real_t x2 = x * x;
            real_t y2 = y * y;
            real_t z2 = z * z;
            real_t w2 = w * w;

            return Mathf.Sqrt(x2 + y2 + z2 + w2);
        }

        public real_t LengthSquared()
        {
            real_t x2 = x * x;
            real_t y2 = y * y;
            real_t z2 = z * z;
            real_t w2 = w * w;

            return x2 + y2 + z2 + w2;
        }

        /// <summary>
        /// Returns the result of the linear interpolation between
        /// this vector and `to` by amount `weight`.
        /// </summary>
        /// <param name="to">The destination vector for interpolation.</param>
        /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting vector of the interpolation.</returns>
        public Vector4 Lerp(Vector4 to, real_t weight)
        {
            return new Vector4
            (
                Mathf.Lerp(x, to.x, weight),
                Mathf.Lerp(y, to.y, weight),
                Mathf.Lerp(z, to.z, weight),
                Mathf.Lerp(w, to.w, weight)
            );
        }

        /// <summary>
        /// Returns the result of the linear interpolation between
        /// this vector and `to` by the vector amount `weight`.
        /// </summary>
        /// <param name="to">The destination vector for interpolation.</param>
        /// <param name="weight">A vector with components on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting vector of the interpolation.</returns>
        public Vector4 Lerp(Vector4 to, Vector4 weight)
        {
            return new Vector4
            (
                Mathf.Lerp(x, to.x, weight.x),
                Mathf.Lerp(y, to.y, weight.y),
                Mathf.Lerp(z, to.z, weight.z),
                Mathf.Lerp(w, to.w, weight.w)
            );
        }

        /// <summary>
        /// Returns the result of the linear interpolation between
        /// this vector and `to` by amount `weight`.
        /// </summary>
        /// <param name="to">The destination vector for interpolation.</param>
        /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting vector of the interpolation.</returns>
        public Vector4 LinearInterpolate(Vector4 to, real_t weight)
        {
            return new Vector4
            (
                Mathf.Lerp(x, to.x, weight),
                Mathf.Lerp(y, to.y, weight),
                Mathf.Lerp(z, to.z, weight),
                Mathf.Lerp(w, to.w, weight)
            );
        }

        /// <summary>
        /// Returns the result of the linear interpolation between
        /// this vector and `to` by the vector amount `weight`.
        /// </summary>
        /// <param name="to">The destination vector for interpolation.</param>
        /// <param name="weight">A vector with components on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting vector of the interpolation.</returns>
        public Vector4 LinearInterpolate(Vector4 to, Vector4 weight)
        {
            return new Vector4
            (
                Mathf.Lerp(x, to.x, weight.x),
                Mathf.Lerp(y, to.y, weight.y),
                Mathf.Lerp(z, to.z, weight.z),
                Mathf.Lerp(w, to.w, weight.w)
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

        public Vector4 Normalized()
        {
            var v = this;
            v.Normalize();
            return v;
        }

#if GODOT
        public Vector4 PosMod(real_t mod)
        {
            Vector4 v = this;
            v.x = Mathf.PosMod(v.x, mod);
            v.y = Mathf.PosMod(v.y, mod);
            v.z = Mathf.PosMod(v.z, mod);
            v.w = Mathf.PosMod(v.w, mod);
            return v;
        }

        public Vector4 PosMod(Vector4 modv)
        {
            Vector4 v = this;
            v.x = Mathf.PosMod(v.x, modv.x);
            v.y = Mathf.PosMod(v.y, modv.y);
            v.z = Mathf.PosMod(v.z, modv.z);
            v.w = Mathf.PosMod(v.w, modv.w);
            return v;
        }
#endif

        public Vector4 Project(Vector4 onNormal)
        {
            return onNormal * (Dot(onNormal) / onNormal.LengthSquared());
        }

        public Vector4 Reflect(Vector4 n)
        {
#if DEBUG
            if (!n.IsNormalized())
                throw new ArgumentException(String.Format("{0} is not normalized", n), nameof(n));
#endif
            return 2.0f * n * Dot(n) - this;
        }

#if GODOT
        /// <summary>
        /// Rotates the given Basis, interpreting this Vector4 as AxisAngle.
        /// </summary>
        public Basis Rotated(Basis b)
        {
            return b * new Basis(XYZ, w);
        }
#endif

        public Vector4 Round()
        {
            return new Vector4(Mathf.Round(x), Mathf.Round(y), Mathf.Round(z), Mathf.Round(w));
        }

        public Vector4 Sign()
        {
            Vector4 v = this;
            v.x = Mathf.Sign(v.x);
            v.y = Mathf.Sign(v.y);
            v.z = Mathf.Sign(v.z);
            v.w = Mathf.Sign(v.w);
            return v;
        }

        public Vector4 Slide(Vector4 n)
        {
            return this - n * Dot(n);
        }

#if GODOT
        public Vector4 Snapped(Vector4 by)
        {
            return new Vector4
            (
                Mathf.Stepify(x, by.x),
                Mathf.Stepify(y, by.y),
                Mathf.Stepify(z, by.z),
                Mathf.Stepify(w, by.w)
            );
        }
#endif

        public Vector2[] UnpackVector2()
        {
            Vector2[] arr = new Vector2[2];
            arr[0] = new Vector2(x, y);
            arr[1] = new Vector2(z, w);
            return arr;
        }

        public void UnpackVector2(out Vector2 xy, out Vector2 zw)
        {
            xy = new Vector2(x, y);
            zw = new Vector2(z, w);
        }

        // Constants
        private static readonly Vector4 _zero = new Vector4(0, 0, 0, 0);
        private static readonly Vector4 _one = new Vector4(1, 1, 1, 1);
        private static readonly Vector4 _negOne = new Vector4(-1, -1, -1, -1);
#if GODOT
        private static readonly Vector4 _inf = new Vector4(Mathf.Inf, Mathf.Inf, Mathf.Inf, Mathf.Inf);
#elif UNITY_5_3_OR_NEWER
        private static readonly Vector4 _inf = new Vector4(real_t.PositiveInfinity, real_t.PositiveInfinity, real_t.PositiveInfinity, real_t.PositiveInfinity);
#endif

        private static readonly Vector4 _unitX = new Vector4(1, 0, 0, 0);
        private static readonly Vector4 _unitY = new Vector4(0, 1, 0, 0);
        private static readonly Vector4 _unitZ = new Vector4(0, 0, 1, 0);
        private static readonly Vector4 _unitW = new Vector4(0, 0, 0, 1);

        public static Vector4 Zero { get { return _zero; } }
        public static Vector4 One { get { return _one; } }
        public static Vector4 NegOne { get { return _negOne; } }
        public static Vector4 Inf { get { return _inf; } }

        public static Vector4 UnitX { get { return _unitX; } }
        public static Vector4 UnitY { get { return _unitY; } }
        public static Vector4 UnitZ { get { return _unitZ; } }
        public static Vector4 UnitW { get { return _unitW; } }

        // Constructors
        public Vector4(real_t x, real_t y, real_t z, real_t w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public Vector4(Vector4 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = v.w;
        }
        public Vector4(Vector3 xyz, real_t w)
        {
            x = xyz.x;
            y = xyz.y;
            z = xyz.z;
            this.w = w;
        }
        public Vector4(Vector2 xy, Vector2 zw)
        {
            x = xy.x;
            y = xy.y;
            z = zw.x;
            w = zw.y;
        }

#if UNITY_5_3_OR_NEWER
        public static implicit operator UnityEngine.Vector4(Vector4 value)
        {
            return new UnityEngine.Vector4(value.x, value.y, value.z, value.w);
        }

        public static explicit operator Vector4(UnityEngine.Vector4 value)
        {
            return new Vector4(value.x, value.y, value.z, value.w);
        }
#endif

        public static Vector4 operator +(Vector4 left, Vector4 right)
        {
            left.x += right.x;
            left.y += right.y;
            left.z += right.z;
            left.w += right.w;
            return left;
        }

        public static Vector4 operator -(Vector4 left, Vector4 right)
        {
            left.x -= right.x;
            left.y -= right.y;
            left.z -= right.z;
            left.w -= right.w;
            return left;
        }

        public static Vector4 operator -(Vector4 vec)
        {
            vec.x = -vec.x;
            vec.y = -vec.y;
            vec.z = -vec.z;
            vec.w = -vec.w;
            return vec;
        }

        public static Vector4 operator *(Vector4 vec, real_t scale)
        {
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            vec.w *= scale;
            return vec;
        }

        public static Vector4 operator *(real_t scale, Vector4 vec)
        {
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            vec.w *= scale;
            return vec;
        }

        public static Vector4 operator *(Vector4 left, Vector4 right)
        {
            left.x *= right.x;
            left.y *= right.y;
            left.z *= right.z;
            left.w *= right.w;
            return left;
        }

        public static Vector4 operator /(Vector4 vec, real_t scale)
        {
            vec.x /= scale;
            vec.y /= scale;
            vec.z /= scale;
            vec.w /= scale;
            return vec;
        }

        public static Vector4 operator /(Vector4 left, Vector4 right)
        {
            left.x /= right.x;
            left.y /= right.y;
            left.z /= right.z;
            left.w /= right.w;
            return left;
        }

        public static Vector4 operator %(Vector4 vec, real_t divisor)
        {
            vec.x %= divisor;
            vec.y %= divisor;
            vec.z %= divisor;
            vec.w %= divisor;
            return vec;
        }

        public static Vector4 operator %(Vector4 vec, Vector4 divisorv)
        {
            vec.x %= divisorv.x;
            vec.y %= divisorv.y;
            vec.z %= divisorv.z;
            vec.w %= divisorv.w;
            return vec;
        }

        public static bool operator ==(Vector4 left, Vector4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(Vector4 left, Vector4 right)
        {
#if GODOT
            if (Mathf.IsEqualApprox(left.x, right.x))
            {
                if (Mathf.IsEqualApprox(left.y, right.y))
                {
                    if (Mathf.IsEqualApprox(left.z, right.z))
                    {
                        return left.w < right.w;
                    }
                    return left.z < right.z;
                }
                return left.y < right.y;
            }
#elif UNITY_5_3_OR_NEWER
            if (Mathf.Approximately(left.x, right.x))
            {
                if (Mathf.Approximately(left.y, right.y))
                {
                    if (Mathf.Approximately(left.z, right.z))
                    {
                        return left.w < right.w;
                    }
                    return left.z < right.z;
                }
                return left.y < right.y;
            }
#endif
            return left.x < right.x;
        }

        public static bool operator >(Vector4 left, Vector4 right)
        {
#if GODOT
            if (Mathf.IsEqualApprox(left.x, right.x))
            {
                if (Mathf.IsEqualApprox(left.y, right.y))
                {
                    if (Mathf.IsEqualApprox(left.z, right.z))
                    {
                        return left.w > right.w;
                    }
                    return left.z > right.z;
                }
                return left.y > right.y;
            }
#elif UNITY_5_3_OR_NEWER
            if (Mathf.Approximately(left.x, right.x))
            {
                if (Mathf.Approximately(left.y, right.y))
                {
                    if (Mathf.Approximately(left.z, right.z))
                    {
                        return left.w > right.w;
                    }
                    return left.z > right.z;
                }
                return left.y > right.y;
            }
#endif
            return left.x > right.x;
        }

        public static bool operator <=(Vector4 left, Vector4 right)
        {
#if GODOT
            if (Mathf.IsEqualApprox(left.x, right.x))
            {
                if (Mathf.IsEqualApprox(left.y, right.y))
                {
                    if (Mathf.IsEqualApprox(left.z, right.z))
                    {
                        return left.w <= right.w;
                    }
                    return left.z < right.z;
                }
                return left.y < right.y;
            }
#elif UNITY_5_3_OR_NEWER
            if (Mathf.Approximately(left.x, right.x))
            {
                if (Mathf.Approximately(left.y, right.y))
                {
                    if (Mathf.Approximately(left.z, right.z))
                    {
                        return left.w <= right.w;
                    }
                    return left.z < right.z;
                }
                return left.y < right.y;
            }
#endif
            return left.x < right.x;
        }

        public static bool operator >=(Vector4 left, Vector4 right)
        {
#if GODOT
            if (Mathf.IsEqualApprox(left.x, right.x))
            {
                if (Mathf.IsEqualApprox(left.y, right.y))
                {
                    if (Mathf.IsEqualApprox(left.z, right.z))
                    {
                        return left.w >= right.w;
                    }
                    return left.z > right.z;
                }
                return left.y > right.y;
            }
#elif UNITY_5_3_OR_NEWER
            if (Mathf.Approximately(left.x, right.x))
            {
                if (Mathf.Approximately(left.y, right.y))
                {
                    if (Mathf.Approximately(left.z, right.z))
                    {
                        return left.w >= right.w;
                    }
                    return left.z > right.z;
                }
                return left.y > right.y;
            }
#endif
            return left.x > right.x;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector4)
            {
                return Equals((Vector4)obj);
            }

            return false;
        }

        public bool Equals(Vector4 other)
        {
#if GODOT
            return Mathf.IsEqualApprox(x, other.x) && Mathf.IsEqualApprox(y, other.y) && Mathf.IsEqualApprox(z, other.z) && Mathf.IsEqualApprox(w, other.w);
#elif UNITY_5_3_OR_NEWER
            return Mathf.Approximately(x, other.x) && Mathf.Approximately(y, other.y) && Mathf.Approximately(z, other.z) && Mathf.Approximately(w, other.w);
#endif
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
