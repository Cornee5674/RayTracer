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
        }
        public override Vector3 materialColor(Ray ray, Intersection intersection, Light light, Vector3 intersectionPoint)
        {
            float distanceSquared = Vector3.DistanceSquared(light.pos, intersectionPoint);
            float distanceAttenuation = 1 / distanceSquared;
            Vector3 newLight = light.color * diffuseColor;
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
    public class NoMaterial: Material
    {
        public NoMaterial()
        {
        }
    }
}
