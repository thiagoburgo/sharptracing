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
using System.Windows.Forms;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Lights;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Shaders
{
    [Serializable]
    public class CookTorranceShader : Shader
    {
        private double D;
        private double F;
        private double G;
        private Vector3D H;
        private Vector3D L;
        private double lightFactor;
        private Vector3D N;
        private double NH;
        private double NL;
        private double NV;
        private Vector3D V;
        private double VH;
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
                double shadowFactor = this.ShadowFactor(intersection, this.L, light);
                if(shadowFactor > 0d){
                    this.L.Normalize();
                    this.lightFactor = light.GetColorFactor(this.L);
                    if(this.lightFactor > 0.0d){
                        this.NL = this.N * this.L;
                        if(material.KDiff > 0.0d){
                            if(this.NL > 0){
                                //Diffuse Term                                
                                if(material.IsTexturized){
                                    color += (material.KDiff
                                              *
                                              material.Texture.GetPixel(
                                                      intersection.CurrentTextureCoordinate) * light.Color
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
                            double eta = intersection.HitFromInSide
                                                ? material.RefractIndex * 1 / this.Scene.RefractIndex
                                                : this.Scene.RefractIndex * 1 / material.RefractIndex;
                            this.F = FresnelTerm(this.VH, eta); //* (1/Math.PI);
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
        public static double DistributionTerm(double NH, double m)
        {
            //double cosSqInv = 1.0 / (NH * NH);
            //double tanpsiSq = cosSqInv - 1.0; // (tan^2 \psi)
            //double rSqInv = 1.0 / (m * m);
            //return (Math.Exp(-tanpsiSq * rSqInv) * (cosSqInv * cosSqInv) * rSqInv * (1 / Math.PI));
            double beta = Math.Acos(NH);
            double exs = (Math.Tan(beta) / m);
            exs = exs * exs * -1.0d;
            double ex = Math.Exp(exs);
            double den = 4.0d * m * m * Math.Pow(NH, 4);
            if(m == 0 || den == 0){
                MessageBox.Show("D");
            }
            return ex / den;
        }
        public static double GeometryOclusionTerm(double VH, double NH, double NL, double NV)
        {
            double twoNH = NH + NH;
            double G = 1;
            double GM = (twoNH * NV) * 1 / VH;
            if(GM < G){
                G = GM;
            }
            double GS = (twoNH * NL) * 1 / VH;
            if(GS < G){
                G = GS;
            }
            if(VH == 0) {
                MessageBox.Show("G");
            }
            return G;
        }
        public static double FresnelTerm(double VH, double eta)
        {
            double c = VH;
            double g = (eta * eta) + (c * c) - 1;
            ////TESTAR ISSO!
            g = g > 0 ? Math.Sqrt(g) : g * -1;
            double g_minus_c = g - c;
            double g_plus_c = g + c;
            if(g_plus_c == 0){
                MessageBox.Show("F");
            }
            double F = 0.5d * ((g_minus_c * g_minus_c) / (g_plus_c * g_plus_c));
            double cg_plus_c = (c * g_plus_c) - 1;
            cg_plus_c = cg_plus_c * cg_plus_c;
            double cg_minus_c = (c * g_minus_c) + 1;
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