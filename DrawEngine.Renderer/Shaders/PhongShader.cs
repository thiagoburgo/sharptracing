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
using DrawEngine.Renderer.Filters;
using DrawEngine.Renderer.Lights;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Shaders {
    [Serializable]
    public class PhongShader : Shader {
        private Vector3D L;
        private float lightFactor;
        private Vector3D N;
        private float NL;
        //private float NV;
        private Vector3D R;
        private float RV;
        private Vector3D V;
        public PhongShader() : base() {}
        public PhongShader(Scene scene) : base(scene) {}

        public static Vector3D BumpNormal(Texture bumpTexture, Intersection intersection) {
            const float scale = 1;
            UVCoordinate uv = intersection.CurrentTextureCoordinate;
            float dx = 1.0f / (bumpTexture.Width - 1);
            float dy = 1.0f / (bumpTexture.Height - 1);
            float b0 = bumpTexture.GetPixel(uv).Luminance;
            float bx = bumpTexture.GetPixel(uv.U + dx, uv.V).Luminance;
            float by = bumpTexture.GetPixel(uv.U, uv.V + dy).Luminance;


            return (intersection.Normal + new Vector3D(scale * (bx - b0) / dx, scale * (by - b0) / dy, 1)).Normalized;


            //RGBColor color = bumpTexture.GetPixel(uv);
            //Vector3D tangent1, tangent2;
            //Vector3D.Orthonormalize(intersection.Normal, out tangent1, out tangent2);
        }

        /* Noise helper functions. */
        // added types 3,4,5,6 including stripes2, stripes3 functions(cos stripes)
        private double F(double x, double y, double z, int type) {
            double w = this.Scene.DefaultCamera.ResX;
            switch (type) {
                case 1:
                    return .03 * PerlinNoiseFilter.Noise(x, y, z, 15);
                case 2:
                    return .01 * PerlinNoiseFilter.Stripes(x + 2 * PerlinNoiseFilter.Turbulence(x, y, z, w, 1), 1.6);
                case 3:
                    return .04 * PerlinNoiseFilter.Stripes2(x + 1.5 * PerlinNoiseFilter.Turbulence(x, y, z, w, 1), 1.6);
                case 4:
                    return .03 * PerlinNoiseFilter.Stripes(PerlinNoiseFilter.Noise(x, y, z, 5), 5);
                case 5:
                    //return .05 * PerlinNoiseFilter.Turbulence(
                    //    PerlinNoiseFilter.Noise(x, y, z, 2),
                    //    PerlinNoiseFilter.Noise(x, y, z, 2),
                    //    PerlinNoiseFilter.Noise(x, y, z, 2), w, 5);
                    //return .05 *
                    //    PerlinNoiseFilter.Noise((float)Math.Cos(x * Math.PI), y, z, 10) *
                    //    PerlinNoiseFilter.Noise(x, (float)Math.Cos(y * Math.PI), z, 10) *
                    //    PerlinNoiseFilter.Noise(x, y, (float)Math.Cos(z * Math.PI), 10);
                    return .03 * Math.Cos(PerlinNoiseFilter.Noise(x, y, z));
                    //return .01 * PerlinNoiseFilter.Stripes(x + 2 * PerlinNoiseFilter.Turbulence(x, y, z, w, 1), 1.6);
                    //return 0.05 * PerlinNoiseFilter.Stripes3(x - PerlinNoiseFilter.Noise(x, y, z, 1), z - PerlinNoiseFilter.Turbulence(x, y, z, w, 1));
                case 6:
                    return -.10 *
                           PerlinNoiseFilter.Turbulence(
                               x - PerlinNoiseFilter.Stripes(PerlinNoiseFilter.Noise(x, y, z, 5), 5),
                               y - PerlinNoiseFilter.Stripes(PerlinNoiseFilter.Noise(x, y, z, 5), 5),
                               z - PerlinNoiseFilter.Stripes(PerlinNoiseFilter.Noise(x, y, z, 5), 5), w, 10);
                case 7:
                    return .04 * PerlinNoiseFilter.Stripes3(x + 2 * PerlinNoiseFilter.Turbulence(x, y, z, w, 1), 1.6);
                default:
                    return -.10 * PerlinNoiseFilter.Turbulence(x, y, z, w, 1);
            }
        }

        public override RGBColor Shade(Ray ray, Intersection intersection) {
            Material material = intersection.HitPrimitive.Material;
            RGBColor color = this.Scene.IAmb * material.KAmb; //Contribuicao ambiental                                  
            this.V = -ray.Direction;

            //{
            //    Vector3D n = intersection.Normal;
            //    int noisetype = 2;
            //    double f0 = F(n[0], n[1], n[2], noisetype),
            //            fx = F(n[0] + .0001, n[1], n[2], noisetype),
            //            fy = F(n[0], n[1] + .0001, n[2], noisetype),
            //            fz = F(n[0], n[1], n[2] + .0001, noisetype);
            //    // SUBTRACT THE FUNCTION'S GRADIENT FROM THE SURFACE NORMAL
            //    n[0] -= (float)((fx - f0) / .0001);
            //    n[1] -= (float)((fy - f0) / .0001);
            //    n[2] -= (float)((fz - f0) / .0001);
            //    n.Normalize();
            //    //intersection.HitPrimitive.Material.Color = new RGBColor(n[0], n[1], n[2]);
            //    intersection.Normal = n;
            //} 
            this.N = intersection.Normal;


            //if(material.IsTexturized){
            //    this.N = BumpNormal(intersection.HitPrimitive.Material.Texture, intersection);
            //    intersection.Normal = this.N;
            //} else{
            //    this.N = intersection.Normal;
            //}

            //this.NV = this.N * this.V;
            foreach (Light light in this.Scene.Lights) {
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
                                              light.Color * this.NL) * this.lightFactor * shadowFactor;
                                } else {
                                    color += (material.KDiff * material.DiffuseColor * light.Color * this.NL) *
                                             this.lightFactor * shadowFactor;
                                }
                            }
                        }
                        if (material.IsReflective) {
                            this.R = (2 * this.NL * this.N) - this.L;
                            this.R.Normalize();
                            this.RV = this.R * this.V;
                            if (this.RV > 0) {
                                //Specular Term
                                color += (material.KSpec * material.SpecularColor * light.Color *
                                          (float) Math.Pow(this.RV, material.Shiness)) * this.lightFactor * shadowFactor;
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