using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace YARG_Renderer.Geometry.Shapes
{
    class Mesh : Shape
    {
        public List<Vertex> Vertices;
        public List<Tri> Faces;

        public Vector3 MinBound;
        public Vector3 MaxBound;

        public Mesh(Vector3 position, Quaternion rotation, Vector3 scale, String path) : base(position, rotation, scale)
        {
            ParseOBJ(path);
            CalculateRoughBounds();
        }

        public override bool Intersect(Ray ray, out float t, out Vector3 normal)
        {
            t = float.MaxValue;
            normal = Vector3.Zero;
            bool isect = false;

            if (!BoundaryIntersect(ray))
            {
                return false;
            }

            for (int i = 1; i < Faces.Count; i++)
            {
                //System.Diagnostics.Debug.WriteLine("Face: " + i + "/" + Faces.Count);

                Tri face = Faces[i];
                Vector3 v0 = face.Vertices[0].Position;
                Vector3 v1 = face.Vertices[1].Position;
                Vector3 v2 = face.Vertices[2].Position;

                

                if (face.Intersect(ray, out float tTemp, out Vector3 normalTemp) && tTemp < t)
                {
                    t = tTemp;
                    normal = normalTemp;
                    isect = true;
                }
            }

            return isect;
        }

        private bool BoundaryIntersect(Ray ray)
        {
            float tmin, tmax, tymin, tymax, tzmin, tzmax;

            Vector3[] bounds = { MinBound, MaxBound };
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

        private void CalculateRoughBounds ()
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
        
        private void ParseOBJ (String path)
        {
            String[] lines = File.ReadAllLines(path);

            Vertices = new List<Vertex>();
            Vertices.Add(new Vertex(Vector3.Zero));

            Vertex[] trashTriVertices = { Vertices[0], Vertices[0], Vertices[0] };
            Faces = new List<Tri>();
            Faces.Add(new Tri(trashTriVertices));

            foreach (String line in lines)
            {
                String[] terms = line.Split(' ');
                switch (terms[0])
                {
                    case ("v"):  // Vertex

                        Vertices.Add(new Vertex(float.Parse(terms[1]), float.Parse(terms[2]), float.Parse(terms[3])));

                        break;
                    case ("f"):  // Face

                        int[] v1 = Array.ConvertAll(terms[1].Split('/'), s => int.Parse(s));
                        int[] v2 = Array.ConvertAll(terms[2].Split('/'), s => int.Parse(s));
                        int[] v3 = Array.ConvertAll(terms[3].Split('/'), s => int.Parse(s));

                        Vertex[] v = { Vertices[v1[0]], Vertices[v2[0]], Vertices[v3[0]] };
                        Faces.Add(new Tri(v, this.Position, Vector3.One));

                        break;
                    case ("vt"): // Texture coordinate
                        break;
                    case ("vn"): // Vertex normal
                        break;
                    case ("s"):  // ???
                        break;
                }
            }
        }
    }
}
