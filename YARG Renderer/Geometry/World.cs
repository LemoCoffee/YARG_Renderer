using System.Collections.Generic;
using YARG_Renderer.Geometry.Shapes;

namespace YARG_Renderer.Geometry
{
    public class World
    {
        public List<Shape> geometry = new List<Shape>();

        public Face[] Faces { get => GetFaces(); }

        public Face[] GetFaces()
        {
            List<Face> faces = new List<Face>();

            foreach (Shape shape in geometry)
            {
                if (shape is Mesh)
                {
                    foreach (Face face in ((Mesh)shape).Faces)
                    {
                        faces.Add(face);
                    }
                }
            }

            return faces.ToArray();
        }
    }
}
