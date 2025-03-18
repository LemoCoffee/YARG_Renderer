using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace YARG_Renderer.Geometry
{
    public class Vertex
    {
        private Vector3 _position;

        public ref Vector3 Position => ref _position;

        public Vertex(Vector3 position)
        {
            Position = position;
        }

        public Vertex(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }

        public static Vertex operator +(Vertex a, Vertex b)
        {
            return new Vertex(a.Position + b.Position);
        }
        public static Vertex operator -(Vertex a, Vertex b)
        {
            return new Vertex(a.Position - b.Position);
        }
        public static Vertex operator +(Vertex a, Vector3 b)
        {
            return new Vertex(a.Position + b);
        }
        public static Vertex operator -(Vertex a, Vector3 b)
        {
            return new Vertex(a.Position - b);
        }

    }
}
