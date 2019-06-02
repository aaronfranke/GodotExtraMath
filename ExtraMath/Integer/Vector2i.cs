#if GODOT
using Godot;
#elif UNITY_5_3_OR_NEWER
using UnityEngine;
#endif
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
    /// 2-element structure that can be used to represent 2D grid coordinates or pairs of integers.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2i : IEquatable<Vector2i>
    {
        public enum Axis
        {
            X = 0,
            Y
        }

        public int x;
        public int y;

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        return;
                    case 1:
                        y = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public Vector2i Abs()
        {
            Vector2i v = this;
            if (v.x < 0)
            {
                v.x = -v.x;
            }
            if (v.y < 0)
            {
                v.y = -v.y;
            }
            return v;
        }

        public int DistanceSquaredTo(Vector2i b)
        {
            return (b - this).LengthSquared();
        }

        public real_t DistanceTo(Vector2i b)
        {
            return (b - this).Length();
        }

        public int Dot(Vector2i b)
        {
            return x * b.x + y * b.y;
        }

        public real_t Length()
        {
            int x2 = x * x;
            int y2 = y * y;

            return Mathf.Sqrt(x2 + y2);
        }

        public int LengthSquared()
        {
            int x2 = x * x;
            int y2 = y * y;

            return x2 + y2;
        }

        public Axis MaxAxis()
        {
            return x < y ? Axis.Y : Axis.X;
        }

        public Axis MinAxis()
        {
            return x < y ? Axis.X : Axis.Y;
        }

#if GODOT
        public Vector2i PosMod(int mod)
        {
            Vector2i v = this;
            v.x = Mathf.PosMod(v.x, mod);
            v.y = Mathf.PosMod(v.y, mod);
            return v;
        }

        public Vector2i PosMod(Vector2i modv)
        {
            Vector2i v = this;
            v.x = Mathf.PosMod(v.x, modv.x);
            v.y = Mathf.PosMod(v.y, modv.y);
            return v;
        }
#endif

        public Vector2i Sign()
        {
            Vector2i v = this;
#if GODOT
            v.x = Mathf.Sign(v.x);
            v.y = Mathf.Sign(v.y);
#else
            v.x = v.x < 0 ? -1 : 1;
            v.y = v.y < 0 ? -1 : 1;
#endif
            return v;
        }

        public Vector2i Tangent()
        {
            return new Vector2i(y, -x);
        }

        // Constants
        private static readonly Vector2i _zero = new Vector2i(0, 0);
        private static readonly Vector2i _one = new Vector2i(1, 1);
        private static readonly Vector2i _negOne = new Vector2i(-1, -1);

        private static readonly Vector2i _up = new Vector2i(0, -1);
        private static readonly Vector2i _down = new Vector2i(0, 1);
        private static readonly Vector2i _right = new Vector2i(1, 0);
        private static readonly Vector2i _left = new Vector2i(-1, 0);

        public static Vector2i Zero { get { return _zero; } }
        public static Vector2i One { get { return _one; } }
        public static Vector2i NegOne { get { return _negOne; } }

        public static Vector2i Up { get { return _up; } }
        public static Vector2i Down { get { return _down; } }
        public static Vector2i Right { get { return _right; } }
        public static Vector2i Left { get { return _left; } }

        // Constructors
        public Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2i(Vector2i v)
        {
            this.x = v.x;
            this.y = v.y;
        }
        public Vector2i(Vector2 v)
        {
            this.x = Mathf.RoundToInt(v.x);
            this.y = Mathf.RoundToInt(v.y);
        }

        public static implicit operator Vector2(Vector2i value)
        {
            return new Vector2(value.x, value.y);
        }

        public static explicit operator Vector2i(Vector2 value)
        {
            return new Vector2i(value);
        }

#if UNITY_5_3_OR_NEWER
        public static implicit operator Vector2Int(Vector2i value)
        {
            return new Vector2Int(value.x, value.y);
        }

        public static explicit operator Vector2i(Vector2Int value)
        {
            return new Vector2i(value);
        }
#endif

        public static Vector2i operator +(Vector2i left, Vector2i right)
        {
            left.x += right.x;
            left.y += right.y;
            return left;
        }

        public static Vector2i operator -(Vector2i left, Vector2i right)
        {
            left.x -= right.x;
            left.y -= right.y;
            return left;
        }

        public static Vector2i operator -(Vector2i vec)
        {
            vec.x = -vec.x;
            vec.y = -vec.y;
            return vec;
        }

        public static Vector2i operator *(Vector2i vec, int scale)
        {
            vec.x *= scale;
            vec.y *= scale;
            return vec;
        }

        public static Vector2i operator *(int scale, Vector2i vec)
        {
            vec.x *= scale;
            vec.y *= scale;
            return vec;
        }

        public static Vector2i operator *(Vector2i left, Vector2i right)
        {
            left.x *= right.x;
            left.y *= right.y;
            return left;
        }

        public static Vector2i operator /(Vector2i vec, int scale)
        {
            vec.x /= scale;
            vec.y /= scale;
            return vec;
        }

        public static Vector2i operator /(Vector2i left, Vector2i right)
        {
            left.x /= right.x;
            left.y /= right.y;
            return left;
        }

        public static Vector2i operator %(Vector2i vec, int divisor)
        {
            vec.x %= divisor;
            vec.y %= divisor;
            return vec;
        }

        public static Vector2i operator %(Vector2i vec, Vector2i divisorv)
        {
            vec.x %= divisorv.x;
            vec.y %= divisorv.y;
            return vec;
        }

        public static Vector2i operator &(Vector2i vec, int and)
        {
            vec.x &= and;
            vec.y &= and;
            return vec;
        }

        public static Vector2i operator &(Vector2i vec, Vector2i andv)
        {
            vec.x &= andv.x;
            vec.y &= andv.y;
            return vec;
        }

        public static bool operator ==(Vector2i left, Vector2i right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2i left, Vector2i right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(Vector2i left, Vector2i right)
        {
            if (left.x.Equals(right.x))
            {
                return left.y < right.y;
            }
            return left.x < right.x;
        }

        public static bool operator >(Vector2i left, Vector2i right)
        {
            if (left.x.Equals(right.x))
            {
                return left.y > right.y;
            }
            return left.x > right.x;
        }

        public static bool operator <=(Vector2i left, Vector2i right)
        {
            if (left.x.Equals(right.x))
            {
                return left.y <= right.y;
            }
            return left.x <= right.x;
        }

        public static bool operator >=(Vector2i left, Vector2i right)
        {
            if (left.x.Equals(right.x))
            {
                return left.y >= right.y;
            }
            return left.x >= right.x;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2i)
            {
                return Equals((Vector2i)obj);
            }

            return false;
        }

        public bool Equals(Vector2i other)
        {
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            return y.GetHashCode() ^ x.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", new object[]
            {
                this.x.ToString(),
                this.y.ToString()
            });
        }

        public string ToString(string format)
        {
            return String.Format("({0}, {1})", new object[]
            {
                this.x.ToString(format),
                this.y.ToString(format)
            });
        }
    }
}



