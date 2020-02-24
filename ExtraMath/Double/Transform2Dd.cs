using System;
using System.Runtime.InteropServices;

namespace ExtraMath
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Transform2Dd : IEquatable<Transform2Dd>
    {
        public Vector2d x;
        public Vector2d y;
        public Vector2d origin;

        public double Rotation
        {
            get
            {
                double det = BasisDeterminant();
                Transform2Dd t = Orthonormalized();
                if (det < 0)
                {
                    t.ScaleBasis(new Vector2d(1, -1));
                }
                return Mathd.Atan2(t.x.y, t.x.x);
            }
            set
            {
                Vector2d scale = Scale;
                x.x = y.y = Mathd.Cos(value);
                x.y = y.x = Mathd.Sin(value);
                y.x *= -1;
                Scale = scale;
            }
        }

        public Vector2d Scale
        {
            get
            {
                double detSign = Mathd.Sign(BasisDeterminant());
                return new Vector2d(x.Length(), detSign * y.Length());
            }
            set
            {
                value /= Scale; // Value becomes what's called "delta_scale" in core.
                x *= value.x;
                y *= value.y;
            }
        }

        /// <summary>
        /// Access whole columns in the form of Vector2. The third column is the origin vector.
        /// </summary>
        /// <param name="column">Which column vector.</param>
        public Vector2d this[int column]
        {
            get
            {
                switch (column)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return origin;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (column)
                {
                    case 0:
                        x = value;
                        return;
                    case 1:
                        y = value;
                        return;
                    case 2:
                        origin = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Access matrix elements in column-major order. The third column is the origin vector.
        /// </summary>
        /// <param name="column">Which column, the matrix horizontal position.</param>
        /// <param name="row">Which row, the matrix vertical position.</param>
        public double this[int column, int row]
        {
            get
            {
                return this[column][row];
            }
            set
            {
                Vector2d columnVector = this[column];
                columnVector[row] = value;
                this[column] = columnVector;
            }
        }

        public Transform2Dd AffineInverse()
        {
            double det = BasisDeterminant();

            if (det == 0)
                throw new InvalidOperationException("Matrix determinant is zero and cannot be inverted.");

            var inv = this;

            double temp = inv[0, 0];
            inv[0, 0] = inv[1, 1];
            inv[1, 1] = temp;

            double detInv = 1.0f / det;

            inv[0] *= new Vector2d(detInv, -detInv);
            inv[1] *= new Vector2d(-detInv, detInv);

            inv[2] = BasisXform(-inv[2]);

            return inv;
        }

        private double BasisDeterminant()
        {
            return x.x * y.y - x.y * y.x;
        }

        public Vector2d BasisXform(Vector2d v)
        {
            return new Vector2d(Tdotx(v), Tdoty(v));
        }

        public Vector2d BasisXformInv(Vector2d v)
        {
            return new Vector2d(x.Dot(v), y.Dot(v));
        }

        public Transform2Dd InterpolateWith(Transform2Dd m, double c)
        {
            double r1 = Rotation;
            double r2 = m.Rotation;

            Vector2d s1 = Scale;
            Vector2d s2 = m.Scale;

            // Slerp rotation
            var v1 = new Vector2d(Mathd.Cos(r1), Mathd.Sin(r1));
            var v2 = new Vector2d(Mathd.Cos(r2), Mathd.Sin(r2));

            double dot = v1.Dot(v2);

            // Clamp dot to [-1, 1]
            dot = dot < -1.0f ? -1.0f : (dot > 1.0f ? 1.0f : dot);

            Vector2d v;

            if (dot > 0.9995f)
            {
                // Linearly interpolate to avoid numerical precision issues
                v = v1.LinearInterpolate(v2, c).Normalized();
            }
            else
            {
                double angle = c * Mathd.Acos(dot);
                Vector2d v3 = (v2 - v1 * dot).Normalized();
                v = v1 * Mathd.Cos(angle) + v3 * Mathd.Sin(angle);
            }

            // Extract parameters
            Vector2d p1 = origin;
            Vector2d p2 = m.origin;

            // Construct matrix
            var res = new Transform2Dd(Mathd.Atan2(v.y, v.x), p1.LinearInterpolate(p2, c));
            Vector2d scale = s1.LinearInterpolate(s2, c);
            res.x *= scale;
            res.y *= scale;

            return res;
        }

        public Transform2Dd Inverse()
        {
            var inv = this;

            // Swap
            double temp = inv.x.y;
            inv.x.y = inv.y.x;
            inv.y.x = temp;

            inv.origin = inv.BasisXform(-inv.origin);

            return inv;
        }

        public Transform2Dd Orthonormalized()
        {
            var on = this;

            Vector2d onX = on.x;
            Vector2d onY = on.y;

            onX.Normalize();
            onY = onY - onX * onX.Dot(onY);
            onY.Normalize();

            on.x = onX;
            on.y = onY;

            return on;
        }

        public Transform2Dd Rotated(double phi)
        {
            return this * new Transform2Dd(phi, new Vector2d());
        }

        public Transform2Dd Scaled(Vector2d scale)
        {
            var copy = this;
            copy.x *= scale;
            copy.y *= scale;
            copy.origin *= scale;
            return copy;
        }

        private void ScaleBasis(Vector2d scale)
        {
            x.x *= scale.x;
            x.y *= scale.y;
            y.x *= scale.x;
            y.y *= scale.y;
        }

        private double Tdotx(Vector2d with)
        {
            return this[0, 0] * with[0] + this[1, 0] * with[1];
        }

        private double Tdoty(Vector2d with)
        {
            return this[0, 1] * with[0] + this[1, 1] * with[1];
        }

        public Transform2Dd Translated(Vector2d offset)
        {
            var copy = this;
            copy.origin += copy.BasisXform(offset);
            return copy;
        }

        public Vector2d Xform(Vector2d v)
        {
            return new Vector2d(Tdotx(v), Tdoty(v)) + origin;
        }

        public Vector2d XformInv(Vector2d v)
        {
            Vector2d vInv = v - origin;
            return new Vector2d(x.Dot(vInv), y.Dot(vInv));
        }

        // Constants
        private static readonly Transform2Dd _identity = new Transform2Dd(1, 0, 0, 1, 0, 0);
        private static readonly Transform2Dd _flipX = new Transform2Dd(-1, 0, 0, 1, 0, 0);
        private static readonly Transform2Dd _flipY = new Transform2Dd(1, 0, 0, -1, 0, 0);

        public static Transform2Dd Identity => _identity;
        public static Transform2Dd FlipX => _flipX;
        public static Transform2Dd FlipY => _flipY;

        // Constructors
        public Transform2Dd(Vector2d xAxis, Vector2d yAxis, Vector2d originPos)
        {
            x = xAxis;
            y = yAxis;
            origin = originPos;
        }

        public Transform2Dd(double xx, double xy, double yx, double yy, double ox, double oy)
        {
            x = new Vector2d(xx, xy);
            y = new Vector2d(yx, yy);
            origin = new Vector2d(ox, oy);
        }

        public Transform2Dd(double rot, Vector2d pos)
        {
            x.x = y.y = Mathd.Cos(rot);
            x.y = y.x = Mathd.Sin(rot);
            y.x *= -1;
            origin = pos;
        }

        public static explicit operator Godot.Transform2D(Transform2Dd value)
        {
            return new Godot.Transform2D((Godot.Vector2)value.x, (Godot.Vector2)value.y, (Godot.Vector2)value.origin);
        }

        public static implicit operator Transform2Dd(Godot.Transform2D value)
        {
            return new Transform2Dd(value.x, value.y, value.origin);
        }

        public static Transform2Dd operator *(Transform2Dd left, Transform2Dd right)
        {
            left.origin = left.Xform(right.origin);

            double x0 = left.Tdotx(right.x);
            double x1 = left.Tdoty(right.x);
            double y0 = left.Tdotx(right.y);
            double y1 = left.Tdoty(right.y);

            left.x.x = x0;
            left.x.y = x1;
            left.y.x = y0;
            left.y.y = y1;

            return left;
        }

        public static bool operator ==(Transform2Dd left, Transform2Dd right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Transform2Dd left, Transform2Dd right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is Transform2Dd Transform2Dd && Equals(Transform2Dd);
        }

        public bool Equals(Transform2Dd other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && origin.Equals(other.origin);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ origin.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", new object[]
            {
                x.ToString(),
                y.ToString(),
                origin.ToString()
            });
        }

        public string ToString(string format)
        {
            return String.Format("({0}, {1}, {2})", new object[]
            {
                x.ToString(format),
                y.ToString(format),
                origin.ToString(format)
            });
        }
    }
}
