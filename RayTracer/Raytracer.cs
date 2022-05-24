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
        int maxZ = 40;
        int minZ = 0;
        int maxX = 20;
        int minX = -20;

        public Raytracer(Surface screen)
        {
            this.baseX = screen.width / 2;

            this.screen = screen;
            this.camera = new Camera();
            this.scene = new Scene();

            scene.AddPrimitive(new Sphere(new Vector3(-8, 0, 6), 3, new Diffuse((1f, 0f, 0f)), (1f, 0f, 0f)));
            scene.AddPrimitive(new Sphere(new Vector3(0, 0, 6), 3, new DiffuseGlossy(2, (0f, 1f, 0f), (0f, 1f, 0f)), (0f, 1f, 0f)));
            scene.AddPrimitive(new Sphere(new Vector3(8, 0, 6), 3, new Glossy(2, (0f, 0f, 1f)), (0f, 0f, 1f)));


            scene.AddLight(new Light((-4, 8, 3), (50f, 50f, 50)));
            //scene.AddLight(new Light((0, 5, 1), (1f, 1f, 1f)));
            scene.AddPrimitive(new Plane(new Vector3(0, -3f, 10), (0, 1, 0), new Diffuse((252f / 255, 178f / 255, 199f / 255)), (252f / 255, 178f / 255, 199f / 255)));

        }

        public void Render()
        {
            for (int y = 0; y < screen.height; y++)
            {
                for (int x = 0; x < screen.width / 2; x++)
                {
                    Ray ray = camera.GetRay(x, y, screen.height, screen.width / 2);
                    Intersection intersection = scene.GetClosestIntersection(ray);
                    if (y == screen.height / 2 && x % 50 == 0)
                    {
                        DrawDebugRay(ray, intersection);
                    }
                    if (intersection.nearestPrimitive != null)
                    {
                        Vector3 color = scene.isInLight(intersection, ray);
                        if (color.X > 1) color.X = 1;
                        if (color.Y > 1) color.Y = 1;
                        if (color.Z > 1) color.Z = 1;
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

            List<Primitive> primitives = scene.primitives;
            int amountOfLines = 100;
            float angle = 360f / amountOfLines;
            for (int i = 0; i < primitives.Count; i++)
            {
                float oldAngle = 0f;
                for (int j = 1; j <= amountOfLines; j++)
                {
                    float oldX = primitives[i].X(oldAngle);                    
                    float oldY = primitives[i].Y(oldAngle);
                    float newX = primitives[i].X(angle * j);
                    float newY = primitives[i].Y(angle * j);
                    oldAngle += angle;
                 
                    Coordinate coordinateOld = GetCoordinate((oldX, 0, oldY));
                    Coordinate coordinateNew = GetCoordinate((newX, 0, newY));
                    screen.Line((int)coordinateOld.x, (int)coordinateOld.z, (int)coordinateNew.x, (int)coordinateNew.z, 255 * 255 * 255);
                }
            }
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
