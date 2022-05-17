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
        int maxX;
        int maxY;

        Vector3 pos;

        Vector3 lookAt;
        Vector3 up;

        struct ScreenPlane
        {
            public Vector3 topLeft;
            public Vector3 topRight;
            public Vector3 bottomLeft;
            public Vector3 bottomRight;
        }

        ScreenPlane screenPlane;

        public Camera(int maxX, int maxY)
        {
            this.maxX = maxX;
            this.maxY = maxY;
            this.pos = new Vector3(0, 0, 0);
            this.lookAt = new Vector3(0, 0, 1);
            this.up = new Vector3(0, 1, 0);

            this.screenPlane = new ScreenPlane();
            screenPlane.topLeft = (-2, 2, 1);
            screenPlane.topRight = (2, 2, 1);
            screenPlane.bottomLeft = (-2, -2, 1);
            screenPlane.bottomRight = (2, -2, 1);
        }

        public Vector3 GetRay(int x, int y)
        {
            float xMinToMax = screenPlane.topRight.X - screenPlane.topLeft.X;
            float xScaled = xMinToMax / maxX;
            float xPos = screenPlane.topLeft.X + (xScaled * x);

            float yMinToMax = screenPlane.topLeft.Y - screenPlane.bottomLeft.Y;
            float yScaled = yMinToMax / maxY;
            float yPos = screenPlane.bottomLeft.Y + (yScaled * y);

            Vector3 screenPos = new Vector3(xPos, yPos, 1);

            return screenPos - pos;
        }
    }
}
