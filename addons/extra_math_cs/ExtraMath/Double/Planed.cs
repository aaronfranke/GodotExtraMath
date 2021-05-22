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
    /// Planed represents a normalized plane equation.
    /// "Over" or "Above" the plane is considered the side of
    /// the plane towards where the normal is pointing.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Planed : IEquatable<Planed>
    {
        private Vector3d _normal;

        /// <summary>
        /// The normal of the plane, which must be normalized.
        /// In the scalar equation of the plane `ax + by + cz = d`, this is
        /// the vector `(a, b, c)`, where `d` is the <see cref="D"/> property.
        /// </summary>
        /// <value>Equivalent to `x`, `y`, and `z`.</value>
        public Vector3d Normal
        {
            get { return _normal; }
            set { _normal = value; }
        }

        /// <summary>
        /// The X component of the plane's normal vector.
        /// </summary>
        /// <value>Equivalent to <see cref="Normal"/>'s X value.</value>
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

        /// <summary>
        /// The Y component of the plane's normal vector.
        /// </summary>
        /// <value>Equivalent to <see cref="Normal"/>'s Y value.</value>
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

        /// <summary>
        /// The Z component of the plane's normal vector.
        /// </summary>
        /// <value>Equivalent to <see cref="Normal"/>'s Z value.</value>
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

        /// <summary>
        /// The distance from the origin to the plane (in the direction of
        /// <see cref="Normal"/>). This value is typically non-negative.
        /// In the scalar equation of the plane `ax + by + cz = d`,
        /// this is `d`, while the `(a, b, c)` coordinates are represented
        /// by the <see cref="Normal"/> property.
        /// </summary>
        /// <value>The plane's distance from the origin.</value>
        public double D { get; set; }

        /// <summary>
        /// The center of the plane, the point where the normal line intersects the plane.
        /// </summary>
        /// <value>Equivalent to <see cref="Normal"/> multiplied by `D`.</value>
        public Vector3d Center
        {
            get
            {
                return _normal * D;
            }
            set
            {
                _normal = value.Normalized();
                D = value.Length();
            }
        }

        /// <summary>
        /// Returns the shortest distance from this plane to the position `point`.
        /// </summary>
        /// <param name="point">The position to use for the calculation.</param>
        /// <returns>The shortest distance.</returns>
        public double DistanceTo(Vector3d point)
        {
            return _normal.Dot(point) - D;
        }

        /// <summary>
        /// Returns true if point is inside the plane.
        /// Comparison uses a custom minimum epsilon threshold.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="epsilon">The tolerance threshold.</param>
        /// <returns>A bool for whether or not the plane has the point.</returns>
        public bool HasPoint(Vector3d point, double epsilon = Mathd.Epsilon)
        {
            double dist = _normal.Dot(point) - D;
            return Mathd.Abs(dist) <= epsilon;
        }

        /// <summary>
        /// Returns the intersection point of the three planes: `b`, `c`,
        /// and this plane. If no intersection is found, `null` is returned.
        /// </summary>
        /// <param name="b">One of the three planes to use in the calculation.</param>
        /// <param name="c">One of the three planes to use in the calculation.</param>
        /// <returns>The intersection, or `null` if none is found.</returns>
        public Vector3d? Intersect3(Planed b, Planed c)
        {
            double denom = _normal.Cross(b._normal).Dot(c._normal);

            if (Mathd.IsZeroApprox(denom))
            {
                return null;
            }

            Vector3d result = b._normal.Cross(c._normal) * D +
                                c._normal.Cross(_normal) * b.D +
                                _normal.Cross(b._normal) * c.D;

            return result / denom;
        }

        /// <summary>
        /// Returns the intersection point of a ray consisting of the
        /// position `from` and the direction normal `dir` with this plane.
        /// If no intersection is found, `null` is returned.
        /// </summary>
        /// <param name="from">The start of the ray.</param>
        /// <param name="dir">The direction of the ray, normalized.</param>
        /// <returns>The intersection, or `null` if none is found.</returns>
        public Vector3d? IntersectRay(Vector3d from, Vector3d dir)
        {
            double den = _normal.Dot(dir);

            if (Mathd.IsZeroApprox(den))
            {
                return null;
            }

            double dist = (_normal.Dot(from) - D) / den;

            // This is a ray, before the emitting pos (from) does not exist
            if (dist > Mathd.Epsilon)
            {
                return null;
            }

            return from + dir * -dist;
        }

        /// <summary>
        /// Returns the intersection point of a line segment from
        /// position `begin` to position `end` with this plane.
        /// If no intersection is found, `null` is returned.
        /// </summary>
        /// <param name="begin">The start of the line segment.</param>
        /// <param name="end">The end of the line segment.</param>
        /// <returns>The intersection, or `null` if none is found.</returns>
        public Vector3d? IntersectSegment(Vector3d begin, Vector3d end)
        {
            Vector3d segment = begin - end;
            double den = _normal.Dot(segment);

            if (Mathd.IsZeroApprox(den))
            {
                return null;
            }

            double dist = (_normal.Dot(begin) - D) / den;

            // Only allow dist to be in the range of 0 to 1, with tolerance.
            if (dist < -Mathd.Epsilon || dist > 1.0f + Mathd.Epsilon)
            {
                return null;
            }

            return begin + segment * -dist;
        }

        /// <summary>
        /// Returns true if `point` is located above the plane.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>A bool for whether or not the point is above the plane.</returns>
        public bool IsPointOver(Vector3d point)
        {
            return _normal.Dot(point) > D;
        }

        /// <summary>
        /// Returns the plane scaled to unit length.
        /// </summary>
        /// <returns>A normalized version of the plane.</returns>
        public Planed Normalized()
        {
            double len = _normal.Length();

            if (len == 0)
            {
                return new Planed(0, 0, 0, 0);
            }

            return new Planed(_normal / len, D / len);
        }

        /// <summary>
        /// Returns the orthogonal projection of `point` into the plane.
        /// </summary>
        /// <param name="point">The point to project.</param>
        /// <returns>The projected point.</returns>
        public Vector3d Project(Vector3d point)
        {
            return point - _normal * DistanceTo(point);
        }

        // Constants
        private static readonly Planed _planeYZ = new Planed(1, 0, 0, 0);
        private static readonly Planed _planeXZ = new Planed(0, 1, 0, 0);
        private static readonly Planed _planeXY = new Planed(0, 0, 1, 0);

        /// <summary>
        /// A plane that extends in the Y and Z axes (normal vector points +X).
        /// </summary>
        /// <value>Equivalent to `new Planed(1, 0, 0, 0)`.</value>
        public static Planed PlaneYZ { get { return _planeYZ; } }

        /// <summary>
        /// A plane that extends in the X and Z axes (normal vector points +Y).
        /// </summary>
        /// <value>Equivalent to `new Planed(0, 1, 0, 0)`.</value>
        public static Planed PlaneXZ { get { return _planeXZ; } }

        /// <summary>
        /// A plane that extends in the X and Y axes (normal vector points +Z).
        /// </summary>
        /// <value>Equivalent to `new Planed(0, 0, 1, 0)`.</value>
        public static Planed PlaneXY { get { return _planeXY; } }

        /// <summary>
        /// Constructs a plane from four values. `a`, `b` and `c` become the
        /// components of the resulting plane's <see cref="Normal"/> vector.
        /// `d` becomes the plane's distance from the origin.
        /// </summary>
        /// <param name="a">The X component of the plane's normal vector.</param>
        /// <param name="b">The Y component of the plane's normal vector.</param>
        /// <param name="c">The Z component of the plane's normal vector.</param>
        /// <param name="d">The plane's distance from the origin. This value is typically non-negative.</param>
        public Planed(double a, double b, double c, double d)
        {
            _normal = new Vector3d(a, b, c);
            this.D = d;
        }

        /// <summary>
        /// Constructs a plane from a normal vector and the plane's distance to the origin.
        /// </summary>
        /// <param name="normal">The normal of the plane, must be normalized.</param>
        /// <param name="d">The plane's distance from the origin. This value is typically non-negative.</param>
        public Planed(Vector3d normal, double d)
        {
            this._normal = normal;
            this.D = d;
        }

        /// <summary>
        /// Constructs a plane from the three points, given in clockwise order.
        /// </summary>
        /// <param name="v1">The first point.</param>
        /// <param name="v2">The second point.</param>
        /// <param name="v3">The third point.</param>
        public Planed(Vector3d v1, Vector3d v2, Vector3d v3)
        {
            _normal = (v1 - v3).Cross(v1 - v2);
            _normal.Normalize();
            D = _normal.Dot(v1);
        }

#if GODOT
        public static explicit operator Godot.Plane(Planed value)
        {
            return new Godot.Plane((Godot.Vector3)value.Normal, (real_t)value.D);
        }

        public static implicit operator Planed(Godot.Plane value)
        {
            return new Planed(value.Normal, value.D);
        }
#elif UNITY_5_3_OR_NEWER
        public static explicit operator UnityEngine.Plane(Planed value)
        {
            return new UnityEngine.Plane((UnityEngine.Vector3)value.Normal, (real_t)value.D);
        }

        public static implicit operator Planed(UnityEngine.Plane value)
        {
            return new Planed(value.normal, value.distance);
        }
#endif

        public static Planed operator -(Planed plane)
        {
            return new Planed(-plane._normal, -plane.D);
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
            return _normal == other._normal && D == other.D;
        }

        /// <summary>
        /// Returns true if this plane and `other` are approximately equal, by running
        /// <see cref="Mathd.IsEqualApprox(double, double)"/> on each component.
        /// </summary>
        /// <param name="other">The other plane to compare.</param>
        /// <returns>Whether or not the planes are approximately equal.</returns>
        public bool IsEqualApprox(Planed other)
        {
            return _normal.IsEqualApprox(other._normal) && Mathd.IsEqualApprox(D, other.D);
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
