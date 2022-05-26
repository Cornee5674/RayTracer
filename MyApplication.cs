namespace RayTracer
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        public Raytracer raytracer;

        // Variables for camera movement and rotation
        int x = 0;
        int y = 0;
        int z = 0;

        int xRotation = 0;
        int yRotation = 0;
        int zRotation = 0;

        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;       
        }

        int fov = -1;

        // initialize
        public void Init()
        {           
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
            this.raytracer = new Raytracer(screen, fov);
            raytracer.Render();
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
                        z++;
                        break;
                    case 'a':
                        x--;                     
                        break;
                    case 'd':
                        x++;
                        break;
                    case 's':
                        z--;
                        break;
                    case 'z':
                        y++;
                        break;
                    case 'x':
                        y--;
                        break;
                    case 'n':
                        yRotation++;
                        break;
                    case 'm':
                        yRotation--;
                        break;
                    case 'k':
                        xRotation++;
                        break;
                    case 'l':
                        xRotation--;
                        break;
                    case 'o':
                        zRotation++;
                        break;
                    case 'p':
                        zRotation--;
                        break;
                }                         
            }
            raytracer.ClearScreen();
            raytracer.MoveTo((x, y, z));
            raytracer.Rotate((xRotation, yRotation, zRotation));
            raytracer.Render();
        }
    }
}
