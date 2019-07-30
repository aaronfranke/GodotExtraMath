using System;
using System.Runtime.InteropServices;

namespace ExtraMath
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Transformd : IEquatable<Transformd>
    {
        public Basisd basis;
        public Vector3d origin;

        public Transformd AffineInverse()
        {
            Basisd basisInv = basis.Inverse();
            return new Transformd(basisInv, basisInv.Xform(-origin));
        }

        public Transformd InterpolateWith(Transformd Transformd, double c)
        {
            /* not sure if very "efficient" but good enough? */

            Vector3d sourceScale = basis.Scale;
            Quatd sourceRotation = basis.RotationQuatd();
            Vector3d sourceLocation = origin;

            Vector3d destinationScale = Transformd.basis.Scale;
            Quatd destinationRotation = Transformd.basis.RotationQuatd();
            Vector3d destinationLocation = Transformd.origin;

            var interpolated = new Transformd();
            interpolated.basis.SetQuatScale(sourceRotation.Slerp(destinationRotation, c).Normalized(), sourceScale.LinearInterpolate(destinationScale, c));
            interpolated.origin = sourceLocation.LinearInterpolate(destinationLocation, c);

            return interpolated;
        }

        public Transformd Inverse()
        {
            Basisd basisTr = basis.Transposed();
            return new Transformd(basisTr, basisTr.Xform(-origin));
        }

        public Transformd LookingAt(Vector3d target, Vector3d up)
        {
            var t = this;
            t.SetLookAt(origin, target, up);
            return t;
        }

        public Transformd Orthonormalized()
        {
            return new Transformd(basis.Orthonormalized(), origin);
        }

        public Transformd Rotated(Vector3d axis, double phi)
        {
            return new Transformd(new Basisd(axis, phi), new Vector3d()) * this;
        }

        public Transformd Scaled(Vector3d scale)
        {
            return new Transformd(basis.Scaled(scale), origin * scale);
        }

        public void SetLookAt(Vector3d eye, Vector3d target, Vector3d up)
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

        public Transformd Translated(Vector3d ofs)
        {
            return new Transformd(basis, new Vector3d
            (
                origin[0] += basis.Row0.Dot(ofs),
                origin[1] += basis.Row1.Dot(ofs),
                origin[2] += basis.Row2.Dot(ofs)
            ));
        }

        public Vector3d Xform(Vector3d v)
        {
            return new Vector3d
            (
                basis.Row0.Dot(v) + origin.x,
                basis.Row1.Dot(v) + origin.y,
                basis.Row2.Dot(v) + origin.z
            );
        }

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

        public static Transformd Identity { get { return _identity; } }
        public static Transformd FlipX { get { return _flipX; } }
        public static Transformd FlipY { get { return _flipY; } }
        public static Transformd FlipZ { get { return _flipZ; } }

        // Constructors
        public Transformd(Vector3d column0, Vector3d column1, Vector3d column2, Vector3d origin)
        {
            basis = new Basisd(column0, column1, column2);
            this.origin = origin;
        }

        public Transformd(Quatd quat, Vector3d origin)
        {
            basis = new Basisd(quat);
            this.origin = origin;
        }

        public Transformd(Basisd basis, Vector3d origin)
        {
            this.basis = basis;
            this.origin = origin;
        }

        public static explicit operator Godot.Transform(Transformd value)
        {
            return new Godot.Transform((Godot.Basis)value.basis, (Godot.Vector3)value.origin);
        }

        public static implicit operator Transformd(Godot.Transform value)
        {
            return new Transformd(value.basis, value.origin);
        }

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
