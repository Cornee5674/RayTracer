using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

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

        struct Coordinate
        {
            public int x;
            public int y;
            public int z;
        }

        public Raytracer(Surface screen)
        {
            this.baseX = screen.width / 2;

            this.screen = screen;
            this.camera = new Camera(screen.width / 2, screen.height);
            this.scene = new Scene();

            scene.AddPrimitive(new Sphere(new Vector3(-8, 5, 20), 8));
            Sphere sphere = new Sphere(new Vector3(2, 0, 15), 8);
            sphere.setColor();
            scene.AddPrimitive(sphere);

            DrawDebug();
        }

        public void Render()
        {
            for (int y = 0; y < screen.height; y++)
            {
                for (int x = 0; x < screen.width / 2; x++)
                {
                    Ray ray = camera.GetRay(x, y, screen.height, screen.width / 2);
                    Intersection intersection = scene.GetClosestIntersection(ray);
                    if (intersection.nearestPrimitive != null)
                    {
                        Vector3 color = intersection.nearestPrimitive.color;
                        int colorInt = mixColor((int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255));
                        screen.pixels[x + screen.width / 2 + y * screen.width] = colorInt;
                    }
                }
            }
        }

        int mixColor(int red, int green, int blue)
        {
            return (red << 16) + (green << 8) + blue;
        }

        void DrawDebug()
        {
            
        }

        //Coordinate GetCoordinate(Vector3 pos)
        //{

        //}
    }
}
