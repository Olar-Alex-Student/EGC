using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenTK_Lab_2
{
    class Laborator2 : GameWindow
    {
        Vector3 objectPosition = new Vector3(0, 0, -5.0f);
        float moveSpeed = 0.1f;

        public Laborator2() : base(800, 600, new GraphicsMode(32, 24, 0, 8))
        {
            VSync = VSyncMode.On;

            Console.WriteLine("OpenGL version: " + GL.GetString(StringName.Version));
            Title = "OpenGL version: " + GL.GetString(StringName.Version) + " (immediate mode)";

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(Color.Gray);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            double aspect_ratio = Width / (double)Height;

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);

            Matrix4 lookat = Matrix4.LookAt(new Vector3(30, 30, 30), Vector3.Zero, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard[Key.Escape])
            {
                Exit();
            }

            if (keyboard[Key.W])
            {
                objectPosition.Y += moveSpeed;
            }
            if (keyboard[Key.S])
            {
                objectPosition.Y -= moveSpeed;
            }
            if (keyboard[Key.A])
            {
                objectPosition.X -= moveSpeed;
            }
            if (keyboard[Key.D])
            {
                objectPosition.X += moveSpeed;
            }
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Translate(objectPosition);

            GL.Begin(PrimitiveType.Quads);
            GL.Color3(1.0, 1.0, 1.0);
            GL.Vertex2(-0.5, -0.5); // Colțul stânga-jos
            GL.Vertex2(0.5, -0.5);  // Colțul dreapta-jos
            GL.Vertex2(0.5, 0.5);   // Colțul dreapta-sus
            GL.Vertex2(-0.5, 0.5);  // Colțul stânga-sus
            GL.End();

            SwapBuffers();
        }

        [STAThread]
        static void Main(string[] args)
        {
            using (Laborator2 example = new Laborator2())
            {
                example.Run(30.0, 0.0);
            }
        }
    }
}
