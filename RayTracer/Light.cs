using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RayTracer
{
    public class Light
    {
        // Member variables, which are the only thing light needs
        public Vector3 pos;
        public Vector3 color;

        public Light(Vector3 pos, Vector3 color)
        {
            this.pos = pos;
            this.color = color;
        }
    }
}
