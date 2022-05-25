namespace RayTracer
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        public Raytracer raytracer;

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
        // initialize
        public void Init()
        {           
            this.raytracer = new Raytracer(screen);
            raytracer.Render();
        }
        // tick: renders one frame

        
        public void Tick()
        {
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
