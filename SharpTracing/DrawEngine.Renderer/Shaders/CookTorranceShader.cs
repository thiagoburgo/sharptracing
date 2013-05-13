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
using System.Diagnostics;
using System.Windows.Forms;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Lights;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Shaders {
    [Serializable]
    public class CookTorranceShader : Shader {
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

        public override RGBColor Shade(Ray ray, Intersection intersection) {
            CookTorranceMaterial material = (CookTorranceMaterial) intersection.HitPrimitive.Material;
            RGBColor color = this.Scene.IAmb * material.KAmb; //Contribuicao ambiental                                  
            this.V = -ray.Direction;
            this.N = intersection.Normal;
            this.NV = this.N * this.V;
            foreach (Light light in this.Scene.Lights) {
                //Vetor do ponto para a Luz               
                this.L = (light.Position - intersection.HitPoint);
                float shadowFactor = this.ShadowFactor(intersection, this.L, light);
                if (shadowFactor > 0f) {
                    this.L.Normalize();
                    this.lightFactor = light.GetColorFactor(this.L);
                    if (this.lightFactor > 0.0f) {
                        this.NL = this.N * this.L;
                        if (material.KDiff > 0.0f) {
                            if (this.NL > 0) {
                                //Diffuse Term                                
                                if (material.IsTexturized) {
                                    color += (material.KDiff *
                                              material.Texture.GetPixel(intersection.CurrentTextureCoordinate) *
                                              light.Color * this.NL); //*this.lightFactor;
                                } else {
                                    color += (material.KDiff * material.DiffuseColor * light.Color * this.NL);
                                    //* this.lightFactor;
                                }
                            }
                        }
                        if (material.IsReflective) {
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

        public static float DistributionTerm(float NH, float m) {
            ////double cosSqInv = 1.0 / (NH * NH);
            ////double tanpsiSq = cosSqInv - 1.0; // (tan^2 \psi)
            ////double rSqInv = 1.0 / (m * m);
            ////return (float)(Math.Exp(-tanpsiSq * rSqInv) * (cosSqInv * cosSqInv) * rSqInv * (1 / Math.PI));
            //float beta = (float) Math.Acos(NH);
            //float exs = (float) (Math.Tan(beta) / m);
            //exs = exs * exs * -1.0f;
            //float ex = (float) Math.Exp(exs);
            //float den = 4.0f * m * m * (float) Math.Pow(NH, 4);
            ////if (m.NearZero() || den.NearZero()) {
            ////     MessageBox.Show("D");
            ////}
            //return ex / den;
            double mPow2 = (m * m);
            double NHPow2 = NH * NH;
            return (float)((1.0f / mPow2 * (NHPow2 * NHPow2)) *
           Math.Exp(-((1.0 /NHPow2 - 1.0) / mPow2)));
        }

        public static float GeometryOclusionTerm(float VH, float NH, float NL, float NV) {
            float twoNH = NH + NH;
            float G = 1;
            float GM = (twoNH * NV) / VH;
            if (GM < G) {
                G = GM;
            }
            float GS = (twoNH * NL) / VH;
            if (GS < G) {
                G = GS;
            }
            //if (VH.NearZero()) {
            //    MessageBox.Show("G");
            //}
            return G;
        }

        public static float FresnelTerm(float VH, float eta) {
            double g = eta * eta + VH * VH - 1.0;
            g = g > 0 ? Math.Sqrt(g) : g * -1.0;
            double f =
                ((0.5) * (Math.Pow((g - VH), 2) / Math.Pow((g + VH), 2)) *
                 (1 + Math.Pow((VH * (g + VH) - 1), 2) / Math.Pow((VH * (g - VH) + 1), 2)));
            return (float)f;
            //float c = VH;
            //float g = (eta * eta) + (c * c) - 1;
            //////TESTAR ISSO!
            //g = g > 0 ? (float) Math.Sqrt(g) : g * -1;
            //float gMinusC = g - c;
            //float gPlusC = g + c;
            //if (gPlusC.NearZero()) {
            //    MessageBox.Show("F");
            //}
            //float F = 0.5f * ((gMinusC * gMinusC) / (gPlusC * gPlusC));
            //float cgPlusC = (c * gPlusC) - 1;
            //cgPlusC = cgPlusC * cgPlusC;
            //float cgMinusC = (c * gMinusC) + 1;
            //cgMinusC = cgMinusC * cgMinusC;
            //F *= (1 + (cgPlusC / cgMinusC));
            ////if (cgMinusC.NearZero()) {
            ////    MessageBox.Show("F");
            ////}
            //return F;


        }

        #endregion
    }
}