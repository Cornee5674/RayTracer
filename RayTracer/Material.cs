using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RayTracer
{
    public class Material
    {
        public Vector3 color;

      
        public virtual Vector3 materialColor(float distance, Vector3 objectColor, Vector3 lightColor, Vector3 normal, Vector3 lightToObject)
        {
            return color;
        }
    }

    public class Diffuse: Material
    {
        public Diffuse(Vector3 color)
        {
            this.color = color;
        }
        public override Vector3 materialColor(float distance, Vector3 objectColor, Vector3 lightColor, Vector3 normal, Vector3 lightToObject)
        {
            float distanceAttenuation = 1 / (distance * distance);
            Vector3 newColor = objectColor * lightColor;
            float angle = Math.Max(0, Vector3.Dot(normal, lightToObject));
            return distanceAttenuation * newColor * angle;
        }
    }
    public class NoMaterial: Material
    {
        public NoMaterial(Vector3 color)
        {
            this.color = color;
        }
    }
}
