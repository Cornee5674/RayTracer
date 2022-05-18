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
            List<Intersection> intersections = new List<Intersection>();
            for (int i = 0; i < primitives.Count; i++)
            {
                intersections.Add(primitives[i].Intersect(ray));
            }
            for (int i = 0; i < intersections.Count; i++)
            {
                if (ray.distance == intersections[i].distance)
                {
                    return intersections[i];
                }
            }
            return new Intersection();
        }

        public void AddPrimitive(Primitive prim)
        {
            primitives.Add(prim);
        }
    }
}
