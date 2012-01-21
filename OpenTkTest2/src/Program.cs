// This code was written for the OpenTK library and has been released
// to the Public Domain.
// It is provided "as is" without express or implied warranty of any kind.

using System;
using System.Diagnostics;
using System.IO;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTkTest2;
using VoxelStuff;
using System.Collections.Generic;


namespace Examples.Tutorial
{
   // [Example("OpenGL 3.0", ExampleCategory.OpenGL, "3.x", Documentation = "HelloGL3")]
    public class HelloGL3 : GameWindow
    {
        Shader shader;
        QuatCamera currentCamera;
        Mesh[] meshes;

        VoxelMap scalar = new VoxelMap();

        public HelloGL3()
            : base(640, 480,
            new GraphicsMode(), "OpenGL 3 Example", 0,
            DisplayDevice.Default, 4, 2,
            GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        {
            TextWriterTraceListener writer = new TextWriterTraceListener(System.Console.Out);
            Debug.Listeners.Add(writer);

            Console.WriteLine(GL.GetString(StringName.Version));
        }

        protected override void OnLoad(System.EventArgs e)
        {
            sbyte a = -100;
            Console.WriteLine(a);
            Console.WriteLine((byte)a);

            VSync = VSyncMode.On;

            float aspectRatio = ClientSize.Width / (float)(ClientSize.Height);
            currentCamera = new QuatCamera(aspectRatio);

            shader = new Shader();
            shader.CreateShader();
            Matrix4 proj = currentCamera.getProjectionMatrix();
            shader.setUniformMatrix4("projection_matrix", ref proj);
            Matrix4 view = currentCamera.getViewMatrix();
            shader.setUniformMatrix4("view_matrix", ref view);

            meshes = new Mesh[1];
            for (int i = 0; i < meshes.Length; i++)
            {
                //meshes[i] = MeshBuilder.create(scalar, (i*5)+1,true);
                meshes[i] = new Mesh();
                List<Vector3f> vert;
                List<int> ind;
                SurfaceExtractor.extract(scalar,out vert,out ind, (i * 5) + 1, false);

                Vector3[] vertA = V2V(vert.ToArray());
                Vector3[] normA = Mesh.calcNormals(vertA,ind).ToArray();
                meshes[i].Create(vertA,normA,ind.ToArray());
            }


            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            // Other state
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(System.Drawing.Color.MidnightBlue);

            shader.Use();
        }

        bool wireframe = true;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Matrix4 mview = currentCamera.getViewMatrix();
            shader.setUniformMatrix4("view_matrix", ref mview);

            // m1.Translate(Matrix4.CreateRotationZ(0.05f));
            // m1.Translate(Matrix4.CreateTranslation(0, 0.01f, 0));

            if (Keyboard[OpenTK.Input.Key.Escape])
            {
                Exit();
            }

            if (Keyboard[OpenTK.Input.Key.W])
                currentCamera.Translate(new Vector3(0, 0, -1f));

            if (Keyboard[OpenTK.Input.Key.A])
                currentCamera.Translate(new Vector3(-1, 0, 0));

            if (Keyboard[OpenTK.Input.Key.S])
                currentCamera.Translate(new Vector3(0, 0, 1));

            if (Keyboard[OpenTK.Input.Key.D])
                currentCamera.Translate(new Vector3(1, 0, 0));

            if (Keyboard[OpenTK.Input.Key.P])
            {
                wireframe = !wireframe;
                if (!wireframe)
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                else
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }

            if (Keyboard[OpenTK.Input.Key.Left])
                currentCamera.RotateAxis(Vector3.UnitY, 0.01f);

            if (Keyboard[OpenTK.Input.Key.Right])
                currentCamera.RotateAxis(Vector3.UnitY, -0.01f);

            if (Keyboard[OpenTK.Input.Key.Up])
                currentCamera.RotateAxis(Vector3.UnitX, 0.01f);

            if (Keyboard[OpenTK.Input.Key.Down])
                currentCamera.RotateAxis(Vector3.UnitX, -0.01f);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Use();

            Matrix4 world = Matrix4.Identity;
            for (int i = 0; i < meshes.Length; i++)
            {
                shader.setUniformMatrix4("world_matrix", ref world);
                meshes[i].Draw();
                world = Matrix4.CreateTranslation(300 * (i + 1), 0, 0);
            }

            shader.UnUse();

            SwapBuffers();
        }

        [STAThread]
        public static void Main()
        {
            using (HelloGL3 example = new HelloGL3())
            {
                example.Run(60);
            }
        }

        public static Vector3f V2V(Vector3 v)
        {
            return new Vector3f(v.X,v.Y,v.Z);
        }

        public static Vector3 V2V(Vector3f v)
        {
            return new Vector3(v.getX(), v.getY(), v.getZ());
        }

        public static Vector3f[] V2V(Vector3 [] v)
        {
            Vector3f [] arr = new Vector3f[v.Length];

            for(int i=0;i<arr.Length;i++)
            {
                arr[i] = V2V(v[i]);
            }

            return arr;
        }

        public static Vector3[] V2V(Vector3f [] v)
        {
            Vector3 [] arr = new Vector3[v.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = V2V(v[i]);
            }

            return arr;
        }
    }
}