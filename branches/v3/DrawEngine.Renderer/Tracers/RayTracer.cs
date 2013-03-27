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

using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.Renderers;

namespace DrawEngine.Renderer.Tracers {
    public sealed class RayTracer : RayCasting {
        public RayTracer(Scene scene, RenderStrategy renderStrategy) : base(scene, renderStrategy) {}
        public RayTracer() {}

        public override RGBColor Trace(Ray ray, int depth) {
            Intersection intersection;
            RGBColor color = RGBColor.Black;
            if (this.scene.FindIntersection(ray, out intersection)) {
                Material material = intersection.HitPrimitive.Material;
                this.scene.Shader = material.CreateShader(this.scene);
                color = this.scene.Shader.Shade(ray, intersection);
                Ray rRay = new Ray();
                if (depth < this.maxDepth) {
                    float n1 = this.scene.RefractIndex;
                    float n2 = material.RefractIndex;
                    //if (ray.PrevRefractIndex == material.RefractIndex)
                    if (ray.PrevPrimitive == intersection.HitPrimitive) {
                        float temp = n1;
                        n1 = n2;
                        n2 = temp;
                    }
                    float kSpec = material.KSpec;
                    bool specFromRefract = false;
                    float fresnelFactor = 0;
                    if (material.KTrans > 0) {
                        Vector3D T;
                        if (Vector3D.Refracted(intersection.Normal, ray.Direction, out T, n1, n2)) {
                            rRay.Origin = intersection.HitPoint;
                            rRay.Direction = T;

                            //rRay.PrevRefractIndex = material.RefractIndex;
                            rRay.PrevPrimitive = intersection.HitPrimitive;

                            fresnelFactor = Vector3D.FresnelBySchlick(intersection.Normal, ray.Direction, n1, n2);
                            float kTrans = material.KTrans - fresnelFactor;
                            kSpec += fresnelFactor;
                            specFromRefract = true;
                            kTrans = kTrans < 0 ? 0 : kTrans;
                            color += this.Trace(rRay, depth + 1) * kTrans;
                            //float term  = (float)Math.Exp(material.Absorptivity * -intersection.TMin);
                            //color *= term * material.DiffuseColor;
                            //RGBColor transparency = new RGBColor((float)Math.Exp(absorbance.R), 
                            //                                     (float)Math.Exp(absorbance.G),
                            //                                     (float)Math.Exp(absorbance.B));
                        }
                    }

                    if (kSpec > 0) {
                        rRay.Origin = intersection.HitPoint;
                        rRay.Direction = Vector3D.Reflected(intersection.Normal, ray.Direction);
                        if (!specFromRefract) {
                            //fresnelFactor = Vector3D.FresnelBySchlick(intersection.Normal, ray.Direction, n1, n2);
                            kSpec = material.KSpec + fresnelFactor;
                            rRay.PrevPrimitive = intersection.HitPrimitive;
                        }
                        kSpec = kSpec > 1 ? 1 : kSpec;
                        color += this.Trace(rRay, depth + 1) * kSpec;
                    }
                }
                return color;
            }
            return this.scene.IsEnvironmentMapped ? this.scene.EnvironmentMap.GetColor(ray) : color;
        }
    }
}