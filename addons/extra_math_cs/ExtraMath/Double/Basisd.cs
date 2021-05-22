using System;
using System.Runtime.InteropServices;

namespace ExtraMath
{
    /// <summary>
    /// 3×3 matrix used for 3D rotation and scale.
    /// Almost always used as an orthogonal basis for a Transform.
    ///
    /// Contains 3 vector fields X, Y and Z as its columns, which are typically
    /// interpreted as the local basis vectors of a 3D transformation. For such use,
    /// it is composed of a scaling and a rotation matrix, in that order (M = R.S).
    ///
    /// Can also be accessed as array of 3D vectors. These vectors are normally
    /// orthogonal to each other, but are not necessarily normalized (due to scaling).
    ///
    /// For more information, read this documentation article:
    /// https://docs.godotengine.org/en/latest/tutorials/math/matrices_and_transforms.html
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Basisd : IEquatable<Basisd>
    {
        // NOTE: x, y and z are public-only. Use Column0, Column1 and Column2 internally.

        /// <summary>
        /// The basis matrix's X vector (column 0).
        /// </summary>
        /// <value>Equivalent to <see cref="Column0"/> and array index `[0]`.</value>
        public Vector3d x
        {
            get => Column0;
            set => Column0 = value;
        }

        /// <summary>
        /// The basis matrix's Y vector (column 1).
        /// </summary>
        /// <value>Equivalent to <see cref="Column1"/> and array index `[1]`.</value>
        public Vector3d y
        {
            get => Column1;
            set => Column1 = value;
        }

        /// <summary>
        /// The basis matrix's Z vector (column 2).
        /// </summary>
        /// <value>Equivalent to <see cref="Column2"/> and array index `[2]`.</value>
        public Vector3d z
        {
            get => Column2;
            set => Column2 = value;
        }

        /// <summary>
        /// Row 0 of the basis matrix. Shows which vectors contribute
        /// to the X direction. Rows are not very useful for user code,
        /// but are more efficient for some internal calculations.
        /// </summary>
        public Vector3d Row0;

        /// <summary>
        /// Row 1 of the basis matrix. Shows which vectors contribute
        /// to the Y direction. Rows are not very useful for user code,
        /// but are more efficient for some internal calculations.
        /// </summary>
        public Vector3d Row1;

        /// <summary>
        /// Row 2 of the basis matrix. Shows which vectors contribute
        /// to the Z direction. Rows are not very useful for user code,
        /// but are more efficient for some internal calculations.
        /// </summary>
        public Vector3d Row2;

        /// <summary>
        /// Column 0 of the basis matrix (the X vector).
        /// </summary>
        /// <value>Equivalent to <see cref="x"/> and array index `[0]`.</value>
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

        /// <summary>
        /// Column 1 of the basis matrix (the Y vector).
        /// </summary>
        /// <value>Equivalent to <see cref="y"/> and array index `[1]`.</value>
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

        /// <summary>
        /// Column 2 of the basis matrix (the Z vector).
        /// </summary>
        /// <value>Equivalent to <see cref="z"/> and array index `[2]`.</value>
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

        /// <summary>
        /// The scale of this basis.
        /// </summary>
        /// <value>Equivalent to the lengths of each column vector, but negative if the determinant is negative.</value>
        public Vector3d Scale
        {
            get
            {
                double detSign = Mathd.Sign(Determinant());
                return detSign * new Vector3d
                (
                    Column0.Length(),
                    Column1.Length(),
                    Column2.Length()
                );
            }
            set
            {
                value /= Scale; // Value becomes what's called "delta_scale" in core.
                Column0 *= value.x;
                Column1 *= value.y;
                Column2 *= value.z;
            }
        }

        /// <summary>
        /// Access whole columns in the form of Vector3d.
        /// </summary>
        /// <param name="column">Which column vector.</param>
        public Vector3d this[int column]
        {
            get
            {
                switch (column)
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
                switch (column)
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

        /// <summary>
        /// Access matrix elements in column-major order.
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
                Vector3d columnVector = this[column];
                columnVector[row] = value;
                this[column] = columnVector;
            }
        }

        public Quatd RotationQuat()
        {
            Basisd orthonormalizedBasisd = Orthonormalized();
            double det = orthonormalizedBasisd.Determinant();
            if (det < 0)
            {
                // Ensure that the determinant is 1, such that result is a proper
                // rotation matrix which can be represented by Euler angles.
                orthonormalizedBasisd = orthonormalizedBasisd.Scaled(-Vector3d.One);
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

        /// <summary>
        /// Returns the determinant of the basis matrix. If the basis is
        /// uniformly scaled, its determinant is the square of the scale.
        ///
        /// A negative determinant means the basis has a negative scale.
        /// A zero determinant means the basis isn't invertible,
        /// and is usually considered invalid.
        /// </summary>
        /// <returns>The determinant of the basis matrix.</returns>
        public double Determinant()
        {
            double cofac00 = Row1[1] * Row2[2] - Row1[2] * Row2[1];
            double cofac10 = Row1[2] * Row2[0] - Row1[0] * Row2[2];
            double cofac20 = Row1[0] * Row2[1] - Row1[1] * Row2[0];

            return Row0[0] * cofac00 + Row0[1] * cofac10 + Row0[2] * cofac20;
        }

        /// <summary>
        /// Returns the basis's rotation in the form of Euler angles
        /// (in the YXZ convention: when *decomposing*, first Z, then X, and Y last).
        /// The returned vector contains the rotation angles in
        /// the format (X angle, Y angle, Z angle).
        ///
        /// Consider using the <see cref="Basisd.Quatd()"/> method instead, which
        /// returns a <see cref="ExtraMath.Quatd"/> quaternion instead of Euler angles.
        /// </summary>
        /// <returns>A Vector3d representing the basis rotation in Euler angles.</returns>
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

        /// <summary>
        /// Get rows by index. Rows are not very useful for user code,
        /// but are more efficient for some internal calculations.
        /// </summary>
        /// <param name="index">Which row.</param>
        /// <returns>One of `Row0`, `Row1`, or `Row2`.</returns>
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

        /// <summary>
        /// Sets rows by index. Rows are not very useful for user code,
        /// but are more efficient for some internal calculations.
        /// </summary>
        /// <param name="index">Which row.</param>
        /// <param name="value">The vector to set the row to.</param>
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

        /// <summary>
        /// This function considers a discretization of rotations into
        /// 24 points on unit sphere, lying along the vectors (x, y, z) with
        /// each component being either -1, 0, or 1, and returns the index
        /// of the point best representing the orientation of the object.
        /// It is mainly used by the <see cref="GridMap"/> editor.
        ///
        /// For further details, refer to the Godot source code.
        /// </summary>
        /// <returns>The orthogonal index.</returns>
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
                    {
                        v = 1.0;
                    }
                    else if (v < -0.5)
                    {
                        v = -1.0;
                    }
                    else
                    {
                        v = 0;
                    }

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

        /// <summary>
        /// Returns the inverse of the matrix.
        /// </summary>
        /// <returns>The inverse matrix.</returns>
        public Basisd Inverse()
        {
            double cofac00 = Row1[1] * Row2[2] - Row1[2] * Row2[1];
            double cofac10 = Row1[2] * Row2[0] - Row1[0] * Row2[2];
            double cofac20 = Row1[0] * Row2[1] - Row1[1] * Row2[0];

            double det = Row0[0] * cofac00 + Row0[1] * cofac10 + Row0[2] * cofac20;

            if (det == 0)
            {
                throw new InvalidOperationException("Matrix determinant is zero and cannot be inverted.");
            }

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

        /// <summary>
        /// Returns the orthonormalized version of the basis matrix (useful to
        /// call occasionally to avoid rounding errors for orthogonal matrices).
        /// This performs a Gram-Schmidt orthonormalization on the basis of the matrix.
        /// </summary>
        /// <returns>An orthonormalized basis matrix.</returns>
        public Basisd Orthonormalized()
        {
            Vector3d column0 = this[0];
            Vector3d column1 = this[1];
            Vector3d column2 = this[2];

            column0.Normalize();
            column1 = column1 - column0 * column0.Dot(column1);
            column1.Normalize();
            column2 = column2 - column0 * column0.Dot(column2) - column1 * column1.Dot(column2);
            column2.Normalize();

            return new Basisd(column0, column1, column2);
        }

        /// <summary>
        /// Introduce an additional rotation around the given `axis`
        /// by `phi` (in radians). The axis must be a normalized vector.
        /// </summary>
        /// <param name="axis">The axis to rotate around. Must be normalized.</param>
        /// <param name="phi">The angle to rotate, in radians.</param>
        /// <returns>The rotated basis matrix.</returns>
        public Basisd Rotated(Vector3d axis, double phi)
        {
            return new Basisd(axis, phi) * this;
        }

        /// <summary>
        /// Introduce an additional rotation around the given `axisAngle` Vector4.
        /// The axis must be a normalized vector, and the angle in radians.
        /// </summary>
        /// <param name="axisAngle">The axisAngle to rotate by.</param>
        /// <returns>The rotated basis matrix.</returns>
        public Basisd Rotated(Vector4d axisAngle)
        {
            return new Basisd(axisAngle.XYZ, axisAngle.w) * this;
        }

        /// <summary>
        /// Introduce an additional scaling specified by the given 3D scaling factor.
        /// </summary>
        /// <param name="scale">The scale to introduce.</param>
        /// <returns>The scaled basis matrix.</returns>
        public Basisd Scaled(Vector3d scale)
        {
            Basisd b = this;
            b.Row0 *= scale.x;
            b.Row1 *= scale.y;
            b.Row2 *= scale.z;
            return b;
        }

        /// <summary>
        /// Assuming that the matrix is a proper rotation matrix, slerp performs
        /// a spherical-linear interpolation with another rotation matrix.
        /// </summary>
        /// <param name="target">The destination basis for interpolation.</param>
        /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The resulting basis matrix of the interpolation.</returns>
        public Basisd Slerp(Basisd target, double weight)
        {
            Quatd from = new Quatd(this);
            Quatd to = new Quatd(target);

            Basisd b = new Basisd(from.Slerp(to, weight));
            b.Row0 *= Mathd.Lerp(Row0.Length(), target.Row0.Length(), weight);
            b.Row1 *= Mathd.Lerp(Row1.Length(), target.Row1.Length(), weight);
            b.Row2 *= Mathd.Lerp(Row2.Length(), target.Row2.Length(), weight);

            return b;
        }

        /// <summary>
        /// Transposed dot product with the X axis of the matrix.
        /// </summary>
        /// <param name="with">A vector to calculate the dot product with.</param>
        /// <returns>The resulting dot product.</returns>
        public double Tdotx(Vector3d with)
        {
            return this.Row0[0] * with[0] + this.Row1[0] * with[1] + this.Row2[0] * with[2];
        }

        /// <summary>
        /// Transposed dot product with the Y axis of the matrix.
        /// </summary>
        /// <param name="with">A vector to calculate the dot product with.</param>
        /// <returns>The resulting dot product.</returns>
        public double Tdoty(Vector3d with)
        {
            return this.Row0[1] * with[0] + this.Row1[1] * with[1] + this.Row2[1] * with[2];
        }

        /// <summary>
        /// Transposed dot product with the Z axis of the matrix.
        /// </summary>
        /// <param name="with">A vector to calculate the dot product with.</param>
        /// <returns>The resulting dot product.</returns>
        public double Tdotz(Vector3d with)
        {
            return this.Row0[2] * with[0] + this.Row1[2] * with[1] + this.Row2[2] * with[2];
        }

        /// <summary>
        /// Returns the transposed version of the basis matrix.
        /// </summary>
        /// <returns>The transposed basis matrix.</returns>
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

        /// <summary>
        /// Returns a vector transformed (multiplied) by the basis matrix.
        /// </summary>
        /// <param name="v">A vector to transform.</param>
        /// <returns>The transformed vector.</returns>
        public Vector3d Xform(Vector3d v)
        {
            return new Vector3d
            (
                this.Row0.Dot(v),
                this.Row1.Dot(v),
                this.Row2.Dot(v)
            );
        }

        /// <summary>
        /// Returns a vector transformed (multiplied) by the transposed basis matrix.
        ///
        /// Note: This results in a multiplication by the inverse of the
        /// basis matrix only if it represents a rotation-reflection.
        /// </summary>
        /// <param name="v">A vector to inversely transform.</param>
        /// <returns>The inversely transformed vector.</returns>
        public Vector3d XformInv(Vector3d v)
        {
            return new Vector3d
            (
                this.Row0[0] * v.x + this.Row1[0] * v.y + this.Row2[0] * v.z,
                this.Row0[1] * v.x + this.Row1[1] * v.y + this.Row2[1] * v.z,
                this.Row0[2] * v.x + this.Row1[2] * v.y + this.Row2[2] * v.z
            );
        }

        /// <summary>
        /// Returns the basis's rotation in the form of a quaternion.
        /// See <see cref="GetEuler()"/> if you need Euler angles, but keep in
        /// mind that quaternions should generally be preferred to Euler angles.
        /// </summary>
        /// <returns>A <see cref="ExtraMath.Quatd"/> representing the basis's rotation.</returns>
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

        /// <summary>
        /// The identity basis, with no rotation or scaling applied.
        /// This is used as a replacement for `Basisd()` in GDScript.
        /// Do not use `new Basisd()` with no arguments in C#, because it sets all values to zero.
        /// </summary>
        /// <value>Equivalent to `new Basisd(Vector3d.Right, Vector3d.Up, Vector3d.Back)`.</value>
        public static Basisd Identity { get { return _identity; } }
        /// <summary>
        /// The basis that will flip something along the X axis when used in a transformation.
        /// </summary>
        /// <value>Equivalent to `new Basisd(Vector3d.Left, Vector3d.Up, Vector3d.Back)`.</value>
        public static Basisd FlipX { get { return _flipX; } }
        /// <summary>
        /// The basis that will flip something along the Y axis when used in a transformation.
        /// </summary>
        /// <value>Equivalent to `new Basisd(Vector3d.Right, Vector3d.Down, Vector3d.Back)`.</value>
        public static Basisd FlipY { get { return _flipY; } }
        /// <summary>
        /// The basis that will flip something along the Z axis when used in a transformation.
        /// </summary>
        /// <value>Equivalent to `new Basisd(Vector3d.Right, Vector3d.Up, Vector3d.Forward)`.</value>
        public static Basisd FlipZ { get { return _flipZ; } }

        /// <summary>
        /// Constructs a pure rotation basis matrix from the given quaternion.
        /// </summary>
        /// <param name="quat">The quaternion to create the basis from.</param>
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

        /// <summary>
        /// Constructs a pure rotation basis matrix from the given Euler angles
        /// (in the YXZ convention: when *composing*, first Y, then X, and Z last),
        /// given in the vector format as (X angle, Y angle, Z angle).
        ///
        /// Consider using the <see cref="Basisd(Quatd)"/> constructor instead, which
        /// uses a <see cref="ExtraMath.Quatd"/> quaternion instead of Euler angles.
        /// </summary>
        /// <param name="eulerYXZ">The Euler angles to create the basis from.</param>
        public Basisd(Vector3d eulerYXZ)
        {
            double c;
            double s;

            c = Mathd.Cos(eulerYXZ.x);
            s = Mathd.Sin(eulerYXZ.x);
            var xmat = new Basisd(1, 0, 0, 0, c, -s, 0, s, c);

            c = Mathd.Cos(eulerYXZ.y);
            s = Mathd.Sin(eulerYXZ.y);
            var ymat = new Basisd(c, 0, s, 0, 1, 0, -s, 0, c);

            c = Mathd.Cos(eulerYXZ.z);
            s = Mathd.Sin(eulerYXZ.z);
            var zmat = new Basisd(c, -s, 0, s, c, 0, 0, 0, 1);

            this = ymat * xmat * zmat;
        }

        /// <summary>
        /// Constructs a pure rotation basis matrix, rotated around the given `axis`
        /// by `phi` (in radians). The axis must be a normalized vector.
        /// </summary>
        /// <param name="axis">The axis to rotate around. Must be normalized.</param>
        /// <param name="phi">The angle to rotate, in radians.</param>
        public Basisd(Vector3d axis, double phi)
        {
            Vector3d axisSq = new Vector3d(axis.x * axis.x, axis.y * axis.y, axis.z * axis.z);
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

        /// <summary>
        /// Constructs a basis matrix from 3 axis vectors (matrix columns).
        /// </summary>
        /// <param name="column0">The X vector, or Column0.</param>
        /// <param name="column1">The Y vector, or Column1.</param>
        /// <param name="column2">The Z vector, or Column2.</param>
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

#if GODOT
        public static explicit operator Godot.Basis(Basisd value)
        {
            return new Godot.Basis((Godot.Vector3)value.x, (Godot.Vector3)value.y, (Godot.Vector3)value.z);
        }

        public static implicit operator Basisd(Godot.Basis value)
        {
            return new Basisd(value.x, value.y, value.z);
        }
#endif

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

        /// <summary>
        /// Returns true if this basis and `other` are approximately equal, by running
        /// <see cref="Vector3d.IsEqualApprox(Vector3d)"/> on each component.
        /// </summary>
        /// <param name="other">The other basis to compare.</param>
        /// <returns>Whether or not the matrices are approximately equal.</returns>
        public bool IsEqualApprox(Basisd other)
        {
            return Row0.IsEqualApprox(other.Row0) && Row1.IsEqualApprox(other.Row1) && Row2.IsEqualApprox(other.Row2);
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
