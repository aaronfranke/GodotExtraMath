using System;
using System.Runtime.InteropServices;

namespace ExtraMath
{
    /// <summary>
    /// Basis25Dd structure for performing 2.5D transform math.
    /// Note: All code assumes that Y is UP in 3D, and DOWN in 2D.
    /// A top-down view has a Y axis component of (0, 0), with a Z axis component of (0, 1).
    /// For a front side view, Y is (0, -1) and Z is (0, 0).
    /// Remember that Godot's 2D mode has the Y axis pointing DOWN on the screen.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Basis25Dd : IEquatable<Basis25Dd>
    {
        // Also matrix columns, the directions to move on screen for each unit change in 3D.
        public Vector2d x;
        public Vector2d y;
        public Vector2d z;

        // Also matrix rows, the parts of each vector that contribute to moving in a screen direction.
        // Setting a row to zero means no movement in that direction.
        public Vector3d Row0
        {
            get { return new Vector3d(x.x, y.x, z.x); }
            set
            {
                x.x = value.x;
                y.x = value.y;
                z.x = value.z;
            }
        }

        public Vector3d Row1
        {
            get { return new Vector3d(x.y, y.y, z.y); }
            set
            {
                x.y = value.x;
                y.y = value.y;
                z.y = value.z;
            }
        }

        public Vector2d this[int columnIndex]
        {
            get
            {
                switch (columnIndex)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (columnIndex)
                {
                    case 0: x = value; return;
                    case 1: y = value; return;
                    case 2: z = value; return;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public double this[int columnIndex, int rowIndex]
        {
            get
            {
                return this[columnIndex][rowIndex];
            }
            set
            {
                Vector2d v = this[columnIndex];
                v[rowIndex] = value;
                this[columnIndex] = v;
            }
        }

        private static readonly Basis25Dd _topDown = new Basis25Dd(1, 0, 0, 0, 0, 1);
        private static readonly Basis25Dd _frontSide = new Basis25Dd(1, 0, 0, -1, 0, 0);
        private static readonly Basis25Dd _fortyFive = new Basis25Dd(1, 0, 0, -0.70710678118, 0, 0.70710678118);
        private static readonly Basis25Dd _isometric = new Basis25Dd(0.86602540378, 0.5, 0, -1, -0.86602540378, 0.5);
        private static readonly Basis25Dd _obliqueY = new Basis25Dd(1, 0, -1, -1, 0, 1);
        private static readonly Basis25Dd _obliqueZ = new Basis25Dd(1, 0, 0, -1, -1, 1);

        public static Basis25Dd TopDown { get { return _topDown; } }
        public static Basis25Dd FrontSide { get { return _frontSide; } }
        public static Basis25Dd FortyFive { get { return _fortyFive; } }
        public static Basis25Dd Isometric { get { return _isometric; } }
        public static Basis25Dd ObliqueY { get { return _obliqueY; } }
        public static Basis25Dd ObliqueZ { get { return _obliqueZ; } }

        /// <summary>
        /// Creates a Dimetric Basis25Dd from the angle between the Y axis and the others.
        /// Dimetric(Tau/3) or Dimetric(2.09439510239) is the same as Isometric.
        /// Try to keep this number away from a multiple of Tau/4 (or Pi/2) radians.
        /// </summary>
        /// <param name="angle">The angle, in radians, between the Y axis and the X/Z axes.</param>
        public static Basis25Dd Dimetric(double angle)
        {
            double sin = Mathd.Sin(angle);
            double cos = Mathd.Cos(angle);
            return new Basis25Dd(sin, -cos, 0, -1, -sin, -cos);
        }

        public Basis25Dd(Basis25Dd b)
        {
            x = b.x;
            y = b.y;
            z = b.z;
        }
        public Basis25Dd(Vector2d xAxis, Vector2d yAxis, Vector2d zAxis)
        {
            x = xAxis;
            y = yAxis;
            z = zAxis;
        }
        public Basis25Dd(double xx, double xy, double yx, double yy, double zx, double zy)
        {
            x = new Vector2d(xx, xy);
            y = new Vector2d(yx, yy);
            z = new Vector2d(zx, zy);
        }

        public static explicit operator Basis25D(Basis25Dd value)
        {
#if GODOT
            return new Basis25D((Godot.Vector2)value.x, (Godot.Vector2)value.y, (Godot.Vector2)value.z);
#elif UNITY_5_3_OR_NEWER
            return new Basis25D((UnityEngine.Vector2)value.x, (UnityEngine.Vector2)value.y, (UnityEngine.Vector2)value.z);
#endif
        }

        public static implicit operator Basis25Dd(Basis25D value)
        {
            return new Basis25Dd(value.x, value.y, value.z);
        }

        public static Basis25Dd operator *(Basis25Dd b, double s)
        {
            b.x *= s;
            b.y *= s;
            b.z *= s;
            return b;
        }

        public static Basis25Dd operator /(Basis25Dd b, double s)
        {
            b.x /= s;
            b.y /= s;
            b.z /= s;
            return b;
        }

        public static bool operator ==(Basis25Dd left, Basis25Dd right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Basis25Dd left, Basis25Dd right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Basis25Dd)
            {
                return Equals((Basis25Dd)obj);
            }
            return false;
        }

        public bool Equals(Basis25Dd other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public override int GetHashCode()
        {
            return y.GetHashCode() ^ x.GetHashCode() ^ z.GetHashCode();
        }

        public override string ToString()
        {
            string s = String.Format("({0}, {1}, {2})", new object[]
            {
                x.ToString(),
                y.ToString(),
                z.ToString()
            });
            return s;
        }

        public string ToString(string format)
        {
            string s = String.Format("({0}, {1}, {2})", new object[]
            {
                x.ToString(format),
                y.ToString(format),
                z.ToString(format)
            });
            return s;
        }
    }
}
