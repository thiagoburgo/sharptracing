using System;
using System.Runtime.InteropServices;

namespace DrawEngine.Renderer.Mathematics.Algebra {
    public enum Component : byte {
        X,
        Y,
        Z
    }

    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Dominant {
        public Component Component;
        public float Value;

        public Dominant(Component comp, float value) {
            this.Component = comp;
            this.Value = value;
        }
    }
}