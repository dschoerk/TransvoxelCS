using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace VoxelStuff
{
    public class VoxelMap
    {
        int size = 100;
        byte[, ,] map;
        
        public VoxelMap()
        {
            map = new byte[size,size,size];

            Vector3f min = new Vector3f(size/2, size/2, size/2);
            Vector3f max = new Vector3f(size, size, size);
            float length = (max - min).Length;

            //setBox(new Vector3i(15, 5, 5), new Vector3i(10, 10, 10), (byte)-100);
            //setBox(new Vector3i(19, 9, 9), new Vector3i(10, 10, 10), -100);

            //setBox(new Vector3i(0, 0, 0), new Vector3i(size, 20, size), 100);

            Random rnd = new Random();
            PerlinNoise perlin = new PerlinNoise(rnd.Next());

            
            for(int x=5;x<size-5;x++)
                for(int y=5;y<size-5;y++)
                    for (int z = 5; z < size-5; z++)
                    {
                        float div = 128.0f;

                        double val = SimplexNoise.noise(x/div,y/div,z/div);
  		                val *= 1 << 7;
                        map[x, y, z] = (byte)(val);
                    }

            /*for (int x = 1; x < size; x++)
                for (int z = 1; z < size; z++)
                {
                    float div = 128.0f;
                    double val = SimplexNoise.noise(x / div, z / div);

                    double grenze = val*100.0;
                    int y = 1;
                    for (; y < grenze; y++)
                    { 
                        map[x, y, z] = (sbyte)(-120);
                    }
                }*/
        }

        public sbyte average(Vector3i min, int size)
        {
            size++;

            int sum = 0;
            int i=0;
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    for (int z = 0; z < size; z++)
                    {
                        sum += getPoint(x+min.getX(), y+min.getY(), z+min.getZ());
                        i++;
                    }
            
            //Console.WriteLine("SUM: "+sum+"  AVG: "+(sum/i)+" @"+i);
            return (sbyte)(sum / i);
        }

        public void setBox(Vector3i min, Vector3i size, byte value)
        { 
            for (int x = min.getX(); x < (min+size).getX(); x++)
                for (int y = min.getY(); y <= (min + size).getY(); y++)
                    for (int z = min.getZ(); z <= (min + size).getZ(); z++)
                    {
                        setPoint(x, y, z,value);
                    }
        }

        public void alterBox(Vector3i min, Vector3i size, sbyte value)
        {
            for (int x = min.getX(); x < (min + size).getX(); x++)
                for (int y = min.getY(); y <= (min + size).getY(); y++)
                    for (int z = min.getZ(); z <= (min + size).getZ(); z++)
                    {
                        alterPoint(x, y, z, value);
                    }
        }

        public byte getPoint(int x, int y, int z)
        {
            if (x < size && y < size && z < size)
                return map[x, y, z];
            else
                return 0;
        }

        public void setPoint(int x, int y, int z, byte c)
        {
            if(x < size && y < size && z < size && x > 0 && y > 0 && z > 0)
                map[x, y, z] = c;
        }

        public void setPoint(Vector3i v, byte c)
        {
            setPoint(v.getX(), v.getY(), v.getZ(), c);
        }

        public void alterPoint(int x, int y, int z, int c)
        {
            setPoint(x, y, z, (byte)(getPoint(x, y, z) + c));
        }

        public byte getPoint(Vector3i i)
        {
            return getPoint(i.getX(), i.getY(), i.getZ());
        }

        public int getSize()
        {
            return size;
        }
    }
}
