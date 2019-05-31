using System;
using System.Runtime.InteropServices;

namespace ExtraMath
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct AABBd : IEquatable<AABBd>
    {
        private Vector3d _position;
        private Vector3d _size;

        public Vector3d Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector3d Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public Vector3d End
        {
            get { return _position + _size; }
            set { _size = value - _position; }
        }

        public bool Encloses(AABBd with)
        {
            Vector3d src_min = _position;
            Vector3d src_max = _position + _size;
            Vector3d dst_min = with._position;
            Vector3d dst_max = with._position + with._size;

            return src_min.x <= dst_min.x &&
                   src_max.x > dst_max.x &&
                   src_min.y <= dst_min.y &&
                   src_max.y > dst_max.y &&
                   src_min.z <= dst_min.z &&
                   src_max.z > dst_max.z;
        }

        public AABBd Expand(Vector3d point)
        {
            Vector3d begin = _position;
            Vector3d end = _position + _size;

            if (point.x < begin.x)
                begin.x = point.x;
            if (point.y < begin.y)
                begin.y = point.y;
            if (point.z < begin.z)
                begin.z = point.z;

            if (point.x > end.x)
                end.x = point.x;
            if (point.y > end.y)
                end.y = point.y;
            if (point.z > end.z)
                end.z = point.z;

            return new AABBd(begin, end - begin);
        }

        public double GetArea()
        {
            return _size.x * _size.y * _size.z;
        }

        public Vector3d GetEndpoint(int idx)
        {
            switch (idx)
            {
                case 0:
                    return new Vector3d(_position.x, _position.y, _position.z);
                case 1:
                    return new Vector3d(_position.x, _position.y, _position.z + _size.z);
                case 2:
                    return new Vector3d(_position.x, _position.y + _size.y, _position.z);
                case 3:
                    return new Vector3d(_position.x, _position.y + _size.y, _position.z + _size.z);
                case 4:
                    return new Vector3d(_position.x + _size.x, _position.y, _position.z);
                case 5:
                    return new Vector3d(_position.x + _size.x, _position.y, _position.z + _size.z);
                case 6:
                    return new Vector3d(_position.x + _size.x, _position.y + _size.y, _position.z);
                case 7:
                    return new Vector3d(_position.x + _size.x, _position.y + _size.y, _position.z + _size.z);
                default:
                    throw new ArgumentOutOfRangeException(nameof(idx), String.Format("Index is {0}, but a value rom 0 to 7 is expected.", idx));
            }
        }

        public Vector3d GetLongestAxis()
        {
            var axis = new Vector3d(1, 0, 0);
            double max_size = _size.x;

            if (_size.y > max_size)
            {
                axis = new Vector3d(0, 1, 0);
                max_size = _size.y;
            }

            if (_size.z > max_size)
            {
                axis = new Vector3d(0, 0, 1);
            }

            return axis;
        }

        public Vector3d.Axis GetLongestAxisIndex()
        {
            var axis = Vector3d.Axis.X;
            double max_size = _size.x;

            if (_size.y > max_size)
            {
                axis = Vector3d.Axis.Y;
                max_size = _size.y;
            }

            if (_size.z > max_size)
            {
                axis = Vector3d.Axis.Z;
            }

            return axis;
        }

        public double GetLongestAxisSize()
        {
            double max_size = _size.x;

            if (_size.y > max_size)
                max_size = _size.y;

            if (_size.z > max_size)
                max_size = _size.z;

            return max_size;
        }

        public Vector3d GetShortestAxis()
        {
            var axis = new Vector3d(1, 0, 0);
            double max_size = _size.x;

            if (_size.y < max_size)
            {
                axis = new Vector3d(0, 1, 0);
                max_size = _size.y;
            }

            if (_size.z < max_size)
            {
                axis = new Vector3d(0, 0, 1);
            }

            return axis;
        }

        public Vector3d.Axis GetShortestAxisIndex()
        {
            var axis = Vector3d.Axis.X;
            double max_size = _size.x;

            if (_size.y < max_size)
            {
                axis = Vector3d.Axis.Y;
                max_size = _size.y;
            }

            if (_size.z < max_size)
            {
                axis = Vector3d.Axis.Z;
            }

            return axis;
        }

        public double GetShortestAxisSize()
        {
            double max_size = _size.x;

            if (_size.y < max_size)
                max_size = _size.y;

            if (_size.z < max_size)
                max_size = _size.z;

            return max_size;
        }

        public Vector3d GetSupport(Vector3d dir)
        {
            Vector3d half_extents = _size * 0.5;
            Vector3d os = _position + half_extents;

            return os + new Vector3d(
                dir.x > 0 ? -half_extents.x : half_extents.x,
                dir.y > 0 ? -half_extents.y : half_extents.y,
                dir.z > 0 ? -half_extents.z : half_extents.z);
        }

        public AABBd Grow(double by)
        {
            var res = this;

            res._position.x -= by;
            res._position.y -= by;
            res._position.z -= by;
            res._size.x += 2.0 * by;
            res._size.y += 2.0 * by;
            res._size.z += 2.0 * by;

            return res;
        }

        public bool HasNoArea()
        {
            return _size.x <= 0 || _size.y <= 0 || _size.z <= 0;
        }

        public bool HasNoSurace()
        {
            return _size.x <= 0 && _size.y <= 0 && _size.z <= 0;
        }

        public bool HasPoint(Vector3d point)
        {
            if (point.x < _position.x)
                return false;
            if (point.y < _position.y)
                return false;
            if (point.z < _position.z)
                return false;
            if (point.x > _position.x + _size.x)
                return false;
            if (point.y > _position.y + _size.y)
                return false;
            if (point.z > _position.z + _size.z)
                return false;

            return true;
        }

        public AABBd Intersection(AABBd with)
        {
            Vector3d src_min = _position;
            Vector3d src_max = _position + _size;
            Vector3d dst_min = with._position;
            Vector3d dst_max = with._position + with._size;

            Vector3d min, max;

            if (src_min.x > dst_max.x || src_max.x < dst_min.x)
            {
                return new AABBd();
            }

            min.x = src_min.x > dst_min.x ? src_min.x : dst_min.x;
            max.x = src_max.x < dst_max.x ? src_max.x : dst_max.x;

            if (src_min.y > dst_max.y || src_max.y < dst_min.y)
            {
                return new AABBd();
            }

            min.y = src_min.y > dst_min.y ? src_min.y : dst_min.y;
            max.y = src_max.y < dst_max.y ? src_max.y : dst_max.y;

            if (src_min.z > dst_max.z || src_max.z < dst_min.z)
            {
                return new AABBd();
            }

            min.z = src_min.z > dst_min.z ? src_min.z : dst_min.z;
            max.z = src_max.z < dst_max.z ? src_max.z : dst_max.z;

            return new AABBd(min, max - min);
        }

        public bool Intersects(AABBd with)
        {
            if (_position.x >= with._position.x + with._size.x)
                return false;
            if (_position.x + _size.x <= with._position.x)
                return false;
            if (_position.y >= with._position.y + with._size.y)
                return false;
            if (_position.y + _size.y <= with._position.y)
                return false;
            if (_position.z >= with._position.z + with._size.z)
                return false;
            if (_position.z + _size.z <= with._position.z)
                return false;

            return true;
        }

        public bool IntersectsPlane(Planed plane)
        {
            Vector3d[] points =
            {
                new Vector3d(_position.x, _position.y, _position.z),
                new Vector3d(_position.x, _position.y, _position.z + _size.z),
                new Vector3d(_position.x, _position.y + _size.y, _position.z),
                new Vector3d(_position.x, _position.y + _size.y, _position.z + _size.z),
                new Vector3d(_position.x + _size.x, _position.y, _position.z),
                new Vector3d(_position.x + _size.x, _position.y, _position.z + _size.z),
                new Vector3d(_position.x + _size.x, _position.y + _size.y, _position.z),
                new Vector3d(_position.x + _size.x, _position.y + _size.y, _position.z + _size.z)
            };

            bool over = false;
            bool under = false;

            for (int i = 0; i < 8; i++)
            {
                if (plane.DistanceTo(points[i]) > 0)
                    over = true;
                else
                    under = true;
            }

            return under && over;
        }

        public bool IntersectsSegment(Vector3d rom, Vector3d to)
        {
            double min = 0;
            double max = 1;

            for (int i = 0; i < 3; i++)
            {
                double segFrom = rom[i];
                double segTo = to[i];
                double boxBegin = _position[i];
                double boxEnd = boxBegin + _size[i];
                double cmin, cmax;

                if (segFrom < segTo)
                {
                    if (segFrom > boxEnd || segTo < boxBegin)
                        return false;

                    double length = segTo - segFrom;
                    cmin = segFrom < boxBegin ? (boxBegin - segFrom) / length : 0;
                    cmax = segTo > boxEnd ? (boxEnd - segFrom) / length : 1;
                }
                else
                {
                    if (segTo > boxEnd || segFrom < boxBegin)
                        return false;

                    double length = segTo - segFrom;
                    cmin = segFrom > boxEnd ? (boxEnd - segFrom) / length : 0;
                    cmax = segTo < boxBegin ? (boxBegin - segFrom) / length : 1;
                }

                if (cmin > min)
                {
                    min = cmin;
                }

                if (cmax < max)
                    max = cmax;
                if (max < min)
                    return false;
            }

            return true;
        }

        public AABBd Merge(AABBd with)
        {
            Vector3d beg1 = _position;
            Vector3d beg2 = with._position;
            var end1 = new Vector3d(_size.x, _size.y, _size.z) + beg1;
            var end2 = new Vector3d(with._size.x, with._size.y, with._size.z) + beg2;

            var min = new Vector3d(
                              beg1.x < beg2.x ? beg1.x : beg2.x,
                              beg1.y < beg2.y ? beg1.y : beg2.y,
                              beg1.z < beg2.z ? beg1.z : beg2.z
                          );

            var max = new Vector3d(
                              end1.x > end2.x ? end1.x : end2.x,
                              end1.y > end2.y ? end1.y : end2.y,
                              end1.z > end2.z ? end1.z : end2.z
                          );

            return new AABBd(min, max - min);
        }

        // Constructors
        public AABBd(Vector3d position, Vector3d size)
        {
            _position = position;
            _size = size;
        }
        public AABBd(Vector3d position, double width, double height, double depth)
        {
            _position = position;
            _size = new Vector3d(width, height, depth);
        }
        public AABBd(double x, double y, double z, Vector3d size)
        {
            _position = new Vector3d(x, y, z);
            _size = size;
        }
        public AABBd(double x, double y, double z, double width, double height, double depth)
        {
            _position = new Vector3d(x, y, z);
            _size = new Vector3d(width, height, depth);
        }

        public static explicit operator Godot.AABB(AABBd value)
        {
            return new Godot.AABB((Godot.Vector3)value.Position, (Godot.Vector3)value.Size);
        }

        public static implicit operator AABBd(Godot.AABB value)
        {
            return new AABBd(value.Position, value.Size);
        }

        public static bool operator ==(AABBd left, AABBd right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AABBd left, AABBd right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is AABBd)
            {
                return Equals((AABBd)obj);
            }

            return false;
        }

        public bool Equals(AABBd other)
        {
            return _position == other._position && _size == other._size;
        }

        public override int GetHashCode()
        {
            return _position.GetHashCode() ^ _size.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", new object[]
                {
                    _position.ToString(),
                    _size.ToString()
                });
        }

        public string ToString(string format)
        {
            return String.Format("{0} - {1}", new object[]
                {
                    _position.ToString(format),
                    _size.ToString(format)
                });
        }
    }
}
