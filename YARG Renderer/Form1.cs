using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YARG_Renderer.Geometry;
using YARG_Renderer.Geometry.Shapes;

namespace YARG_Renderer
{
    public partial class Form1: Form
    {
        public Form1()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
        }

        public Color GetColor(Camera camera, Shape shape, Ray ray, float time, Vector3 normal)
        {
            Color pixelColor;
            Vector3 normalVec;
            float fogClose = 5;
            float fogFar = 25;
            switch (camera.CurrentShader)
            {
                case Camera.ShaderMode.Flat:
                    return shape.Material.Color;
                case Camera.ShaderMode.YMagnitude:
                    pixelColor = shape.Material.Color;
                    normalVec = normal + Vector3.One;
                    normalVec /= 2;
                    return Color.FromArgb(255, (byte)(pixelColor.R * normalVec.Y), (byte)(pixelColor.G * normalVec.Y), (byte)(pixelColor.B * normalVec.Y));
                case Camera.ShaderMode.Magnitude:
                    normal.X = (normal.X + 1) * 128;
                    normal.Y = (normal.Y + 1) * 128;
                    normal.Z = (normal.Z + 1) * 128;
                    return Color.FromArgb(
                        255,
                        (byte)(normal.X),
                        (byte)(normal.Y),
                        (byte)(normal.Z)
                    );
                case Camera.ShaderMode.Distance:
                    float farDist = 50;
                    time = (time > farDist) ? farDist : time;
                    var mag = (1 - time / farDist);
                    pixelColor = shape.Material.Color;
                    byte R = (byte)(mag * pixelColor.R);
                    byte G = (byte)(mag * pixelColor.G);
                    byte B = (byte)(mag * pixelColor.B);
                    return Color.FromArgb(255, R, G, B);
                    break;
                case Camera.ShaderMode.DistanceYMag:
                    time = (time - fogClose > 0) ? time - fogClose : 0;
                    time = (time > fogFar) ? fogFar : time;
                    var Ymag = 1 - (time / fogFar);

                    pixelColor = shape.Material.Color;
                    normalVec = normal + Vector3.One;
                    normalVec /= 2;

                    return Color.FromArgb(255, (byte)(pixelColor.R * normalVec.Y * Ymag), (byte)(pixelColor.G * normalVec.Y * Ymag), (byte)(pixelColor.B * normalVec.Y * Ymag));
                    break;
            }
            return Color.White;
        }

        public bool Render(Camera camera, List<Shape> geometry, Graphics g)
        {
            (Shape, float, Vector3)[] contacts = camera.CastRays(geometry);

            int pixelWidth = this.Width / (int)camera.Resolution.X;
            int pixelHeight = this.Height / (int)camera.Resolution.Y;

            pixelHeight = pixelWidth;

            Brush bgPen = new SolidBrush(Color.Black);

            for (int i = 0; i < contacts.Length; i++)
            {
                int pX = ((contacts.Length - i) % (int)camera.Resolution.X) * pixelWidth;
                int pY = ((contacts.Length - i) / (int)camera.Resolution.X) * pixelHeight;

                if (contacts[i].Item1 != null && contacts[i].Item2 > 0)
                {
                    Color pixelColor = GetColor(camera, contacts[i].Item1, camera.Rays[i], contacts[i].Item2, contacts[i].Item3);
                    g.FillRectangle(new SolidBrush(pixelColor), pX, pY, pixelWidth, pixelHeight);
                } else
                {
                    g.FillRectangle(bgPen, pX, pY, pixelWidth, pixelHeight);
                }
            }

            return true;
        }

        public bool RenderXZ(Camera camera, List<Shape> geometry, Graphics g)
        {
            g.ScaleTransform(5, 5);
            g.TranslateTransform(50, 50);

            Pen objectPen = new Pen(Color.Black, 1);
            Pen redPen = new Pen(Color.Red, 1);
            Pen bluePen = new Pen(Color.Blue, 1);
            Pen greenPen = new Pen(Color.Green, 1);

            Brush bgPen = new SolidBrush(Color.White);

            g.FillRectangle(bgPen, 0, 0, (int)camera.Resolution.X, (int)camera.Resolution.Y);

            g.DrawLine(redPen, -1000, 0, 1000, 0);
            g.DrawLine(bluePen, 0, -1000, 0, 1000);

            foreach (Shape shape in geometry)
            {
                // Treat all shapes as circles for now because I cannot be bothered
                float x = shape.Position.X;
                float z = shape.Position.Z;
                g.DrawEllipse(objectPen, x - (shape.Scale.X / 2), z - (shape.Scale.Z / 2), shape.Scale.X, shape.Scale.Z);
            }

            Vector3 start = camera.Position;
            //Vector3 end = start + new Vector3((float)Math.Sin(LUtils.ToEulerAngles(camera.Rotation).Y) * 25, 0, (float)Math.Cos(LUtils.ToEulerAngles(camera.Rotation).Y) * 25);
            camera.RegenerateRays();
            Vector3 end = start + (camera.Rays[0].Direction * 10);
            g.DrawLine(redPen, start.X, start.Z, end.X, end.Z);

            end = start + (camera.Rays[249].Direction * 10);
            g.DrawLine(redPen, start.X, start.Z, end.X, end.Z);

            g.DrawEllipse(redPen, start.X - 1, start.Z - 1, 2, 2);

            objectPen.Dispose();
            redPen.Dispose();
            bgPen.Dispose();

            return true;
        }

        public bool RenderXY(Camera camera, List<Shape> geometry, Graphics g)
        {
            g.ScaleTransform(5, 5);
            g.TranslateTransform(50, 50);

            Pen objectPen = new Pen(Color.Black, 1);
            Pen redPen = new Pen(Color.Red, 1);
            Pen bluePen = new Pen(Color.Blue, 1);
            Pen greenPen = new Pen(Color.Green, 1);

            Brush bgPen = new SolidBrush(Color.White);

            g.FillRectangle(bgPen, 0, 0, (int)camera.Resolution.X, (int)camera.Resolution.Y);

            g.DrawLine(redPen, -1000, 0, 1000, 0);
            g.DrawLine(greenPen, 0, -1000, 0, 1000);

            foreach (Shape shape in geometry)
            {
                // Treat all shapes as circles for now because I cannot be bothered
                float x = shape.Position.X;
                float y = shape.Position.Y;
                g.DrawEllipse(objectPen, x - (shape.Scale.X / 2), y - (shape.Scale.Y / 2), shape.Scale.X, shape.Scale.Y);
            }

            Vector3 start = camera.Position;
            camera.RegenerateRays();
            Vector3 end = start + (camera.Rays[0].Direction * 10);
            g.DrawLine(redPen, start.X, start.Y, end.X, end.Y);

            end = start + (camera.Rays[camera.Rays.Length - 1].Direction * 10);
            g.DrawLine(redPen, start.X, start.Y, end.X, end.Z);

            g.DrawEllipse(redPen, start.X - 1, start.Y - 1, 2, 2);

            objectPen.Dispose();
            redPen.Dispose();
            bgPen.Dispose();

            return true;
        }
    }
}

