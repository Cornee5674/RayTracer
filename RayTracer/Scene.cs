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
            bool isInShadow = false;
            for (int i = 0; i < lights.Count; i++)
            {
                Vector3 direction = lights[i].pos - intersectionPoint;
                direction.Normalize();
                for (int j = 0; j < primitives.Count; j++)
                {
                    if (primitives[j].Intersect(new Ray(intersectionPoint, direction)))
                    {
                        isInShadow = true;
                    }
                }
            }
            Vector3 color = (0, 0, 0);
            if (!isInShadow)
            {
                for (int i = 0; i < lights.Count; i++)
                {
                    color += intersection.nearestPrimitive.material.materialColor(ray, intersection, lights[i], intersectionPoint);
                }
            }else
            {
                color = (0f, 0f, 0f);
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
