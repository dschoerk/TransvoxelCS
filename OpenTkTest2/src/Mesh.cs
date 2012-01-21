using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OpenTkTest2
{
    public class Mesh
    {
        VBO ind;
        VAO vao;

        int length;

        public Mesh()
        {
            
        }

        public void Create(Vector3[] positionVboData,Vector3[] normalVboData, int[] indicesVboData)
        {
            length = indicesVboData.Length;
            VBO pos, nor;

            pos = new VBO();
            pos.Create<Vector3>(BufferTarget.ArrayBuffer, positionVboData);

            nor = new VBO();
            nor.Create<Vector3>(BufferTarget.ArrayBuffer, normalVboData);

            ind = new VBO();
            ind.Create<int>(BufferTarget.ElementArrayBuffer, indicesVboData);

            vao = new VAO();
            vao.Create();
            vao.SetVertexAttribPointer(0, 3, pos);
            vao.SetVertexAttribPointer(1, 3, nor);
            VAO.Unbind();
        }

        public void setBuffer()
        { 
            
        }
     
        public void Draw()
        {
            vao.Bind();
            ind.Bind();
            
            GL.DrawElements(BeginMode.Triangles, length,
                    DrawElementsType.UnsignedInt, IntPtr.Zero);

            VAO.Unbind();
            ind.Unbind();
        }

        public static List<Vector3> calcNormals(Vector3 [] vertices,List<int> indizes)
        {
            List<Vector3> normals = new List<Vector3>(new Vector3[vertices.Length]);

            for (int i = 0; i < indizes.Count; i += 3)
            {
                Vector3 v0 = vertices[indizes[i]];
                Vector3 v1 = vertices[indizes[i + 1]];
                Vector3 v2 = vertices[indizes[i + 2]];

                Vector3 normal = Vector3.Cross(v2 - v0, v1 - v0);

                normals[indizes[i]] += normal;
                normals[indizes[i + 1]] += normal;
                normals[indizes[i + 2]] += normal;
            }

            return normals;
        }
    }
}
