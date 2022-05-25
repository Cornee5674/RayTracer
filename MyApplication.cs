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
            raytracer.ClearScreen();
            z++;
            raytracer.MoveTo((x, y, z));
            raytracer.Render();
        }
    }
}