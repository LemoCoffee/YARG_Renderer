using System.Drawing;

namespace YARG_Renderer.Geometry
{
    public class Material
    {
        public Color Color { get; set; } // Ew
        public Material() { }

        public Material SetColor(Color color)
        {
            this.Color = color;
            return this;
        }
    }
}
