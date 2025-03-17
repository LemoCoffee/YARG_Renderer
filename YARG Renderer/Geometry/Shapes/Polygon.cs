using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace YARG_Renderer.Geometry.Shapes
{
    class Polygon : Shape
    {
        public Vector3[] Vertices { get; set; }

        public Polygon(Vector3[] vertices, Vector3 scale) : this(vertices, Vector3.Zero, scale) { }

        public Polygon(Vector3[] vertices, Vector3 origin, Vector3 scale) : base(origin, Quaternion.Identity, scale) 
        {
            Vertices = vertices;
        }

        public override bool Intersect(Ray ray, out float t, out Vector3 normal)
        {
            throw new NotImplementedException();
        }

        public Vector3[] LocalVertices()
        {
            Vector3[] output = new Vector3[Vertices.Length];

            for (int i = 0; i < Vertices.Length; i++)
            {
                output[i] = Vertices[i] - (Vertices[0] + Position);
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

        protected bool Inside(Vector3 p, Vector3 v1, Vector3 v2)
        {
            throw new NotImplementedException();
        }
    }
}
