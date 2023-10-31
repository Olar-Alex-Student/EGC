using System;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

public class TriunghiWindow : GameWindow
{
    private Color currentColor = Color.White;
    private float[] vertices;
    private float rotationAngle = 0;
    private Vector2 cameraPosition = Vector2.Zero;

    public TriunghiWindow(int width, int height) : base(width, height, GraphicsMode.Default, "Triunghi OpenTK") { }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

        // Coordonatele triunghiului
        vertices = new float[]
        {
            -0.5f, -0.5f,
            0.5f, -0.5f,
            0.0f, 0.5f
        };
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Key.R))
        {
            // Generați o culoare aleatorie
            currentColor = GetRandomColor();
        }

        // Modificați unghiul camerei cu ajutorul mouse-ului
        var mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed)
        {
            rotationAngle += 0.01f * mouse.X;
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
        GL.Translate(-cameraPosition.X, -cameraPosition.Y, 0);
        GL.Rotate(rotationAngle, 0, 0, 1);

        GL.Begin(PrimitiveType.Triangles);
        GL.Color3(currentColor);

        for (int i = 0; i < vertices.Length; i += 2)
        {
            GL.Vertex2(vertices[i], vertices[i + 1]);
        }

        GL.End();

        Context.SwapBuffers();
    }

    private Color GetRandomColor()
    {
        Random rand = new Random();
        int r = rand.Next(0, 256);
        int g = rand.Next(0, 256);
        int b = rand.Next(0, 256);

        return Color.FromArgb(r, g, b);
    }

    static void Main()
    {
        using (var window = new TriunghiWindow(800, 600))
        {
            window.Run(60.0);
        }
    }
}
