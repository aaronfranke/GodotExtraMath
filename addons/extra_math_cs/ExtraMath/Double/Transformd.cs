using System;
using System.Runtime.InteropServices;

namespace ExtraMath
{
    /// <summary>
    /// 3×4 matrix (3 rows, 4 columns) used for 3D linear transformations.
    /// It can represent transformations such as translation, rotation, or scaling.
    /// It consists of a <see cref="Basisd"/> (first 3 columns) and a
    /// <see cref="Vector3d"/> for the origin (last column).
    ///
    /// For more information, read this documentation article:
    /// https://docs.godotengine.org/en/latest/tutorials/math/matrices_and_transforms.html
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Transformd : IEquatable<Transformd>
    {
        /// <summary>
        /// The <see cref="Basisd"/> of this transform. Contains the X, Y, and Z basis
        /// vectors (columns 0 to 2) and is responsible for rotation and scale.
        /// </summary>
        public Basisd basis;

        /// <summary>
        /// The origin vector (column 3, the fourth column). Equivalent to array index `[3]`.
        /// </summary>
        public Vector3d origin;

        /// <summary>
        /// Access whole columns in the form of Vector3d. The fourth column is the origin vector.
        /// </summary>
        /// <param name="column">Which column vector.</param>
        public Vector3d this[int column]
        {
            get
            {
                switch (column)
                {
                    case 0:
                        return basis.Column0;
                    case 1:
                        return basis.Column1;
                    case 2:
                        return basis.Column2;
                    case 3:
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
                        basis.Column0 = value;
                        return;
                    case 1:
                        basis.Column1 = value;
                        return;
                    case 2:
                        basis.Column2 = value;
                        return;
                    case 3:
                        origin = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Access matrix elements in column-major order. The fourth column is the origin vector.
        /// </summary>
        /// <param name="column">Which column, the matrix horizontal position.</param>
        /// <param name="row">Which row, the matrix vertical position.</param>
        public double this[int column, int row]
        {
            get
            {
                if (column == 3)
                {
                    return origin[row];
                }
                return basis[column, row];
            }
            set
            {
                if (column == 3)
                {
                    origin[row] = value;
                    return;
                }
                basis[column, row] = value;
            }
        }

        /// <summary>
        /// Returns the inverse of the transform, under the assumption that
        /// the transformation is composed of rotation, scaling, and translation.
        /// </summary>
        /// <returns>The inverse transformation matrix.</returns>
        public Transformd AffineInverse()
        {
            Basisd basisInv = basis.Inverse();
            return new Transformd(basisInv, basisInv.Xform(-origin));
        }

        /// <summary>
        /// Interpolates this transform to the other `transform` by `weight`.
        /// </summary>
        /// <param name="transform">The other transform.</param>
        /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
        /// <returns>The interpolated transform.</returns>
        public Transformd InterpolateWith(Transformd transform, double weight)
        {
            /* not sure if very "efficient" but good enough? */

            Vector3d sourceScale = basis.Scale;
            Quatd sourceRotation = basis.RotationQuat();
            Vector3d sourceLocation = origin;

            Vector3d destinationScale = transform.basis.Scale;
            Quatd destinationRotation = transform.basis.RotationQuat();
            Vector3d destinationLocation = transform.origin;

            var interpolated = new Transformd();
            interpolated.basis.SetQuatScale(sourceRotation.Slerp(destinationRotation, weight).Normalized(), sourceScale.Lerp(destinationScale, weight));
            interpolated.origin = sourceLocation.Lerp(destinationLocation, weight);

            return interpolated;
        }

        /// <summary>
        /// Returns the inverse of the transform, under the assumption that
        /// the transformation is composed of rotation and translation
        /// (no scaling, use <see cref="AffineInverse"/> for transforms with scaling).
        /// </summary>
        /// <returns>The inverse matrix.</returns>
        public Transformd Inverse()
        {
            Basisd basisTr = basis.Transposed();
            return new Transformd(basisTr, basisTr.Xform(-origin));
        }

        /// <summary>
        /// Returns a copy of the transform rotated such that its
        /// -Z axis (forward) points towards the target position.
        ///
        /// The transform will first be rotated around the given up vector,
        /// and then fully aligned to the target by a further rotation around
        /// an axis perpendicular to both the target and up vectors.
        ///
        /// Operations take place in global space.
        /// </summary>
        /// <param name="target">The object to look at.</param>
        /// <param name="up">The relative up direction</param>
        /// <returns>The resulting transform.</returns>
        public Transformd LookingAt(Vector3d target, Vector3d up)
        {
            var t = this;
            t.SetLookAt(origin, target, up);
            return t;
        }

        /// <summary>
        /// Returns the transform with the basis orthogonal (90 degrees),
        /// and normalized axis vectors (scale of 1 or -1).
        /// </summary>
        /// <returns>The orthonormalized transform.</returns>
        public Transformd Orthonormalized()
        {
            return new Transformd(basis.Orthonormalized(), origin);
        }

        /// <summary>
        /// Rotates the transform around the given `axis` by `phi` (in radians),
        /// using matrix multiplication. The axis must be a normalized vector.
        /// </summary>
        /// <param name="axis">The axis to rotate around. Must be normalized.</param>
        /// <param name="phi">The angle to rotate, in radians.</param>
        /// <returns>The rotated transformation matrix.</returns>
        public Transformd Rotated(Vector3d axis, double phi)
        {
            return new Transformd(new Basisd(axis, phi), new Vector3d()) * this;
        }

        /// <summary>
        /// Scales the transform by the given 3D scaling factor, using matrix multiplication.
        /// </summary>
        /// <param name="scale">The scale to introduce.</param>
        /// <returns>The scaled transformation matrix.</returns>
        public Transformd Scaled(Vector3d scale)
        {
            return new Transformd(basis.Scaled(scale), origin * scale);
        }

        private void SetLookAt(Vector3d eye, Vector3d target, Vector3d up)
        {
            // Make rotation matrix
            // Z vector
            Vector3d column2 = eye - target;

            column2.Normalize();

            Vector3d column1 = up;

            Vector3d column0 = column1.Cross(column2);

            // Recompute Y = Z cross X
            column1 = column2.Cross(column0);

            column0.Normalize();
            column1.Normalize();

            basis = new Basisd(column0, column1, column2);

            origin = eye;
        }

        /// <summary>
        /// Translates the transform by the given `offset`,
        /// relative to the transform's basis vectors.
        ///
        /// Unlike <see cref="Rotated"/> and <see cref="Scaled"/>,
        /// this does not use matrix multiplication.
        /// </summary>
        /// <param name="offset">The offset to translate by.</param>
        /// <returns>The translated matrix.</returns>
        public Transformd Translated(Vector3d offset)
        {
            return new Transformd(basis, new Vector3d
            (
                origin[0] += basis.Row0.Dot(offset),
                origin[1] += basis.Row1.Dot(offset),
                origin[2] += basis.Row2.Dot(offset)
            ));
        }

        /// <summary>
        /// Returns a vector transformed (multiplied) by this transformation matrix.
        /// </summary>
        /// <param name="v">A vector to transform.</param>
        /// <returns>The transformed vector.</returns>
        public Vector3d Xform(Vector3d v)
        {
            return new Vector3d
            (
                basis.Row0.Dot(v) + origin.x,
                basis.Row1.Dot(v) + origin.y,
                basis.Row2.Dot(v) + origin.z
            );
        }

        /// <summary>
        /// Returns a vector transformed (multiplied) by the transposed transformation matrix.
        ///
        /// Note: This results in a multiplication by the inverse of the
        /// transformation matrix only if it represents a rotation-reflection.
        /// </summary>
        /// <param name="v">A vector to inversely transform.</param>
        /// <returns>The inversely transformed vector.</returns>
        public Vector3d XformInv(Vector3d v)
        {
            Vector3d vInv = v - origin;

            return new Vector3d
            (
                basis.Row0[0] * vInv.x + basis.Row1[0] * vInv.y + basis.Row2[0] * vInv.z,
                basis.Row0[1] * vInv.x + basis.Row1[1] * vInv.y + basis.Row2[1] * vInv.z,
                basis.Row0[2] * vInv.x + basis.Row1[2] * vInv.y + basis.Row2[2] * vInv.z
            );
        }

        // Constants
        private static readonly Transformd _identity = new Transformd(Basisd.Identity, Vector3d.Zero);
        private static readonly Transformd _flipX = new Transformd(new Basisd(-1, 0, 0, 0, 1, 0, 0, 0, 1), Vector3d.Zero);
        private static readonly Transformd _flipY = new Transformd(new Basisd(1, 0, 0, 0, -1, 0, 0, 0, 1), Vector3d.Zero);
        private static readonly Transformd _flipZ = new Transformd(new Basisd(1, 0, 0, 0, 1, 0, 0, 0, -1), Vector3d.Zero);

        /// <summary>
        /// The identity transform, with no translation, rotation, or scaling applied.
        /// This is used as a replacement for `Transformd()` in GDScript.
        /// Do not use `new Transformd()` with no arguments in C#, because it sets all values to zero.
        /// </summary>
        /// <value>Equivalent to `new Transformd(Vector3d.Right, Vector3d.Up, Vector3d.Back, Vector3d.Zero)`.</value>
        public static Transformd Identity { get { return _identity; } }
        /// <summary>
        /// The transform that will flip something along the X axis.
        /// </summary>
        /// <value>Equivalent to `new Transformd(Vector3d.Left, Vector3d.Up, Vector3d.Back, Vector3d.Zero)`.</value>
        public static Transformd FlipX { get { return _flipX; } }
        /// <summary>
        /// The transform that will flip something along the Y axis.
        /// </summary>
        /// <value>Equivalent to `new Transformd(Vector3d.Right, Vector3d.Down, Vector3d.Back, Vector3d.Zero)`.</value>
        public static Transformd FlipY { get { return _flipY; } }
        /// <summary>
        /// The transform that will flip something along the Z axis.
        /// </summary>
        /// <value>Equivalent to `new Transformd(Vector3d.Right, Vector3d.Up, Vector3d.Forward, Vector3d.Zero)`.</value>
        public static Transformd FlipZ { get { return _flipZ; } }

        /// <summary>
        /// Constructs a transformation matrix from 4 vectors (matrix columns).
        /// </summary>
        /// <param name="column0">The X vector, or column index 0.</param>
        /// <param name="column1">The Y vector, or column index 1.</param>
        /// <param name="column2">The Z vector, or column index 2.</param>
        /// <param name="origin">The origin vector, or column index 3.</param>
        public Transformd(Vector3d column0, Vector3d column1, Vector3d column2, Vector3d origin)
        {
            basis = new Basisd(column0, column1, column2);
            this.origin = origin;
        }

        /// <summary>
        /// Constructs a transformation matrix from the given quaternion and origin vector.
        /// </summary>
        /// <param name="quat">The <see cref="ExtraMath.Quatd"/> to create the basis from.</param>
        /// <param name="origin">The origin vector, or column index 3.</param>
        public Transformd(Quatd quat, Vector3d origin)
        {
            basis = new Basisd(quat);
            this.origin = origin;
        }

        /// <summary>
        /// Constructs a transformation matrix from the given basis and origin vector.
        /// </summary>
        /// <param name="basis">The <see cref="ExtraMath.Basisd"/> to create the basis from.</param>
        /// <param name="origin">The origin vector, or column index 3.</param>
        public Transformd(Basisd basis, Vector3d origin)
        {
            this.basis = basis;
            this.origin = origin;
        }

#if GODOT
        public static explicit operator Godot.Transform(Transformd value)
        {
            return new Godot.Transform((Godot.Basis)value.basis, (Godot.Vector3)value.origin);
        }

        public static implicit operator Transformd(Godot.Transform value)
        {
            return new Transformd(value.basis, value.origin);
        }
#elif UNITY_5_3_OR_NEWER
        // Transform in Unity is a component, not a struct.
        // Operators are not possible, so put methods here instead.
        // Use "Apply" and "Store" instead of "Set" and "Get" to be unambiguous.
        /// <summary>
        /// Applies the values of this Transformd struct to the given Transform. Note: Very inefficient.
        /// </summary>
        public void ApplyTransform(UnityEngine.Transform transform)
        {
            transform.localPosition = (UnityEngine.Vector3)origin;
            transform.localRotation = (UnityEngine.Quaternion)basis.Quat();
        }

        /// <summary>
        /// Stores the values of the given Transform in a Transformd struct. Note: Very inefficient.
        /// </summary>
        public static Transformd StoreTransform(UnityEngine.Transform transform)
        {
            Quatd rotation = transform.localRotation;
            Vector3d position = transform.localPosition;
            return new Transformd(rotation, position);
        }
#endif

        public static Transformd operator *(Transformd left, Transformd right)
        {
            left.origin = left.Xform(right.origin);
            left.basis *= right.basis;
            return left;
        }

        public static bool operator ==(Transformd left, Transformd right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Transformd left, Transformd right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Transformd)
            {
                return Equals((Transformd)obj);
            }

            return false;
        }

        public bool Equals(Transformd other)
        {
            return basis.Equals(other.basis) && origin.Equals(other.origin);
        }

        /// <summary>
        /// Returns true if this transform and `other` are approximately equal, by running
        /// <see cref="Vector3d.IsEqualApprox(Vector3d)"/> on each component.
        /// </summary>
        /// <param name="other">The other transform to compare.</param>
        /// <returns>Whether or not the matrices are approximately equal.</returns>
        public bool IsEqualApprox(Transformd other)
        {
            return basis.IsEqualApprox(other.basis) && origin.IsEqualApprox(other.origin);
        }

        public override int GetHashCode()
        {
            return basis.GetHashCode() ^ origin.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", new object[]
            {
                basis.ToString(),
                origin.ToString()
            });
        }

        public string ToString(string format)
        {
            return String.Format("{0} - {1}", new object[]
            {
                basis.ToString(format),
                origin.ToString(format)
            });
        }
    }
}
