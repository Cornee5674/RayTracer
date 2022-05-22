using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RayTracer
{
    //which stores the location and intensity of a light source.For a Whitted-style ray tracer,
    //this will be a point light.Intensity should be stored using float values for red, green and blue.

    public class Light
    {
        public Vector3 pos;
        public Vector3 color;

        public Light(Vector3 pos, Vector3 color)
        {
            this.pos = pos;
            this.color = color;
        }
    }
}
