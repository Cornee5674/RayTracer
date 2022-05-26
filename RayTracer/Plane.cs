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
        Texture texture;

        public string name = "Plane";

        public Plane(Vector3 location, Vector3 normal, Material material, Vector3 color)
        {
            this.knownPoint = location;
            this.normal = normal;
            this.material = material;
            this.color = color;
            this.hasTexture = true;
            this.texture = new CheckerBoard();
        }

        public override bool Intersect(Ray ray)
        {
            // Function to intersect an ray with a plane. Result is the distance of the directional vector.
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

        // Simple functions that return variables.
        public override Vector3 GetTextureCol(float u, float v)
        {
            return texture.getColor(u, v);
        }

        public override Vector3 GetO()
        {
            return knownPoint;
        }

        public override Vector3 GetNormal(Ray ray)
        {
            return normal;
        }
    }
}
