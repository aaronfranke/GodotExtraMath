using System;
using System.Runtime.InteropServices;

#if GODOT_REAL_T_IS_DOUBLE
using real_t = System.Double;
#else
using real_t = System.Single;
#endif

namespace ExtraMath
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Quatd : IEquatable<Quatd>
    {
        public double x;
        public double y;
        public double z;
        public double w;

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
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public double Length
        {
            get { return Mathd.Sqrt(LengthSquared); }
        }

        public double LengthSquared
        {
            get { return Dot(this); }
        }

        public Quatd CubicSlerp(Quatd b, Quatd preA, Quatd postB, double t)
        {
            double t2 = (1.0f - t) * t * 2f;
            Quatd sp = Slerp(b, t);
            Quatd sq = preA.Slerpni(postB, t);
            return sp.Slerpni(sq, t2);
        }

        public double Dot(Quatd b)
        {
            return x * b.x + y * b.y + z * b.z + w * b.w;
        }

        public Vector3d GetEuler()
        {
#if DEBUG
            if (!IsNormalized())
                throw new InvalidOperationException("Quat is not normalized");
#endif
            var basis = new Basisd(this);
            return basis.GetEuler();
        }

        public Quatd Inverse()
        {
#if DEBUG
            if (!IsNormalized())
                throw new InvalidOperationException("Quat is not normalized");
#endif
            return new Quatd(-x, -y, -z, w);
        }

        public Quatd Normalized()
        {
            return this / Length;
        }

        public Quatd Slerp(Quatd b, double t)
        {
#if DEBUG
            if (!IsNormalized())
                throw new InvalidOperationException("Quat is not normalized");
            if (!b.IsNormalized())
                throw new ArgumentException("Argument is not normalized", nameof(b));
#endif
            // Calculate cosine
            double cosom = x * b.x + y * b.y + z * b.z + w * b.w;

            var to1 = new Quatd();

            // Adjust signs if necessary
            if (cosom < 0.0)
            {
                cosom = -cosom;
                to1.x = -b.x;
                to1.y = -b.y;
                to1.z = -b.z;
                to1.w = -b.w;
            }
            else
            {
                to1.x = b.x;
                to1.y = b.y;
                to1.z = b.z;
                to1.w = b.w;
            }

            double sinom, scale0, scale1;

            // Calculate coefficients
            if (1.0 - cosom > Mathd.Epsilon)
            {
                // Standard case (Slerp)
                double omega = Mathd.Acos(cosom);
                sinom = Mathd.Sin(omega);
                scale0 = Mathd.Sin((1.0f - t) * omega) / sinom;
                scale1 = Mathd.Sin(t * omega) / sinom;
            }
            else
            {
                // Quatdernions are very close so we can do a linear interpolation
                scale0 = 1.0f - t;
                scale1 = t;
            }

            // Calculate final values
            return new Quatd
            (
                scale0 * x + scale1 * to1.x,
                scale0 * y + scale1 * to1.y,
                scale0 * z + scale1 * to1.z,
                scale0 * w + scale1 * to1.w
            );
        }

        public Quatd Slerpni(Quatd b, double t)
        {
            double dot = Dot(b);

            if (Mathd.Abs(dot) > 0.9999f)
            {
                return this;
            }

            double theta = Mathd.Acos(dot);
            double sinT = 1.0f / Mathd.Sin(theta);
            double newFactor = Mathd.Sin(t * theta) * sinT;
            double invFactor = Mathd.Sin((1.0f - t) * theta) * sinT;

            return new Quatd
            (
                invFactor * x + newFactor * b.x,
                invFactor * y + newFactor * b.y,
                invFactor * z + newFactor * b.z,
                invFactor * w + newFactor * b.w
            );
        }

        public Vector3d Xform(Vector3d v)
        {
#if DEBUG
            if (!IsNormalized())
                throw new InvalidOperationException("Quat is not normalized");
#endif
            var u = new Vector3d(x, y, z);
            Vector3d uv = u.Cross(v);
            return v + ((uv * w) + u.Cross(uv)) * 2;
        }

        // Static Readonly Properties
        public static Quatd Identity { get; } = new Quatd(0f, 0f, 0f, 1f);

        // Constructors
        public Quatd(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public bool IsNormalized()
        {
            return Mathd.Abs(LengthSquared - 1) <= Mathd.Epsilon;
        }

        public Quatd(Quatd q)
        {
            this = q;
        }

        public Quatd(Basisd basis)
        {
            this = basis.Quat();
        }

        public Quatd(Vector3d eulerYXZ)
        {
            double half_a1 = eulerYXZ.y * (double)0.5;
            double half_a2 = eulerYXZ.x * (double)0.5;
            double half_a3 = eulerYXZ.z * (double)0.5;

            // R = Y(a1).X(a2).Z(a3) convention for Euler angles.
            // Conversion to Quatdernion as listed in https://ntrs.nasa.gov/archive/nasa/casi.ntrs.nasa.gov/19770024290.pdf (page A-6)
            // a3 is the angle of the first rotation, following the notation in this reference.

            double cos_a1 = Mathd.Cos(half_a1);
            double sin_a1 = Mathd.Sin(half_a1);
            double cos_a2 = Mathd.Cos(half_a2);
            double sin_a2 = Mathd.Sin(half_a2);
            double cos_a3 = Mathd.Cos(half_a3);
            double sin_a3 = Mathd.Sin(half_a3);

            x = sin_a1 * cos_a2 * sin_a3 + cos_a1 * sin_a2 * cos_a3;
            y = sin_a1 * cos_a2 * cos_a3 - cos_a1 * sin_a2 * sin_a3;
            z = -sin_a1 * sin_a2 * cos_a3 + cos_a1 * cos_a2 * sin_a3;
            w = sin_a1 * sin_a2 * sin_a3 + cos_a1 * cos_a2 * cos_a3;
        }

        public Quatd(Vector3d axis, double angle)
        {
#if DEBUG
            if (!axis.IsNormalized())
                throw new ArgumentException("Argument is not normalized", nameof(axis));
#endif
            double d = axis.Length();
            double angle_t = angle;

            if (d == 0f)
            {
                x = 0f;
                y = 0f;
                z = 0f;
                w = 0f;
            }
            else
            {
                double sinAngle = Mathd.Sin(angle * 0.5);
                double cosAngle = Mathd.Cos(angle * 0.5);
                double s = sinAngle / d;

                x = axis.x * s;
                y = axis.y * s;
                z = axis.z * s;
                w = cosAngle;
            }
        }

        public static explicit operator Godot.Quat(Quatd value)
        {
            return new Godot.Quat((real_t)value.x, (real_t)value.y, (real_t)value.z, (real_t)value.w);
        }

        public static implicit operator Quatd(Godot.Quat value)
        {
            return new Quatd(value.x, value.y, value.z, value.w);
        }

        public static Quatd operator *(Quatd left, Quatd right)
        {
            return new Quatd
            (
                left.w * right.x + left.x * right.w + left.y * right.z - left.z * right.y,
                left.w * right.y + left.y * right.w + left.z * right.x - left.x * right.z,
                left.w * right.z + left.z * right.w + left.x * right.y - left.y * right.x,
                left.w * right.w - left.x * right.x - left.y * right.y - left.z * right.z
            );
        }

        public static Quatd operator +(Quatd left, Quatd right)
        {
            return new Quatd(left.x + right.x, left.y + right.y, left.z + right.z, left.w + right.w);
        }

        public static Quatd operator -(Quatd left, Quatd right)
        {
            return new Quatd(left.x - right.x, left.y - right.y, left.z - right.z, left.w - right.w);
        }

        public static Quatd operator -(Quatd left)
        {
            return new Quatd(-left.x, -left.y, -left.z, -left.w);
        }

        public static Quatd operator *(Quatd left, Vector3d right)
        {
            return new Quatd
            (
                left.w * right.x + left.y * right.z - left.z * right.y,
                left.w * right.y + left.z * right.x - left.x * right.z,
                left.w * right.z + left.x * right.y - left.y * right.x,
                -left.x * right.x - left.y * right.y - left.z * right.z
            );
        }

        public static Quatd operator *(Vector3d left, Quatd right)
        {
            return new Quatd
            (
                right.w * left.x + right.y * left.z - right.z * left.y,
                right.w * left.y + right.z * left.x - right.x * left.z,
                right.w * left.z + right.x * left.y - right.y * left.x,
                -right.x * left.x - right.y * left.y - right.z * left.z
            );
        }

        public static Quatd operator *(Quatd left, double right)
        {
            return new Quatd(left.x * right, left.y * right, left.z * right, left.w * right);
        }

        public static Quatd operator *(double left, Quatd right)
        {
            return new Quatd(right.x * left, right.y * left, right.z * left, right.w * left);
        }

        public static Quatd operator /(Quatd left, double right)
        {
            return left * (1.0f / right);
        }

        public static bool operator ==(Quatd left, Quatd right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Quatd left, Quatd right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Quatd)
            {
                return Equals((Quatd)obj);
            }

            return false;
        }

        public bool Equals(Quatd other)
        {
            return Mathd.IsEqualApprox(x, other.x) && Mathd.IsEqualApprox(y, other.y) && Mathd.IsEqualApprox(z, other.z) && Mathd.IsEqualApprox(w, other.w);
        }

        public override int GetHashCode()
        {
            return y.GetHashCode() ^ x.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2}, {3})", x.ToString(), y.ToString(), z.ToString(), w.ToString());
        }

        public string ToString(string format)
        {
            return String.Format("({0}, {1}, {2}, {3})", x.ToString(format), y.ToString(format), z.ToString(format), w.ToString(format));
        }
    }
}
