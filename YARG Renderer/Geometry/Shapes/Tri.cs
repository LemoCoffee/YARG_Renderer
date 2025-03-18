﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace YARG_Renderer.Geometry.Shapes
{
    class Tri : Shape
    {
        public Vertex[] Vertices { get; set; }

        public Vector3 MinBound;
        public Vector3 MaxBound;

        public Tri(Vertex[] vertices) : this(vertices, Vector3.One) { }

        public Tri(Vertex[] vertices, Vector3 scale) : this(vertices, Vector3.Zero, scale) { }

        public Tri(Vertex[] vertices, Vector3 origin, Vector3 scale) : base(origin, Quaternion.Identity, scale) 
        {
            Vertices = vertices;
        }

        public override bool Intersect(Ray ray, out float t, out Vector3 normal)
        {
            normal = this.GetNormal();
            t = -1;

            float denom = Vector3.Dot(ray.Direction, normal);

            if (Math.Abs(denom) > ray.EPSILON)
            {
                Vector3 dist = ray.Origin - (GetNormal() * (Vector3.Dot(GetNormal(), Vertices[0].Position)));
                t = -Vector3.Dot(dist, normal) / denom;
            }

            return t >= 0 && Inside(ray.PointAt(t));
        }

        public Vector3[] LocalVertices()
        {
            Vector3[] output = new Vector3[Vertices.Length];

            for (int i = 0; i < Vertices.Length; i++)
            {
                output[i] = Vertices[i].Position - (Vertices[0].Position + Position);
            }

            return output;
        }

        public Vector3 GetNormal()
        {
            Vector3[] localVerts = LocalVertices();
            Vector3 normal = Vector3.Zero;

            for (int i = 0; i < localVerts.Length; i++)
            {
                Vector3 vc = localVerts[i];
                Vector3 vn = localVerts[(i + 1) % localVerts.Length];

                normal.X += (vc.Y - vn.Y) * (vc.Z + vn.Z);
                normal.Y += (vc.Z - vn.Z) * (vc.X + vn.X);
                normal.Z += (vc.X - vn.X) * (vc.Y + vn.Y);
            }

            return Vector3.Normalize(normal);
        }

        protected bool Inside(Vector3 p)
        {
            Vector3 P = p - Position;
            Vector3 N = GetNormal();

            Vector3 v01 = Vertices[1].Position - Vertices[0].Position;
            Vector3 v12 = Vertices[2].Position - Vertices[1].Position;
            Vector3 v02 = Vertices[0].Position - Vertices[2].Position;

            Vector3 v0p = P - Vertices[0].Position;
            Vector3 v1p = P - Vertices[1].Position;
            Vector3 v2p = P - Vertices[2].Position;

            return (
                (Vector3.Dot(N, Vector3.Cross(v01, v0p)) > 0) &&
                (Vector3.Dot(N, Vector3.Cross(v12, v1p)) > 0) &&
                (Vector3.Dot(N, Vector3.Cross(v02, v2p)) > 0)
            );
        }

        private void CalculateRoughBounds()
        {
            MinBound = Vertices[0].Position;
            MaxBound = Vertices[0].Position;

            foreach (Vertex v in Vertices)
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
    }
}
