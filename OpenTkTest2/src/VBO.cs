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
    public class VBO
    {
        private int handle;
        private BufferTarget target;

        public int Create<T>(BufferTarget target, T[] data) where T : struct
        {
            this.target = target;

            GL.GenBuffers(1, out handle);
            GL.BindBuffer(target, handle);

            Debug.Assert(data.Length != 0, "VBO Creation failed because length of data is Zero");

            GL.BufferData(target,
                new IntPtr(data.Length * Marshal.SizeOf(data[0].GetType())),
                data, BufferUsageHint.StaticDraw);

            GL.BindBuffer(target, 0);

            return handle;
        }

        public void Bind()
        {
            GL.BindBuffer(target, handle);
        }

        public void Unbind()
        {
            GL.BindBuffer(target, 0);
        }

        public int getHandle()
        {
            return handle;
        }
    }
}
