using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

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
