using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using YARG_Renderer.Geometry;
using System.Numerics;

namespace YARG_Renderer
{
    class WindowRenderer
    {
        Form1 Window;
        Camera Camera;
        World World;
        public Bitmap buffer;
        public delegate void RenderFrame();
        public bool renderMap = false;
        private int _mapMode = 0;
        public int mapMode { get => _mapMode; set => _mapMode = value % 2; }

        public WindowRenderer (ref Form1 _window, ref World _world)
        {
            Init(ref _window, ref _world);
        }

        protected void Init (ref Form1 _window, ref World _world)
        {
            this.Window = _window;
            this.World = _world;
            this.buffer = new Bitmap(Window.ClientSize.Width, Window.ClientSize.Height);
        }

        public WindowRenderer SetCamera(Camera _camera)
        {
            this.Camera = _camera;
            return this;
        }

        public void Update ()
        {
            if (Window.IsHandleCreated)
            {
                var RenderDelegate = new RenderFrame(RenderFrameMethod);
                Window.Invoke(RenderDelegate);
                //Window.Invalidate();
            }
        }

        public void RenderFrameMethod ()
        {
            if (!Window.IsDisposed)
            {
                //((Geometry.Shapes.Tri)World.geometry[3]).Vertices[1].Position += Vector3.UnitX * 0.1f;
                using (Graphics g = Graphics.FromImage(buffer))
                {

                    if (renderMap)
                    {
                        g.Clear(Color.White);
                        switch (mapMode)
                        {
                            case (0):
                                Window.Render2D(Camera, World.geometry, g, 'X', 'Z');
                                break;
                            case (1):
                                Window.Render2D(Camera, World.geometry, g, 'X', 'Y');
                                break;
                        }
                        
                    }
                    else
                    {
                        g.Clear(Color.Black);
                        
                        Window.Render(Camera, World.geometry, g);
                    }
                }

                Window.CreateGraphics().DrawImage(buffer, 0, 0);
            }
        }

        public Bitmap GetBuffer ()
        {
            return buffer;
        }
    }
}
