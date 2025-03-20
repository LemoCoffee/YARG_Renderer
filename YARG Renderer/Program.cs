using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using YARG_Renderer.Geometry;
using YARG_Renderer.Geometry.Shapes;
using Plane = YARG_Renderer.Geometry.Shapes.Plane;
using System.IO;

namespace YARG_Renderer
{
    static class Program
    {
        static World world;
        static Camera camera;
        static Form1 window;
        static System.Windows.Forms.Timer RenderClock;
        static WindowRenderer renderer = null;
        static bool lookMode = false;

        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            window = new Form1();

            PopulateGeometry();
            InitializeCamera();
            InitializeKeyInput();

            renderer = new WindowRenderer(ref window, ref world).SetCamera(camera);

            RenderClock = new System.Windows.Forms.Timer();
            RenderClock.Interval = 1; // Set the interval to 1 second (1000 ms)
            RenderClock.Tick += (sender, e) => renderer.Update();
            RenderClock.Start();

            System.Diagnostics.Debug.WriteLine("Starting...");
            Application.Run(window);
          
        }

        #region Initialization
        static void InitializeCamera()
        {
            camera = new Camera(new Vector3(10, 0, 0), LUtils.ToQuaternion(new Vector3(0, 0 * (float)Math.PI / 4, 0)), (float)Math.PI / 2, (float)Math.PI / 2);
        }

        static void InitializeKeyInput()
        {
            window.KeyDown += new KeyEventHandler(OnKeyDown);
            window.KeyUp += new KeyEventHandler(OnKeyUp);
        }
        #endregion

        static void PopulateGeometry()
        {
            Material blue = new Material().SetColor(Color.AliceBlue);
            Material deep = new Material().SetColor(Color.Blue);
            Material lime = new Material().SetColor(Color.Lime);
            Material gold = new Material().SetColor(Color.Gold);
            Material darn = new Material().SetColor(Color.FromArgb(255, 31, 30, 51));
            world = new World();
            
            String suzanne = Path.Combine(Directory.GetCurrentDirectory(), "suzanne.obj");
            world.geometry.Add(new Mesh(new Vector3(0, 0, 0), Quaternion.Identity, Vector3.One, suzanne).SetMaterial(lime));
        }

        static void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Q:
                    lookMode = false;
                    break;
            }
        }

        static void OnKeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Tab: // Toggle 2D View
                    renderer.renderMap = !renderer.renderMap;
                    break;
                case Keys.Oemtilde: // Change Shader
                    var shaders = Enum.GetValues(typeof(Camera.ShaderMode));
                    int newIndex = (Array.IndexOf(shaders, camera.CurrentShader) + 1) % shaders.Length;
                    camera.CurrentShader = (Camera.ShaderMode)shaders.GetValue(newIndex);
                    break;
                case Keys.Q: // Enable look mode
                    lookMode = true;
                    break;
                case Keys.Space: // Change 2D view axis
                    if (renderer.renderMap)
                    {
                        renderer.mapMode += 1;
                    }
                    break;
            }

            if (lookMode)
            {
                RotateCamera(e.KeyCode);
            }
            else
            {
                MoveCamera(e.KeyCode);
            }

            renderer.SetCamera(camera);
        }

        static void RotateCamera(Keys key)
        {
            float rotationStep = (float)Math.PI / 32;
            switch (key)
            {
                case Keys.Left:
                    camera.Rotation = Quaternion.Multiply(camera.Rotation, Quaternion.CreateFromAxisAngle(Vector3.UnitY, rotationStep));
                    break;
                case Keys.Right:
                    camera.Rotation = Quaternion.Multiply(camera.Rotation, Quaternion.CreateFromAxisAngle(Vector3.UnitY, -rotationStep));
                    break;
                case Keys.Up:
                    camera.Rotation = Quaternion.Normalize(Quaternion.Multiply(camera.Rotation, Quaternion.CreateFromAxisAngle(Vector3.UnitX, -rotationStep)));
                    break;
                case Keys.Down:
                    camera.Rotation = Quaternion.Normalize(Quaternion.Multiply(camera.Rotation, Quaternion.CreateFromAxisAngle(Vector3.UnitX, rotationStep)));
                    break;
            }
        }

        static void MoveCamera(Keys key)
        {
            Vector3 movement;
            switch (key)
            {
                case Keys.Space:
                    movement = Vector3.UnitY;
                    break;
                case Keys.ShiftKey:
                    movement = -Vector3.UnitY;
                    break;
                case Keys.Left:
                    movement = Vector3.UnitX;
                    break;
                case Keys.Right:
                    movement = -Vector3.UnitX;
                    break;
                case Keys.Up:
                    movement = Vector3.UnitZ;
                    break;
                case Keys.Down:
                    movement = -Vector3.UnitZ;
                    break;
                default:
                    movement = Vector3.Zero;
                    break;
            }

            camera.Position += Vector3.Transform(movement, camera.Rotation);
        }
    }
}
