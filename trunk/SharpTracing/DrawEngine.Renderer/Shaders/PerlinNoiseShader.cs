using System;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Filters;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Shaders
{
    [Serializable]
    public class PerlinNoiseShader : Shader
    {
        private PhongShader shader;
        public PerlinNoiseShader(Scene scene) : base(scene)
        {
            this.shader = new PhongShader(scene);
            //PerlinNoiseFilter.Noise(
        }
        public override RGBColor Shade(Ray ray, Intersection intersection)
        {
            Vector3D n = intersection.Normal;
            //int noisetype = 6;
            //double f0 = f(n[0], n[1], n[2], noisetype),
            //        fx = f(n[0] + .0001, n[1], n[2], noisetype),
            //        fy = f(n[0], n[1] + .0001, n[2], noisetype),
            //        fz = f(n[0], n[1], n[2] + .0001, noisetype);
            //// SUBTRACT THE FUNCTION'S GRADIENT FROM THE SURFACE NORMAL
            //n[0] -= (float)((fx - f0) / .0001);
            //n[1] -= (float)((fy - f0) / .0001);
            //n[2] -= (float)((fz - f0) / .0001);
            //n.Normalize();
            ////intersection.HitPrimitive.Material.Color = new RGBColor(n[0], n[1], n[2]);
            //intersection.Normal = n;
            if(intersection.HitPrimitive.Material.IsTexturized){
                RGBColor col =
                        intersection.HitPrimitive.Material.Texture.GetPixel(
                                intersection.HitPrimitive.CurrentTextureCoordinate);
                Vector3D v = new Vector3D(col.R, col.G, col.B);
                Vector3D.Orthogonalize(ref v, n);
                v.Normalize();
                n += v;
            }
            intersection.Normal = n.Normalized;
            return this.shader.Shade(ray, intersection);
        }
        /* Noise helper functions. */
        // added types 3,4,5,6 including stripes2, stripes3 functions(cos stripes)
        double f(double x, double y, double z, int type)
        {
            double w = this.Scene.DefaultCamera.ResX;
            switch(type){
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
                    return -.10
                           *
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
    }
}