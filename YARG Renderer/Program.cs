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
        static bool isRunning = false;
        static World world;
        static Camera camera;
        static Form1 window;
        static System.Windows.Forms.Timer RenderClock;
        static Bitmap[] renderBuffers = new Bitmap[2];
        static int currentBufferIndex = 0;
        static bool renderMap = false;
        static WindowRenderer renderer = null;
        static bool lookMode = false;
        static Vector3 moveDir = Vector3.Zero;

        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            window = new Form1();

            PopulateGeometry();
            InitializeCamera();
            InitializeRenderBuffers();
            InitializeKeyInput();

            renderer = new WindowRenderer(ref window, ref world).SetCamera(camera);

            RenderClock = new System.Windows.Forms.Timer();
            RenderClock.Interval = 1; // Set the interval to 1 second (1000 ms)
            RenderClock.Tick += (sender, e) => renderer.Update();
            RenderClock.Start();

            System.Diagnostics.Debug.WriteLine("Starting...");
            Application.Run(window);

            /*foreach (String i in File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "suzanne.obj")))
            {
                System.Diagnostics.Debug.WriteLine(i);
            }*/
            
        }

        static void InitializeCamera()
        {
            camera = new Camera(new Vector3(10, 0, 0), LUtils.ToQuaternion(new Vector3(0, 0 * (float)Math.PI / 4, 0)), (float)Math.PI / 2, (float)Math.PI / 2);
        }

        static void PopulateGeometry()
        {
            Material blue = new Material().SetColor(Color.AliceBlue);
            Material deep = new Material().SetColor(Color.Blue);
            Material lime = new Material().SetColor(Color.Lime);
            Material gold = new Material().SetColor(Color.Gold);
            Material darn = new Material().SetColor(Color.FromArgb(255, 31, 30, 51));
            world = new World();
            //world.geometry.Add(new Sphere(new Vector3(15, 0, 15), new Quaternion(0, 0, 0, 1), new Vector3(3, 3, 3)).SetMaterial(blue));
            /*world.geometry.Add(new Sphere(new Vector3(5, 10, 15), new Quaternion(0, 0, 0, 1), new Vector3(3, 3, 3)).SetMaterial(lime));
            world.geometry.Add(new Sphere(new Vector3(3, 10, 15), new Quaternion(0, 0, 0, 1), new Vector3(3, 3, 3)).SetMaterial(gold));
            //world.geometry.Add(new Column(new Vector3(5, 10, -15), new Quaternion(0, 0, 0, 1), new Vector3(3, 3, 3)).SetMaterial(gold));
            world.geometry.Add(new Plane(new Vector3(0, -1, 0), -15).SetMaterial(gold));
            world.geometry.Add(new Plane(new Vector3(0, 1, 0), -15).SetMaterial(darn));
            world.geometry.Add(new Plane(new Vector3(0, 0, -1), -20).SetMaterial(blue));
            world.geometry.Add(new Plane(new Vector3(1, 0, 0), -20).SetMaterial(blue));
            world.geometry.Add(new Plane(new Vector3(-1, 0, 0), -40).SetMaterial(blue));
            world.geometry.Add(new Sphere(new Vector3(0, 10, 0), new Quaternion(0, 0, 0, 1), new Vector3(1, 1, 1)));
            world.geometry.Add(new Mesh(Vector3.Zero, Quaternion.Identity, Vector3.One, Path.Combine(Directory.GetCurrentDirectory(), "suzanne.obj")));
            Vertex[] verts = { new Vertex(-1, -1, 0), new Vertex(0, -3, -4), new Vertex(1, -1, 0) };
            Vertex[] verts2 = { verts[0], verts[1], new Vertex(-1, -3, 0) };

            Tri poly = new Tri(verts, new Vector3(1, 1, 1));
            Tri poly2 = new Tri(verts2, new Vector3(1, 1, 1));*/
            //world.geometry.Add(new Sphere(poly.Vertices[0].Position, new Quaternion(0, 0, 0, 1), new Vector3(0.5f, 0.5f, 0.5f)).SetMaterial(lime));
            //world.geometry.Add(new Sphere(poly.Vertices[1].Position, new Quaternion(0, 0, 0, 1), new Vector3(0.5f, 0.5f, 0.5f)).SetMaterial(gold));
            //world.geometry.Add(new Sphere(poly.Vertices[2].Position, new Quaternion(0, 0, 0, 1), new Vector3(0.5f, 0.5f, 0.5f)).SetMaterial(deep));
            //world.geometry.Add(poly);
            //world.geometry.Add(poly2);
            //world.geometry.Add(new Plane(poly.GetNormal(), Vector3.Dot(poly.GetNormal(), verts[0])));
            String suzanne = Path.Combine(Directory.GetCurrentDirectory(), "suzanne.obj");
            world.geometry.Add(new Mesh(new Vector3(0, 0, 0), Quaternion.Identity, Vector3.One, suzanne));
        }

        static void InitializeRenderBuffers()
        {
            renderBuffers[0] = new Bitmap(window.ClientSize.Width, window.ClientSize.Height);
            renderBuffers[1] = new Bitmap(window.ClientSize.Width, window.ClientSize.Height);
        }

        static void InitializeKeyInput()
        {
            window.KeyDown += new KeyEventHandler(OnKeyDown);
            window.KeyUp += new KeyEventHandler(OnKeyUp);
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
                case Keys.Tab:
                    // Handle Tab key press
                    renderer.renderMap = !renderer.renderMap;
                    break;
                case Keys.Oemtilde:
                    var shaders = Enum.GetValues(typeof(Camera.ShaderMode));
                    int newIndex = (Array.IndexOf(shaders, camera.CurrentShader) + 1) % shaders.Length;
                    camera.CurrentShader = (Camera.ShaderMode)shaders.GetValue(newIndex);
                    break;
                case Keys.Q:
                    lookMode = true;
                    break;
                case Keys.Space:
                    // Handle Tab key press
                    if (renderer.renderMap)
                    {
                        renderer.mapMode += 1;
                    } else if (lookMode)
                    {
                        camera.Rotation = Quaternion.Identity;
                    } else
                    {
                        camera.Position -= Vector3.UnitY;
                    }

                    break;
                case Keys.ShiftKey:
                    camera.Position += Vector3.UnitY;
                    break;
                case Keys.Left:
                    // Handle Left Arrow key press
                    if (lookMode)
                    {
                        camera.Rotation = Quaternion.Multiply(camera.Rotation, Quaternion.CreateFromAxisAngle(Vector3.UnitY, -(float)Math.PI / 32));
                    }
                    else
                    {
                        camera.Position += Vector3.UnitZ * (float)Math.Sin(LUtils.ToEulerAngles(camera.Rotation).Y);
                        camera.Position -= Vector3.UnitX * (float)Math.Cos(LUtils.ToEulerAngles(camera.Rotation).Y);
                    }
                    break;
                case Keys.Right:
                    // Handle Right Arrow key press
                    if (lookMode)
                    {
                        camera.Rotation = Quaternion.Multiply(camera.Rotation, Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI / 32));
                    }
                    else
                    {
                        camera.Position -= Vector3.UnitZ * (float)Math.Sin(LUtils.ToEulerAngles(camera.Rotation).Y);
                        camera.Position += Vector3.UnitX * (float)Math.Cos(LUtils.ToEulerAngles(camera.Rotation).Y);
                    }
                    break;
                case Keys.Up:
                    // Handle Up Arrow key press
                    if (lookMode)
                    {
                        camera.Rotation = Quaternion.Normalize(Quaternion.Multiply(camera.Rotation, Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI / 32)));
                    }
                    else
                    {
                        camera.Position += Vector3.UnitZ * (float)Math.Cos(LUtils.ToEulerAngles(camera.Rotation).Y);
                        camera.Position += Vector3.UnitX * (float)Math.Sin(LUtils.ToEulerAngles(camera.Rotation).Y);
                    }
                    break;
                case Keys.Down:
                    // Handle Down Arrow key press
                    if (lookMode)
                    {
                        camera.Rotation = Quaternion.Normalize(Quaternion.Multiply(camera.Rotation, Quaternion.CreateFromAxisAngle(Vector3.UnitX, -(float)Math.PI / 32)));
                    }
                    else
                    {
                        camera.Position -= Vector3.UnitZ * (float)Math.Cos(LUtils.ToEulerAngles(camera.Rotation).Y);
                        camera.Position -= Vector3.UnitX * (float)Math.Sin(LUtils.ToEulerAngles(camera.Rotation).Y);
                    }
                    break;
            }
            renderer.SetCamera(camera);
        }
    }
}
