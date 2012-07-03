using System.Runtime.InteropServices;
using System;
namespace DrawEngine.Renderer.Algebra
{
    public enum Component
    {
        X,
        Y,
        Z
    }
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Dominant
    {
        public Component Component;
        public float Value;
        public Dominant(Component comp, float value)
        {
            this.Component = comp;
            this.Value = value;
        }
    }
}