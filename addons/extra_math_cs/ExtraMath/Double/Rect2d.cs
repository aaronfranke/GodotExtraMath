using System;
using System.Runtime.InteropServices;

namespace ExtraMath
{
    /// <summary>
    /// 2D axis-aligned bounding box. Rect2d consists of a position, a size, and
    /// several utility functions. It is typically used for fast overlap tests.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect2d : IEquatable<Rect2d>
    {
        private Vector2d _position;
        private Vector2d _size;

        /// <summary>
        /// Beginning corner. Typically has values lower than End.
        /// </summary>
        /// <value>Directly uses a private field.</value>
        public Vector2d Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Size from Position to End. Typically all components are positive.
        /// If the size is negative, you can use <see cref="Abs"/> to fix it.
        /// </summary>
        /// <value>Directly uses a private field.</value>
        public Vector2d Size
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// Ending corner. This is calculated as <see cref="Position"/> plus
        /// <see cref="Size"/>. Setting this value will change the size.
        /// </summary>
        /// <value>Getting is equivalent to `value = Position + Size`, setting is equivalent to `Size = value - Position`.</value>
        public Vector2d End
        {
            get { return _position + _size; }
            set { _size = value - _position; }
        }

        /// <summary>
        /// The area of this Rect2d.
        /// </summary>
        /// <value>Equivalent to <see cref="GetArea()"/>.</value>
        public double Area
        {
            get { return GetArea(); }
        }

        /// <summary>
        /// Returns a Rect2d with equivalent position and size, modified so that
        /// the top-left corner is the origin and width and height are positive.
        /// </summary>
        /// <returns>The modified Rect2d.</returns>
        public Rect2d Abs()
        {
            Vector2d end = End;
            Vector2d topLeft = new Vector2d(Mathd.Min(_position.x, end.x), Mathd.Min(_position.y, end.y));
            return new Rect2d(topLeft, _size.Abs());
        }

        /// <summary>
        /// Returns the intersection of this Rect2d and `b`.
        /// If the rectangles do not intersect, an empty Rect2d is returned.
        /// </summary>
        /// <param name="b">The other Rect2d.</param>
        /// <returns>The intersection of this Rect2d and `b`, or an empty Rect2d if they do not intersect.</returns>
        public Rect2d Clip(Rect2d b)
        {
            var newRect = b;

            if (!Intersects(newRect))
            {
                return new Rect2d();
            }

            newRect._position.x = Mathd.Max(b._position.x, _position.x);
            newRect._position.y = Mathd.Max(b._position.y, _position.y);

            Vector2d bEnd = b._position + b._size;
            Vector2d end = _position + _size;

            newRect._size.x = Mathd.Min(bEnd.x, end.x) - newRect._position.x;
            newRect._size.y = Mathd.Min(bEnd.y, end.y) - newRect._position.y;

            return newRect;
        }

        /// <summary>
        /// Returns true if this Rect2d completely encloses another one.
        /// </summary>
        /// <param name="b">The other Rect2d that may be enclosed.</param>
        /// <returns>A bool for whether or not this Rect2d encloses `b`.</returns>
        public bool Encloses(Rect2d b)
        {
            return b._position.x >= _position.x && b._position.y >= _position.y &&
               b._position.x + b._size.x < _position.x + _size.x &&
               b._position.y + b._size.y < _position.y + _size.y;
        }

        /// <summary>
        /// Returns this Rect2d expanded to include a given point.
        /// </summary>
        /// <param name="to">The point to include.</param>
        /// <returns>The expanded Rect2d.</returns>
        public Rect2d Expand(Vector2d to)
        {
            var expanded = this;

            Vector2d begin = expanded._position;
            Vector2d end = expanded._position + expanded._size;

            if (to.x < begin.x)
            {
                begin.x = to.x;
            }
            if (to.y < begin.y)
            {
                begin.y = to.y;
            }

            if (to.x > end.x)
            {
                end.x = to.x;
            }
            if (to.y > end.y)
            {
                end.y = to.y;
            }

            expanded._position = begin;
            expanded._size = end - begin;

            return expanded;
        }

        /// <summary>
        /// Returns the area of the Rect2d.
        /// </summary>
        /// <returns>The area.</returns>
        public double GetArea()
        {
            return _size.x * _size.y;
        }

        /// <summary>
        /// Returns a copy of the Rect2d grown by the specified amount on all sides.
        /// </summary>
        /// <param name="by">The amount to grow by.</param>
        /// <returns>The grown Rect2d.</returns>
        public Rect2d Grow(double by)
        {
            var g = this;

            g._position.x -= by;
            g._position.y -= by;
            g._size.x += 2 * by;
            g._size.y += 2 * by;

            return g;
        }

        /// <summary>
        /// Returns a copy of the Rect2d grown by the specified amount on each side individually.
        /// </summary>
        /// <param name="left">The amount to grow by on the left side.</param>
        /// <param name="top">The amount to grow by on the top side.</param>
        /// <param name="right">The amount to grow by on the right side.</param>
        /// <param name="bottom">The amount to grow by on the bottom side.</param>
        /// <returns>The grown Rect2d.</returns>
        public Rect2d GrowIndividual(double left, double top, double right, double bottom)
        {
            var g = this;

            g._position.x -= left;
            g._position.y -= top;
            g._size.x += left + right;
            g._size.y += top + bottom;

            return g;
        }

#if GODOT
        /// <summary>
        /// Returns a copy of the Rect2d grown by the specified amount on the specified Side.
        /// </summary>
        /// <param name="side">The side to grow.</param>
        /// <param name="by">The amount to grow by.</param>
        /// <returns>The grown Rect2d.</returns>
        public Rect2d GrowMargin(Godot.Margin margin, double by)
        {
            var g = this;

            g = g.GrowIndividual(Godot.Margin.Left == margin ? by : 0,
                    Godot.Margin.Top == margin ? by : 0,
                    Godot.Margin.Right == margin ? by : 0,
                    Godot.Margin.Bottom == margin ? by : 0);

            return g;
        }

        /// <summary>
        /// Returns a copy of the Rect2d grown by the specified amount on the specified Side.
        /// </summary>
        /// <param name="side">The side to grow.</param>
        /// <param name="by">The amount to grow by.</param>
        /// <returns>The grown Rect2d.</returns>
        public Rect2d GrowSide(Godot.Margin side, double by)
        {
            var g = this;

            g = g.GrowIndividual(Godot.Margin.Left == side ? by : 0,
                    Godot.Margin.Top == side ? by : 0,
                    Godot.Margin.Right == side ? by : 0,
                    Godot.Margin.Bottom == side ? by : 0);

            return g;
        }
#endif // GODOT

        /// <summary>
        /// Returns true if the Rect2d is flat or empty, or false otherwise.
        /// </summary>
        /// <returns>A bool for whether or not the Rect2d has area.</returns>
        public bool HasNoArea()
        {
            return _size.x <= 0 || _size.y <= 0;
        }

        /// <summary>
        /// Returns true if the Rect2d contains a point, or false otherwise.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>A bool for whether or not the Rect2d contains `point`.</returns>
        public bool HasPoint(Vector2d point)
        {
            if (point.x < _position.x)
                return false;
            if (point.y < _position.y)
                return false;

            if (point.x >= _position.x + _size.x)
                return false;
            if (point.y >= _position.y + _size.y)
                return false;

            return true;
        }

        /// <summary>
        /// Returns the intersection of this Rect2d and `b`.
        /// If the rectangles do not intersect, an empty Rect2d is returned.
        /// </summary>
        /// <param name="b">The other Rect2d.</param>
        /// <returns>The intersection of this Rect2d and `b`, or an empty Rect2d if they do not intersect.</returns>
        public Rect2d Intersection(Rect2d b)
        {
            var newRect = b;

            if (!Intersects(newRect))
            {
                return new Rect2d();
            }

            newRect._position.x = Mathd.Max(b._position.x, _position.x);
            newRect._position.y = Mathd.Max(b._position.y, _position.y);

            Vector2d bEnd = b._position + b._size;
            Vector2d end = _position + _size;

            newRect._size.x = Mathd.Min(bEnd.x, end.x) - newRect._position.x;
            newRect._size.y = Mathd.Min(bEnd.y, end.y) - newRect._position.y;

            return newRect;
        }

        /// <summary>
        /// Returns true if the Rect2d overlaps with `b`
        /// (i.e. they have at least one point in common).
        ///
        /// If `includeBorders` is true, they will also be considered overlapping
        /// if their borders touch, even without intersection.
        /// </summary>
        /// <param name="b">The other Rect2d to check for intersections with.</param>
        /// <param name="includeBorders">Whether or not to consider borders.</param>
        /// <returns>A bool for whether or not they are intersecting.</returns>
        public bool Intersects(Rect2d b, bool includeBorders = false)
        {
            if (includeBorders)
            {
                if (_position.x > b._position.x + b._size.x)
                {
                    return false;
                }
                if (_position.x + _size.x < b._position.x)
                {
                    return false;
                }
                if (_position.y > b._position.y + b._size.y)
                {
                    return false;
                }
                if (_position.y + _size.y < b._position.y)
                {
                    return false;
                }
            }
            else
            {
                if (_position.x >= b._position.x + b._size.x)
                {
                    return false;
                }
                if (_position.x + _size.x <= b._position.x)
                {
                    return false;
                }
                if (_position.y >= b._position.y + b._size.y)
                {
                    return false;
                }
                if (_position.y + _size.y <= b._position.y)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a larger Rect2d that contains this Rect2d and `b`.
        /// </summary>
        /// <param name="b">The other Rect2d.</param>
        /// <returns>The merged Rect2d.</returns>
        public Rect2d Merge(Rect2d b)
        {
            Rect2d newRect;

            newRect._position.x = Mathd.Min(b._position.x, _position.x);
            newRect._position.y = Mathd.Min(b._position.y, _position.y);

            newRect._size.x = Mathd.Max(b._position.x + b._size.x, _position.x + _size.x);
            newRect._size.y = Mathd.Max(b._position.y + b._size.y, _position.y + _size.y);

            newRect._size -= newRect._position; // Make relative again

            return newRect;
        }

        /// <summary>
        /// Constructs a Rect2d from a position and size.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="size">The size.</param>
        public Rect2d(Vector2d position, Vector2d size)
        {
            _position = position;
            _size = size;
        }

        /// <summary>
        /// Constructs a Rect2d from a position, width, and height.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Rect2d(Vector2d position, double width, double height)
        {
            _position = position;
            _size = new Vector2d(width, height);
        }

        /// <summary>
        /// Constructs a Rect2d from x, y, and size.
        /// </summary>
        /// <param name="x">The position's X coordinate.</param>
        /// <param name="y">The position's Y coordinate.</param>
        /// <param name="size">The size.</param>
        public Rect2d(double x, double y, Vector2d size)
        {
            _position = new Vector2d(x, y);
            _size = size;
        }

        /// <summary>
        /// Constructs a Rect2d from x, y, width, and height.
        /// </summary>
        /// <param name="x">The position's X coordinate.</param>
        /// <param name="y">The position's Y coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Rect2d(double x, double y, double width, double height)
        {
            _position = new Vector2d(x, y);
            _size = new Vector2d(width, height);
        }

#if GODOT
        public static explicit operator Godot.Rect2(Rect2d value)
        {
            return new Godot.Rect2((Godot.Vector2)value.Position, (Godot.Vector2)value.Size);
        }

        public static implicit operator Rect2d(Godot.Rect2 value)
        {
            return new Rect2d(value.Position, value.Size);
        }
#elif UNITY_5_3_OR_NEWER
        public static explicit operator UnityEngine.Rect(Rect2d value)
        {
            return new UnityEngine.Rect((UnityEngine.Vector2)value.Position, (UnityEngine.Vector2)value.Size);
        }

        public static implicit operator Rect2d(UnityEngine.Rect value)
        {
            return new Rect2d(value.position, value.size);
        }
#endif

        public static explicit operator Rect2i(Rect2d value)
        {
            return new Rect2i((Vector2i)value.Position, (Vector2i)value.Size);
        }

        public static implicit operator Rect2d(Rect2i value)
        {
            return new Rect2d(value.Position, value.Size);
        }

        public static bool operator ==(Rect2d left, Rect2d right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Rect2d left, Rect2d right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Rect2d)
            {
                return Equals((Rect2d)obj);
            }

            return false;
        }

        public bool Equals(Rect2d other)
        {
            return _position.Equals(other._position) && _size.Equals(other._size);
        }

        /// <summary>
        /// Returns true if this Rect2d and `other` are approximately equal, by running
        /// <see cref="Vector2d.IsEqualApprox(Vector2d)"/> on each component.
        /// </summary>
        /// <param name="other">The other Rect2d to compare.</param>
        /// <returns>Whether or not the Rect2ds are approximately equal.</returns>
        public bool IsEqualApprox(Rect2d other)
        {
            return _position.IsEqualApprox(other._position) && _size.IsEqualApprox(other.Size);
        }

        public override int GetHashCode()
        {
            return _position.GetHashCode() ^ _size.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", new object[]
            {
                _position.ToString(),
                _size.ToString()
            });
        }

        public string ToString(string format)
        {
            return String.Format("({0}, {1})", new object[]
            {
                _position.ToString(format),
                _size.ToString(format)
            });
        }
    }
}
