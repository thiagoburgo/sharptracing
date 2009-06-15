using System;
using System.Windows.Forms;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Lights;
using DrawEngine.Renderer.Materials;

namespace DrawEngine.Renderer.Shaders
{
    [Serializable]
    public class CookTorranceShader : Shader
    {
        private float D;
        private float F;
        private float G;
        private Vector3D H;
        private Vector3D L;
        private float lightFactor;
        private Vector3D N;
        private float NH;
        private float NL;
        private float NV;
        private Vector3D V;
        private float VH;
        public CookTorranceShader() : base() {}
        public CookTorranceShader(Scene scene) : base(scene) {}
        public override RGBColor Shade(Ray ray, Intersection intersection)
        {
            CookTorranceMaterial material = (CookTorranceMaterial)intersection.HitPrimitive.Material;
            RGBColor color = this.Scene.IAmb * material.KAmb; //Contribuicao ambiental                                  
            this.V = -ray.Direction;
            this.N = intersection.Normal;
            this.NV = this.N * this.V;
            foreach(Light light in this.Scene.Lights){
                //Vetor do ponto para a Luz               
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
                                              * this.NL); //*this.lightFactor;
                                } else{
                                    color += (material.KDiff * material.DiffuseColor * light.Color * this.NL);
                                            //* this.lightFactor;
                                }
                            }
                        }
                        if(material.IsReflective){
                            this.H = this.L + this.V;
                            this.H.Normalize();
                            this.NH = this.N * this.H;
                            this.VH = this.V * this.H;
                            //verificar se entrando ou saindo...
                            float eta = intersection.HitFromInSide
                                                ? material.RefractIndex * 1 / this.Scene.RefractIndex
                                                : this.Scene.RefractIndex * 1 / material.RefractIndex;
                            this.F = FresnelTerm(this.VH, eta); //* (float)(1/Math.PI);
                            this.D = DistributionTerm(this.NH, material.Roughness);
                            this.G = GeometryOclusionTerm(this.VH, this.NH, this.NL, this.NV);
                            color += ((material.Shiness * this.F * this.D * this.G * light.Color) * (this.NL * this.NV));
                        }
                    }
                    color *= this.lightFactor * shadowFactor;
                }
            }
            return color;
        }

        #region COOK - TORRANCE TERMS
        public static float DistributionTerm(float NH, float m)
        {
            //double cosSqInv = 1.0 / (NH * NH);
            //double tanpsiSq = cosSqInv - 1.0; // (tan^2 \psi)
            //double rSqInv = 1.0 / (m * m);
            //return (float)(Math.Exp(-tanpsiSq * rSqInv) * (cosSqInv * cosSqInv) * rSqInv * (1 / Math.PI));
            float beta = (float)Math.Acos(NH);
            float exs = (float)(Math.Tan(beta) / m);
            exs = exs * exs * -1.0f;
            float ex = (float)Math.Exp(exs);
            float den = 4.0f * m * m * (float)Math.Pow(NH, 4);
            if(m == 0 || den == 0){
                MessageBox.Show("D");
            }
            return ex / den;
        }
        public static float GeometryOclusionTerm(float VH, float NH, float NL, float NV)
        {
            float twoNH = NH + NH;
            float G = 1;
            float GM = (twoNH * NV) * 1 / VH;
            if(GM < G){
                G = GM;
            }
            float GS = (twoNH * NL) * 1 / VH;
            if(GS < G){
                G = GS;
            }
            if(VH == 0) {
                MessageBox.Show("G");
            }
            return G;
        }
        public static float FresnelTerm(float VH, float eta)
        {
            float c = VH;
            float g = (eta * eta) + (c * c) - 1;
            ////TESTAR ISSO!
            g = g > 0 ? (float)Math.Sqrt(g) : g * -1;
            float g_minus_c = g - c;
            float g_plus_c = g + c;
            if(g_plus_c == 0){
                MessageBox.Show("F");
            }
            float F = 0.5f * ((g_minus_c * g_minus_c) / (g_plus_c * g_plus_c));
            float cg_plus_c = (c * g_plus_c) - 1;
            cg_plus_c = cg_plus_c * cg_plus_c;
            float cg_minus_c = (c * g_minus_c) + 1;
            cg_minus_c = cg_minus_c * cg_minus_c;
            F *= (1 + (cg_plus_c / cg_minus_c));
            if(cg_minus_c == 0) {
                MessageBox.Show("F");
            }
            return F;
        }
        #endregion
    }
}