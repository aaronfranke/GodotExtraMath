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
    public struct Planed : IEquatable<Planed>
    {
        private Vector3d _normal;

        public Vector3d Normal
        {
            get { return _normal; }
            set { _normal = value; }
        }

        public double x
        {
            get
            {
                return _normal.x;
            }
            set
            {
                _normal.x = value;
            }
        }

        public double y
        {
            get
            {
                return _normal.y;
            }
            set
            {
                _normal.y = value;
            }
        }

        public double z
        {
            get
            {
                return _normal.z;
            }
            set
            {
                _normal.z = value;
            }
        }

        public double D { get; set; }

        public Vector3d Center
        {
            get
            {
                return _normal * D;
            }
        }

        public double DistanceTo(Vector3d point)
        {
            return _normal.Dot(point) - D;
        }

        public Vector3d GetAnyPoint()
        {
            return _normal * D;
        }

        public bool HasPoint(Vector3d point, double epsilon = Mathd.Epsilon)
        {
            double dist = _normal.Dot(point) - D;
            return Mathd.Abs(dist) <= epsilon;
        }

        public Vector3d? Intersect3(Planed b, Planed c)
        {
            double denom = _normal.Cross(b._normal).Dot(c._normal);

            if (Mathd.IsZeroApprox(denom))
                return null;

            Vector3d result = b._normal.Cross(c._normal) * D +
                                c._normal.Cross(_normal) * b.D +
                                _normal.Cross(b._normal) * c.D;

            return result / denom;
        }

        public Vector3d? IntersectRay(Vector3d from, Vector3d dir)
        {
            double den = _normal.Dot(dir);

            if (Mathd.IsZeroApprox(den))
                return null;

            double dist = (_normal.Dot(from) - D) / den;

            // This is a ray, before the emitting pos (from) does not exist
            if (dist > Mathd.Epsilon)
                return null;

            return from + dir * -dist;
        }

        public Vector3d? IntersectSegment(Vector3d begin, Vector3d end)
        {
            Vector3d segment = begin - end;
            double den = _normal.Dot(segment);

            if (Mathd.IsZeroApprox(den))
                return null;

            double dist = (_normal.Dot(begin) - D) / den;

            // Only allow dist to be in the range of 0 to 1, with tolerance.
            if (dist < -Mathd.Epsilon || dist > 1.0f + Mathd.Epsilon)
                return null;

            return begin + segment * -dist;
        }

        public bool IsPointOver(Vector3d point)
        {
            return _normal.Dot(point) > D;
        }

        public Planed Normalized()
        {
            double len = _normal.Length();

            if (len == 0)
                return new Planed(0, 0, 0, 0);

            return new Planed(_normal / len, D / len);
        }

        public Vector3d Project(Vector3d point)
        {
            return point - _normal * DistanceTo(point);
        }

        // Constants
        private static readonly Planed _PlanedYZ = new Planed(1, 0, 0, 0);
        private static readonly Planed _PlanedXZ = new Planed(0, 1, 0, 0);
        private static readonly Planed _PlanedXY = new Planed(0, 0, 1, 0);

        public static Planed PlanedYZ { get { return _PlanedYZ; } }
        public static Planed PlanedXZ { get { return _PlanedXZ; } }
        public static Planed PlanedXY { get { return _PlanedXY; } }

        // Constructors
        public Planed(double a, double b, double c, double d)
        {
            _normal = new Vector3d(a, b, c);
            this.D = d;
        }
        public Planed(Vector3d normal, double d)
        {
            this._normal = normal;
            this.D = d;
        }

        public Planed(Vector3d v1, Vector3d v2, Vector3d v3)
        {
            _normal = (v1 - v3).Cross(v1 - v2);
            _normal.Normalize();
            D = _normal.Dot(v1);
        }

        public static explicit operator Godot.Plane(Planed value)
        {
            return new Godot.Plane((Godot.Vector3)value.Normal, (real_t)value.D);
        }

        public static implicit operator Planed(Godot.Plane value)
        {
            return new Planed(value.Normal, value.D);
        }

        public static Planed operator -(Planed Planed)
        {
            return new Planed(-Planed._normal, -Planed.D);
        }

        public static bool operator ==(Planed left, Planed right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Planed left, Planed right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Planed)
            {
                return Equals((Planed)obj);
            }

            return false;
        }

        public bool Equals(Planed other)
        {
            return _normal == other._normal && Mathd.IsEqualApprox(D, other.D);
        }

        public override int GetHashCode()
        {
            return _normal.GetHashCode() ^ D.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", new object[]
            {
                _normal.ToString(),
                D.ToString()
            });
        }

        public string ToString(string format)
        {
            return String.Format("({0}, {1})", new object[]
            {
                _normal.ToString(format),
                D.ToString(format)
            });
        }
    }
}
