using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace YARG_Renderer.Geometry
{
    class AABB
    {
        public Vector3 MinimumBound;
        public Vector3 MaximumBound;

        public static AABB FromVertices(Vertex[] vertices) 
        {
            AABB output = new AABB();

            CalculateRoughBounds(vertices, out Vector3 minBound, out Vector3 maxBound);
            output.MinimumBound = minBound;
            output.MaximumBound = maxBound;

            return output; 
        }

        private static void CalculateRoughBounds(Vertex[] vertices, out Vector3 MinBound, out Vector3 MaxBound)
        {
            MinBound = vertices[0].Position;
            MaxBound = vertices[0].Position;

            foreach (Vertex v in vertices)
            {
                if (v.Position.X > MaxBound.X)
                {
                    MaxBound.X = v.Position.X;
                }
                else if (v.Position.X < MinBound.X)
                {
                    MinBound.X = v.Position.X;
                }

                if (v.Position.Y > MaxBound.Y)
                {
                    MaxBound.Y = v.Position.Y;
                }
                else if (v.Position.Y < MinBound.Y)
                {
                    MinBound.Y = v.Position.Y;
                }

                if (v.Position.Z > MaxBound.Z)
                {
                    MaxBound.Z = v.Position.Z;
                }
                else if (v.Position.Z < MinBound.Z)
                {
                    MinBound.Z = v.Position.Z;
                }
            }
        }

        public bool Intersect(Ray ray)
        {
            float tmin, tmax, tymin, tymax, tzmin, tzmax;

            Vector3[] bounds = { MinimumBound, MaximumBound };
            int[] sign = {
                ray.Direction.X < 0 ? 1 : 0,
                ray.Direction.Y < 0 ? 1 : 0,
                ray.Direction.Z < 0 ? 1 : 0
            };

            tmin = (bounds[sign[0]].X - ray.Origin.X) / ray.Direction.X;
            tmax = (bounds[1 - sign[0]].X - ray.Origin.X) / ray.Direction.X;
            tymin = (bounds[sign[1]].Y - ray.Origin.Y) / ray.Direction.Y;
            tymax = (bounds[1 - sign[1]].Y - ray.Origin.Y) / ray.Direction.Y;

            if ((tmin > tymax) || (tymin > tmax))
                return false;

            if (tymin > tmin)
                tmin = tymin;
            if (tymax < tmax)
                tmax = tymax;

            tzmin = (bounds[sign[2]].Z - ray.Origin.Z) / ray.Direction.Z;
            tzmax = (bounds[1 - sign[2]].Z - ray.Origin.Z) / ray.Direction.Z;

            if ((tmin > tzmax) || (tzmin > tmax))
                return false;

            if (tzmin > tmin)
                tmin = tzmin;
            if (tzmax < tmax)
                tmax = tzmax;

            return true;
        }


    }
}
