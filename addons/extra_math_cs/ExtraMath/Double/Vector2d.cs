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
        /// <summary>
        /// Enumerated index values for the axes.
        /// Returned by <see cref="MaxAxis"/> and <see cref="MinAxis"/>.
        /// </summary>
        public enum Axis
        {
            X = 0,
            Y
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
        /// Access vector components using their index.
        /// </summary>
        /// <value>`[0]` is equivalent to `.x`, `[1]` is equivalent to `.y`.</value>
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

        /// <summary>
        /// Returns a new vector with all components in absolute values (i.e. positive).
        /// </summary>
        /// <returns>A vector with <see cref="Mathd.Abs(double)"/> called on each component.</returns>
        public Vector2d Abs()
        {
            return new Vector2d(Mathd.Abs(x), Mathd.Abs(y));
        }

        /// <summary>
        /// Returns this vector's angle with respect to the X axis, or (1, 0) vector, in radians.
        ///
        /// Equivalent to the result of <see cref="Mathd.Atan2(double, double)"/> when
        /// called with the vector's `y` and `x` as parameters: `Mathd.Atan2(v.y, v.x)`.
        /// </summary>
        /// <returns>The angle of this vector, in radians.</returns>
        public double Angle()
        {
            return Mathd.Atan2(y, x);
        }

        /// <summary>
        /// Returns the angle to the given vector, in radians.
        /// </summary>
        /// <param name="to">The other vector to compare this vector to.</param>
        /// <returns>The angle between the two vectors, in radians.</returns>
        public double AngleTo(Vector2d to)
        {
            return Mathd.Atan2(Cross(to), Dot(to));
        }

        /// <summary>
        /// Returns the angle between the line connecting the two points and the X axis, in radians.
        /// </summary>
        /// <param name="to">The other vector to compare this vector to.</param>
        /// <returns>The angle between the two vectors, in radians.</returns>
        public double AngleToPoint(Vector2d to)
        {
            return Mathd.Atan2(y - to.y, x - to.x);
        }

        /// <summary>
        /// Returns the aspect ratio of this vector, the ratio of `x` to `y`.
        /// </summary>
        /// <returns>The `x` component divided by the `y` component.</returns>
        public double Aspect()
        {
            return x / y;
        }

        /// <summary>
        /// Returns the vector "bounced off" from a plane defined by the given normal.
        /// </summary>
        /// <param name="normal">The normal vector defining the plane to bounce off. Must be normalized.</param>
        /// <returns>The bounced vector.</returns>
        public Vector2d Bounce(Vector2d normal)
        {
            return -Reflect(normal);
        }

        /// <summary>
        /// Returns a new vector with all components rounded up (towards positive infinity).
        /// </summary>
        /// <returns>A vector with <see cref="Mathd.Ceil"/> called on each component.</returns>
        public Vector2d Ceil()
        {
            return new Vector2d(Mathd.Ceil(x), Mathd.Ceil(y));
        }

        /// <summary>
        /// Returns the vector with a maximum length by limiting its length to `length`.
        /// </summary>
        /// <param name="length">The length to limit to.</param>
        /// <returns>The vector with its length limited.</returns>
        public Vector2d Clamped(double length)
        {
            Vector2d v = this;
            double l = Length();

            if (l > 0 && length < l)
            {
                v /= l;
                v *= length;
            }

            return v;
        }

        /// <summary>
        /// Returns the cross product of this vector and `b`.
        /// </summary>
        /// <param name="b">The other vector.</param>
        /// <returns>The cross product value.</returns>
        public double Cross(Vector2d b)
        {
            return x * b.y - y * b.x;
        }

        /// <summary>
        /// Performs a cubic interpolation between vectors `preA`, this vector, `b`, and `postB`, by the given amount `t`.
        /// </summary>
        /// <param name="b">The destination vector.</param>
        /// <param name="preA">A vector before this vector.</param>
        /// <param name="postB">A vector after `b`.</param>
        /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The interpolated vector.</returns>
        public Vector2d CubicInterpolate(Vector2d b, Vector2d preA, Vector2d postB, double weight)
        {
            Vector2d p0 = preA;
            Vector2d p1 = this;
            Vector2d p2 = b;
            Vector2d p3 = postB;

            double t = weight;
            double t2 = t * t;
            double t3 = t2 * t;

            return 0.5f * (p1 * 2.0f +
                                (-p0 + p2) * t +
                                (2.0f * p0 - 5.0f * p1 + 4 * p2 - p3) * t2 +
                                (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3);
        }

        /// <summary>
        /// Returns the normalized vector pointing from this vector to `b`.
        /// </summary>
        /// <param name="b">The other vector to point towards.</param>
        /// <returns>The direction from this vector to `b`.</returns>
        public Vector2d DirectionTo(Vector2d b)
        {
            return new Vector2d(b.x - x, b.y - y).Normalized();
        }

        /// <summary>
        /// Returns the squared distance between this vector and `to`.
        /// This method runs faster than <see cref="DistanceTo"/>, so prefer it if
        /// you need to compare vectors or need the squared distance for some formula.
        /// </summary>
        /// <param name="to">The other vector to use.</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public double DistanceSquaredTo(Vector2d to)
        {
            return (x - to.x) * (x - to.x) + (y - to.y) * (y - to.y);
        }

        /// <summary>
        /// Returns the distance between this vector and `to`.
        /// </summary>
        /// <param name="to">The other vector to use.</param>
        /// <returns>The distance between the two vectors.</returns>
        public double DistanceTo(Vector2d to)
        {
            return Mathd.Sqrt((x - to.x) * (x - to.x) + (y - to.y) * (y - to.y));
        }

        /// <summary>
        /// Returns the dot product of this vector and `with`.
        /// </summary>
        /// <param name="with">The other vector to use.</param>
        /// <returns>The dot product of the two vectors.</returns>
        public double Dot(Vector2d with)
        {
            return x * with.x + y * with.y;
        }

        /// <summary>
        /// Returns a new vector with all components rounded down (towards negative infinity).
        /// </summary>
        /// <returns>A vector with <see cref="Mathd.Floor"/> called on each component.</returns>
        public Vector2d Floor()
        {
            return new Vector2d(Mathd.Floor(x), Mathd.Floor(y));
        }

        /// <summary>
        /// Returns the inverse of this vector. This is the same as `new Vector2d(1 / v.x, 1 / v.y)`.
        /// </summary>
        /// <returns>The inverse of this vector.</returns>
        public Vector2d Inverse()
        {
            return new Vector2d(1 / x, 1 / y);
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
            return Mathd.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// Returns the squared length (squared magnitude) of this vector.
        /// This method runs faster than <see cref="Length"/>, so prefer it if
        /// you need to compare vectors or need the squared length for some formula.
        /// </summary>
        /// <returns>The squared length of this vector.</returns>
        public double LengthSquared()
        {
            return x * x + y * y;
        }

        /// <summary>
        /// Returns the result of the linear interpolation between
        /// this vector and `to` by amount `weight`.
        /// </summary>
        /// <param name="to">The destination vector for interpolation.</param>
        /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting vector of the interpolation.</returns>
        public Vector2d Lerp(Vector2d to, double weight)
        {
            return new Vector2d
            (
                Mathd.Lerp(x, to.x, weight),
                Mathd.Lerp(y, to.y, weight)
            );
        }

        /// <summary>
        /// Returns the result of the linear interpolation between
        /// this vector and `to` by the vector amount `weight`.
        /// </summary>
        /// <param name="to">The destination vector for interpolation.</param>
        /// <param name="weight">A vector with components on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting vector of the interpolation.</returns>
        public Vector2d Lerp(Vector2d to, Vector2d weight)
        {
            return new Vector2d
            (
                Mathd.Lerp(x, to.x, weight.x),
                Mathd.Lerp(y, to.y, weight.y)
            );
        }

        /// <summary>
        /// Returns the result of the linear interpolation between
        /// this vector and `to` by amount `weight`.
        /// </summary>
        /// <param name="to">The destination vector for interpolation.</param>
        /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting vector of the interpolation.</returns>
        public Vector2d LinearInterpolate(Vector2d to, double weight)
        {
            return new Vector2d
            (
                Mathd.Lerp(x, to.x, weight),
                Mathd.Lerp(y, to.y, weight)
            );
        }

        /// <summary>
        /// Returns the result of the linear interpolation between
        /// this vector and `to` by the vector amount `weight`.
        /// </summary>
        /// <param name="to">The destination vector for interpolation.</param>
        /// <param name="weight">A vector with components on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting vector of the interpolation.</returns>
        public Vector2d LinearInterpolate(Vector2d to, Vector2d weight)
        {
            return new Vector2d
            (
                Mathd.Lerp(x, to.x, weight.x),
                Mathd.Lerp(y, to.y, weight.y)
            );
        }

        /// <summary>
        /// Returns the axis of the vector's largest value. See <see cref="Axis"/>.
        /// If both components are equal, this method returns <see cref="Axis.X"/>.
        /// </summary>
        /// <returns>The index of the largest axis.</returns>
        public Axis MaxAxis()
        {
            return x < y ? Axis.Y : Axis.X;
        }

        /// <summary>
        /// Returns the axis of the vector's smallest value. See <see cref="Axis"/>.
        /// If both components are equal, this method returns <see cref="Axis.Y"/>.
        /// </summary>
        /// <returns>The index of the smallest axis.</returns>
        public Axis MinAxis()
        {
            return x < y ? Axis.X : Axis.Y;
        }

        /// <summary>
        /// Moves this vector toward `to` by the fixed `delta` amount.
        /// </summary>
        /// <param name="to">The vector to move towards.</param>
        /// <param name="delta">The amount to move towards by.</param>
        /// <returns>The resulting vector.</returns>
        public Vector2d MoveToward(Vector2d to, double delta)
        {
            Vector2d v = this;
            Vector2d vd = to - v;
            double len = vd.Length();
            return len <= delta || len < Mathd.Epsilon ? to : v + vd / len * delta;
        }

        /// <summary>
        /// Returns the vector scaled to unit length. Equivalent to `v / v.Length()`.
        /// </summary>
        /// <returns>A normalized version of the vector.</returns>
        public Vector2d Normalized()
        {
            Vector2d v = this;
            v.Normalize();
            return v;
        }

        /// <summary>
        /// Returns a vector composed of the <see cref="Mathd.PosMod(double, double)"/> of this vector's components and `mod`.
        /// </summary>
        /// <param name="mod">A value representing the divisor of the operation.</param>
        /// <returns>A vector with each component <see cref="Mathd.PosMod(double, double)"/> by `mod`.</returns>
        public Vector2d PosMod(double mod)
        {
            Vector2d v;
            v.x = Mathd.PosMod(x, mod);
            v.y = Mathd.PosMod(y, mod);
            return v;
        }

        /// <summary>
        /// Returns a vector composed of the <see cref="Mathd.PosMod(double, double)"/> of this vector's components and `modv`'s components.
        /// </summary>
        /// <param name="modv">A vector representing the divisors of the operation.</param>
        /// <returns>A vector with each component <see cref="Mathd.PosMod(double, double)"/> by `modv`'s components.</returns>
        public Vector2d PosMod(Vector2d modv)
        {
            Vector2d v;
            v.x = Mathd.PosMod(x, modv.x);
            v.y = Mathd.PosMod(y, modv.y);
            return v;
        }

        /// <summary>
        /// Returns this vector projected onto another vector `b`.
        /// </summary>
        /// <param name="onNormal">The vector to project onto.</param>
        /// <returns>The projected vector.</returns>
        public Vector2d Project(Vector2d onNormal)
        {
            return onNormal * (Dot(onNormal) / onNormal.LengthSquared());
        }

        /// <summary>
        /// Returns this vector reflected from a plane defined by the given `normal`.
        /// </summary>
        /// <param name="normal">The normal vector defining the plane to reflect from. Must be normalized.</param>
        /// <returns>The reflected vector.</returns>
        public Vector2d Reflect(Vector2d normal)
        {
#if DEBUG
            if (!normal.IsNormalized())
            {
                throw new ArgumentException("Argument  is not normalized", nameof(normal));
            }
#endif
            return 2 * Dot(normal) * normal - this;
        }

        /// <summary>
        /// Rotates this vector by `phi` radians.
        /// </summary>
        /// <param name="phi">The angle to rotate by, in radians.</param>
        /// <returns>The rotated vector.</returns>
        public Vector2d Rotated(double phi)
        {
            double sine = Mathd.Sin(phi);
            double cosi = Mathd.Cos(phi);
            return new Vector2d(
                x * cosi - y * sine,
                x * sine + y * cosi);
        }

        /// <summary>
        /// Returns this vector with all components rounded to the nearest integer,
        /// with halfway cases rounded towards the nearest multiple of two.
        /// </summary>
        /// <returns>The rounded vector.</returns>
        public Vector2d Round()
        {
            return new Vector2d(Mathd.Round(x), Mathd.Round(y));
        }

        /// <summary>
        /// Returns a vector with each component set to one or negative one, depending
        /// on the signs of this vector's components, or zero if the component is zero,
        /// by calling <see cref="Mathd.Sign(double)"/> on each component.
        /// </summary>
        /// <returns>A vector with all components as either `1`, `-1`, or `0`.</returns>
        public Vector2d Sign()
        {
            Vector2d v;
            v.x = Mathd.Sign(x);
            v.y = Mathd.Sign(y);
            return v;
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
        public Vector2d Slerp(Vector2d to, double weight)
        {
#if DEBUG
            if (!IsNormalized())
            {
                throw new InvalidOperationException("Vector2d.Slerp: From vector is not normalized.");
            }
            if (!to.IsNormalized())
            {
                throw new InvalidOperationException("Vector2d.Slerp: `to` is not normalized.");
            }
#endif
            return Rotated(AngleTo(to) * weight);
        }

        /// <summary>
        /// Returns this vector slid along a plane defined by the given normal.
        /// </summary>
        /// <param name="normal">The normal vector defining the plane to slide on.</param>
        /// <returns>The slid vector.</returns>
        public Vector2d Slide(Vector2d normal)
        {
            return this - normal * Dot(normal);
        }

        /// <summary>
        /// Returns this vector with each component snapped to the nearest multiple of `step`.
        /// This can also be used to round to an arbitrary number of decimals.
        /// </summary>
        /// <param name="step">A vector value representing the step size to snap to.</param>
        /// <returns>The snapped vector.</returns>
        public Vector2d Snapped(Vector2d step)
        {
            return new Vector2d(Mathd.Snapped(x, step.x), Mathd.Snapped(y, step.y));
        }

        /// <summary>
        /// Returns a perpendicular vector rotated 90 degrees counter-clockwise
        /// compared to the original, with the same length.
        /// </summary>
        /// <returns>The perpendicular vector.</returns>
        public Vector2d Orthogonal()
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

        /// <summary>
        /// Zero vector, a vector with all components set to `0`.
        /// </summary>
        /// <value>Equivalent to `new Vector2d(0, 0)`</value>
        public static Vector2d Zero { get { return _zero; } }
        /// <summary>
        /// One vector, a vector with all components set to `1`.
        /// </summary>
        /// <value>Equivalent to `new Vector2d(1, 1)`</value>
        public static Vector2d One { get { return _one; } }
        /// <summary>
        /// NegOne vector, a vector with all components set to `-1`.
        /// </summary>
        /// <value>Equivalent to `new Vector2d(-1, -1)`</value>
        public static Vector2d NegOne { get { return _negOne; } }
        /// <summary>
        /// Infinity vector, a vector with all components set to `Mathd.Inf`.
        /// </summary>
        /// <value>Equivalent to `new Vector2d(Mathd.Inf, Mathd.Inf)`</value>
        public static Vector2d Inf { get { return _inf; } }

        /// <summary>
        /// Up unit vector. Y is down in 2D, so this vector points -Y.
        /// </summary>
        /// <value>Equivalent to `new Vector2d(0, -1)`</value>
        public static Vector2d Up { get { return _up; } }
        /// <summary>
        /// Down unit vector. Y is down in 2D, so this vector points +Y.
        /// </summary>
        /// <value>Equivalent to `new Vector2d(0, 1)`</value>
        public static Vector2d Down { get { return _down; } }
        /// <summary>
        /// Right unit vector. Represents the direction of right.
        /// </summary>
        /// <value>Equivalent to `new Vector2d(1, 0)`</value>
        public static Vector2d Right { get { return _right; } }
        /// <summary>
        /// Left unit vector. Represents the direction of left.
        /// </summary>
        /// <value>Equivalent to `new Vector2d(-1, 0)`</value>
        public static Vector2d Left { get { return _left; } }

        /// <summary>
        /// Constructs a new <see cref="Vector2d"/> with the given components.
        /// </summary>
        /// <param name="x">The vector's X component.</param>
        /// <param name="y">The vector's Y component.</param>
        public Vector2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Constructs a new <see cref="Vector2d"/> from an existing <see cref="Vector2d"/>.
        /// </summary>
        /// <param name="v">The existing <see cref="Vector2d"/>.</param>
        public Vector2d(Vector2d v)
        {
            x = v.x;
            y = v.y;
        }

#if GODOT
        public static explicit operator Godot.Vector2(Vector2d value)
        {
            return new Godot.Vector2((real_t)value.x, (real_t)value.y);
        }

        public static implicit operator Vector2d(Godot.Vector2 value)
        {
            return new Vector2d(value.x, value.y);
        }
#elif UNITY_5_3_OR_NEWER
        public static explicit operator UnityEngine.Vector2(Vector2d value)
        {
            return new UnityEngine.Vector2((real_t)value.x, (real_t)value.y);
        }

        public static implicit operator Vector2d(UnityEngine.Vector2 value)
        {
            return new Vector2d(value.x, value.y);
        }
#endif

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

        public static Vector2d operator /(Vector2d vec, double divisor)
        {
            vec.x /= divisor;
            vec.y /= divisor;
            return vec;
        }

        public static Vector2d operator /(Vector2d vec, Vector2d divisorv)
        {
            vec.x /= divisorv.x;
            vec.y /= divisorv.y;
            return vec;
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
            if (left.x == right.x)
            {
                return left.y < right.y;
            }
            return left.x < right.x;
        }

        public static bool operator >(Vector2d left, Vector2d right)
        {
            if (left.x == right.x)
            {
                return left.y > right.y;
            }
            return left.x > right.x;
        }

        public static bool operator <=(Vector2d left, Vector2d right)
        {
            if (left.x == right.x)
            {
                return left.y <= right.y;
            }
            return left.x <= right.x;
        }

        public static bool operator >=(Vector2d left, Vector2d right)
        {
            if (left.x == right.x)
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
            return x == other.x && y == other.y;
        }

        /// <summary>
        /// Returns true if this vector and `other` are approximately equal, by running
        /// <see cref="Mathd.IsEqualApprox(double, double)"/> on each component.
        /// </summary>
        /// <param name="other">The other vector to compare.</param>
        /// <returns>Whether or not the vectors are approximately equal.</returns>
        public bool IsEqualApprox(Vector2d other)
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
