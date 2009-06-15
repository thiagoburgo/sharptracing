using System;
using System.Drawing;
using DrawEngine.Renderer.BasicStructures;

namespace DrawEngine.Renderer.Cameras
{
    [Serializable]
    public class ThinLensCamera : Camera
    {
        private int countInstance;
        public override Ray CreateRayFromScreen(PointF pointOnScreen)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override Ray CreateRayFromScreen(float x, float y)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}