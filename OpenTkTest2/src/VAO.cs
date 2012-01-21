using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OpenTkTest2
{
    public class VAO
    {
        private int handle;

        public void Create()
        {
            GL.GenVertexArrays(1, out handle);
            GL.BindVertexArray(handle);
        }

        public void SetVertexAttribPointer(int index, int size, VBO buffer)
        { 
            GL.EnableVertexAttribArray(index);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.getHandle());
            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, true, 0, 0);
        }

        public void Bind()
        {
            GL.BindVertexArray(handle);
        }

        public static void Unbind()
        {
            GL.BindVertexArray(0);
        }
    }
}
