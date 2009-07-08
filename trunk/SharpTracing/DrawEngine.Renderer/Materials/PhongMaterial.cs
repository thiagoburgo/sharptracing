using System;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Shaders;

namespace DrawEngine.Renderer.Materials
{
    [Serializable]
    public class PhongMaterial : Material
    {
        private readonly PhongShader shader = new PhongShader();
        public PhongMaterial() : base() {}
        public PhongMaterial(float kdiff, float kspec, float kamb, float refractIndex, float ktrans, float glossy, float shiness,
                             RGBColor color) : base(kdiff, kspec, kamb, refractIndex, ktrans, glossy, shiness, color) {}
        public PhongMaterial(float kdiff, float kspec, float kamb, float refractIndex, float ktrans, float glossy, float shiness,
                             Texture texture)
            : base(kdiff, kspec, kamb, refractIndex, ktrans, glossy, shiness, texture) { }
        public override Shader CreateShader(Scene scene)
        {
            this.shader.Scene = scene;
            return this.shader;
        }
    }
}