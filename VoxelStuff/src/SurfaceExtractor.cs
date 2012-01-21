using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelStuff
{
    public class SurfaceExtractor
    {
        public static void extract(VoxelMap data, out List<Vector3f> vertices, out List<int> indizes, int lod,bool reuse)
        {
            vertices = new List<Vector3f>();
            indizes = new List<int>();

            int cellCnt = data.getSize();

            Cell[, ,] cells = new Cell[cellCnt, cellCnt, cellCnt];
            for (int xx = 0; xx < cellCnt; xx += lod)
                for (int yy = 0; yy < cellCnt; yy += lod)
                    for (int zz = 0; zz < cellCnt; zz += lod)
                    {
                        Cell c = new Cell(data, new Vector3i(xx, yy, zz));
                        cells[xx / lod, yy / lod, zz / lod] = c;
                        c.build(vertices, indizes, cells, lod, reuse);
                    }

            Console.WriteLine("VertexCount: " + vertices.Count + " TriCount: " + indizes.Count / 3);
        }
    }
}
