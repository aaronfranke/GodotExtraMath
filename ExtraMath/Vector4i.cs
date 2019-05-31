using Godot;
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
    /// 3-element structure that can be used to represent 3D grid coordinates or sets of integers.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4i : IEquatable<Vector4i>
    {
        public enum Axis
        {
            X = 0,
            Y,
            Z,
            W
        }

        public int x;
        public int y;
        public int z;
        public int w;

        public Vector3i XYZ
        {
            get
            {
                return new Vector3i(x, y, z);
            }
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }

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
                    case 2:
                        return z;
                    case 3:
                        return w;
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
                    case 2:
                        z = value;
                        return;
                    case 3:
                        w = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public Vector4i Abs()
        {
            return new Vector4i(Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z), Mathf.Abs(w));
        }

        public int DistanceSquaredTo(Vector4i b)
        {
            return (b - this).LengthSquared();
        }

        public real_t DistanceTo(Vector4i b)
        {
            return (b - this).Length();
        }

        public int Dot(Vector4i b)
        {
            return x * b.x + y * b.y + z * b.z + w * b.w;
        }

        public real_t Length()
        {
            int x2 = x * x;
            int y2 = y * y;
            int z2 = z * z;
            int w2 = w * w;

            return Mathf.Sqrt(x2 + y2 + z2 + w2);
        }

        public int LengthSquared()
        {
            int x2 = x * x;
            int y2 = y * y;
            int z2 = z * z;
            int w2 = w * w;

            return x2 + y2 + z2 + w2;
        }

        public Axis MaxAxis()
        {
            byte index = 0;
            for (byte i = 1; i < 4; i++)
            {
                if (this[i] > this[index])
                {
                    index = i;
                }
            }
            return (Axis)index;
        }

        public Axis MinAxis()
        {
            byte index = 0;
            for (byte i = 1; i < 4; i++)
            {
                if (this[i] < this[index])
                {
                    index = i;
                }
            }
            return (Axis)index;
        }

        public Vector4i PosMod(int mod)
        {
            Vector4i v = this;
            v.x = Mathf.PosMod(v.x, mod);
            v.y = Mathf.PosMod(v.y, mod);
            v.z = Mathf.PosMod(v.z, mod);
            v.w = Mathf.PosMod(v.w, mod);
            return v;
        }

        public Vector4i PosMod(Vector4i modv)
        {
            Vector4i v = this;
            v.x = Mathf.PosMod(v.x, modv.x);
            v.y = Mathf.PosMod(v.y, modv.y);
            v.z = Mathf.PosMod(v.z, modv.z);
            v.w = Mathf.PosMod(v.w, modv.w);
            return v;
        }

        public void Set(real_t x, real_t y, real_t z, real_t w)
        {
            this.x = Mathf.RoundToInt(x);
            this.y = Mathf.RoundToInt(y);
            this.z = Mathf.RoundToInt(z);
            this.w = Mathf.RoundToInt(w);
        }
        public void Set(int x, int y, int z, int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public void Set(Vector4i v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = v.w;
        }
        public void Set(Vector4 v)
        {
            this.x = Mathf.RoundToInt(v.x);
            this.y = Mathf.RoundToInt(v.y);
            this.z = Mathf.RoundToInt(v.z);
            this.w = Mathf.RoundToInt(v.w);
        }

        public Vector4i Sign()
        {
            Vector4i v = this;
            v.x = Mathf.Sign(v.x);
            v.y = Mathf.Sign(v.y);
            v.z = Mathf.Sign(v.z);
            v.w = Mathf.Sign(v.w);
            return v;
        }

        public Vector2i[] UnpackVector2()
        {
            Vector2i[] arr = new Vector2i[2];
            arr[0] = new Vector2i(x, y);
            arr[1] = new Vector2i(z, w);
            return arr;
        }

        public void UnpackVector2(out Vector2i xy, out Vector2i zw)
        {
            xy = new Vector2i(x, y);
            zw = new Vector2i(z, w);
        }

        // Constants
        private static readonly Vector4i _zero = new Vector4i(0, 0, 0, 0);
        private static readonly Vector4i _one = new Vector4i(1, 1, 1, 1);
        private static readonly Vector4i _negOne = new Vector4i(-1, -1, -1, -1);

        private static readonly Vector4i _unitX = new Vector4i(1, 0, 0, 0);
        private static readonly Vector4i _unitY = new Vector4i(0, 1, 0, 0);
        private static readonly Vector4i _unitZ = new Vector4i(0, 0, 1, 0);
        private static readonly Vector4i _unitW = new Vector4i(0, 0, 0, 1);

        public static Vector4i Zero { get { return _zero; } }
        public static Vector4i One { get { return _one; } }
        public static Vector4i NegOne { get { return _negOne; } }

        public static Vector4i UnitX { get { return _unitX; } }
        public static Vector4i UnitY { get { return _unitY; } }
        public static Vector4i UnitZ { get { return _unitZ; } }
        public static Vector4i UnitW { get { return _unitW; } }

        // Constructors
        public Vector4i(int x, int y, int z, int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public Vector4i(Vector4i v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = v.w;
        }
        public Vector4i(Vector4 v)
        {
            this.x = Mathf.RoundToInt(v.x);
            this.y = Mathf.RoundToInt(v.y);
            this.z = Mathf.RoundToInt(v.z);
            this.w = Mathf.RoundToInt(v.w);
        }
        public Vector4i(Vector3i xyz, int w)
        {
            x = xyz.x;
            y = xyz.y;
            z = xyz.z;
            this.w = w;
        }
        public Vector4i(Vector2i xy, Vector2i zw)
        {
            x = xy.x;
            y = xy.y;
            z = zw.x;
            w = zw.y;
        }

        public static implicit operator Vector4(Vector4i value)
        {
            return new Vector4(value.x, value.y, value.z, value.w);
        }

        public static explicit operator Vector4i(Vector4 value)
        {
            return new Vector4i(value);
        }

        public static Vector4i operator +(Vector4i left, Vector4i right)
        {
            left.x += right.x;
            left.y += right.y;
            left.z += right.z;
            left.w += right.w;
            return left;
        }

        public static Vector4i operator -(Vector4i left, Vector4i right)
        {
            left.x -= right.x;
            left.y -= right.y;
            left.z -= right.z;
            left.w -= right.w;
            return left;
        }

        public static Vector4i operator -(Vector4i vec)
        {
            vec.x = -vec.x;
            vec.y = -vec.y;
            vec.z = -vec.z;
            vec.w = -vec.w;
            return vec;
        }

        public static Vector4i operator *(Vector4i vec, int scale)
        {
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            vec.w *= scale;
            return vec;
        }

        public static Vector4i operator *(int scale, Vector4i vec)
        {
            vec.x *= scale;
            vec.y *= scale;
            vec.z *= scale;
            vec.w *= scale;
            return vec;
        }

        public static Vector4i operator *(Vector4i left, Vector4i right)
        {
            left.x *= right.x;
            left.y *= right.y;
            left.z *= right.z;
            left.w *= right.w;
            return left;
        }

        public static Vector4i operator /(Vector4i vec, int scale)
        {
            vec.x /= scale;
            vec.y /= scale;
            vec.z /= scale;
            vec.w /= scale;
            return vec;
        }

        public static Vector4i operator /(Vector4i left, Vector4i right)
        {
            left.x /= right.x;
            left.y /= right.y;
            left.z /= right.z;
            left.w /= right.w;
            return left;
        }

        public static Vector4i operator %(Vector4i vec, int divisor)
        {
            vec.x %= divisor;
            vec.y %= divisor;
            vec.z %= divisor;
            vec.w %= divisor;
            return vec;
        }

        public static Vector4i operator %(Vector4i vec, Vector4i divisorv)
        {
            vec.x %= divisorv.x;
            vec.y %= divisorv.y;
            vec.z %= divisorv.z;
            vec.w %= divisorv.w;
            return vec;
        }

        public static Vector4i operator &(Vector4i vec, int and)
        {
            vec.x &= and;
            vec.y &= and;
            vec.z &= and;
            vec.w &= and;
            return vec;
        }

        public static Vector4i operator &(Vector4i vec, Vector4i andv)
        {
            vec.x &= andv.x;
            vec.y &= andv.y;
            vec.z &= andv.z;
            vec.w &= andv.w;
            return vec;
        }

        public static bool operator ==(Vector4i left, Vector4i right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector4i left, Vector4i right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(Vector4i left, Vector4i right)
        {
            if (left.x == right.x)
            {
                if (left.y == right.y)
                {
                    if (left.z == right.z)
                    {
                        return left.w < right.w;
                    }
                    return left.z < right.z;
                }
                return left.y < right.y;
            }
            return left.x < right.x;
        }

        public static bool operator >(Vector4i left, Vector4i right)
        {
            if (left.x == right.x)
            {
                if (left.y == right.y)
                {
                    if (left.z == right.z)
                    {
                        return left.w > right.w;
                    }
                    return left.z > right.z;
                }
                return left.y > right.y;
            }
            return left.x > right.x;
        }

        public static bool operator <=(Vector4i left, Vector4i right)
        {
            if (left.x == right.x)
            {
                if (left.y == right.y)
                {
                    if (left.z == right.z)
                    {
                        return left.w <= right.w;
                    }
                    return left.z < right.z;
                }
                return left.y < right.y;
            }
            return left.x < right.x;
        }

        public static bool operator >=(Vector4i left, Vector4i right)
        {
            if (left.x == right.x)
            {
                if (left.y == right.y)
                {
                    if (left.z == right.z)
                    {
                        return left.w >= right.w;
                    }
                    return left.z > right.z;
                }
                return left.y > right.y;
            }
            return left.x > right.x;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector4i)
            {
                return Equals((Vector4i)obj);
            }

            return false;
        }

        public bool Equals(Vector4i other)
        {
            return x == other.x && y == other.y && z == other.z && w == other.w;
        }

        public override int GetHashCode()
        {
            return y.GetHashCode() ^ x.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2}, {3})", new object[]
            {
                x.ToString(),
                y.ToString(),
                z.ToString(),
                w.ToString()
            });
        }

        public string ToString(string format)
        {
            return String.Format("({0}, {1}, {2}, {3})", new object[]
            {
                x.ToString(format),
                y.ToString(format),
                z.ToString(format),
                w.ToString(format)
            });
        }
    }
}



