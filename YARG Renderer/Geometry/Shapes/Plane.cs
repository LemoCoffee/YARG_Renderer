using System;
using System.Numerics;

namespace YARG_Renderer.Geometry.Shapes
{
    class Plane : Shape
    {
        public Vector3 Normal { get; set; }
        public float Distance { get; set; }

        public Plane(Vector3 normal, float distance) : base(Vector3.Normalize(normal) * distance, Quaternion.Identity, Vector3.One)
        {
            Normal = Vector3.Normalize(normal);
            Distance = distance;
        }
        public override bool Intersect(Ray ray, out float t, out Vector3 normal)
        {
            normal = this.Normal;
            t = -1;

            float denom = Vector3.Dot(ray.Direction, Normal);

            if (Math.Abs(denom) > ray.EPSILON)
            {
                Vector3 dist = ray.Origin - this.Position;
                t = -Vector3.Dot(dist, normal) / denom;
            }

            return t >= 0;
        }
    }
}
