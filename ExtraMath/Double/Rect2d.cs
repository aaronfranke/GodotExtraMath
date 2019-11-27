using System;
using System.Runtime.InteropServices;

namespace ExtraMath
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect2d : IEquatable<Rect2d>
    {
        private Vector2d _position;
        private Vector2d _size;

        public Vector2d Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2d Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public Vector2d End
        {
            get { return _position + _size; }
            set { _size = value - _position; }
        }

        public double Area
        {
            get { return GetArea(); }
        }

        public Rect2d Abs()
        {
            Vector2d end = End;
            Vector2d topLeft = new Vector2d(Mathd.Min(_position.x, end.x), Mathd.Min(_position.y, end.y));
            return new Rect2d(topLeft, _size.Abs());
        }

        public Rect2d Clip(Rect2d b)
        {
            var newRect = b;

            if (!Intersects(newRect))
                return new Rect2d();

            newRect._position.x = Mathd.Max(b._position.x, _position.x);
            newRect._position.y = Mathd.Max(b._position.y, _position.y);

            Vector2d bEnd = b._position + b._size;
            Vector2d end = _position + _size;

            newRect._size.x = Mathd.Min(bEnd.x, end.x) - newRect._position.x;
            newRect._size.y = Mathd.Min(bEnd.y, end.y) - newRect._position.y;

            return newRect;
        }

        public bool Encloses(Rect2d b)
        {
            return b._position.x >= _position.x && b._position.y >= _position.y &&
               b._position.x + b._size.x < _position.x + _size.x &&
               b._position.y + b._size.y < _position.y + _size.y;
        }

        public Rect2d Expand(Vector2d to)
        {
            var expanded = this;

            Vector2d begin = expanded._position;
            Vector2d end = expanded._position + expanded._size;

            if (to.x < begin.x)
                begin.x = to.x;
            if (to.y < begin.y)
                begin.y = to.y;

            if (to.x > end.x)
                end.x = to.x;
            if (to.y > end.y)
                end.y = to.y;

            expanded._position = begin;
            expanded._size = end - begin;

            return expanded;
        }

        public double GetArea()
        {
            return _size.x * _size.y;
        }

        public Rect2d Grow(double by)
        {
            var g = this;

            g._position.x -= by;
            g._position.y -= by;
            g._size.x += 2 * by;
            g._size.y += 2 * by;

            return g;
        }

        public Rect2d GrowIndividual(double left, double top, double right, double bottom)
        {
            var g = this;

            g._position.x -= left;
            g._position.y -= top;
            g._size.x += left + right;
            g._size.y += top + bottom;

            return g;
        }

        public Rect2d GrowMargin(Godot.Margin margin, double by)
        {
            var g = this;

            g.GrowIndividual(Godot.Margin.Left == margin ? by : 0,
                    Godot.Margin.Top == margin ? by : 0,
                    Godot.Margin.Right == margin ? by : 0,
                    Godot.Margin.Bottom == margin ? by : 0);

            return g;
        }

        public bool HasNoArea()
        {
            return _size.x <= 0 || _size.y <= 0;
        }

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

        public bool Intersects(Rect2d b)
        {
            if (_position.x >= b._position.x + b._size.x)
                return false;
            if (_position.x + _size.x <= b._position.x)
                return false;
            if (_position.y >= b._position.y + b._size.y)
                return false;
            if (_position.y + _size.y <= b._position.y)
                return false;

            return true;
        }

        public Rect2d Merge(Rect2d b)
        {
            Rect2d newRect;

            newRect._position.x = Mathd.Min(b._position.x, _position.x);
            newRect._position.y = Mathd.Min(b._position.y, _position.y);

            newRect._size.x = Mathd.Max(b._position.x + b._size.x, _position.x + _size.x);
            newRect._size.y = Mathd.Max(b._position.y + b._size.y, _position.y + _size.y);

            newRect._size = newRect._size - newRect._position; // Make relative again

            return newRect;
        }

        // Constructors
        public Rect2d(Vector2d position, Vector2d size)
        {
            _position = position;
            _size = size;
        }
        public Rect2d(Vector2d position, double width, double height)
        {
            _position = position;
            _size = new Vector2d(width, height);
        }
        public Rect2d(double x, double y, Vector2d size)
        {
            _position = new Vector2d(x, y);
            _size = size;
        }
        public Rect2d(double x, double y, double width, double height)
        {
            _position = new Vector2d(x, y);
            _size = new Vector2d(width, height);
        }

        public static explicit operator Godot.Rect2(Rect2d value)
        {
            return new Godot.Rect2((Godot.Vector2)value.Position, (Godot.Vector2)value.Size);
        }

        public static implicit operator Rect2d(Godot.Rect2 value)
        {
            return new Rect2d(value.Position, value.Size);
        }

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
