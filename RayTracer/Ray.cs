using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RayTracer
{
    public class Ray
    {
        // Member variables, distance is set to 1000
        // This means that the max view distance is 1000
        public Vector3 origin;
        public Vector3 direction;
        public float distance = 1000;

        public int numberOfBounces = 0;

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
    }
}
