using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RayTracer
{
    public class Plane:Primitive
    {
        Vector3 knownPoint;
        Vector3 normal;

        public string name = "Plane";

        public Plane(Vector3 location, Vector3 normal)
        {
            this.knownPoint = location;
            this.normal = normal;
        }

        public override bool Intersect(Ray ray)
        {
            float denominator = Vector3.Dot(ray.direction, normal);
            if (Math.Abs(denominator) > 0.0001f)
            {
                float top = Vector3.Dot(knownPoint - ray.origin, normal);
                float t = top / denominator;
                if (t > 0)
                {
                    ray.distance = t;
                    return true;
                }

            }
            ray.distance = -1;
            return false;
        }

        public override Vector3 GetNormal(Ray ray)
        {
            return normal;
        }
    }
}
