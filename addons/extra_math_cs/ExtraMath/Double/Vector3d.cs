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
        /// <summary>
        /// Enumerated index values for the axes.
        /// Returned by <see cref="MaxAxis"/> and <see cref="MinAxis"/>.
        /// </summary>
        public enum Axis
        {
            X = 0,
            Y,
            Z
        }

        /// <summary>
        /// The vector's X component. Also accessible by using the index position `[0]`.
        /// </summary>
        public double x;
        /// <summary>
        /// The vector's Y component. Also accessible by using the index position `[1]`.
        /// </summary>
        public double y;
        /// <summary>
        /// The vector's Z component. Also accessible by using the index position `[2]`.
        /// </summary>
        public double z;

        /// <summary>
        /// Access vector components using their index.
        /// </summary>
        /// <value>`[0]` is equivalent to `.x`, `[1]` is equivalent to `.y`, `[2]` is equivalent to `.z`.</value>
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

        /// <summary>
        /// Returns a new vector with all components in absolute values (i.e. positive).
        /// </summary>
        /// <returns>A vector with <see cref="Mathd.Abs(double)"/> called on each component.</returns>
        public Vector3d Abs()
        {
            return new Vector3d(Mathd.Abs(x), Mathd.Abs(y), Mathd.Abs(z));
        }

        /// <summary>
        /// Returns the unsigned minimum angle to the given vector, in radians.
        /// </summary>
        /// <param name="to">The other vector to compare this vector to.</param>
        /// <returns>The unsigned angle between the two vectors, in radians.</returns>
        public double AngleTo(Vector3d to)
        {
            return Mathd.Atan2(Cross(to).Length(), Dot(to));
        }

        /// <summary>
        /// Returns this vector "bounced off" from a plane defined by the given normal.
        /// </summary>
        /// <param name="normal">The normal vector defining the plane to bounce off. Must be normalized.</param>
        /// <returns>The bounced vector.</returns>
        public Vector3d Bounce(Vector3d normal)
        {
            return -Reflect(normal);
        }

        /// <summary>
        /// Returns a new vector with all components rounded up (towards positive infinity).
        /// </summary>
        /// <returns>A vector with <see cref="Mathd.Ceil"/> called on each component.</returns>
        public Vector3d Ceil()
        {
            return new Vector3d(Mathd.Ceil(x), Mathd.Ceil(y), Mathd.Ceil(z));
        }

        /// <summary>
        /// Returns the cross product of this vector and `b`.
        /// </summary>
        /// <param name="b">The other vector.</param>
        /// <returns>The cross product vector.</returns>
        public Vector3d Cross(Vector3d b)
        {
            return new Vector3d
            (
                y * b.z - z * b.y,
                z * b.x - x * b.z,
                x * b.y - y * b.x
            );
        }

        /// <summary>
        /// Performs a cubic interpolation between vectors `preA`, this vector,
        /// `b`, and `postB`, by the given amount `t`.
        /// </summary>
        /// <param name="b">The destination vector.</param>
        /// <param name="preA">A vector before this vector.</param>
        /// <param name="postB">A vector after `b`.</param>
        /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The interpolated vector.</returns>
        public Vector3d CubicInterpolate(Vector3d b, Vector3d preA, Vector3d postB, double weight)
        {
            Vector3d p0 = preA;
            Vector3d p1 = this;
            Vector3d p2 = b;
            Vector3d p3 = postB;

            double t = weight;
            double t2 = t * t;
            double t3 = t2 * t;

            return 0.5f * (
                        p1 * 2.0f + (-p0 + p2) * t +
                        (2.0f * p0 - 5.0f * p1 + 4f * p2 - p3) * t2 +
                        (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3
                    );
        }

        /// <summary>
        /// Returns the normalized vector pointing from this vector to `b`.
        /// </summary>
        /// <param name="b">The other vector to point towards.</param>
        /// <returns>The direction from this vector to `b`.</returns>
        public Vector3d DirectionTo(Vector3d b)
        {
            return new Vector3d(b.x - x, b.y - y, b.z - z).Normalized();
        }

        /// <summary>
        /// Returns the squared distance between this vector and `b`.
        /// This method runs faster than <see cref="DistanceTo"/>, so prefer it if
        /// you need to compare vectors or need the squared distance for some formula.
        /// </summary>
        /// <param name="b">The other vector to use.</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public double DistanceSquaredTo(Vector3d b)
        {
            return (b - this).LengthSquared();
        }

        /// <summary>
        /// Returns the distance between this vector and `b`.
        /// </summary>
        /// <param name="b">The other vector to use.</param>
        /// <returns>The distance between the two vectors.</returns>
        public double DistanceTo(Vector3d b)
        {
            return (b - this).Length();
        }

        /// <summary>
        /// Returns the dot product of this vector and `b`.
        /// </summary>
        /// <param name="b">The other vector to use.</param>
        /// <returns>The dot product of the two vectors.</returns>
        public double Dot(Vector3d b)
        {
            return x * b.x + y * b.y + z * b.z;
        }

        /// <summary>
        /// Returns a new vector with all components rounded down (towards negative infinity).
        /// </summary>
        /// <returns>A vector with <see cref="Mathd.Floor"/> called on each component.</returns>
        public Vector3d Floor()
        {
            return new Vector3d(Mathd.Floor(x), Mathd.Floor(y), Mathd.Floor(z));
        }

        /// <summary>
        /// Returns the inverse of this vector. This is the same as `new Vector3d(1 / v.x, 1 / v.y, 1 / v.z)`.
        /// </summary>
        /// <returns>The inverse of this vector.</returns>
        public Vector3d Inverse()
        {
            return new Vector3d(1 / x, 1 / y, 1 / z);
        }

        /// <summary>
        /// Returns true if the vector is normalized, and false otherwise.
        /// </summary>
        /// <returns>A bool indicating whether or not the vector is normalized.</returns>
        public bool IsNormalized()
        {
            return Mathd.Abs(LengthSquared() - 1.0f) < Mathd.Epsilon;
        }

        /// <summary>
        /// Returns the length (magnitude) of this vector.
        /// </summary>
        /// <returns>The length of this vector.</returns>
        public double Length()
        {
            double x2 = x * x;
            double y2 = y * y;
            double z2 = z * z;

            return Mathd.Sqrt(x2 + y2 + z2);
        }

        /// <summary>
        /// Returns the squared length (squared magnitude) of this vector.
        /// This method runs faster than <see cref="Length"/>, so prefer it if
        /// you need to compare vectors or need the squared length for some formula.
        /// </summary>
        /// <returns>The squared length of this vector.</returns>
        public double LengthSquared()
        {
            double x2 = x * x;
            double y2 = y * y;
            double z2 = z * z;

            return x2 + y2 + z2;
        }

        /// <summary>
        /// Returns the result of the linear interpolation between
        /// this vector and `to` by amount `weight`.
        /// </summary>
        /// <param name="to">The destination vector for interpolation.</param>
        /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting vector of the interpolation.</returns>
        public Vector3d Lerp(Vector3d to, double weight)
        {
            return new Vector3d
            (
                Mathd.Lerp(x, to.x, weight),
                Mathd.Lerp(y, to.y, weight),
                Mathd.Lerp(z, to.z, weight)
            );
        }

        /// <summary>
        /// Returns the result of the linear interpolation between
        /// this vector and `to` by the vector amount `weight`.
        /// </summary>
        /// <param name="to">The destination vector for interpolation.</param>
        /// <param name="weight">A vector with components on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting vector of the interpolation.</returns>
        public Vector3d Lerp(Vector3d to, Vector3d weight)
        {
            return new Vector3d
            (
                Mathd.Lerp(x, to.x, weight.x),
                Mathd.Lerp(y, to.y, weight.y),
                Mathd.Lerp(z, to.z, weight.z)
            );
        }

        /// <summary>
        /// Returns the result of the linear interpolation between
        /// this vector and `to` by amount `weight`.
        /// </summary>
        /// <param name="to">The destination vector for interpolation.</param>
        /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting vector of the interpolation.</returns>
        public Vector3d LinearInterpolate(Vector3d to, double weight)
        {
            return new Vector3d
            (
                Mathd.Lerp(x, to.x, weight),
                Mathd.Lerp(y, to.y, weight),
                Mathd.Lerp(z, to.z, weight)
            );
        }

        /// <summary>
        /// Returns the result of the linear interpolation between
        /// this vector and `to` by the vector amount `weight`.
        /// </summary>
        /// <param name="to">The destination vector for interpolation.</param>
        /// <param name="weight">A vector with components on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting vector of the interpolation.</returns>
        public Vector3d LinearInterpolate(Vector3d to, Vector3d weight)
        {
            return new Vector3d
            (
                Mathd.Lerp(x, to.x, weight.x),
                Mathd.Lerp(y, to.y, weight.y),
                Mathd.Lerp(z, to.z, weight.z)
            );
        }

        /// <summary>
        /// Returns the axis of the vector's largest value. See <see cref="Axis"/>.
        /// If all components are equal, this method returns <see cref="Axis.X"/>.
        /// </summary>
        /// <returns>The index of the largest axis.</returns>
        public Axis MaxAxis()
        {
            return x < y ? (y < z ? Axis.Z : Axis.Y) : (x < z ? Axis.Z : Axis.X);
        }

        /// <summary>
        /// Returns the axis of the vector's smallest value. See <see cref="Axis"/>.
        /// If all components are equal, this method returns <see cref="Axis.Z"/>.
        /// </summary>
        /// <returns>The index of the smallest axis.</returns>
        public Axis MinAxis()
        {
            return x < y ? (x < z ? Axis.X : Axis.Z) : (y < z ? Axis.Y : Axis.Z);
        }

        /// <summary>
        /// Moves this vector toward `to` by the fixed `delta` amount.
        /// </summary>
        /// <param name="to">The vector to move towards.</param>
        /// <param name="delta">The amount to move towards by.</param>
        /// <returns>The resulting vector.</returns>
        public Vector3d MoveToward(Vector3d to, double delta)
        {
            Vector3d v = this;
            Vector3d vd = to - v;
            double len = vd.Length();
            return len <= delta || len < Mathd.Epsilon ? to : v + vd / len * delta;
        }

        /// <summary>
        /// Returns the vector scaled to unit length. Equivalent to `v / v.Length()`.
        /// </summary>
        /// <returns>A normalized version of the vector.</returns>
        public Vector3d Normalized()
        {
            Vector3d v = this;
            v.Normalize();
            return v;
        }

        /// <summary>
        /// Returns the outer product with `b`.
        /// </summary>
        /// <param name="b">The other vector.</param>
        /// <returns>A <see cref="Basisd"/> representing the outer product matrix.</returns>
        public Basisd Outer(Vector3d b)
        {
            return new Basisd(
                x * b.x, x * b.y, x * b.z,
                y * b.x, y * b.y, y * b.z,
                z * b.x, z * b.y, z * b.z
            );
        }

        /// <summary>
        /// Returns a vector composed of the <see cref="Mathd.PosMod(double, double)"/> of this vector's components and `mod`.
        /// </summary>
        /// <param name="mod">A value representing the divisor of the operation.</param>
        /// <returns>A vector with each component <see cref="Mathd.PosMod(double, double)"/> by `mod`.</returns>
        public Vector3d PosMod(double mod)
        {
            Vector3d v;
            v.x = Mathd.PosMod(x, mod);
            v.y = Mathd.PosMod(y, mod);
            v.z = Mathd.PosMod(z, mod);
            return v;
        }

        /// <summary>
        /// Returns a vector composed of the <see cref="Mathd.PosMod(double, double)"/> of this vector's components and `modv`'s components.
        /// </summary>
        /// <param name="modv">A vector representing the divisors of the operation.</param>
        /// <returns>A vector with each component <see cref="Mathd.PosMod(double, double)"/> by `modv`'s components.</returns>
        public Vector3d PosMod(Vector3d modv)
        {
            Vector3d v;
            v.x = Mathd.PosMod(x, modv.x);
            v.y = Mathd.PosMod(y, modv.y);
            v.z = Mathd.PosMod(z, modv.z);
            return v;
        }

        /// <summary>
        /// Returns this vector projected onto another vector `b`.
        /// </summary>
        /// <param name="onNormal">The vector to project onto.</param>
        /// <returns>The projected vector.</returns>
        public Vector3d Project(Vector3d onNormal)
        {
            return onNormal * (Dot(onNormal) / onNormal.LengthSquared());
        }

        /// <summary>
        /// Returns this vector reflected from a plane defined by the given `normal`.
        /// </summary>
        /// <param name="normal">The normal vector defining the plane to reflect from. Must be normalized.</param>
        /// <returns>The reflected vector.</returns>
        public Vector3d Reflect(Vector3d normal)
        {
#if DEBUG
            if (!normal.IsNormalized())
            {
                throw new ArgumentException("Argument  is not normalized", nameof(normal));
            }
#endif
            return 2.0f * Dot(normal) * normal - this;
        }

        /// <summary>
        /// Rotates this vector around a given `axis` vector by `phi` radians.
        /// The `axis` vector must be a normalized vector.
        /// </summary>
        /// <param name="axis">The vector to rotate around. Must be normalized.</param>
        /// <param name="phi">The angle to rotate by, in radians.</param>
        /// <returns>The rotated vector.</returns>
        public Vector3d Rotated(Vector3d axis, double phi)
        {
#if DEBUG
            if (!axis.IsNormalized())
            {
                throw new ArgumentException("Argument  is not normalized", nameof(axis));
            }
#endif
            return new Basisd(axis, phi).Xform(this);
        }

        /// <summary>
        /// Returns this vector with all components rounded to the nearest integer,
        /// with halfway cases rounded towards the nearest multiple of two.
        /// </summary>
        /// <returns>The rounded vector.</returns>
        public Vector3d Round()
        {
            return new Vector3d(Mathd.Round(x), Mathd.Round(y), Mathd.Round(z));
        }

        /// <summary>
        /// Returns a vector with each component set to one or negative one, depending
        /// on the signs of this vector's components, or zero if the component is zero,
        /// by calling <see cref="Mathd.Sign(double)"/> on each component.
        /// </summary>
        /// <returns>A vector with all components as either `1`, `-1`, or `0`.</returns>
        public Vector3d Sign()
        {
            Vector3d v;
            v.x = Mathd.Sign(x);
            v.y = Mathd.Sign(y);
            v.z = Mathd.Sign(z);
            return v;
        }

        /// <summary>
        /// Returns the signed angle to the given vector, in radians.
        /// The sign of the angle is positive in a counter-clockwise
        /// direction and negative in a clockwise direction when viewed
        /// from the side specified by the `axis`.
        /// </summary>
        /// <param name="to">The other vector to compare this vector to.</param>
        /// <param name="axis">The reference axis to use for the angle sign.</param>
        /// <returns>The signed angle between the two vectors, in radians.</returns>
        public double SignedAngleTo(Vector3d to, Vector3d axis)
        {
            Vector3d crossTo = Cross(to);
            double unsignedAngle = Mathd.Atan2(crossTo.Length(), Dot(to));
            double sign = crossTo.Dot(axis);
            return (sign < 0) ? -unsignedAngle : unsignedAngle;
        }

        /// <summary>
        /// Returns the result of the spherical linear interpolation between
        /// this vector and `to` by amount `weight`.
        ///
        /// Note: Both vectors must be normalized.
        /// </summary>
        /// <param name="to">The destination vector for interpolation. Must be normalized.</param>
        /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting vector of the interpolation.</returns>
        public Vector3d Slerp(Vector3d to, double weight)
        {
#if DEBUG
            if (!IsNormalized())
            {
                throw new InvalidOperationException("Vector3d.Slerp: From vector is not normalized.");
            }
            if (!to.IsNormalized())
            {
                throw new InvalidOperationException("Vector3d.Slerp: `to` is not normalized.");
            }
#endif
            double theta = AngleTo(to);
            return Rotated(Cross(to), theta * weight);
        }

        /// <summary>
        /// Returns this vector slid along a plane defined by the given normal.
        /// </summary>
        /// <param name="normal">The normal vector defining the plane to slide on.</param>
        /// <returns>The slid vector.</returns>
        public Vector3d Slide(Vector3d normal)
        {
            return this - normal * Dot(normal);
        }

        /// <summary>
        /// Returns this vector with each component snapped to the nearest multiple of `step`.
        /// This can also be used to round to an arbitrary number of decimals.
        /// </summary>
        /// <param name="step">A vector value representing the step size to snap to.</param>
        /// <returns>The snapped vector.</returns>
        public Vector3d Snapped(Vector3d step)
        {
            return new Vector3d
            (
                Mathd.Snapped(x, step.x),
                Mathd.Snapped(y, step.y),
                Mathd.Snapped(z, step.z)
            );
        }

        /// <summary>
        /// Returns a diagonal matrix with the vector as main diagonal.
        ///
        /// This is equivalent to a Basisd with no rotation or shearing and
        /// this vector's components set as the scale.
        /// </summary>
        /// <returns>A Basisd with the vector as its main diagonal.</returns>
        public Basisd ToDiagonalMatrix()
        {
            return new Basisd(
                x, 0, 0,
                0, y, 0,
                0, 0, z
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

        /// <summary>
        /// Zero vector, a vector with all components set to `0`.
        /// </summary>
        /// <value>Equivalent to `new Vector3d(0, 0, 0)`</value>
        public static Vector3d Zero { get { return _zero; } }
        /// <summary>
        /// One vector, a vector with all components set to `1`.
        /// </summary>
        /// <value>Equivalent to `new Vector3d(1, 1, 1)`</value>
        public static Vector3d One { get { return _one; } }
        /// <summary>
        /// NegOne vector, a vector with all components set to `-1`.
        /// </summary>
        /// <value>Equivalent to `new Vector3d(-1, -1, -1)`</value>
        public static Vector3d NegOne { get { return _negOne; } }
        /// <summary>
        /// Infinity vector, a vector with all components set to `Mathd.Inf`.
        /// </summary>
        /// <value>Equivalent to `new Vector3d(Mathd.Inf, Mathd.Inf, Mathd.Inf)`</value>
        public static Vector3d Inf { get { return _inf; } }

        /// <summary>
        /// Up unit vector.
        /// </summary>
        /// <value>Equivalent to `new Vector3d(0, 1, 0)`</value>
        public static Vector3d Up { get { return _up; } }
        /// <summary>
        /// Down unit vector.
        /// </summary>
        /// <value>Equivalent to `new Vector3d(0, -1, 0)`</value>
        public static Vector3d Down { get { return _down; } }
        /// <summary>
        /// Right unit vector. Represents the local direction of right,
        /// and the global direction of east.
        /// </summary>
        /// <value>Equivalent to `new Vector3d(1, 0, 0)`</value>
        public static Vector3d Right { get { return _right; } }
        /// <summary>
        /// Left unit vector. Represents the local direction of left,
        /// and the global direction of west.
        /// </summary>
        /// <value>Equivalent to `new Vector3d(-1, 0, 0)`</value>
        public static Vector3d Left { get { return _left; } }
        /// <summary>
        /// Forward unit vector. Represents the local direction of forward,
        /// and the global direction of north.
        /// </summary>
        /// <value>Equivalent to `new Vector3d(0, 0, -1)`</value>
        public static Vector3d Forward { get { return _forward; } }
        /// <summary>
        /// Back unit vector. Represents the local direction of back,
        /// and the global direction of south.
        /// </summary>
        /// <value>Equivalent to `new Vector3d(0, 0, 1)`</value>
        public static Vector3d Back { get { return _back; } }

        /// <summary>
        /// Constructs a new <see cref="Vector3d"/> with the given components.
        /// </summary>
        /// <param name="x">The vector's X component.</param>
        /// <param name="y">The vector's Y component.</param>
        /// <param name="z">The vector's Z component.</param>
        public Vector3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Constructs a new <see cref="Vector3d"/> from an existing <see cref="Vector3d"/>.
        /// </summary>
        /// <param name="v">The existing <see cref="Vector3d"/>.</param>
        public Vector3d(Vector3d v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

#if GODOT
        public static explicit operator Godot.Vector3(Vector3d value)
        {
            return new Godot.Vector3((real_t)value.x, (real_t)value.y, (real_t)value.z);
        }

        public static implicit operator Vector3d(Godot.Vector3 value)
        {
            return new Vector3d(value.x, value.y, value.z);
        }
#elif UNITY_5_3_OR_NEWER
        public static explicit operator UnityEngine.Vector3(Vector3d value)
        {
            return new UnityEngine.Vector3((real_t)value.x, (real_t)value.y, (real_t)value.z);
        }

        public static implicit operator Vector3d(UnityEngine.Vector3 value)
        {
            return new Vector3d(value.x, value.y, value.z);
        }
#endif

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

        public static Vector3d operator /(Vector3d vec, double divisor)
        {
            vec.x /= divisor;
            vec.y /= divisor;
            vec.z /= divisor;
            return vec;
        }

        public static Vector3d operator /(Vector3d vec, Vector3d divisorv)
        {
            vec.x /= divisorv.x;
            vec.y /= divisorv.y;
            vec.z /= divisorv.z;
            return vec;
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
            if (left.x == right.x)
            {
                if (left.y == right.y)
                {
                    return left.z < right.z;
                }
                return left.y < right.y;
            }
            return left.x < right.x;
        }

        public static bool operator >(Vector3d left, Vector3d right)
        {
            if (left.x == right.x)
            {
                if (left.y == right.y)
                {
                    return left.z > right.z;
                }
                return left.y > right.y;
            }
            return left.x > right.x;
        }

        public static bool operator <=(Vector3d left, Vector3d right)
        {
            if (left.x == right.x)
            {
                if (left.y == right.y)
                {
                    return left.z <= right.z;
                }
                return left.y < right.y;
            }
            return left.x < right.x;
        }

        public static bool operator >=(Vector3d left, Vector3d right)
        {
            if (left.x == right.x)
            {
                if (left.y == right.y)
                {
                    return left.z >= right.z;
                }
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
            return x == other.x && y == other.y && z == other.z;
        }

        /// <summary>
        /// Returns true if this vector and `other` are approximately equal, by running
        /// <see cref="Mathd.IsEqualApprox(double, double)"/> on each component.
        /// </summary>
        /// <param name="other">The other vector to compare.</param>
        /// <returns>Whether or not the vectors are approximately equal.</returns>
        public bool IsEqualApprox(Vector3d other)
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
