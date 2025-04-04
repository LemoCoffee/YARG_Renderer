﻿using System.Numerics;

namespace YARG_Renderer.Geometry
{
    public class Ray
    {
        public Vector3 Origin { get; set; }
        public Vector3 Direction { get; set; }
        public static double EPSILON { get => 0.000001; }

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Vector3 PointAt(float t)
        {
            return Origin + t * Direction;
        }

        public bool Intersect(Shapes.Shape shape, out float t, out Vector3 normal)
        {
            return shape.Intersect(this, out t, out normal);
        }
    }
}
