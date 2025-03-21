using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using YARG_Renderer.Geometry;
using YARG_Renderer.Geometry.Shapes;

namespace YARG_Renderer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
        }

        /// <summary>
        /// Gets the color of a pixel based on the camera's shader mode.
        /// </summary>
        /// <param name="camera">The camera object.</param>
        /// <param name="shape">The shape being rendered.</param>
        /// <param name="ray">The ray being cast.</param>
        /// <param name="time">The time of intersection.</param>
        /// <param name="normal">The normal vector at the intersection point.</param>
        /// <returns>The color of the pixel.</returns>
        public Color GetColor(Camera camera, Shape shape, Ray ray, float time, Vector3 normal)
        {
            Color pixelColor = shape.Material.Color;
            Vector3 normalVec = (normal + Vector3.One) / 2;
            float fogClose = 5;
            float fogFar = 25;
            float mag, Ymag;

            switch (camera.CurrentShader)
            {
                case Camera.ShaderMode.Flat:
                    return pixelColor;

                case Camera.ShaderMode.YMagnitude:
                    return MultiplyColor(pixelColor, normalVec.Y);

                case Camera.ShaderMode.Magnitude:
                    normal = (normal + Vector3.One) * 128;
                    return Color.FromArgb(255, (byte)normal.X, (byte)normal.Y, (byte)normal.Z);

                case Camera.ShaderMode.Distance:
                    time = Math.Min(time, fogFar);
                    mag = 1 - time / fogFar;
                    return MultiplyColor(pixelColor, mag);

                case Camera.ShaderMode.DistanceYMag:
                    time = Math.Max(0, time - fogClose);
                    time = Math.Min(time, fogFar);
                    Ymag = 1 - time / fogFar;
                    return MultiplyColor(pixelColor, normalVec.Y * Ymag);

                default:
                    return Color.White;
            }
        }

        private Color MultiplyColor(Color pixelColor, float magnitude)
        {
            return Color.FromArgb(255,
                (byte)(pixelColor.R * magnitude),
                (byte)(pixelColor.G * magnitude),
                (byte)(pixelColor.B * magnitude));
        }

        public bool Render(Camera camera, List<Shape> geometry, Graphics g)
        {
            var contacts = camera.CastRays(geometry);
            int pixelWidth = this.Width / (int)camera.Resolution.X;
            int pixelHeight = this.Height / (int)camera.Resolution.Y;
            pixelHeight = pixelWidth;

            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                for (int i = 0; i < contacts.Length; i++)
                {
                    int pX = ((contacts.Length - i) % (int)camera.Resolution.X) * pixelWidth;
                    int pY = ((contacts.Length - i) / (int)camera.Resolution.X) * pixelHeight;

                    if (contacts[i].Item1 != null && contacts[i].Item2 > 0)
                    {
                        brush.Color = GetColor(camera, contacts[i].Item1, camera.Rays[i], contacts[i].Item2, contacts[i].Item3);
                    }
                    else
                    {
                        brush.Color = Color.Black;
                    }

                    g.FillRectangle(brush, pX, pY, pixelWidth, pixelHeight);
                }
            }

            return true;
        }

        public bool Render2D(Camera camera, List<Shape> geometry, Graphics g, char axis1, char axis2)
        {
            g.ScaleTransform(5, 5);
            g.TranslateTransform(50, 50);

            using (Pen objectPen = new Pen(Color.Black, 1))
            using (Pen redPen = new Pen(Color.Red, 1))
            using (Pen axisPen = new Pen(axis2 == 'Z' ? Color.Blue : Color.Green, 1))
            using (Brush bgPen = new SolidBrush(Color.White))
            {
                g.FillRectangle(bgPen, 0, 0, (int)camera.Resolution.X, (int)camera.Resolution.Y);
                g.DrawLine(redPen, -1000, 0, 1000, 0);
                g.DrawLine(axisPen, 0, -1000, 0, 1000);

                g.DrawEllipse(objectPen, camera.Position.X - 2, camera.Position.Y - 2, 4, 4);

                // Draw camera direction line
                Vector3 direction = Vector3.Transform(new Vector3(0, 0, -1), camera.Rotation);
                float dirX = GetAxisValue(direction, axis1) * 10;
                float dirY = GetAxisValue(direction, axis2) * 10;
                g.DrawLine(objectPen, camera.Position.X, camera.Position.Y, camera.Position.X + dirX, camera.Position.Y + dirY);

                foreach (Shape shape in geometry)
                {
                    float x = GetAxisValue(shape.Position, axis1);
                    float y = GetAxisValue(shape.Position, axis2);
                    float scaleX = GetAxisValue(shape.Scale, axis1);
                    float scaleY = GetAxisValue(shape.Scale, axis2);
                    g.DrawEllipse(objectPen, x - (scaleX / 2), y - (scaleY / 2), scaleX, scaleY);
                }
            }
            return true;
        }

        private float GetAxisValue(Vector3 vector, char axis)
        {
            switch (axis)
            {
                case ('X'):
                    return vector.X;
                case ('Y'):
                    return vector.Y;
                case ('Z'):
                    return vector.Z;
            }

            System.Diagnostics.Debug.WriteLine("Invalid axis '" + axis + "'");

            return 0;
        }
    }
}

