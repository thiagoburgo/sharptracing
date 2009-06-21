using System;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Lights;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Shaders
{
    [Serializable]
    public class PhongShader : Shader
    {
        private Vector3D L;
        private float lightFactor;
        private Vector3D N;
        private float NL;
        private float NV;
        private Vector3D R;
        private float RV;
        private Vector3D V;
        public PhongShader() : base() {}
        public PhongShader(Scene scene) : base(scene) {}
        public override RGBColor Shade(Ray ray, Intersection intersection)
        {
            Material material = intersection.HitPrimitive.Material;
            RGBColor color = this.Scene.IAmb * material.KAmb; //Contribuicao ambiental                                  
            this.V = -ray.Direction;
            this.N = intersection.Normal;
            this.NV = this.N * this.V;
            foreach(Light light in this.Scene.Lights){
                this.L = (light.Position - intersection.HitPoint);
                float shadowFactor = this.ShadowFactor(intersection, this.L, light);
                if(shadowFactor > 0f){
                    this.L.Normalize();
                    this.lightFactor = light.GetColorFactor(this.L);
                    if(this.lightFactor > 0.0f){
                        this.NL = this.N * this.L;
                        if(material.KDiff > 0.0f){
                            if(this.NL > 0){
                                //Diffuse Term
                                if(material.IsTexturized){
                                    color += (material.KDiff
                                              *
                                              material.Texture.GetPixel(
                                                      intersection.HitPrimitive.CurrentTextureCoordinate) * light.Color
                                              * this.NL) * this.lightFactor * shadowFactor;
                                } else{
                                    color += (material.KDiff * material.DiffuseColor * light.Color * this.NL)
                                             * this.lightFactor * shadowFactor;
                                }
                            }
                        }
                        if(material.IsReflective){
                            this.R = (2 * this.NL * this.N) - this.L;
                            this.R.Normalize();
                            this.RV = this.R * this.V;
                            if(this.RV > 0){
                                //Specular Term
                                color += (material.KSpec * material.SpecularColor * light.Color
                                          * (float)Math.Pow(this.RV, material.Shiness)) * this.lightFactor
                                         * shadowFactor;
                            }
                        }
                        //color *= lightFactor * shadowFactor;
                    }
                }
            }
            return color;
        }
    }
}