using System;
using System.Collections.Generic;
using System.Numerics;

namespace YARG_Renderer.Geometry
{
    public struct Face
    {
        public Face(List<Vertex> vertices, ushort vertexA, ushort vertexB, ushort vertexC)
        {
            VertexA = vertexA;
            VertexB = vertexB;
            VertexC = vertexC;

            Normal = GetNormal(vertices, vertexA, vertexB, vertexC);
        }

        public Vector3 Normal;
        public ushort VertexA;
        public ushort VertexB;
        public ushort VertexC;

        public static Vector3 GetNormal(List<Vertex> v, Face f)
        {
            return GetNormal(v, f.VertexA, f.VertexB, f.VertexC);
        }

        public static Vector3 GetNormal(List<Vertex> vertices, ushort a, ushort b, ushort c)
        {
            Vector3[] localVerts = new Vector3[] { vertices[a].Position - vertices[a].Position, vertices[b].Position - vertices[a].Position, vertices[c].Position - vertices[a].Position };
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

        public bool Inside(Vector3 p, List<Vertex> vertices)
        {
            Vector3 v01 = vertices[VertexB].Position - vertices[VertexA].Position;
            Vector3 v12 = vertices[VertexC].Position - vertices[VertexB].Position;
            Vector3 v02 = vertices[VertexA].Position - vertices[VertexC].Position;

            Vector3 v0p = p - vertices[VertexA].Position;
            Vector3 v1p = p - vertices[VertexB].Position;
            Vector3 v2p = p - vertices[VertexC].Position;

            return (
                (Vector3.Dot(Normal, Vector3.Cross(v01, v0p)) > 0) &&
                (Vector3.Dot(Normal, Vector3.Cross(v12, v1p)) > 0) &&
                (Vector3.Dot(Normal, Vector3.Cross(v02, v2p)) > 0)
            );
        }

        public bool Intersect(List<Vertex> vertices, Ray ray, out float t)
        {
            t = -1;

            float denom = Vector3.Dot(ray.Direction, Normal);

            if (Math.Abs(denom) > Ray.EPSILON)
            {
                Vector3 dist = ray.Origin - (Normal * (Vector3.Dot(Normal, vertices[VertexA].Position)));
                t = -Vector3.Dot(dist, Normal) / denom;
            }

            return t >= 0 && Inside(ray.PointAt(t), vertices);
        }
    }
}
