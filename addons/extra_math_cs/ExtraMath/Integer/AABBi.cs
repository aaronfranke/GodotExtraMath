#if GODOT
using Godot;
#elif UNITY_5_3_OR_NEWER
using UnityEngine;
#endif
using System;
using System.Runtime.InteropServices;

namespace ExtraMath
{
    /// <summary>
    /// Axis-Aligned Bounding Box. AABBi consists of a position, a size, and
    /// several utility functions. It is typically used for fast overlap tests.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct AABBi : IEquatable<AABBi>
    {
        private Vector3i _position;
        private Vector3i _size;

        /// <summary>
        /// Beginning corner. Typically has values lower than End.
        /// </summary>
        /// <value>Directly uses a private field.</value>
        public Vector3i Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Size from Position to End. Typically all components are positive.
        /// If the size is negative, you can use <see cref="Abs"/> to fix it.
        /// </summary>
        /// <value>Directly uses a private field.</value>
        public Vector3i Size
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// Ending corner. This is calculated as <see cref="Position"/> plus
        /// <see cref="Size"/>. Setting this value will change the size.
        /// </summary>
        /// <value>Getting is equivalent to `value = Position + Size`, setting is equivalent to `Size = value - Position`.</value>
        public Vector3i End
        {
            get { return _position + _size; }
            set { _size = value - _position; }
        }

        /// <summary>
        /// Returns an AABBi with equivalent position and size, modified so that
        /// the most-negative corner is the origin and the size is positive.
        /// </summary>
        /// <returns>The modified AABBi.</returns>
        public AABBi Abs()
        {
            Vector3i end = End;
            Vector3i topLeft = new Vector3i(Mathf.Min(_position.x, end.x), Mathf.Min(_position.y, end.y), Mathf.Min(_position.z, end.z));
            return new AABBi(topLeft, _size.Abs());
        }

        /// <summary>
        /// Returns true if this AABBi completely encloses another one.
        /// </summary>
        /// <param name="with">The other AABBi that may be enclosed.</param>
        /// <returns>A bool for whether or not this AABBi encloses `b`.</returns>
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

        /// <summary>
        /// Returns this AABBi expanded to include a given point.
        /// </summary>
        /// <param name="point">The point to include.</param>
        /// <returns>The expanded AABBi.</returns>
        public AABBi Expand(Vector3i point)
        {
            Vector3i begin = _position;
            Vector3i end = _position + _size;

            if (point.x < begin.x)
            {
                begin.x = point.x;
            }
            if (point.y < begin.y)
            {
                begin.y = point.y;
            }
            if (point.z < begin.z)
            {
                begin.z = point.z;
            }

            if (point.x > end.x)
            {
                end.x = point.x;
            }
            if (point.y > end.y)
            {
                end.y = point.y;
            }
            if (point.z > end.z)
            {
                end.z = point.z;
            }

            return new AABBi(begin, end - begin);
        }

        /// <summary>
        /// Returns the area of the AABBi.
        /// </summary>
        /// <returns>The area.</returns>
        public int GetArea()
        {
            return _size.x * _size.y * _size.z;
        }

        /// <summary>
        /// Gets the position of one of the 8 endpoints of the AABBi.
        /// </summary>
        /// <param name="idx">Which endpoint to get.</param>
        /// <returns>An endpoint of the AABBi.</returns>
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

        /// <summary>
        /// Returns the normalized longest axis of the AABBi.
        /// </summary>
        /// <returns>A vector representing the normalized longest axis of the AABBi.</returns>
        public Vector3i GetLongestAxis()
        {
            Vector3i axis = new Vector3i(1, 0, 0);
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

        /// <summary>
        /// Returns the <see cref="Vector3i.Axis"/> index of the longest axis of the AABBi.
        /// </summary>
        /// <returns>A <see cref="Vector3i.Axis"/> index for which axis is longest.</returns>
        public Vector3i.Axis GetLongestAxisIndex()
        {
            Vector3i.Axis axis = Vector3i.Axis.X;
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

        /// <summary>
        /// Returns the scalar length of the longest axis of the AABBi.
        /// </summary>
        /// <returns>The scalar length of the longest axis of the AABBi.</returns>
        public int GetLongestAxisSize()
        {
            int max_size = _size.x;

            if (_size.y > max_size)
            {
                max_size = _size.y;
            }

            if (_size.z > max_size)
            {
                max_size = _size.z;
            }

            return max_size;
        }

        /// <summary>
        /// Returns the normalized shortest axis of the AABBi.
        /// </summary>
        /// <returns>A vector representing the normalized shortest axis of the AABBi.</returns>
        public Vector3i GetShortestAxis()
        {
            Vector3i axis = new Vector3i(1, 0, 0);
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

        /// <summary>
        /// Returns the <see cref="Vector3i.Axis"/> index of the shortest axis of the AABBi.
        /// </summary>
        /// <returns>A <see cref="Vector3i.Axis"/> index for which axis is shortest.</returns>
        public Vector3i.Axis GetShortestAxisIndex()
        {
            Vector3i.Axis axis = Vector3i.Axis.X;
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

        /// <summary>
        /// Returns the scalar length of the shortest axis of the AABBi.
        /// </summary>
        /// <returns>The scalar length of the shortest axis of the AABBi.</returns>
        public int GetShortestAxisSize()
        {
            int min_size = _size.x;

            if (_size.y < min_size)
            {
                min_size = _size.y;
            }

            if (_size.z < min_size)
            {
                min_size = _size.z;
            }

            return min_size;
        }

        /// <summary>
        /// Returns a copy of the AABBi grown a given amount of units towards all the sides.
        /// </summary>
        /// <param name="by">The amount to grow by.</param>
        /// <returns>The grown AABBi.</returns>
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

        /// <summary>
        /// Returns true if the AABBi is flat or empty, or false otherwise.
        /// </summary>
        /// <returns>A bool for whether or not the AABBi has area.</returns>
        public bool HasNoArea()
        {
            return _size.x <= 0 || _size.y <= 0 || _size.z <= 0;
        }

        /// <summary>
        /// Returns true if the AABBi has no surface (no size), or false otherwise.
        /// </summary>
        /// <returns>A bool for whether or not the AABBi has area.</returns>
        public bool HasNoSurface()
        {
            return _size.x <= 0 && _size.y <= 0 && _size.z <= 0;
        }

        /// <summary>
        /// Returns true if the AABBi contains a point, or false otherwise.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>A bool for whether or not the AABBi contains `point`.</returns>
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

        /// <summary>
        /// Returns the intersection of this AABBi and `b`.
        /// </summary>
        /// <param name="with">The other AABBi.</param>
        /// <returns>The clipped AABBi.</returns>
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

        /// <summary>
        /// Returns true if the AABBi overlaps with `b`
        /// (i.e. they have at least one point in common).
        ///
        /// If `includeBorders` is true, they will also be considered overlapping
        /// if their borders touch, even without intersection.
        /// </summary>
        /// <param name="with">The other AABBi to check for intersections with.</param>
        /// <param name="includeBorders">Whether or not to consider borders.</param>
        /// <returns>A bool for whether or not they are intersecting.</returns>
        public bool Intersects(AABBi with, bool includeBorders = false)
        {
            if (includeBorders)
            {
                if (_position.x > with._position.x + with._size.x)
                    return false;
                if (_position.x + _size.x < with._position.x)
                    return false;
                if (_position.y > with._position.y + with._size.y)
                    return false;
                if (_position.y + _size.y < with._position.y)
                    return false;
                if (_position.z > with._position.z + with._size.z)
                    return false;
                if (_position.z + _size.z < with._position.z)
                    return false;
            }
            else
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
            }

            return true;
        }

        /// <summary>
        /// Returns true if the AABBi is on both sides of `plane`.
        /// </summary>
        /// <param name="plane">The plane to check for intersection.</param>
        /// <returns>A bool for whether or not the AABBi intersects the plane.</returns>
        public bool IntersectsPlane(Plane plane)
        {
            Vector3i[] points =
            {
                new Vector3i(_position.x, _position.y, _position.z),
                new Vector3i(_position.x, _position.y, _position.z + _size.z),
                new Vector3i(_position.x, _position.y + _size.y, _position.z),
                new Vector3i(_position.x, _position.y + _size.y, _position.z + _size.z),
                new Vector3i(_position.x + _size.x, _position.y, _position.z),
                new Vector3i(_position.x + _size.x, _position.y, _position.z + _size.z),
                new Vector3i(_position.x + _size.x, _position.y + _size.y, _position.z),
                new Vector3i(_position.x + _size.x, _position.y + _size.y, _position.z + _size.z)
            };

            bool over = false;
            bool under = false;

            for (int i = 0; i < 8; i++)
            {
#if GODOT
                if (plane.DistanceTo(points[i]) > 0)
#elif UNITY_5_3_OR_NEWER
                if (plane.GetDistanceToPoint(points[i]) > 0)
#endif
                {
                    over = true;
                }
                else
                {
                    under = true;
                }
            }

            return under && over;
        }

        /// <summary>
        /// Returns true if the AABBi intersects the line segment between `from` and `to`.
        /// </summary>
        /// <param name="from">The start of the line segment.</param>
        /// <param name="to">The end of the line segment.</param>
        /// <returns>A bool for whether or not the AABBi intersects the line segment.</returns>
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
                    {
                        return false;
                    }

                    int length = segTo - segFrom;
                    cmin = segFrom < boxBegin ? (boxBegin - segFrom) / length : 0;
                    cmax = segTo > boxEnd ? (boxEnd - segFrom) / length : 1;
                }
                else
                {
                    if (segTo > boxEnd || segFrom < boxBegin)
                    {
                        return false;
                    }

                    int length = segTo - segFrom;
                    cmin = segFrom > boxEnd ? (boxEnd - segFrom) / length : 0;
                    cmax = segTo < boxBegin ? (boxBegin - segFrom) / length : 1;
                }

                if (cmin > min)
                {
                    min = cmin;
                }

                if (cmax < max)
                {
                    max = cmax;
                }
                if (max < min)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a larger AABBi that contains this AABBi and `b`.
        /// </summary>
        /// <param name="with">The other AABBi.</param>
        /// <returns>The merged AABBi.</returns>
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

        /// <summary>
        /// Constructs an AABBi from a position and size.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="size">The size, typically positive.</param>
        public AABBi(Vector3i position, Vector3i size)
        {
            _position = position;
            _size = size;
        }

        /// <summary>
        /// Constructs an AABBi from a position, width, height, and depth.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="width">The width, typically positive.</param>
        /// <param name="height">The height, typically positive.</param>
        /// <param name="depth">The depth, typically positive.</param>
        public AABBi(Vector3i position, int width, int height, int depth)
        {
            _position = position;
            _size = new Vector3i(width, height, depth);
        }

        /// <summary>
        /// Constructs an AABBi from x, y, z, and size.
        /// </summary>
        /// <param name="x">The position's X coordinate.</param>
        /// <param name="y">The position's Y coordinate.</param>
        /// <param name="z">The position's Z coordinate.</param>
        /// <param name="size">The size, typically positive.</param>
        public AABBi(int x, int y, int z, Vector3i size)
        {
            _position = new Vector3i(x, y, z);
            _size = size;
        }

        /// <summary>
        /// Constructs an AABBi from x, y, z, width, height, and depth.
        /// </summary>
        /// <param name="x">The position's X coordinate.</param>
        /// <param name="y">The position's Y coordinate.</param>
        /// <param name="z">The position's Z coordinate.</param>
        /// <param name="width">The width, typically positive.</param>
        /// <param name="height">The height, typically positive.</param>
        /// <param name="depth">The depth, typically positive.</param>
        public AABBi(int x, int y, int z, int width, int height, int depth)
        {
            _position = new Vector3i(x, y, z);
            _size = new Vector3i(width, height, depth);
        }

#if GODOT
        public static explicit operator AABBi(Godot.AABB value)
        {
            return new AABBi((Vector3i)value.Position, (Vector3i)value.Size);
        }

        public static implicit operator Godot.AABB(AABBi value)
        {
            return new Godot.AABB(value.Position, value.Size);
        }
#endif

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
