using System;
using System.Runtime.InteropServices;

namespace ExtraMath
{
    /// <summary>
    /// Calculates the 2D transformation from a 3D position and a Basis25D.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Transform25Dd : IEquatable<Transform25Dd>
    {
        // Public fields store information that is used to calculate the properties.

        /// <summary>
        /// Controls how the 3D position is transformed into 2D.
        /// </summary>
        public Basis25Dd basis;

        /// <summary>
        /// The 3D position of the object. Should be updated on every frame before everything else.
        /// </summary>
        public Vector3d spatialPosition;

        // Public properties calculate on-the-fly.

        /// <summary>
        /// The 2D transformation of this object. Slower than FlatPosition.
        /// </summary>
        public Transform2Dd FlatTransform
        {
            get
            {
                return new Transform2Dd(0, FlatPosition);
            }
        }

        /// <summary>
        /// The 2D position of this object.
        /// </summary>
        public Vector2d FlatPosition
        {
            get
            {
                Vector2d pos = spatialPosition.x * basis.x;
                pos += spatialPosition.y * basis.y;
                pos += spatialPosition.z * basis.z;
                return pos;
            }
        }

        // Constructors
        public Transform25Dd(Transform25Dd transform25D)
        {
            basis = transform25D.basis;
            spatialPosition = transform25D.spatialPosition;
        }
        public Transform25Dd(Basis25Dd basis25D)
        {
            basis = basis25D;
            spatialPosition = Vector3d.Zero;
        }
        public Transform25Dd(Basis25Dd basis25D, Vector3d position3D)
        {
            basis = basis25D;
            spatialPosition = position3D;
        }
        public Transform25Dd(Vector2d xAxis, Vector2d yAxis, Vector2d zAxis)
        {
            basis = new Basis25Dd(xAxis, yAxis, zAxis);
            spatialPosition = Vector3d.Zero;
        }
        public Transform25Dd(Vector2d xAxis, Vector2d yAxis, Vector2d zAxis, Vector3d position3D)
        {
            basis = new Basis25Dd(xAxis, yAxis, zAxis);
            spatialPosition = position3D;
        }

        public static explicit operator Transform25D(Transform25Dd value)
        {
            return new Transform25D((Basis25D)value.basis, (Godot.Vector3)value.spatialPosition);
        }

        public static implicit operator Transform25Dd(Transform25D value)
        {
            return new Transform25Dd(value.basis, value.spatialPosition);
        }

        public static bool operator ==(Transform25Dd left, Transform25Dd right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Transform25Dd left, Transform25Dd right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Transform25Dd)
            {
                return Equals((Transform25Dd)obj);
            }
            return false;
        }

        public bool Equals(Transform25Dd other)
        {
            return basis.Equals(other.basis) && spatialPosition.Equals(other.spatialPosition);
        }

        public override int GetHashCode()
        {
            return basis.GetHashCode() ^ spatialPosition.GetHashCode();
        }

        public override string ToString()
        {
            string s = String.Format("({0}, {1})", new object[]
            {
                basis.ToString(),
                spatialPosition.ToString()
            });
            return s;
        }

        public string ToString(string format)
        {
            string s = String.Format("({0}, {1})", new object[]
            {
                basis.ToString(format),
                spatialPosition.ToString(format)
            });
            return s;
        }
    }
}
