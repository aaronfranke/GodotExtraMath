using System;
using System.Runtime.InteropServices;

namespace ExtraMath
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct AABBi : IEquatable<AABBi>
    {
        private Vector3i _position;
        private Vector3i _size;

        public Vector3i Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector3i Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public Vector3i End
        {
            get { return _position + _size; }
            set { _size = value - _position; }
        }

        public bool Encloses(AABBi with)
        {
            Vector3i src_min = _position;
            Vector3i src_max = _position + _size;
            Vector3i dst_min = with._position;
            Vector3i dst_max = with._position + with._size;

            return src_min.x <= dst_min.x &&
                   src_max.x > dst_max.x &&
                   src_min.y <= dst_min.y &&
                   src_max.y > dst_max.y &&
                   src_min.z <= dst_min.z &&
                   src_max.z > dst_max.z;
        }

        public AABBi Expand(Vector3i point)
        {
            Vector3i begin = _position;
            Vector3i end = _position + _size;

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

            return new AABBi(begin, end - begin);
        }

        public int GetArea()
        {
            return _size.x * _size.y * _size.z;
        }

        public Vector3i GetEndpoint(int idx)
        {
            switch (idx)
            {
                case 0:
                    return new Vector3i(_position.x, _position.y, _position.z);
                case 1:
                    return new Vector3i(_position.x, _position.y, _position.z + _size.z);
                case 2:
                    return new Vector3i(_position.x, _position.y + _size.y, _position.z);
                case 3:
                    return new Vector3i(_position.x, _position.y + _size.y, _position.z + _size.z);
                case 4:
                    return new Vector3i(_position.x + _size.x, _position.y, _position.z);
                case 5:
                    return new Vector3i(_position.x + _size.x, _position.y, _position.z + _size.z);
                case 6:
                    return new Vector3i(_position.x + _size.x, _position.y + _size.y, _position.z);
                case 7:
                    return new Vector3i(_position.x + _size.x, _position.y + _size.y, _position.z + _size.z);
                default:
                    throw new ArgumentOutOfRangeException(nameof(idx), String.Format("Index is {0}, but a value from 0 to 7 is expected.", idx));
            }
        }

        public Vector3i GetLongestAxis()
        {
            var axis = new Vector3i(1, 0, 0);
            int max_size = _size.x;

            if (_size.y > max_size)
            {
                axis = new Vector3i(0, 1, 0);
                max_size = _size.y;
            }

            if (_size.z > max_size)
            {
                axis = new Vector3i(0, 0, 1);
            }

            return axis;
        }

        public Vector3i.Axis GetLongestAxisIndex()
        {
            var axis = Vector3i.Axis.X;
            int max_size = _size.x;

            if (_size.y > max_size)
            {
                axis = Vector3i.Axis.Y;
                max_size = _size.y;
            }

            if (_size.z > max_size)
            {
                axis = Vector3i.Axis.Z;
            }

            return axis;
        }

        public int GetLongestAxisSize()
        {
            int max_size = _size.x;

            if (_size.y > max_size)
                max_size = _size.y;

            if (_size.z > max_size)
                max_size = _size.z;

            return max_size;
        }

        public Vector3i GetShortestAxis()
        {
            var axis = new Vector3i(1, 0, 0);
            int max_size = _size.x;

            if (_size.y < max_size)
            {
                axis = new Vector3i(0, 1, 0);
                max_size = _size.y;
            }

            if (_size.z < max_size)
            {
                axis = new Vector3i(0, 0, 1);
            }

            return axis;
        }

        public Vector3i.Axis GetShortestAxisIndex()
        {
            var axis = Vector3i.Axis.X;
            int max_size = _size.x;

            if (_size.y < max_size)
            {
                axis = Vector3i.Axis.Y;
                max_size = _size.y;
            }

            if (_size.z < max_size)
            {
                axis = Vector3i.Axis.Z;
            }

            return axis;
        }

        public int GetShortestAxisSize()
        {
            int max_size = _size.x;

            if (_size.y < max_size)
                max_size = _size.y;

            if (_size.z < max_size)
                max_size = _size.z;

            return max_size;
        }

        public AABBi Grow(int by)
        {
            var res = this;

            res._position.x -= by;
            res._position.y -= by;
            res._position.z -= by;
            res._size.x += 2 * by;
            res._size.y += 2 * by;
            res._size.z += 2 * by;

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

        public bool HasPoint(Vector3i point)
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

        public AABBi Intersection(AABBi with)
        {
            Vector3i src_min = _position;
            Vector3i src_max = _position + _size;
            Vector3i dst_min = with._position;
            Vector3i dst_max = with._position + with._size;

            Vector3i min, max;

            if (src_min.x > dst_max.x || src_max.x < dst_min.x)
            {
                return new AABBi();
            }

            min.x = src_min.x > dst_min.x ? src_min.x : dst_min.x;
            max.x = src_max.x < dst_max.x ? src_max.x : dst_max.x;

            if (src_min.y > dst_max.y || src_max.y < dst_min.y)
            {
                return new AABBi();
            }

            min.y = src_min.y > dst_min.y ? src_min.y : dst_min.y;
            max.y = src_max.y < dst_max.y ? src_max.y : dst_max.y;

            if (src_min.z > dst_max.z || src_max.z < dst_min.z)
            {
                return new AABBi();
            }

            min.z = src_min.z > dst_min.z ? src_min.z : dst_min.z;
            max.z = src_max.z < dst_max.z ? src_max.z : dst_max.z;

            return new AABBi(min, max - min);
        }

        public bool Intersects(AABBi with)
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

        public bool IntersectsSegment(Vector3i from, Vector3i to)
        {
            int min = 0;
            int max = 1;

            for (int i = 0; i < 3; i++)
            {
                int segFrom = from[i];
                int segTo = to[i];
                int boxBegin = _position[i];
                int boxEnd = boxBegin + _size[i];
                int cmin, cmax;

                if (segFrom < segTo)
                {
                    if (segFrom > boxEnd || segTo < boxBegin)
                        return false;

                    int length = segTo - segFrom;
                    cmin = segFrom < boxBegin ? (boxBegin - segFrom) / length : 0;
                    cmax = segTo > boxEnd ? (boxEnd - segFrom) / length : 1;
                }
                else
                {
                    if (segTo > boxEnd || segFrom < boxBegin)
                        return false;

                    int length = segTo - segFrom;
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

        public AABBi Merge(AABBi with)
        {
            Vector3i beg1 = _position;
            Vector3i beg2 = with._position;
            var end1 = new Vector3i(_size.x, _size.y, _size.z) + beg1;
            var end2 = new Vector3i(with._size.x, with._size.y, with._size.z) + beg2;

            var min = new Vector3i(
                              beg1.x < beg2.x ? beg1.x : beg2.x,
                              beg1.y < beg2.y ? beg1.y : beg2.y,
                              beg1.z < beg2.z ? beg1.z : beg2.z
                          );

            var max = new Vector3i(
                              end1.x > end2.x ? end1.x : end2.x,
                              end1.y > end2.y ? end1.y : end2.y,
                              end1.z > end2.z ? end1.z : end2.z
                          );

            return new AABBi(min, max - min);
        }

        // Constructors
        public AABBi(Vector3i position, Vector3i size)
        {
            _position = position;
            _size = size;
        }
        public AABBi(Vector3i position, int width, int height, int depth)
        {
            _position = position;
            _size = new Vector3i(width, height, depth);
        }
        public AABBi(int x, int y, int z, Vector3i size)
        {
            _position = new Vector3i(x, y, z);
            _size = size;
        }
        public AABBi(int x, int y, int z, int width, int height, int depth)
        {
            _position = new Vector3i(x, y, z);
            _size = new Vector3i(width, height, depth);
        }

        public static explicit operator AABBi(Godot.AABB value)
        {
            return new AABBi((Vector3i)value.Position, (Vector3i)value.Size);
        }

        public static implicit operator Godot.AABB(AABBi value)
        {
            return new Godot.AABB(value.Position, value.Size);
        }

        public static bool operator ==(AABBi left, AABBi right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AABBi left, AABBi right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is AABBi)
            {
                return Equals((AABBi)obj);
            }

            return false;
        }

        public bool Equals(AABBi other)
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
