using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RayTracer
{
    //which encapsulates the ray/primitive intersection functionality.Two classes can be
    //derived from the base primitive: Sphere and Plane.A sphere is defined by a position and a
    //radius; a plane is defined by a normal and a distance to the origin.Initially(until you implement
    //materials) it may also be useful to add a color to the primitive class.

    public class Primitive
    {
        public string name = "Primitive";
        public Material material;
        public Vector3 color;

        public Primitive()
        {
            //color = new Vector3(252f / 255, 178f / 255, 199f / 255);
        }


        public virtual bool Intersect(Ray ray)
        {
            return false;
        }

        public virtual Vector3 GetNormal(Ray ray)
        {
            return new Vector3(0, 0, 0);
        }
        public virtual float X(float angle)
        {
            return 0;
        }
        public virtual float Y(float angle)
        {
            return 0;
        }
    }
}
