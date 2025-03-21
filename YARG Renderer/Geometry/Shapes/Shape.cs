using System.Numerics;

namespace YARG_Renderer.Geometry.Shapes
{
    public abstract class Shape
    {
        public static readonly Material fallbackMaterial = new Material().SetColor(System.Drawing.Color.HotPink);

        public Material Material { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public Shape(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Init(position, rotation, scale);
        }

        protected void Init(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;

            this.Material = fallbackMaterial;
        }

        public Shape SetMaterial(Material _material)
        {
            this.Material = _material;
            return this;
        }

        public abstract bool Intersect(Ray ray, out float t, out Vector3 normal);
    }
}
