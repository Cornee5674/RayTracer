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
        public Vector3 ambientLight = (0.17f, 0.17f, 0.17f);
        public bool isMirror;
      
        public virtual Vector3 materialColor(Ray ray, Intersection intersection, Light light, Vector3 intersectionPoint)
        {
            return (0, 0, 0);
        }
    }

    public class Diffuse: Material
    {
        Vector3 diffuseColor;
        public Diffuse(Vector3 diffuseColor)
        {
            this.diffuseColor = diffuseColor;
            this.isMirror = false;
        }
        public override Vector3 materialColor(Ray ray, Intersection intersection, Light light, Vector3 intersectionPoint)
        {
            float distanceSquared = Vector3.DistanceSquared(light.pos, intersectionPoint);
            float distanceAttenuation = 1 / distanceSquared;
            Vector3 newLight;
            if (!intersection.nearestPrimitive.hasTexture)
            {
                newLight = light.color * diffuseColor;
            }else
            {
                Vector3 normal = intersection.nearestPrimitive.GetNormal(ray);
                Vector3 O = intersection.nearestPrimitive.GetO();
                Vector3 u;
                Vector3.Transform(normal, new Quaternion(9 * 0.174533f, 0, 0), out u);
                Vector3 v = Vector3.Cross(normal, u);

                Vector3 PtoO = intersectionPoint - O;
                float uD = Vector3.Dot(PtoO, u);
                float vD = Vector3.Dot(PtoO, v);
                return intersection.nearestPrimitive.GetTextureCol(uD, vD);
                //newLight = light.color * intersection.nearestPrimitive.GetTextureCol(uD, vD);
            }
            Vector3 toLight = light.pos - intersectionPoint;
            toLight.Normalize();
            float angle = Math.Max(0, Vector3.Dot(intersection.normal, toLight));
            return distanceAttenuation * newLight * angle;
        }
    }
    public class Glossy: Material
    {
        float glossiness;
        Vector3 specularCoefficient;
        public Glossy(float glossiness, Vector3 specularCoefficient)
        {
            this.glossiness = glossiness;
            this.specularCoefficient = specularCoefficient;
            this.isMirror=false;
        }
        public override Vector3 materialColor(Ray ray, Intersection intersection, Light light, Vector3 intersectionPoint)
        {
            float distanceSquared = Vector3.DistanceSquared(light.pos, intersectionPoint);
            float distanceAttenuation = 1 / distanceSquared;
            Vector3 newLight = light.color * specularCoefficient;
            Vector3 toLight = light.pos - intersectionPoint;
            toLight.Normalize();

            Vector3 R = toLight - 2 * Vector3.Dot(toLight, intersection.normal) * intersection.normal;
            Vector3 toEye = ray.origin - intersectionPoint;
            toEye.Normalize();

            float angle = (float)Math.Max(0, Math.Pow(Vector3.Dot(toEye, R), glossiness));
            return distanceAttenuation * newLight * angle;
        }
    }
    public class DiffuseGlossy: Material
    {
        float glossiness;
        Vector3 specularCoefficient;
        Vector3 diffuseColor;
        public DiffuseGlossy(float glossiness, Vector3 specularCoefficient, Vector3 diffuseColor)
        {
            this.glossiness= glossiness;
            this.specularCoefficient= specularCoefficient;
            this.diffuseColor= diffuseColor;
            this.isMirror = false;
        }

        public override Vector3 materialColor(Ray ray, Intersection intersection, Light light, Vector3 intersectionPoint)
        {
            float distanceSquared = Vector3.DistanceSquared(light.pos, intersectionPoint);
            float distanceAttenuation = 1 / distanceSquared;
            Vector3 toLight = light.pos - intersectionPoint;
            toLight.Normalize();
            float angle1 = Math.Max(0, Vector3.Dot(intersection.normal, toLight));
            Vector3 R = toLight - 2 * Vector3.Dot(toLight, intersection.normal) * intersection.normal;
            Vector3 toEye = ray.origin - intersectionPoint;
            toEye.Normalize();
            float angle2 = (float)Math.Max(0, Math.Pow(Vector3.Dot(toEye, R), glossiness));
            return distanceAttenuation * light.color * (diffuseColor * angle1 + specularCoefficient * angle2);
        }
    }
    public class Mirror: Material
    {
        public Mirror()
        {
            this.isMirror = true;
        }
    }
}
