using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RayTracer
{
    public class Primitive
    {
        public string name = "Primitive";
        public Material material;
        public Vector3 color;
        public bool hasTexture = false;
        public Primitive()
        {
        }
        

        // Functions that primitives can override
        // The function that intersects a ray with a primitive
        public virtual bool Intersect(Ray ray)
        {
            return false;
        }
        // Function that returns the normal of planes
        public virtual Vector3 GetNormal(Ray ray)
        {
            return new Vector3(0, 0, 0);
        }
        // Function to return a texture color
        public virtual Vector3 GetTextureCol(float u, float v)
        {
            return (0, 0, 0);
        }
        // Function to return knownpoint of plane
        public virtual Vector3 GetO()
        {
            return (0, 0, 0);
        }
        // Function to return x value of sphere after 'angle' degrees
        public virtual float X(float angle)
        {
            return 0;
        }
        // Function to return y value of sphere after 'angle' degrees
        public virtual float Y(float angle)
        {
            return 0;
        }
    }
}
