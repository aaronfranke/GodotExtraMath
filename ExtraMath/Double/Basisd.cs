using System;
using System.Runtime.InteropServices;

namespace ExtraMath
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Basisd : IEquatable<Basisd>
    {
        // NOTE: x, y and z are public-only. Use Column0, Column1 and Column2 internally.

        /// <summary>
        /// Returns the basis matrix’s x vector.
        /// This is equivalent to <see cref="Column0"/>.
        /// </summary>
        public Vector3d x
        {
            get => Column0;
            set => Column0 = value;
        }

        /// <summary>
        /// Returns the basis matrix’s y vector.
        /// This is equivalent to <see cref="Column1"/>.
        /// </summary>
        public Vector3d y
        {
            get => Column1;
            set => Column1 = value;
        }

        /// <summary>
        /// Returns the basis matrix’s z vector.
        /// This is equivalent to <see cref="Column2"/>.
        /// </summary>
        public Vector3d z
        {
            get => Column2;
            set => Column2 = value;
        }

        public Vector3d Row0;
        public Vector3d Row1;
        public Vector3d Row2;

        public Vector3d Column0
        {
            get => new Vector3d(Row0.x, Row1.x, Row2.x);
            set
            {
                this.Row0.x = value.x;
                this.Row1.x = value.y;
                this.Row2.x = value.z;
            }
        }
        public Vector3d Column1
        {
            get => new Vector3d(Row0.y, Row1.y, Row2.y);
            set
            {
                this.Row0.y = value.x;
                this.Row1.y = value.y;
                this.Row2.y = value.z;
            }
        }
        public Vector3d Column2
        {
            get => new Vector3d(Row0.z, Row1.z, Row2.z);
            set
            {
                this.Row0.z = value.x;
                this.Row1.z = value.y;
                this.Row2.z = value.z;
            }
        }

        public Vector3d Scale
        {
            get
            {
                double detSign = Mathd.Sign(Determinant());
                return detSign * new Vector3d
                (
                    new Vector3d(this.Row0[0], this.Row1[0], this.Row2[0]).Length(),
                    new Vector3d(this.Row0[1], this.Row1[1], this.Row2[1]).Length(),
                    new Vector3d(this.Row0[2], this.Row1[2], this.Row2[2]).Length()
                );
            }
        }

        public Vector3d this[int columnIndex]
        {
            get
            {
                switch (columnIndex)
                {
                    case 0:
                        return Column0;
                    case 1:
                        return Column1;
                    case 2:
                        return Column2;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (columnIndex)
                {
                    case 0:
                        Column0 = value;
                        return;
                    case 1:
                        Column1 = value;
                        return;
                    case 2:
                        Column2 = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public double this[int columnIndex, int rowIndex]
        {
            get
            {
                switch (columnIndex)
                {
                    case 0:
                        return Column0[rowIndex];
                    case 1:
                        return Column1[rowIndex];
                    case 2:
                        return Column2[rowIndex];
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (columnIndex)
                {
                    case 0:
                    {
                        var column0 = Column0;
                        column0[rowIndex] = value;
                        Column0 = column0;
                        return;
                    }
                    case 1:
                    {
                        var column1 = Column1;
                        column1[rowIndex] = value;
                        Column1 = column1;
                        return;
                    }
                    case 2:
                    {
                        var column2 = Column2;
                        column2[rowIndex] = value;
                        Column2 = column2;
                        return;
                    }
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        internal Quatd RotationQuatd()
        {
            Basisd orthonormalizedBasisd = Orthonormalized();
            double det = orthonormalizedBasisd.Determinant();
            if (det < 0)
            {
                // Ensure that the determinant is 1, such that result is a proper rotation matrix which can be represented by Euler angles.
                orthonormalizedBasisd = orthonormalizedBasisd.Scaled(Vector3d.NegOne);
            }

            return orthonormalizedBasisd.Quat();
        }

        internal void SetQuatScale(Quatd quat, Vector3d scale)
        {
            SetDiagonal(scale);
            Rotate(quat);
        }

        private void Rotate(Quatd quat)
        {
            this *= new Basisd(quat);
        }

        private void SetDiagonal(Vector3d diagonal)
        {
            Row0 = new Vector3d(diagonal.x, 0, 0);
            Row1 = new Vector3d(0, diagonal.y, 0);
            Row2 = new Vector3d(0, 0, diagonal.z);

        }

        public double Determinant()
        {
            double cofac00 = Row1[1] * Row2[2] - Row1[2] * Row2[1];
            double cofac10 = Row1[2] * Row2[0] - Row1[0] * Row2[2];
            double cofac20 = Row1[0] * Row2[1] - Row1[1] * Row2[0];

            return Row0[0] * cofac00 + Row0[1] * cofac10 + Row0[2] * cofac20;
        }

        public Vector3d GetEuler()
        {
            Basisd m = Orthonormalized();

            Vector3d euler;
            euler.z = 0.0;

            double mzy = m.Row1[2];

            if (mzy < 1.0)
            {
                if (mzy > -1.0)
                {
                    euler.x = Mathd.Asin(-mzy);
                    euler.y = Mathd.Atan2(m.Row0[2], m.Row2[2]);
                    euler.z = Mathd.Atan2(m.Row1[0], m.Row1[1]);
                }
                else
                {
                    euler.x = Mathd.Pi * 0.5;
                    euler.y = -Mathd.Atan2(-m.Row0[1], m.Row0[0]);
                }
            }
            else
            {
                euler.x = -Mathd.Pi * 0.5;
                euler.y = -Mathd.Atan2(-m.Row0[1], m.Row0[0]);
            }

            return euler;
        }

        public Vector3d GetRow(int index)
        {
            switch (index)
            {
                case 0:
                    return Row0;
                case 1:
                    return Row1;
                case 2:
                    return Row2;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public void SetRow(int index, Vector3d value)
        {
            switch (index)
            {
                case 0:
                    Row0 = value;
                    return;
                case 1:
                    Row1 = value;
                    return;
                case 2:
                    Row2 = value;
                    return;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public Vector3d GetColumn(int index)
        {
            return this[index];
        }

        public void SetColumn(int index, Vector3d value)
        {
            this[index] = value;
        }

        [Obsolete("GetAxis is deprecated. Use GetColumn instead.")]
        public Vector3d GetAxis(int axis)
        {
            return new Vector3d(this.Row0[axis], this.Row1[axis], this.Row2[axis]);
        }

        public int GetOrthogonalIndex()
        {
            var orth = this;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var row = orth.GetRow(i);

                    double v = row[j];

                    if (v > 0.5)
                        v = 1.0;
                    else if (v < -0.5)
                        v = -1.0;
                    else
                        v = 0;

                    row[j] = v;

                    orth.SetRow(i, row);
                }
            }

            for (int i = 0; i < 24; i++)
            {
                if (orth == _orthoBases[i])
                {
                    return i;
                }
            }

            return 0;
        }

        public Basisd Inverse()
        {
            double cofac00 = Row1[1] * Row2[2] - Row1[2] * Row2[1];
            double cofac10 = Row1[2] * Row2[0] - Row1[0] * Row2[2];
            double cofac20 = Row1[0] * Row2[1] - Row1[1] * Row2[0];

            double det = Row0[0] * cofac00 + Row0[1] * cofac10 + Row0[2] * cofac20;

            if (det == 0)
                throw new InvalidOperationException("Matrix determinant is zero and cannot be inverted.");

            double detInv = 1.0 / det;

            double cofac01 = Row0[2] * Row2[1] - Row0[1] * Row2[2];
            double cofac02 = Row0[1] * Row1[2] - Row0[2] * Row1[1];
            double cofac11 = Row0[0] * Row2[2] - Row0[2] * Row2[0];
            double cofac12 = Row0[2] * Row1[0] - Row0[0] * Row1[2];
            double cofac21 = Row0[1] * Row2[0] - Row0[0] * Row2[1];
            double cofac22 = Row0[0] * Row1[1] - Row0[1] * Row1[0];

            return new Basisd
            (
                cofac00 * detInv, cofac01 * detInv, cofac02 * detInv,
                cofac10 * detInv, cofac11 * detInv, cofac12 * detInv,
                cofac20 * detInv, cofac21 * detInv, cofac22 * detInv
            );
        }

        public Basisd Orthonormalized()
        {
            Vector3d column0 = GetColumn(0);
            Vector3d column1 = GetColumn(1);
            Vector3d column2 = GetColumn(2);

            column0.Normalize();
            column1 = column1 - column0 * column0.Dot(column1);
            column1.Normalize();
            column2 = column2 - column0 * column0.Dot(column2) - column1 * column1.Dot(column2);
            column2.Normalize();

            return new Basisd(column0, column1, column2);
        }

        public Basisd Rotated(Vector3d axis, double phi)
        {
            return new Basisd(axis, phi) * this;
        }

        public Basisd Rotated(Vector4d axisAngle)
        {
            return new Basisd(axisAngle.XYZ, axisAngle.w) * this;
        }

        public Basisd Scaled(Vector3d scale)
        {
            var b = this;
            b.Row0 *= scale.x;
            b.Row1 *= scale.y;
            b.Row2 *= scale.z;
            return b;
        }

        public double Tdotx(Vector3d with)
        {
            return this.Row0[0] * with[0] + this.Row1[0] * with[1] + this.Row2[0] * with[2];
        }

        public double Tdoty(Vector3d with)
        {
            return this.Row0[1] * with[0] + this.Row1[1] * with[1] + this.Row2[1] * with[2];
        }

        public double Tdotz(Vector3d with)
        {
            return this.Row0[2] * with[0] + this.Row1[2] * with[1] + this.Row2[2] * with[2];
        }

        public Basisd Transposed()
        {
            var tr = this;

            double temp = tr.Row0[1];
            tr.Row0[1] = tr.Row1[0];
            tr.Row1[0] = temp;

            temp = tr.Row0[2];
            tr.Row0[2] = tr.Row2[0];
            tr.Row2[0] = temp;

            temp = tr.Row1[2];
            tr.Row1[2] = tr.Row2[1];
            tr.Row2[1] = temp;

            return tr;
        }

        public Vector3d Xform(Vector3d v)
        {
            return new Vector3d
            (
                this.Row0.Dot(v),
                this.Row1.Dot(v),
                this.Row2.Dot(v)
            );
        }

        public Vector3d XformInv(Vector3d v)
        {
            return new Vector3d
            (
                this.Row0[0] * v.x + this.Row1[0] * v.y + this.Row2[0] * v.z,
                this.Row0[1] * v.x + this.Row1[1] * v.y + this.Row2[1] * v.z,
                this.Row0[2] * v.x + this.Row1[2] * v.y + this.Row2[2] * v.z
            );
        }

        public Quatd Quat()
        {
            double trace = Row0[0] + Row1[1] + Row2[2];

            if (trace > 0.0)
            {
                double s = Mathd.Sqrt(trace + 1.0) * 2;
                double inv_s = 1 / s;
                return new Quatd(
                    (Row2[1] - Row1[2]) * inv_s,
                    (Row0[2] - Row2[0]) * inv_s,
                    (Row1[0] - Row0[1]) * inv_s,
                    s * 0.25
                );
            }

            if (Row0[0] > Row1[1] && Row0[0] > Row2[2])
            {
                double s = Mathd.Sqrt(Row0[0] - Row1[1] - Row2[2] + 1.0) * 2;
                double inv_s = 1 / s;
                return new Quatd(
                    s * 0.25,
                    (Row0[1] + Row1[0]) * inv_s,
                    (Row0[2] + Row2[0]) * inv_s,
                    (Row2[1] - Row1[2]) * inv_s
                );
            }

            if (Row1[1] > Row2[2])
            {
                double s = Mathd.Sqrt(-Row0[0] + Row1[1] - Row2[2] + 1.0) * 2;
                double inv_s = 1 / s;
                return new Quatd(
                    (Row0[1] + Row1[0]) * inv_s,
                    s * 0.25,
                    (Row1[2] + Row2[1]) * inv_s,
                    (Row0[2] - Row2[0]) * inv_s
                );
            }
            else
            {
                double s = Mathd.Sqrt(-Row0[0] - Row1[1] + Row2[2] + 1.0) * 2;
                double inv_s = 1 / s;
                return new Quatd(
                    (Row0[2] + Row2[0]) * inv_s,
                    (Row1[2] + Row2[1]) * inv_s,
                    s * 0.25,
                    (Row1[0] - Row0[1]) * inv_s
                );
            }
        }

        private static readonly Basisd[] _orthoBases = {
            new Basisd(1, 0, 0, 0, 1, 0, 0, 0, 1),
            new Basisd(0, -1, 0, 1, 0, 0, 0, 0, 1),
            new Basisd(-1, 0, 0, 0, -1, 0, 0, 0, 1),
            new Basisd(0, 1, 0, -1, 0, 0, 0, 0, 1),
            new Basisd(1, 0, 0, 0, 0, -1, 0, 1, 0),
            new Basisd(0, 0, 1, 1, 0, 0, 0, 1, 0),
            new Basisd(-1, 0, 0, 0, 0, 1, 0, 1, 0),
            new Basisd(0, 0, -1, -1, 0, 0, 0, 1, 0),
            new Basisd(1, 0, 0, 0, -1, 0, 0, 0, -1),
            new Basisd(0, 1, 0, 1, 0, 0, 0, 0, -1),
            new Basisd(-1, 0, 0, 0, 1, 0, 0, 0, -1),
            new Basisd(0, -1, 0, -1, 0, 0, 0, 0, -1),
            new Basisd(1, 0, 0, 0, 0, 1, 0, -1, 0),
            new Basisd(0, 0, -1, 1, 0, 0, 0, -1, 0),
            new Basisd(-1, 0, 0, 0, 0, -1, 0, -1, 0),
            new Basisd(0, 0, 1, -1, 0, 0, 0, -1, 0),
            new Basisd(0, 0, 1, 0, 1, 0, -1, 0, 0),
            new Basisd(0, -1, 0, 0, 0, 1, -1, 0, 0),
            new Basisd(0, 0, -1, 0, -1, 0, -1, 0, 0),
            new Basisd(0, 1, 0, 0, 0, -1, -1, 0, 0),
            new Basisd(0, 0, 1, 0, -1, 0, 1, 0, 0),
            new Basisd(0, 1, 0, 0, 0, 1, 1, 0, 0),
            new Basisd(0, 0, -1, 0, 1, 0, 1, 0, 0),
            new Basisd(0, -1, 0, 0, 0, -1, 1, 0, 0)
        };

        private static readonly Basisd _identity = new Basisd(1, 0, 0, 0, 1, 0, 0, 0, 1);
        private static readonly Basisd _flipX = new Basisd(-1, 0, 0, 0, 1, 0, 0, 0, 1);
        private static readonly Basisd _flipY = new Basisd(1, 0, 0, 0, -1, 0, 0, 0, 1);
        private static readonly Basisd _flipZ = new Basisd(1, 0, 0, 0, 1, 0, 0, 0, -1);

        public static Basisd Identity { get { return _identity; } }
        public static Basisd FlipX { get { return _flipX; } }
        public static Basisd FlipY { get { return _flipY; } }
        public static Basisd FlipZ { get { return _flipZ; } }

        public Basisd(Quatd quat)
        {
            double s = 2.0 / quat.LengthSquared;

            double xs = quat.x * s;
            double ys = quat.y * s;
            double zs = quat.z * s;
            double wx = quat.w * xs;
            double wy = quat.w * ys;
            double wz = quat.w * zs;
            double xx = quat.x * xs;
            double xy = quat.x * ys;
            double xz = quat.x * zs;
            double yy = quat.y * ys;
            double yz = quat.y * zs;
            double zz = quat.z * zs;

            Row0 = new Vector3d(1.0 - (yy + zz), xy - wz, xz + wy);
            Row1 = new Vector3d(xy + wz, 1.0 - (xx + zz), yz - wx);
            Row2 = new Vector3d(xz - wy, yz + wx, 1.0 - (xx + yy));
        }

        public Basisd(Vector3d euler)
        {
            double c;
            double s;

            c = Mathd.Cos(euler.x);
            s = Mathd.Sin(euler.x);
            var xmat = new Basisd(1, 0, 0, 0, c, -s, 0, s, c);

            c = Mathd.Cos(euler.y);
            s = Mathd.Sin(euler.y);
            var ymat = new Basisd(c, 0, s, 0, 1, 0, -s, 0, c);

            c = Mathd.Cos(euler.z);
            s = Mathd.Sin(euler.z);
            var zmat = new Basisd(c, -s, 0, s, c, 0, 0, 0, 1);

            this = ymat * xmat * zmat;
        }

        public Basisd(Vector3d axis, double phi)
        {
            var axisSq = new Vector3d(axis.x * axis.x, axis.y * axis.y, axis.z * axis.z);
            double cosine = Mathd.Cos(phi);
            Row0.x = axisSq.x + cosine * (1.0 - axisSq.x);
            Row1.y = axisSq.y + cosine * (1.0 - axisSq.y);
            Row2.z = axisSq.z + cosine * (1.0 - axisSq.z);

            double sine = Mathd.Sin(phi);
            double t = 1.0 - cosine;

            double xyzt = axis.x * axis.y * t;
            double zyxs = axis.z * sine;
            Row0.y = xyzt - zyxs;
            Row1.x = xyzt + zyxs;

            xyzt = axis.x * axis.z * t;
            zyxs = axis.y * sine;
            Row0.z = xyzt + zyxs;
            Row2.x = xyzt - zyxs;

            xyzt = axis.y * axis.z * t;
            zyxs = axis.x * sine;
            Row1.z = xyzt - zyxs;
            Row2.y = xyzt + zyxs;
        }

        public Basisd(Vector3d column0, Vector3d column1, Vector3d column2)
        {
            Row0 = new Vector3d(column0.x, column1.x, column2.x);
            Row1 = new Vector3d(column0.y, column1.y, column2.y);
            Row2 = new Vector3d(column0.z, column1.z, column2.z);
            // Same as:
            // Column0 = column0;
            // Column1 = column1;
            // Column2 = column2;
            // We need to assign the struct fields here first so we can't do it that way...
        }

        // Arguments are named such that xy is equal to calling x.y
        internal Basisd(double xx, double yx, double zx, double xy, double yy, double zy, double xz, double yz, double zz)
        {
            Row0 = new Vector3d(xx, yx, zx);
            Row1 = new Vector3d(xy, yy, zy);
            Row2 = new Vector3d(xz, yz, zz);
        }

        public static explicit operator Godot.Basis(Basisd value)
        {
            return new Godot.Basis((Godot.Vector3)value.x, (Godot.Vector3)value.y, (Godot.Vector3)value.z);
        }

        public static implicit operator Basisd(Godot.Basis value)
        {
            return new Basisd(value.x, value.y, value.z);
        }

        public static Basisd operator *(Basisd left, Basisd right)
        {
            return new Basisd
            (
                right.Tdotx(left.Row0), right.Tdoty(left.Row0), right.Tdotz(left.Row0),
                right.Tdotx(left.Row1), right.Tdoty(left.Row1), right.Tdotz(left.Row1),
                right.Tdotx(left.Row2), right.Tdoty(left.Row2), right.Tdotz(left.Row2)
            );
        }

        public static bool operator ==(Basisd left, Basisd right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Basisd left, Basisd right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Basisd)
            {
                return Equals((Basisd)obj);
            }

            return false;
        }

        public bool Equals(Basisd other)
        {
            return Row0.Equals(other.Row0) && Row1.Equals(other.Row1) && Row2.Equals(other.Row2);
        }

        public override int GetHashCode()
        {
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", new object[]
            {
                Row0.ToString(),
                Row1.ToString(),
                Row2.ToString()
            });
        }

        public string ToString(string format)
        {
            return String.Format("({0}, {1}, {2})", new object[]
            {
                Row0.ToString(format),
                Row1.ToString(format),
                Row2.ToString(format)
            });
        }
    }
}
