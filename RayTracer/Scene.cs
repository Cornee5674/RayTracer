using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    //which stores a list of primitives and light sources.It implements a scene-level Intersect
    //method, which loops over the primitives and returns the closest intersection.
    public class Scene
    {
        List<Primitive> primitives = new List<Primitive>();
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

        public void AddPrimitive(Primitive prim)
        {
            primitives.Add(prim);
        }
    }
}
