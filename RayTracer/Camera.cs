using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace RayTracer
{
    //with data members position, look-at direction, and up direction.The camera also
    //stores the screen plane, specified by its four corners, which are updated whenever camera
    //position and/or direction is modified.Hardcoded coordinates and directions allow for an easy
    //start.Use e.g. (0,0,0) as the camera origin, (0,0,1) as the look-at direction, and (0, 1, 0) as the up
    //direction; this way the screen corners can also be hardcoded for the time being.Once the basic
    //setup works, you must make this more flexible.
    public class Camera
    {
        public Vector3 pos;

        Vector3 lookAt;
        Vector3 up;
        Vector3 right;

        struct ScreenPlane
        {
            public Vector3 topLeft;
            public Vector3 topRight;
            public Vector3 bottomLeft;
            public Vector3 bottomRight;
        }

        ScreenPlane screenPlane;

        public Camera()
        {
            this.pos = new Vector3(0, 0, 0);
            this.lookAt = new Vector3(0, 0, 1);
            this.up = new Vector3(0, 1, 0);
            this.right = new Vector3(1, 0, 0);

            this.screenPlane = new ScreenPlane();
            Vector3 C = pos + 0.5f*lookAt;
            screenPlane.topLeft = C + up - right;
            screenPlane.topRight = C + up + right;
            screenPlane.bottomLeft = C - up - right;
            screenPlane.bottomRight = C - up + right;
        }

        public Ray GetRay(int x, int y, int width, int height)
        {
            float widthNorm = (float)x / width;
            float heightNorm = (float)y / height;
            Vector3 direction = screenPlane.topLeft + widthNorm * (screenPlane.topRight - screenPlane.topLeft) + heightNorm * (screenPlane.bottomLeft - screenPlane.topLeft);
            direction.Normalize();
            direction -= pos;
            Ray ray = new Ray(pos, direction);
            return ray;         
        }

        public Vector3 GetTopLeft()
        {
            return screenPlane.topLeft;
        }
        public Vector3 GetTopRight()
        {
            return screenPlane.topRight;
        }

    }
}
