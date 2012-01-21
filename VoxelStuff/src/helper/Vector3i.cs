using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using T = System.Int32;

namespace VoxelStuff
{
    public struct Vector3i
    {
        internal T x, y, z;

        public Vector3i(T x,T y,T z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3i operator + (Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        
        public static Vector3i operator - (Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3i operator *(Vector3i a, T i)
        {
            return new Vector3i(a.x * i, a.y * i, a.z * i);
        }

        public static Vector3i operator /(Vector3i a, T i)
        {
            return new Vector3i(a.x / i, a.y / i, a.z / i);
        }

        public static Vector3i operator %(Vector3i a, T i)
        {
            return new Vector3i(a.x % i, a.y % i, a.z % i);   
        }

        public override bool Equals(object p)
        {
            if ((object)p == null)
            {
                return false;
            }

            if (p is Vector3i)
            {
                Vector3i a = (Vector3i)p;
                return (x == a.x) && (y == a.y) && (z == a.z);
            }

            return false;
        }

        public int ToIndex()
        { 
            return 4*y+2*z+x;
        }

        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + ")";
        }


        public int getX() { return x; }
        public int getY() { return y; }
        public int getZ() { return z; }
    }
}
