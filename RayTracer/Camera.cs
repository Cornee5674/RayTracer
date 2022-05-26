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
    public class Camera
    {
        // Member variables
        public Vector3 pos;

        float FOV;

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

        bool foundD = false;
        float d = 0;

        ScreenPlane screenPlane;

        public Camera(float FOV)
        {
            // We start with the camera at position 0, 0, 0, and standard rotation.
            // We set FOV and calculate the screenplane positions in CalculateNew
            this.pos = (0, 0, 0);
            lookAt = (0, 0, 1);
            up = (0, 1, 0);
            right = new Vector3(1, 0, 0);

            copyLookAt = lookAt;
            copyUp = up;
            copyRight = right;
            this.FOV = FOV;

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
            // We can calculate the angle of a straight vector and the Vector to the left of the screenplane.
            // Knowing this we multiply the distance of those vectors with variable i in a for loop. With the numbers chosen, we are certain we can get every degree of FOV.
            // When the right distance is found, we store it for later use.
            this.screenPlane = new ScreenPlane();
            if (!foundD)
            {
                for (float i = 0; i < 50f; i += 0.01f)
                {
                    Vector3 C = pos + i * lookAt;
                    Vector3 left = (pos + i * lookAt) - right;
                    float angle = (float)Math.Acos(Vector3.Dot(C, left) / (C.Length * left.Length)) * 2 * 57.2958f;
                    if (angle >= FOV - 1 && angle <= FOV + 1)
                    {
                        d = i;
                        foundD = true;
                        MakeScreenplane(C);
                        break;
                    }
                }
            }
            else
            {
                Vector3 C = pos + d * lookAt;
                MakeScreenplane(C);
            }
        }

        void MakeScreenplane(Vector3 C)
        {
            // Set every corner of the screenplane with the right vectors
            screenPlane.topLeft = C + up - right;
            screenPlane.topRight = C + up + right;
            screenPlane.bottomLeft = C - up - right;
            screenPlane.bottomRight = C - up + right;
        }

        public Ray GetRay(int x, int y, int width, int height)
        {
            // Create 2 vectors along the screenplane.
            Vector3 u = screenPlane.topRight - screenPlane.topLeft;
            Vector3 v = screenPlane.bottomLeft - screenPlane.topLeft;
            // Determine where on the plane the x and y are and create a vector storing this point
            float widthNorm = (float)x / width;
            float heightNorm = (float)y / height;
            Vector3 point = screenPlane.topLeft + widthNorm*u + heightNorm*v;
            Vector3 direction = point - pos;
            direction.Normalize();
            // Create a new ray and return this
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
