namespace RayTracer
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        public Raytracer raytracer;
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
            //screen.Clear(0);
            //screen.Print("hello world", 2, 2, 0xffffff);
            //screen.Line(2, 20, 160, 20, 0xff0000);
        }
    }
}