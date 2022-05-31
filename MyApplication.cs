using OpenTK.Mathematics;
namespace RayTracer
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        public Raytracer raytracer;

        // Variables for camera movement and rotation
        Vector3 moveTo = (0, 0, 0);

        int xRotation = 0;
        int yRotation = 0;
        int zRotation = 0;

        Vector3 copyMoveTo;

        int copyXRotation;
        int copyYRotation;
        int copyZRotation;


        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;       
        }

        int fov = -1;

        bool drawDebugRays = false;
        bool drawShadowRays = false;
        bool drawSecondaryRays = false;

        // initialize
        public void Init()
        {
            setCopys();
            // Read fov from console
            while (fov == -1)
            {
                Console.WriteLine("Choose FOV in degrees, 90 works best for current scene");
                int f;
                if (int.TryParse(Console.ReadLine(), out f))
                {
                    if (f > 0) fov = f;
                }
            }
            // Create the raytracer and render first scene
            Console.WriteLine("Set the debug rays: typing anything else then 'true' means not drawing those debug rays");
            Console.WriteLine("Note that for shadow rays or secondary rays to show, debug rays must be enabled");
            Console.WriteLine("Debug Rays (green color):");
            bool.TryParse(Console.ReadLine(), out drawDebugRays);
            Console.WriteLine("Shadow Rays: (blue color)");
            bool.TryParse(Console.ReadLine(), out drawShadowRays);
            Console.WriteLine("Secondary Rays: (green color)");
            bool.TryParse(Console.ReadLine(), out drawSecondaryRays);
            this.raytracer = new Raytracer(screen, fov, drawDebugRays, drawShadowRays, drawSecondaryRays);
            raytracer.StartRender();
            // Instructions for the camera
            Console.WriteLine("When pressing certain keys in the console, the camera's position or rotation will change. Note: You have to have clicked on the console so you have focus before it works");
            Console.WriteLine("You can now move the camera with WASD to move in the horizontal and vertical directions");
            Console.WriteLine("You can use Z and X to go up and down respectively");
            Console.WriteLine("You can press N or M to turn left or right");
            Console.WriteLine("You can press K or K to turn upwards or downwards");
            Console.WriteLine("You can press O or P to tilt the camera left or right");
        }
        // tick: renders one frame

        
        public void Tick()
        {
            // Every frame we check if a key is held, if so we move the camera to the new position
            // The held key is switched to know which variable to update.
            while(Console.KeyAvailable)
            {
                ConsoleKeyInfo cl = Console.ReadKey(true);
                switch (cl.KeyChar)
                {
                    case 'w':
                        moveTo += raytracer.cameraLookAt() * 4;
                        break;
                    case 'a':
                        moveTo -= raytracer.cameraRight() * 4;                     
                        break;
                    case 'd':
                        moveTo += raytracer.cameraRight() * 4;
                        break;
                    case 's':
                        moveTo -= raytracer.cameraLookAt() * 4;
                        break;
                    case 'z':
                        moveTo += raytracer.cameraUp() * 4;
                        break;
                    case 'x':
                        moveTo -= raytracer.cameraUp() * 4;
                        break;
                    case 'n':
                        yRotation += 4;
                        break;
                    case 'm':
                        yRotation -= 4;
                        break;
                    case 'k':
                        xRotation += 4;
                        break;
                    case 'l':
                        xRotation -= 4;
                        break;
                    case 'o':
                        zRotation += 4;
                        break;
                    case 'p':
                        zRotation -= 4;
                        break;
                }                         
            }
            raytracer.MakeCanRender();
            if (raytracer.CanRender())
            {
                if (moveTo != copyMoveTo || xRotation != copyXRotation || yRotation != copyYRotation || zRotation != copyZRotation)
                {
                    raytracer.ClearScreen();
                    raytracer.MoveTo(moveTo);
                    raytracer.Rotate((xRotation, yRotation, zRotation));
                    raytracer.StartRender();

                    setCopys();

                }             
            }          
        }

        void setCopys()
        {
            copyMoveTo = moveTo;
            copyXRotation = xRotation;
            copyYRotation = yRotation;
            copyZRotation = zRotation;
        }
    }
}
