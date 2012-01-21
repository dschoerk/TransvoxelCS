using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace VoxelStuff
{
	public class Cell
	{
		VoxelMap data;
		Vector3i pos;
		public int[] rind;

		int voxelStep = 1;

		public Cell(VoxelMap data, Vector3i pos)
		{
			this.data = data;
			this.pos = pos;         
		}

		public void build(List<Vector3f> vert, List<int> ind, Cell[, ,] cells,int lod,bool vertexReuseFlag = true)
		{
			byte[] density = new byte[8];
			for (int i = 0; i < density.Length; i++)
			{
				density[i] = data.getPoint(pos + VoxelConstants.cornerIndex[i] * lod);
			}

			rind = new int[4]{-1,-1,-1,-1};

			byte caseCode = getCaseCode(density);

			//Debug.Assert(((caseCode ^ ((density[7] >> 7) & 0xFF)) == 0) == (caseCode == 0x00 || caseCode == 0xFF), "Bit twidling trick failed");

			if ((caseCode ^ ((density[7] >> 7) & 0xFF)) == 0) //for this cases there is no triangulation
				return;

			byte regularCellClass = VoxelConstants.regularCellClass[caseCode];
			ushort[] vertexLocations = VoxelConstants.regularVertexData[caseCode];

			VoxelConstants.RegularCellData c = VoxelConstants.regularCellData[regularCellClass];
			long vertexCount = c.GetVertexCount();
			long triangleCount = c.GetTriangleCount();
			byte[] indexOffset = c.getIndizes(); //index offsets for current cell
			int[] mappedIndizes = new int[indexOffset.Length]; //array with real indizes for current cell

			for (int i = 0; i < vertexCount; i++)
			{
				byte edge = (byte)(vertexLocations[i] >> 8);
				byte reuseIndex = (byte)(edge & 0xF); //Vertex id which should be created or reused 1,2 or 3

				byte v1 = (byte)((vertexLocations[i]) & 0x0F); //Second Corner Index
				byte v0 = (byte)((vertexLocations[i] >> 4) & 0x0F); //First Corner Index

				byte d0 = density[v0];
				byte d1 = density[v1];

				long t = (d1 << 8) / (d1 - d0);
                
                if (t != 0 && t != 256 && t != -1 && t != -2 && t != 257 && t != 258)
                    Console.WriteLine(t);
				
				byte rDir = (byte)(edge >> 4); //the direction to go to reach a previous cell for reusing 

				bool allowReuseOnCorners = false;

				if (!allowReuseOnCorners || (t & 0x00FF) != 0)
				{
					if (v1 == 7)
					{
						Debug.Assert(v1 > v0, "Wrong corner order");

						Vector3i iP0 = (pos + VoxelConstants.cornerIndex[v0] * lod) * voxelStep;
						Vector3f P0 = new Vector3f(iP0.getX(), iP0.getY(), iP0.getZ());
						Vector3i iP1 = (pos + VoxelConstants.cornerIndex[v1] * lod) * voxelStep;
						Vector3f P1 = new Vector3f(iP1.getX(), iP1.getY(), iP1.getZ());

						//EliminateLodPositionShift(lod, ref d0, ref d1, ref t, ref iP0, ref P0, ref iP1, ref P1);

						Vector3f Q = InterpolateVoxelVector(t, P0, P1);
                        /*float fd0 = (float)d0;
                        float fd1 = (float)d1;

                        float inter = fd1/(fd1-fd0);

                        iP0 = VoxelConstants.cornerIndex[v0];
                        iP1 = VoxelConstants.cornerIndex[v1];
                        P0 = new Vector3(iP0.getX(),iP0.getY(),iP0.getZ());
                        P1 = new Vector3(iP1.getX(), iP1.getY(), iP1.getZ());
                        Vector3 Q = P0 * inter + P1 * (1 - inter);
                        Q += new Vector3(pos.getX(),pos.getY(),pos.getZ());*/


						//Console.WriteLine(P0 + " " + P1 + " " + Q);

						vert.Add(Q);
						mapIndizes2Vertice(i, vert.Count-1, mappedIndizes, indexOffset);
						rind[reuseIndex] = vert.Count - 1; //für reuse registrieren
					}
					else
					{
						Cell ccc = getReuseCell(cells, lod, rDir,pos);
						Debug.Assert(ccc != null, "Cell for reuse is null");
						int reI = ccc.rind[reuseIndex];
						mapIndizes2Vertice(i, reI, mappedIndizes, indexOffset);
					}
				}
				/* corner reuse is buggy */
				else if (t == 0)
				{
					if (v1 == 7)
					{
						Vector3i v = (pos + VoxelConstants.cornerIndex[v1] * lod) * voxelStep;
						Vector3f Q = new Vector3f(v.getX(), v.getY(), v.getZ());
						vert.Add(Q);
						mapIndizes2Vertice(i, vert.Count - 1, mappedIndizes, indexOffset);
						rind[0] = vert.Count - 1;
					}
					else
					{
						rDir = v1;
						rDir ^= (byte)7;
						Cell ccc = getReuseCell(cells, lod, rDir, pos);
						Debug.Assert(ccc != null, "Cell for reuse is null");
						int reI = ccc.rind[0];
						mapIndizes2Vertice(i, reI, mappedIndizes, indexOffset);
					}
				}
				else
				{
					rDir = v0;
					rDir ^= (byte)7;
					Cell ccc = getReuseCell(cells, lod, rDir, pos);
					Debug.Assert(ccc != null, "Cell for reuse is null");
					int reI = ccc.rind[0];

					if (reI == -1)
					{
						//The vertex doesn't exist, create it
						Vector3i v = (pos + VoxelConstants.cornerIndex[v0] * lod) * voxelStep;
						Vector3f Q = new Vector3f(v.getX(), v.getY(), v.getZ());
						vert.Add(Q);
						mapIndizes2Vertice(i, vert.Count - 1, mappedIndizes, indexOffset);
						rind[0] = vert.Count - 1;
					}
					else
					{
						mapIndizes2Vertice(i, reI, mappedIndizes, indexOffset);
					}
				}
			}

			for (int i = 0; i < triangleCount; i++)
			{
				int i1 = mappedIndizes[i * 3 + 0];
				int i2 = mappedIndizes[i * 3 + 1];
				int i3 = mappedIndizes[i * 3 + 2];

				ind.Add(i1);
				ind.Add(i2);
				ind.Add(i3);
			}
		}

		private void EliminateLodPositionShift(int lod, ref byte d0, ref byte d1, ref long t, ref Vector3i iP0, ref Vector3f P0, ref Vector3i iP1, ref Vector3f P1)
		{
			for (int k = 0; k < lod - 1; k++)
			{
				Vector3f vm = (P0 + P1) / 2.0f;
				Vector3i pm = (iP0 + iP1) / 2;
				byte sm = data.getPoint(pm);

				if (Math.Sign(d0) != Math.Sign(sm))
				{
					P1 = vm;
					iP1 = pm;
					d1 = sm;
				}
				else
				{
					P0 = vm;
					iP0 = pm;
					d0 = sm;
				}
			}

			t = (d1 << 8) / (d1 - d0); // recalc
		}

		private static Cell getReuseCell(Cell[, ,] cells, int lod, byte rDir,Vector3i pos)
		{
			int rx = rDir & 0x01;
			int rz = (rDir >> 1) & 0x01;
			int ry = (rDir >> 2) & 0x01;

			int dx = pos.getX() / lod - rx;
			int dy = pos.getY() / lod - ry;
			int dz = pos.getZ() / lod - rz;

			Cell ccc = cells[dx, dy, dz];
			return ccc;
		}

		private static Vector3f InterpolateVoxelVector(long t, Vector3f P0, Vector3f P1)
		{
			long u = 0x0100 - t; //256 - t

            //Console.WriteLine(t + " " + u);

			float s = 1.0f / 256.0f;



		    // Console.WriteLine(t);
			
			Vector3f Q = P0 * t + P1 * u; //Density Interpolation
		   
			Q *= s; // shift to shader ! 
			return Q;
		}

		private void mapIndizes2Vertice(int vertexNr, int index, int[] mappedIndizes,byte []indexOffset)
		{
			for (int j = 0; j < mappedIndizes.Length; j++)
			{
				if (vertexNr == indexOffset[j])
				{
					mappedIndizes[j] = index;
				}
			}
		}

		public static byte getCaseCode(byte[] density)
		{
			byte code = 0;
			byte konj = 0x01;
			for (int i = 0; i < density.Length; i++)
			{
				code |= (byte)((density[i] >> (density.Length - 1 - i)) & konj);
				konj <<= 1;
			}

			return code;
		}
	}
}
