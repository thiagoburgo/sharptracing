using System;

namespace DrawEngine.Renderer.Algebra
{
    public struct UVCoordinate
    {
        public float U;
        public float V;
        public UVCoordinate(float u, float v)
        {
            this.U = u;
            this.V = v;
            if(!this.IsValid){
                throw new ArgumentException("Invalid coordinates!");
            }
        }
        public bool IsValid
        {
            get { return !(this.U < 0.0f || this.U > 1.0f || this.V < 0.0f || this.V > 1.0f || (this.U + this.V) > 1.0f); }
        }
    }
}