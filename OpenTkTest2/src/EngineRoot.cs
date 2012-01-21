using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTkTest2;
using System.Diagnostics;

namespace OpenTkTest2
{
    public class EngineRoot : GameWindow
    {
        private QuatCamera currentCamera;

        public EngineRoot()
            : base(640, 480,
            new GraphicsMode(), "OpenGL Example", 0,
            DisplayDevice.Default, 4, 0,
            GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        {
            //Enable Debugger
            EnableConsoleDebug();

            currentCamera = new QuatCamera(640/480);
        }

        public void EnableConsoleDebug()
        {
            TextWriterTraceListener writer = new TextWriterTraceListener(System.Console.Out);
            Debug.Listeners.Add(writer);
        }
    }
}
