using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RayTracer
{
    // Base class for a material
    // Base class stores the color for ambient light and if the material is a mirror
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
            // Calculate 1 over distance^2 of the intersectionPoint to a light source
            float distanceSquared = Vector3.DistanceSquared(light.pos, intersectionPoint);
            float distanceAttenuation = 1 / distanceSquared;
            Vector3 newLight;
            if (!intersection.nearestPrimitive.hasTexture)
            {
                // If no texture is applied, we make a new color
                newLight = light.color * diffuseColor;
            }else
            {
                // If a texture is applied, we get the normal and origin of the plane
                Vector3 normal = intersection.nearestPrimitive.GetNormal(ray);
                Vector3 O = intersection.nearestPrimitive.GetO();
                Vector3 u;
                // We can create a vector u along the plane by rotating on the x axis by 90 degrees. Quaternions rotations only work if you divide the angle by 10 for some reason.
                Vector3.Transform(normal, new Quaternion(9 * 0.174533f, 0, 0), out u);
                // We can create vector v with the cross product to get a orthonormal basis.
                Vector3 v = Vector3.Cross(normal, u);

                // Now we can get the u and v variables with the dot product and get a color from the texture that is applied
                Vector3 PtoO = intersectionPoint - O;
                float uD = Vector3.Dot(PtoO, u);
                float vD = Vector3.Dot(PtoO, v);
                return intersection.nearestPrimitive.GetTextureCol(uD, vD);
            }
            // We calculate the angle between the vector from the intersectionpoint to the light source and the normal of that point
            Vector3 toLight = light.pos - intersectionPoint;
            toLight.Normalize();
            float angle = Math.Max(0, Vector3.Dot(intersection.normal, toLight));
            // With that we can calculate a diffuse color
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
            // This part is the same as with the diffuse
            float distanceSquared = Vector3.DistanceSquared(light.pos, intersectionPoint);
            float distanceAttenuation = 1 / distanceSquared;
            Vector3 newLight = light.color * specularCoefficient;
            Vector3 toLight = light.pos - intersectionPoint;
            toLight.Normalize();

            // We create a vector R, which determines the glossiness and a vector toEye, which is the vector from the intersectionpoint to the origin of the camera.
            Vector3 R = toLight - 2 * Vector3.Dot(toLight, intersection.normal) * intersection.normal;
            Vector3 toEye = ray.origin - intersectionPoint;
            toEye.Normalize();
            // Now we create the angle variable with R and toEye to get a color
            // Note that this material only colors places on which the glossy part is visible, for the other parts to be colored, choose the DiffuseGlossy material
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
            // We combine the diffuse and glossy formulae
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
            // The only thing that is needed for a mirror is to set the variable to true
            this.isMirror = true;
        }
    }
}
