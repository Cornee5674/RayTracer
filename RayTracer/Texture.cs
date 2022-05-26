using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace RayTracer
{
    public class Texture
    {
        public virtual Vector3 getColor(float u, float v)
        {
            return (0, 0, 0);
        }
    }

    public class CheckerBoard: Texture
    {
        public override Vector3 getColor(float u, float v)
        {
            if (UVCheck(u, v) % 2 == 0)
            {
                return (0, 0, 0);
            }else
            {
                return (1, 1, 1);
            }
        }
        int UVCheck(float u, float v)
        {
            return ((int)(u) + (int)(v)) & 1;
        }
    }
}
