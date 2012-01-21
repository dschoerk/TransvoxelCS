using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using T = System.Single;

namespace VoxelStuff
{
    public struct Vector3f
    {
        internal T x, y, z;

        public Vector3f(T x, T y, T z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3f operator +(Vector3f a, Vector3f b)
        {
            return new Vector3f(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3f operator -(Vector3f a, Vector3f b)
        {
            return new Vector3f(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3f operator *(Vector3f a, T i)
        {
            return new Vector3f(a.x * i, a.y * i, a.z * i);
        }

        public static Vector3f operator /(Vector3f a, T i)
        {
            return new Vector3f(a.x / i, a.y / i, a.z / i);
        }

        public static Vector3f operator %(Vector3f a, T i)
        {
            return new Vector3f(a.x % i, a.y % i, a.z % i);
        }

        public T Length
        {
            get { return (T)Math.Sqrt(x * x + y * y + z * z); }
        }

        public override bool Equals(object p)
        {
            if ((object)p == null)
            {
                return false;
            }

            if (p is Vector3f)
            {
                Vector3f a = (Vector3f)p;
                return (x == a.x) && (y == a.y) && (z == a.z);
            }

            return false;
        }

        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + ")";
        }


        public T getX() { return x; }
        public T getY() { return y; }
        public T getZ() { return z; }
    }
}
