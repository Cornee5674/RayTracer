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
        public Vector3 position;
        public float radius;

        public string name = "Sphere";

        public Sphere(Vector3 pos, float radius, Material material, Vector3 color)
        {
            this.position = pos;
            this.radius = radius;
            this.material = material;
            this.color = color;
        }
        public override bool Intersect(Ray ray)
        {
            // Function to intersect a ray with a sphere, and setting the distance of the directional vector
            Vector3 c = position - ray.origin;
            float t = Vector3.Dot(c, ray.direction);
            Vector3 q = c - t * ray.direction;
            float p2 = q.LengthSquared;
            if (p2 > radius * radius)
            {
                ray.distance = t;
                return false;
            }
            t -= (float) Math.Sqrt(radius * radius - p2);
            if (t > 0)
            {
                ray.distance = t;
                return true;
            }      
            ray.distance = t;
            return false;
        }



        public override Vector3 GetNormal(Ray ray)
        {
            // Creates a normal from a given ray. First we get the intersectionpoint, and if we subtract the spheres position we get a normal.
            Vector3 intersect = ray.origin + ray.distance * ray.direction;
            Vector3 normal = intersect - position;
            normal.Normalize();
            return normal;
        }

        public override float X(float angle)
        {
            // Function to return the X value of the sphere after 'angle' degrees
            return (radius * (float)Math.Cos(angle * Math.PI / 180)) + position.X;
        }
        public override float Y(float angle)
        {
            // Function to return the Y value of the sphere after 'angle' degrees
            return (radius * (float)Math.Sin(angle * Math.PI / 180)) + position.Z;
        }
    }
}
