using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace RayTracer
{
    //which owns the scene, camera and the display surface. The Raytracer implements a
    //method Render, which uses the camera to loop over the pixels of the screen plane and to
    //generate a ray for each pixel, which is then used to find the nearest intersection.The result is
    //then visualized by plotting a pixel.For the middle row of pixels (typically line 256 for a 512x512
    //window), it generates debug output by visualizing every Nth ray(where N is e.g. 10).
    public class Raytracer
    {
        Surface screen;
        Camera camera;
        Scene scene;

        int baseX;

        public Raytracer(Surface screen)
        {
            this.screen = screen;
            this.baseX = screen.width / 2;
            this.camera = new Camera();
            this.scene = new Scene();
        }

        public void Render()
        {
        }
    }
}
