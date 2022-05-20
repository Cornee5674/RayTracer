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

        public string name = "Sphere";

        public Sphere(Vector3 pos, float radius)
        {
            this.position = pos;
            this.radius = radius;
        }

        //TODO: Als object in camera zit, gekke dingen
        public override bool Intersect(Ray ray)
        {
            Vector3 c = position - ray.origin;
            float t = Vector3.Dot(c, ray.direction);
            Vector3 q = c - t * ray.direction;
            float p2 = q.LengthSquared;
            if (p2 > radius * radius)
            {
                ray.distance = -1;
                return false;
            }
            t -= (float) Math.Sqrt(radius * radius - p2);
            if (t > 0)
            {
                ray.distance = t;
                return true;
            }
            ray.distance = -1;
            return false;
        }

        public override Vector3 GetNormal(Ray ray)
        {
            Vector3 intersect = ray.origin + ray.distance * ray.direction;
            Vector3 normal = intersect - position;
            normal.Normalize();
            return normal;
        }
    }
}
