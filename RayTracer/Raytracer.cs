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
    public class Raytracer
    {
        Surface screen;
        Camera camera;
        Scene scene;

        // Variables to set for the debug mode
        bool drawDebugRays = true;
        bool drawShadowRays = true;
        bool drawSecondaryRays = true;

        int threads;
        int perThread;

        public void MoveTo(Vector3 moveTo)
        {
            // If we move to a new position, we set the camera's position to this vector and with calculatenew we calculate the new screenplane
            camera.pos = moveTo;
            camera.CalculateNew();
        }

        public void Rotate(Vector3 rotate)
        {
            camera.Rotate(rotate);
        }

        public void ClearScreen()
        {
            // After rendering a frame and we want to render a new one, we first clear the whole screen by setting all pixels to black
            for (int y = 0; y < screen.height; y++)
            {
                for (int x = 0; x < screen.width; x++)
                {
                    screen.pixels[x + y * screen.width] = 0;
                }
            }
        }

        public Raytracer(Surface screen, int fov, bool drawDebugRays, bool drawShadowRays, bool drawSecondaryRays)
        {
            this.screen = screen;
            this.drawDebugRays = drawDebugRays;
            this.drawShadowRays = drawShadowRays;
            this.drawSecondaryRays = drawSecondaryRays;
            threads = Environment.ProcessorCount;
            // Rotating over Y means going right
            // Rotating over X means going down
            // Rotating over Z means tilting left

            this.camera = new Camera(fov);
            this.scene = new Scene(screen, camera, drawShadowRays);

            // Here we can add primitives
            // To add a sphere: new Sphere(Vector3 position, float radius, Material material, Vector3 color);
            // To add a plane: new Plane(Vector3 knownpoint, Vector3 normal, Material material, Vector3 color);

            // Materials:
            // Diffuse(Vector3 diffusecolor)
            // Mirror()
            // DiffuseGlossy(float glossiness, Vector3 specularcoefficient, Vector3 diffusecolor)
            // Glossy(float glossiness, Vector3 specularcoefficient)

            // Lights:
            // Light(Vector3 position, Vector3 color)

            scene.AddPrimitive(new Sphere(new Vector3(-8, 0, 6), 3, new Diffuse((1f, 0f, 0f)), (1f, 0f, 0f)));
            scene.AddPrimitive(new Sphere(new Vector3(-0.35f, 0, 10), 4, new Mirror(), (.3f, .5f, .8f)));
            scene.AddPrimitive(new Sphere(new Vector3(8, 0, 9), 3, new DiffuseGlossy(50, (0f, 1f, 0f), (0f, 1f, 0f)), (0f, 1f, 0f)));
            scene.AddPrimitive(new Sphere(new Vector3(5, -2, 4), 1, new Glossy(50, (0f, 0f, 1f)), (0f, 0f, 1f)));

            scene.AddLight(new Light((-4, 8, 2), (50f, 50f, 50f)));
            scene.AddLight(new Light((4, 8, 2), (50f, 50f, 50f)));
            scene.AddPrimitive(new Plane(new Vector3(0, -3f, 10), (0, 1, 0), new Diffuse((252f / 255, 178f / 255, 199f / 255)), (0, 0, 0)));

            perThread = screen.height / threads;
            threadArray = new Thread[threads];
            bools = new bool[threads];

        }
        Thread[] threadArray;
        bool[] bools;
        bool canRender = true;
        public bool CanRender()
        {
            return canRender;
        }

        public void StartRender()
        {
            if (canRender)
            {
                // Starting threads
                for (int i = 0; i < threads; i++)
                {
                    bools[i] = false;
                    int startY = i * perThread;
                    int endY = i * perThread + perThread;
                    if (i == threads - 1) endY = screen.height;
                    threadArray[i] = new Thread(() => Render(startY, endY));
                    threadArray[i].Start();
                }
                canRender = false;
            }
        }

        public Vector3 cameraLookAt()
        {
            return camera.lookAt;
        }

        public Vector3 cameraRight()
        {
            return camera.right;
        }

        public Vector3 cameraUp()
        {
            return camera.up;
        }

        public void MakeCanRender()
        {
            // Check to see if rendering is done
            bool t = true;
            for (int i = 0; i < bools.Length; i++)
            {
                if (!bools[i])
                {
                    t = false;
                }
            }
            if (t)
            {
                canRender = true;
            }
        }


        public void Render(int startY, int endY)
        {
            // We want to calculate a color for every x and y, x however only for half of the values because the left hand side of the view is for the debug mode
            for (int y = startY; y < endY; y++)
            {
                for (int x = 0; x < screen.width / 2; x++)
                {
                    // isDrawn will be true if that ray happens to be a debug ray. The variable will be used to draw secondary or shadow rays from this ray.
                    bool isDrawn = false;
                    // We get a ray for this x and y position and the closest intersection.
                    Ray ray = camera.GetRay(x, y, screen.height, screen.width / 2);
                    Intersection intersection = scene.GetClosestIntersection(ray);
                    if (y == screen.height / 2 && x % 50 == 0 && drawDebugRays)
                    {
                        // For some x,y values with y=0 we draw a debug ray
                        isDrawn = true;
                        scene.DrawDebugRay(ray, intersection);
                    }
                    if (intersection.nearestPrimitive != null)
                    {
                        // If an intersection has been found, we trace the color along the ray and set the pixel to this color
                        Vector3 color = Trace(intersection, ray, isDrawn);
                        int colorInt = mixColor((int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255));
                        screen.pixels[x + screen.width / 2 + y * screen.width] = colorInt;
                    }else
                    {
                        // If we want to give the background a color, we do that here
                        //screen.pixels[x + screen.width / 2 + y * screen.width] = mixColor(186, 245, 255);
                    }
                    
                }
            }
            bools[startY / perThread] = true;
            DrawLine();
        }


        void DrawLine()
        {
            // To differentiate between the debug view and scene view, we draw a line on exactly half of the screen
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
                // If the material is a mirror, we make a new ray that has the same angle as the incoming ray, but the other way.
                Vector3 R = ray.direction - 2 * Vector3.Dot(ray.direction, intersection.normal) * intersection.normal;
                ray.origin = intersection.distance * ray.direction + ray.origin;
                ray.direction = R;
                ray.distance = 1000;
                // We get a new intersection and add to the numberofbounces.
                Intersection newIntersection = scene.GetClosestIntersection(ray);
                ray.numberOfBounces++;
                // Since we want to draw the secondary ray of some rays, we do that here.
                if (isDrawn && ray.numberOfBounces == 1 && drawSecondaryRays)
                {
                    scene.DrawDebugRay(ray, newIntersection);
                }
                // We now recursively send rays until we found an intersection on a material that is not a mirror.
                if (newIntersection.nearestPrimitive != null) {
                    // Since a bit of the mirror's color will be reflected, we multiply this by the next ray's color
                    return intersection.nearestPrimitive.color * Trace(newIntersection, ray, isDrawn);
                }
            }
            // If the material of the intersection is not a mirror, we simply get the color of that pixel.
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
