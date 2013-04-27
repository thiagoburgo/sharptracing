/**
 * Criado por: Thiago Burgo Belo (thiagoburgo@gmail.com)
 * SharpTracing é um projeto feito inicialmente para disciplina
 * Computação Gráfica da UFPE e depois melhorado nos tempos livres
 * sinta-se a vontade para copiar modificar e mandar correções e 
 * sugestões. Mantenha os créditos!
 * **************************************************************
 * SharpTracing is a project originally created to discipline 
 * Computer Graphics of UFPE and was improved in my free time.
 * Feel free to copy, modify and  give fixes 
 * suggestions. Keep the credits!
 */

using System;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Shaders;

namespace DrawEngine.Renderer.Materials {
    [Serializable]
    public class PhongMaterial : Material {
        
        public PhongMaterial() : base() {}

        public PhongMaterial(float kdiff, float kspec, float kamb, float refractIndex, float ktrans, float glossy,
                             float shiness, RGBColor color)
            : base(kdiff, kspec, kamb, refractIndex, ktrans, glossy, shiness, color) {}

        public PhongMaterial(float kdiff, float kspec, float kamb, float refractIndex, float ktrans, float glossy,
                             float shiness, Texture texture)
            : base(kdiff, kspec, kamb, refractIndex, ktrans, glossy, shiness, texture) {}

        public override Shader CreateShader(Scene scene) {
            return new PhongShader(scene);
        }
    }
}