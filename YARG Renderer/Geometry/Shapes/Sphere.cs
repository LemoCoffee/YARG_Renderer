using System;
using System.Numerics;

namespace YARG_Renderer.Geometry.Shapes
{
    class Sphere : Shape
    {
        public Sphere(Vector3 position, Quaternion rotation, Vector3 scale) : base(position, rotation, scale)
        {
        }

        public override bool Intersect(Ray ray, out float t, out Vector3 normal)
        {
            Vector3 relativeCenter = Position - ray.Origin;
            Vector3 projectedDistance = Vector3.Dot(relativeCenter, ray.Direction) * ray.Direction;
            Vector3 d = relativeCenter - projectedDistance;
            float distance = d.Length();

            float radius = Scale.X;

            if (distance > radius)
            {
                t = 0;
                normal = Vector3.Zero;
                return false;
            }

            float m = (float)Math.Sqrt(radius * radius - distance * distance);

            t = projectedDistance.Length() - m;
            normal = Vector3.Normalize(ray.PointAt(t) - Position);

            return t > 0;
        }
    }
}
