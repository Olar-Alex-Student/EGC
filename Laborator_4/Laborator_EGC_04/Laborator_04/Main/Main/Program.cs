using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Main
{

    internal class Window3D : GameWindow
    {

        const float rotation_speed = 180.0f;
        float angle;
        bool showCube = true;
        KeyboardState lastKeyPress;
        private const int XYZ_SIZE = 75;
        private bool axesControl = true;
        private int transStep = 0;
        private int radStep = 0;
        private int attStep = 0;

        private bool newStatus = false;

        private int[,] objVertices = {};
            //= {
            //{5, 10, 5,
            //    10, 5, 10,
            //    5, 10, 5,
            //    10, 5, 10,
            //    5, 5, 5,
            //    5, 5, 5,
            //    5, 10, 5,
            //    10, 10, 5,
            //    10, 10, 10,
            //    10, 10, 10,
            //    5, 10, 5,
            //    10, 10, 5},
            //{5, 5, 12,
            //    5, 12, 12,
            //    5, 5, 5,
            //    5, 5, 5,
            //    5, 12, 5,
            //    12, 5, 12,
            //    12, 12, 12,
            //    12, 12, 12,
            //    5, 12, 5,
            //    12, 5, 12,
            //    5, 5, 12,
            //    5, 12, 12},
            //{6, 6, 6,
            //    6, 6, 6,
            //    6, 6, 12,
            //    6, 12, 12,
            //    6, 6, 12,
            //    6, 12, 12,
            //    6, 6, 12,
            //    6, 12, 12,
            //    6, 6, 12,
            //    6, 12, 12,
            //    12, 12, 12,
            //    12, 12, 12}};
        private Color[] colorVertices = { Color.White, Color.LawnGreen, Color.WhiteSmoke, Color.Tomato, Color.Turquoise, Color.OldLace, Color.Olive, Color.MidnightBlue, Color.PowderBlue, Color.PeachPuff, Color.LavenderBlush, Color.MediumAquamarine };

        private Window3D() : base(800, 600, new GraphicsMode(32, 24, 0, 8)) {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);

            // Citirea coordonatelor cubului din fișier
            string[] lines = File.ReadAllLines("coordonate_cub.txt");

            if (lines.Length != 12)
            {
                throw new Exception("Fișierul trebuie să conțină 12 linii de coordonate.");
            }

            for (int i = 0; i < 12; i++)
            {
                string[] coords = lines[i].Split(',');
                if (coords.Length != 3)
                {
                    throw new Exception("Fiecare linie din fișier trebuie să conțină 3 coordonate (x, y, z).");
                }

                objVertices[i, 0] = Int32.Parse(coords[0]);
                objVertices[i, 1] = Int32.Parse(coords[1]);
                objVertices[i, 2] = Int32.Parse(coords[2]);
            }
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            double aspect_ratio = Width / (double)Height;

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);

            Matrix4 lookat = Matrix4.LookAt(30, 30, 30, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            showCube = true;
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (mouse[MouseButton.Left]) {
                Console.WriteLine("Click non-accelerat (" + mouse.X + "," + mouse.Y + "); accelerat (" + Mouse.X + "," + Mouse.Y + ")");
                IntPtr pix = new IntPtr();
                GL.ReadPixels(Mouse.X, Mouse.Y, 1, 1, PixelFormat.Rgb, PixelType.Int, pix);
                Console.WriteLine("Pixel colour (" + IntPtr.Size + " - 32 or 64 bits process);");
                Console.WriteLine("");
            }


            // Se utilizeaza mecanismul de control input oferit de OpenTK (include perifcerice multiple, inclusiv
            // pentru gaminig - gamepads, joysticks, etc.).
            if (keyboard[Key.Escape]) {
                Exit();
                return;
            }

            if (keyboard[Key.P] && !keyboard.Equals(lastKeyPress)) {
                // Ascundere comandată, prin apăsarea unei taste - cu verificare de remanență! Timpul de reacție
                // uman << calculator.
                if (showCube) {
                    showCube = false;
                } else {
                    showCube = true;
                }
            }
            if (keyboard[Key.R] && !keyboard.Equals(lastKeyPress)) {
                if (newStatus) {
                    newStatus = false;
                } else {
                    newStatus = true;
                }
            }

            if (keyboard[Key.A]) {
                transStep--;
            }
            if (keyboard[Key.D]) {
                transStep++;
            }

            if (keyboard[Key.W]) {
                radStep--;
            }
            if (keyboard[Key.S]) {
                radStep++;
            }

            if (keyboard[Key.Up]) {
                attStep++;
            }
            if (keyboard[Key.Down]) {
                attStep--;
            }

            lastKeyPress = keyboard;
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (newStatus) {
                DrawCube();
            }

            //angle += rotation_speed * (float)e.Time;
            //GL.Rotate(angle, 0.0f, 1.0f, 0.0f);

            if (axesControl) {
                DrawAxes();
            }

            if (showCube == true) {
                GL.PushMatrix();
                GL.Translate(transStep, attStep, radStep);
                //GL.Translate(0, 0, radStep);
                //GL.Translate(0, attStep, 0);
                DrawCube();
                GL.PopMatrix();
            }

            //GL.Flush();


            SwapBuffers();
        }

        private void DrawAxes() {
            // Desenează axa Ox (cu roșu).
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(XYZ_SIZE, 0, 0);
            GL.End();

            // Desenează axa Oy (cu galben).
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Yellow);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, XYZ_SIZE, 0); ;
            GL.End();

            // Desenează axa Oz (cu verde).
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, XYZ_SIZE);
            GL.End();
        }

        private void DrawCube() {
            GL.Begin(PrimitiveType.Triangles);
            for (int i = 0; i < 35; i = i + 3) {
                //For i As Integer = 0 To 35 Step 3
                GL.Color3(colorVertices[i / 3]);
                GL.Vertex3(objVertices[0, i], objVertices[1, i], objVertices[2, i]);
                GL.Vertex3(objVertices[0, i + 1], objVertices[1, i + 1], objVertices[2, i + 1]);
                GL.Vertex3(objVertices[0, i + 2], objVertices[1, i + 2], objVertices[2, i + 2]);
            }
            GL.End();
        }

        [STAThread]
        static void Main(string[] args) {

            using (Window3D example = new Window3D()) {
                example.Run(30.0, 0.0);
            }

        }
    }

}
