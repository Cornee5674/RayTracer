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
        // We set some variables for the debug view. Can be changed by user.
        int maxZ = 40;
        int minZ = 0;
        int maxX = 20;
        int minX = -20;

        // The list of primitives and lights
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
            // To return the closest intersection, we make a new intersection with the max range of a ray, 1000
            // Then we loop over all of the primitives, and if the ray intersects with that primitive, we check if the distance to that primitive is less then the current intersection
            // If so, we set the variables of the intersection to that primitive.
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
            // First we create the intersection point and offset it with 0.0001f in the direction of the normal of that point to avoid a grainy view.
            Vector3 intersectionPoint = intersection.distance * ray.direction + ray.origin;
            intersectionPoint += intersection.normal * 0.0001f;
            bool[] isInLight = new bool[lights.Count];
            // For every light we create a ray from the intersectionpoint to that ray
            // Then we go over every primitive and check if the ray intersects.
            // If it intersects with only 1 of them, we set the bool in the array isInLight on index i to true
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
            // If the primitive does not have a texture, we simply calculate the color with our calcColor function
            Vector3 color = (0, 0, 0);
            if (!intersection.nearestPrimitive.hasTexture)
            {
                color = calcColor(isInLight, color, intersection, ray, intersectionPoint);
            }else
            {
                // If it does have a texture, we need to make sure that all lights reach a point, otherwise shadows will not work properly
                bool light = true;
                for (int i = 0; i < isInLight.Length; i++)
                {
                    if (isInLight[i]) light = false;
                }
                if (light)
                {
                    color = calcColor(isInLight, color, intersection, ray, intersectionPoint);
                }
            }
            return color;
        }

        Vector3 calcColor(bool[] isInLight, Vector3 color, Intersection intersection, Ray ray, Vector3 intersectionPoint)
        {
            // Now we add a material color to our base color for every light that the intersectionpoint can reach
            for (int i = 0; i < lights.Count; i++)
            {
                if (!isInLight[i])
                {
                    color += intersection.nearestPrimitive.material.materialColor(ray, intersection, lights[i], intersectionPoint);
                }
            }
            if (!intersection.nearestPrimitive.hasTexture)
            {
                color += intersection.nearestPrimitive.material.ambientLight * intersection.nearestPrimitive.color;
            }
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
            // We calculate the coordinate of the camera in screen space and draw it if fits within the screen.
            Coordinate max = GetCoordinate((maxX, 0, maxZ));
            Coordinate min = GetCoordinate((minX, 0, minZ));
            Coordinate cameraCoordinate = GetCoordinate(camera.pos);
            if (!(cameraCoordinate.x < min.x || cameraCoordinate.x > max.x || cameraCoordinate.z > min.z || cameraCoordinate.z < max.z))
            {
                int location = (int)cameraCoordinate.x + (int)cameraCoordinate.z * screen.width;
                if (location + 1 + screen.width <= screen.width * screen.height)
                {
                    screen.pixels[location] = 255;
                    screen.pixels[location + 1] = 255;
                    screen.pixels[location + screen.width] = 255;
                    screen.pixels[location + 1 + screen.width] = 255;
                }             
            }
            
            // We do the same but now with the screen
            Coordinate screenCoordinateLeft = GetCoordinate(camera.GetTopLeft());
            Coordinate screenCoordinateRight = GetCoordinate(camera.GetTopRight());
            if (!(screenCoordinateLeft.x < min.x || screenCoordinateRight.x < min.x || screenCoordinateLeft.x > max.x || screenCoordinateRight.x > max.x || screenCoordinateLeft.z > min.z || screenCoordinateRight.z > min.z || screenCoordinateLeft.z < max.z || screenCoordinateRight.z < max.z))
            {
                screen.Line((int)screenCoordinateLeft.x, (int)screenCoordinateLeft.z, (int)screenCoordinateRight.x, (int)screenCoordinateRight.z, 255 * 255 * 255);
            }
            // For every primitive (sphere) we create 100 angles, and calculate the x and y position of those angles and draw them
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

            // For every light we calculate the screen space coordinates and draw a white dot
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
            // We calculate the beginning and end of a ray

            Coordinate coordinate = GetCoordinate(ray.origin);
            Vector3 end = ray.direction * intersection.distance + ray.origin;
            Coordinate endCoordinate = GetCoordinate(end);

            // We calculate the slope so we can calculate if the ray will go into the scene view
            // If so we just create a new endposition with the slope and x position
            float slope = (endCoordinate.z - coordinate.z) / (endCoordinate.x - coordinate.x);
            Coordinate originCoord = GetCoordinate(ray.origin);
            int restX = (int)((screen.width / 2) - originCoord.x);
            float newY = originCoord.z + restX * slope;
            if (coordinate.x < screen.width / 2)
            {
                if (endCoordinate.x > screen.width / 2)
                {
                    endCoordinate.x = screen.width / 2;
                    endCoordinate.z = newY;
                }
                screen.Line((int)coordinate.x, (int)coordinate.z, (int)endCoordinate.x, (int)endCoordinate.z, 255 * 255);
            }          
        }

        public Coordinate GetCoordinate(Vector3 pos)
        {
            // Because the debug view is topdown, we dont need the y coordinate and set it to 0
            Coordinate coordinate = new Coordinate();
            coordinate.y = 0;

            // We calculate if the minZ is less than 0
            // We add this difference to both min and maxz
            // Now we can calculate how many pixels per z we can use
            // With this information we calculate where the given Vector3 is in screenspace
            float difference = 0 - minZ;
            float newMinZ = minZ + difference;
            float newMaxZ = maxZ + difference;
            float perHeight = screen.height / (newMaxZ - newMinZ);
            float newZ = (pos.Z + difference + 1) * perHeight;
            newZ = screen.height - newZ;
            coordinate.z = newZ;

            // We also do it with x
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
