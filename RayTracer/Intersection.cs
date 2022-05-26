using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RayTracer
{
    public class Intersection
    {
        // Basic variables of an intersection
        public Primitive nearestPrimitive;
        public float distance;
        public Vector3 normal;

        public Intersection()
        {

        }
    }
}
