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

        bool drawDebugRays = true;
        bool drawShadowRays = true;
        bool drawSecondaryRays = true;

        public void MoveTo(Vector3 moveTo)
        {
            camera.pos = moveTo;
            camera.CalculateNew();
        }

        public void ClearScreen()
        {
            for (int y = 0; y < screen.height; y++)
            {
                for (int x = 0; x < screen.width; x++)
                {
                    screen.pixels[x + y * screen.width] = 0;
                }
            }
        }

        public Raytracer(Surface screen)
        {
            this.screen = screen;
            // Rotating over Y means going right
            // Rotating over X means going down
            // Rotating over Z means tilting left

            this.camera = new Camera((0, 0, 0), 0, 0, 0);
            this.scene = new Scene(screen, camera, drawShadowRays);
             
            scene.AddPrimitive(new Sphere(new Vector3(-8, 0, 6), 3, new Diffuse((1f, 0f, 0f)), (1f, 0f, 0f)));
            scene.AddPrimitive(new Sphere(new Vector3(-0.35f, 0, 10), 4, new Mirror(), (.3f, .5f, .8f)));
            scene.AddPrimitive(new Sphere(new Vector3(8, 0, 9), 3, new DiffuseGlossy(50, (0f, 1f, 0f), (0f, 1f, 0f)), (0f, 1f, 0f)));
            scene.AddPrimitive(new Sphere(new Vector3(5, -2, 4), 1, new Glossy(50, (0f, 0f, 1f)), (0f, 0f, 1f)));

            scene.AddLight(new Light((-4, 8, 12), (50f, 50f, 50f)));
            scene.AddLight(new Light((4, 8, 12), (50f, 50f, 50f)));
            scene.AddPrimitive(new Plane(new Vector3(0, -3f, 10), (0, 1, 0), new Diffuse((252f / 255, 178f / 255, 199f / 255)), (252f / 255, 178f / 255, 199f / 255)));

        }

        public void Render()
        {
            for (int y = 0; y < screen.height; y++)
            {
                for (int x = 0; x < screen.width / 2; x++)
                {
                    bool isDrawn = false;
                    Ray ray = camera.GetRay(x, y, screen.height, screen.width / 2);
                    Intersection intersection = scene.GetClosestIntersection(ray);
                    if (y == screen.height / 2 && x % 50 == 0 && drawDebugRays)
                    {
                        isDrawn = true;
                        scene.DrawDebugRay(ray, intersection);
                    }
                    if (intersection.nearestPrimitive != null)
                    {
                        Vector3 color = Trace(intersection, ray, isDrawn);
                        int colorInt = mixColor((int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255));
                        screen.pixels[x + screen.width / 2 + y * screen.width] = colorInt;
                    }else
                    {
                        screen.pixels[x + screen.width / 2 + y * screen.width] = mixColor(0, 0, 0);
                    }
                    
                }
            }
            for (int y = 0; y < screen.height; y++)
            {
                for (int x = 0; x < screen.width; x++)
                {
                    if (x == screen.width / 2)
                    {
                        screen.pixels[x + y * screen.width] = mixColor(254, 57, 139);
                    }
                }
            }
            scene.DrawDebug();
        }

        Vector3 Trace(Intersection intersection, Ray ray, bool isDrawn)
        {
            if (intersection.nearestPrimitive.material.isMirror && ray.numberOfBounces < 100)
            {
                Vector3 R = ray.direction - 2 * Vector3.Dot(ray.direction, intersection.normal) * intersection.normal;
                ray.origin = intersection.distance * ray.direction + ray.origin;
                ray.direction = R;
                ray.distance = 1000;
                Intersection newIntersection = scene.GetClosestIntersection(ray);
                ray.numberOfBounces++;
                if (isDrawn && ray.numberOfBounces == 1 && drawSecondaryRays)
                {
                    scene.DrawDebugRay(ray, newIntersection);
                }
                if (newIntersection.nearestPrimitive != null) {
                    return intersection.nearestPrimitive.color * Trace(newIntersection, ray, isDrawn);
                }
            }
            Vector3 color = scene.isInLight(intersection, ray, isDrawn);
            if (color.X > 1) color.X = 1;
            if (color.Y > 1) color.Y = 1;
            if (color.Z > 1) color.Z = 1;
            return color;
        }

        int mixColor(int red, int green, int blue)
        {
            return (red << 16) + (green << 8) + blue;
        }
    }
}
