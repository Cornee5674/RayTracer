using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RayTracer
{
    //which stores a list of primitives and light sources.It implements a scene-level Intersect
    //method, which loops over the primitives and returns the closest intersection.
    public class Scene
    {
        bool drawShadowRays;
        // TODO: Automatiseer dit ipv hardcode
        int maxZ = 40;
        int minZ = 0;
        int maxX = 20;
        int minX = -20;

        public List<Primitive> primitives = new List<Primitive>();
        public List<Light> lights = new List<Light>();
        public struct Coordinate
        {
            public float x;
            public float y;
            public float z;
        }
        Surface screen;
        Camera camera;
        public Scene(Surface screen, Camera camera, bool drawShadowRays)
        {
            this.camera = camera;
            this.screen = screen;
            this.drawShadowRays = drawShadowRays;  
        }
        public Intersection GetClosestIntersection(Ray ray)
        {
            Intersection closestIntersection = new Intersection();
            closestIntersection.distance = ray.distance;
            for (int i = 0; i < primitives.Count; i++)
            {
                bool intersects = primitives[i].Intersect(ray);
                if (intersects && ray.distance != -1 && ray.distance < closestIntersection.distance)
                {
                    closestIntersection.distance = ray.distance;
                    closestIntersection.nearestPrimitive = primitives[i];
                    closestIntersection.normal = closestIntersection.nearestPrimitive.GetNormal(ray);
                }
            }
            return closestIntersection;
        }

        public Vector3 isInLight(Intersection intersection, Ray ray, bool isDrawn)
        {
            Vector3 intersectionPoint = intersection.distance * ray.direction + ray.origin;
            intersectionPoint += intersection.normal * 0.0001f;
            bool[] isInLight = new bool[lights.Count];
            for (int i = 0; i < lights.Count; i++)
            {
                Vector3 direction = lights[i].pos - intersectionPoint;
                direction.Normalize();

                float shortestDistance = Vector3.Distance(lights[i].pos, intersectionPoint);
                Vector3 end = intersectionPoint + shortestDistance * direction;

                bool hasIntersection = false;
                for (int j = 0; j < primitives.Count; j++)
                {
                    Ray newRay = new Ray(intersectionPoint, direction);
                    if (primitives[j].Intersect(newRay))
                    {
                        if (newRay.distance < shortestDistance && isDrawn)
                        {                           
                            shortestDistance = newRay.distance;
                            end = intersectionPoint + shortestDistance * direction;
                        }
                        hasIntersection = true;
                    }
                }
                if (isDrawn && drawShadowRays)
                {
                    Coordinate start = GetCoordinate(intersectionPoint);
                    Coordinate endP = GetCoordinate(end);
                    screen.Line((int)start.x, (int)start.z, (int)endP.x, (int)endP.z, 255);
                }
                isInLight[i] = hasIntersection;
            }
            Vector3 color = (0, 0, 0);
            for (int i = 0; i < lights.Count; i++)
            {
                if (!isInLight[i])
                {
                    color += intersection.nearestPrimitive.material.materialColor(ray, intersection, lights[i], intersectionPoint);
                }
            }
            color += intersection.nearestPrimitive.material.ambientLight * intersection.nearestPrimitive.color;
            return color;
        }

        public void AddPrimitive(Primitive prim)
        {
            primitives.Add(prim);
        }

        public void AddLight(Light light)
        {
            lights.Add(light);
        }





        public void DrawDebug()
        {
            Coordinate cameraCoordinate = GetCoordinate(camera.pos);
            int location = (int)cameraCoordinate.x + (int)cameraCoordinate.z * screen.width;
            screen.pixels[location] = 255;
            screen.pixels[location + 1] = 255;
            screen.pixels[location + screen.width] = 255;
            screen.pixels[location + 1 + screen.width] = 255;

            Coordinate screenCoordinateLeft = GetCoordinate(camera.GetTopLeft());
            Coordinate screenCoordinateRight = GetCoordinate(camera.GetTopRight());
            screen.Line((int)screenCoordinateLeft.x, (int)screenCoordinateLeft.z, (int)screenCoordinateRight.x, (int)screenCoordinateRight.z, 255 * 255 * 255);

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

            for (int i = 0; i < lights.Count; i++)
            {
                Coordinate coordinate = GetCoordinate(lights[i].pos);
                int loc = (int)coordinate.x + (int)coordinate.z * screen.width;
                int color = mixColor(255, 255, 255);
                screen.pixels[loc] = color;
                screen.pixels[loc + 1] = color;
                screen.pixels[loc + screen.width] = color;
                screen.pixels[loc + 1 + screen.width] = color;
            }
        }

        public void DrawDebugRay(Ray ray, Intersection intersection)
        {
            Coordinate coordinate = GetCoordinate(ray.origin);
            Vector3 end = ray.direction * intersection.distance + ray.origin;
            Coordinate endCoordinate = GetCoordinate(end);


            float slope = (endCoordinate.z - coordinate.z) / (endCoordinate.x - coordinate.x);
            Coordinate originCoord = GetCoordinate(ray.origin);
            int restX = (int)((screen.width / 2) - originCoord.x);
            float newY = originCoord.z + restX * slope;
            if (endCoordinate.x > screen.width / 2)
            {
                endCoordinate.x = screen.width / 2;
                endCoordinate.z = newY;
            }
            screen.Line((int)coordinate.x, (int)coordinate.z, (int)endCoordinate.x, (int)endCoordinate.z, 255 * 255);
        }

        public Coordinate GetCoordinate(Vector3 pos)
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

        int mixColor(int red, int green, int blue)
        {
            return (red << 16) + (green << 8) + blue;
        }
    }
}
