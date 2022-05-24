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
        public List<Primitive> primitives = new List<Primitive>();
        List<Light> lights = new List<Light>();

        public Intersection GetClosestIntersection(Ray ray)
        {
            Intersection closestIntersection = new Intersection();
            closestIntersection.distance = ray.distance;
            for (int i = 0; i < primitives.Count; i++)
            {
                primitives[i].Intersect(ray);
                if (ray.distance != -1 && ray.distance < closestIntersection.distance)
                {
                    closestIntersection.distance = ray.distance;
                    closestIntersection.nearestPrimitive = primitives[i];
                    closestIntersection.normal = closestIntersection.nearestPrimitive.GetNormal(ray);
                }
            }
            return closestIntersection;
        }

        public Vector3 isInLight(Intersection intersection, Ray ray)
        {
            Vector3 intersectionPoint = intersection.distance * ray.direction;
            intersectionPoint += intersection.normal * 0.0001f;
            bool[] isInLight = new bool[lights.Count];
            for (int i = 0; i < lights.Count; i++)
            {
                Vector3 direction = lights[i].pos - intersectionPoint;
                direction.Normalize();
                bool hasIntersection = false;
                for (int j = 0; j < primitives.Count; j++)
                {
                    if (primitives[j].Intersect(new Ray(intersectionPoint, direction)))
                    {
                        hasIntersection = true;
                    }
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

        
    }
}
