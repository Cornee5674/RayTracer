using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RayTracer
{
    public class Sphere: Primitive
    {
        Vector3 position;
        float radius;

        public Sphere(Vector3 pos, float radius)
        {
            this.position = pos;
            this.radius = radius;
        }

        public override Intersection Intersect(Ray ray)
        {
            Vector3 c = position - ray.origin;
            float t = Vector3.Dot(c, ray.direction);
            Vector3 q = c - t * ray.direction;
            float p2 = q.LengthSquared;
            if (p2 > radius * radius) return new Intersection();
            t -= (float) Math.Sqrt(radius * radius - p2);
            if ((t < ray.distance) && (t > 0)) ray.distance = t;
            Intersection intersection = new Intersection();
            intersection.distance = t;
            intersection.nearestPrimitive = this;
            return intersection;
        }
    }
}
