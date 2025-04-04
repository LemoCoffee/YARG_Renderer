using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using YARG_Renderer.Geometry.Shapes;

namespace YARG_Renderer.Geometry
{
    public class Camera
    {
        public float HorizontalFOV { get; set; }
        public float VerticalFOV { get; set; }
        public Vector2 _resolution;
        public Vector2 Resolution { get => _resolution; set { _resolution = value; RegenerateRays(); } }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        private int _renderTime = DateTime.MinValue.Millisecond;
        private int _tris = 969;
        private int _rayCount = 0;
        private int _rayTriTests = 0;
        private int _rayTriIntersects = 0;

        public enum ShaderMode
        {
            Flat,
            Magnitude,
            YMagnitude,
            Distance,
            DistanceYMag,
        }

        public ShaderMode CurrentShader = ShaderMode.YMagnitude;

        public Ray[] Rays;

        public Camera(Vector3 position, Quaternion rotation, float horizontalFOV, float verticalFOV)
        {
            this.Position = position;
            //this.Rotation = LUtils.ToQuaternion(LUtils.ToEulerAngles(rotation) + new Vector3(0, (float)(-Math.PI/4), 0));
            this.Rotation = LUtils.ToQuaternion(new Vector3(0, (float)(-Math.PI / 4), 0));

            this.HorizontalFOV = horizontalFOV;
            this.VerticalFOV = verticalFOV;
            //this.Resolution = new Vector2(1920/4, 1080/4);
            this.Resolution = new Vector2(250, 250);
            //this.Resolution = new Vector2(50, 50);
        }

        // Repopulate the rays array with new rays
        public void RegenerateRays()
        {

            double dYaw = HorizontalFOV / Resolution.X;
            double dPitch = VerticalFOV / Resolution.Y;

            double hResX = Resolution.X / 2;
            double hResY = Resolution.Y / 2;

            Rays = new Ray[(int)Resolution.X * (int)Resolution.Y];

            for (int h = 0; h < Resolution.Y; h++)
            {

                float depth = (float)Math.Cos((h - hResY) * dPitch);
                float y = (float)Math.Sin((h - hResY) * dPitch);

                // FIXME: Center y direction instead of starting at midpoints
                // Current = [y, y + Resolution.Y]
                // Desired = [y - Resolution.Y / 2, y + Resolution.Y / 2]

                for (int w = 0; w < Resolution.X; w++)
                {
                    double rayAngle = (w - hResX) * dYaw;
                    double sliceDepth = depth / Math.Cos(LUtils.ToEulerAngles(Rotation).Y);

                    float dx = (float)(Math.Sin(rayAngle) * depth);
                    float dz = (float)(Math.Cos(rayAngle) * depth);

                    float x = (float)(Math.Sin(rayAngle) * sliceDepth);
                    float z = (float)(Math.Cos(rayAngle) * sliceDepth);

                    Vector3 direction = new Vector3(dx, y, dz);
                    Vector3 origin = /*new Vector3(x, 0, z) + */Position;

                    Rays[(h * (int)Resolution.X) + w] = new Ray(origin, Vector3.Transform(direction, Rotation));
                }
            }
        }

        public (Shape, float, Vector3)[] CastRays(List<Shape> geometry, bool debug = true)
        {
            if (debug)
            {
                _renderTime = DateTime.Now.Millisecond;
                /*_tris = 0;
                foreach (Shape s in geometry)
                {
                    if (s.GetType() == typeof(Mesh))
                    {
                        _tris += ((Mesh)(s)).Faces.Count;
                    }
                }*/
                _rayCount = 0;
                _rayTriTests = 0;
                _rayTriIntersects = 0;
            }

            RegenerateRays();

            _rayCount = Rays.Length;

            (Shape, float, Vector3)[] contacts = new (Shape, float, Vector3)[(int)Resolution.X * (int)Resolution.Y];

            Parallel.For(0, Rays.Length, i =>
            {
                float minT = float.MaxValue;
                foreach (Shape s in geometry)
                {
                    _rayTriTests++;
                    if (Rays[i].Intersect(s, out float t, out Vector3 normal) && t < minT)
                    {
                        contacts[i] = (s, t, normal);
                        minT = t;
                        _rayTriIntersects++;
                    }
                }
            });

            if (debug)
            {
                _renderTime = DateTime.Now.Millisecond - _renderTime;
            }

            return contacts;
        }

        public String[] RenderStats()
        {
            String[] output = new String[5];

            output[0] = String.Format($"Render time: {_renderTime}ms");
            output[1] = String.Format($"Total Tris: {_tris}");
            output[2] = String.Format($"Total Rays: {_rayCount}");
            output[3] = String.Format($"Ray-Tri Tests: {_rayTriTests}");
            output[4] = String.Format($"Ray-Tri Contacts: {_rayTriIntersects}");

            return output;
        }
    }
}
