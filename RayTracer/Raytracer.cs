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
            public float x;
            public float y;
            public float z;
        }

        // TODO: Automatiseer dit ipv hardcode
        int maxZ = 32;
        int minZ = 0;
        int maxX = 20;
        int minX = -20;

        public Raytracer(Surface screen)
        {
            this.baseX = screen.width / 2;

            this.screen = screen;
            this.camera = new Camera(screen.width / 2, screen.height);
            this.scene = new Scene();

            Sphere sphere3 = new Sphere(new Vector3(-5, 0, 8), 3);
            Sphere sphere = new Sphere(new Vector3(0, 0, 15), 2);
            Sphere sphere1 = new Sphere(new Vector3(12, 0, 15), 3);
            sphere.setColor();
            sphere1.setColor();
            sphere3.setColor();
            scene.AddPrimitive(sphere);
            scene.AddPrimitive(sphere1);
            scene.AddPrimitive(sphere3);

            scene.AddPrimitive(new Plane(new Vector3(0, -5, 100), (0, 1, 0)));

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
                    if (y == screen.height / 2 && x % 20 == 0)
                    {
                        DrawDebugRay(ray, intersection);
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
            DrawDebug();

        }

        int mixColor(int red, int green, int blue)
        {
            return (red << 16) + (green << 8) + blue;
        }

        void DrawDebug()
        {            
            Coordinate cameraCoordinate = GetCoordinate(camera.pos);
            int location = (int)(cameraCoordinate.x + cameraCoordinate.z * screen.width);
            screen.pixels[location] = 255;
            screen.pixels[location + 1] = 255;
            screen.pixels[location + screen.width] = 255;
            screen.pixels[location + 1 + screen.width] = 255;

            Coordinate screenCoordinateLeft = GetCoordinate(camera.GetTopLeft());
            Coordinate screenCoordinateRight = GetCoordinate(camera.GetTopRight());
            screen.Line((int)screenCoordinateLeft.x, (int)screenCoordinateLeft.z, (int)screenCoordinateRight.x, (int)screenCoordinateRight.z, 255 * 255 * 255);
        }

        void DrawDebugRay(Ray ray, Intersection intersection)
        {
            Coordinate coordinate = GetCoordinate(ray.origin);
            Vector3 end = ray.direction * intersection.distance;
            Coordinate endCoordinate = GetCoordinate(end);
            screen.Line((int)coordinate.x, (int)coordinate.z, (int)endCoordinate.x, (int)endCoordinate.z, 255 * 255);
        }

        Coordinate GetCoordinate(Vector3 pos)
        {
            Coordinate coordinate = new Coordinate();
            coordinate.y = 0;


            float difference = 0 - minZ;
            float newMinZ = minZ + difference;
            float newMaxZ = maxZ + difference;
            float perHeight = screen.height / (newMaxZ - newMinZ);
            float newZ = (pos.Z + difference + 1) * perHeight;
            newZ = screen.height - newZ;
            coordinate.z = newZ;

            float differenceX = 0 - minX;
            float newMinX = minX + differenceX;
            float newMaxX = maxX + differenceX;
            float perWidth = screen.width / 2 / (newMaxX - newMinX);
            float newX = (pos.X + differenceX) * perWidth;
            coordinate.x = newX;            
            
            return coordinate;
        }
    }
}
