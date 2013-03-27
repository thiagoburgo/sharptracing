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
using System.ComponentModel;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Shaders;

namespace DrawEngine.Renderer.Materials {
    [Serializable]
    public class CookTorranceMaterial : Material {
        private readonly CookTorranceShader shader = new CookTorranceShader();
        private float roughness;

        public CookTorranceMaterial() : base() {
            this.roughness = 1.5f;
        }

        public CookTorranceMaterial(float kdiff, float kspec, float kamb, float refractIndex, float ktrans, float glossy,
                                    float shiness, float roughness, RGBColor color)
            : base(kdiff, kspec, kamb, refractIndex, ktrans, glossy, shiness, color) {
            this.roughness = roughness;
        }

        public CookTorranceMaterial(float kdiff, float kspec, float kamb, float refractIndex, float ktrans, float glossy,
                                    float shiness, float roughness, Texture texture)
            : base(kdiff, kspec, kamb, refractIndex, ktrans, glossy, shiness, texture) {
            this.roughness = roughness;
        }

        [DefaultValue(1.5f)]
        public float Roughness {
            get { return this.roughness; }
            set {
                if (value > 0) {
                    this.roughness = value;
                } else {
                    this.roughness = 1;
                }
            }
        }

        public override Shader CreateShader(Scene scene) {
            this.shader.Scene = scene;
            return this.shader;
        }
    }
}