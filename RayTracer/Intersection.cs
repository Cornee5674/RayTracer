using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RayTracer
{
    //which stores the result of an intersection.Apart from the intersection distance,
    //you will at least want to store the nearest primitive, but perhaps also the normal at the
    //intersection point.
    public class Intersection
    {
        public Primitive nearestPrimitive;
        public float distance;
        public Vector3 normal;

        public Intersection()
        {

        }
    }
}
