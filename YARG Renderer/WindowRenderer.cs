using System;
using System.Drawing;
using System.Windows.Forms;
using YARG_Renderer.Geometry;

namespace YARG_Renderer
{
    class WindowRenderer
    {
        Form1 Window;
        Camera Camera;
        World World;
        public Bitmap Buffer;
        public bool DebugStats = true;
        public delegate void RenderFrame();
        public bool renderMap = false;
        private int _mapMode = 0;
        public int mapMode { get => _mapMode; set => _mapMode = value % 2; }

        public WindowRenderer(ref Form1 _window, ref World _world)
        {
            Init(ref _window, ref _world);
        }

        protected void Init(ref Form1 _window, ref World _world)
        {
            this.Window = _window;
            this.World = _world;
            this.Buffer = new Bitmap(Window.ClientSize.Width, Window.ClientSize.Height);
        }

        public WindowRenderer SetCamera(Camera _camera)
        {
            this.Camera = _camera;
            return this;
        }

        public void Update()
        {
            if (Window.IsHandleCreated)
            {
                var RenderDelegate = new RenderFrame(RenderFrameMethod);
                Window.Invoke(RenderDelegate);
                //Window.Invalidate();
            }
        }

        public void RenderFrameMethod()
        {
            if (!Window.IsDisposed)
            {
                using (Graphics g = Graphics.FromImage(Buffer))
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

                Window.CreateGraphics().DrawImage(Buffer, 0, 0);
            }
        }

        public Bitmap GetBuffer()
        {
            return Buffer;
        }
    }
}
