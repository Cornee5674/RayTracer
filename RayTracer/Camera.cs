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

        Vector3 copyLookAt;
        Vector3 copyUp;
        Vector3 copyRight;

        struct ScreenPlane
        {
            public Vector3 topLeft;
            public Vector3 topRight;
            public Vector3 bottomLeft;
            public Vector3 bottomRight;
        }

        ScreenPlane screenPlane;

        public Camera(Vector3 cameraPos, Vector3 rotation)
        {
            this.pos = cameraPos;
            lookAt = (0, 0, 1);
            up = (0, 1, 0);
            right = (1, 0, 0);

            copyLookAt = lookAt;
            copyUp = up;
            copyRight = right;


            Rotate(rotation);

            CalculateNew();
        }

        public void Rotate(Vector3 rotate)
        {
            rotate.Y /= 10;
            rotate.X /= 10;
            rotate.Z /= 10;

            Vector3.Transform(copyLookAt, new Quaternion(rotate.X * 0.174533f, rotate.Y * 0.174533f, rotate.Z * 0.174533f), out lookAt);
            Vector3.Transform(copyUp, new Quaternion(rotate.X * 0.174533f, rotate.Y * 0.174533f, rotate.Z * 0.174533f), out up);
            Vector3.Transform(copyRight, new Quaternion(rotate.X * 0.174533f, rotate.Y * 0.174533f, rotate.Z * 0.174533f), out right);
        }

        public void CalculateNew()
        {
            this.screenPlane = new ScreenPlane();
            Vector3 C = pos + 0.5f * lookAt;
            screenPlane.topLeft = C + up - right;
            screenPlane.topRight = C + up + right;
            screenPlane.bottomLeft = C - up - right;
            screenPlane.bottomRight = C - up + right;
        }

        public Ray GetRay(int x, int y, int width, int height)
        {
            Vector3 u = screenPlane.topRight - screenPlane.topLeft;
            Vector3 v = screenPlane.bottomLeft - screenPlane.topLeft;
            float widthNorm = (float)x / width;
            float heightNorm = (float)y / height;
            Vector3 point = screenPlane.topLeft + widthNorm*u + heightNorm*v;
            Vector3 direction = point - pos;
            direction.Normalize();
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
