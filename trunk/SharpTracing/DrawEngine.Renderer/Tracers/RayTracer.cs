using System;
using System.Drawing;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Tracers
{
    public sealed class RayTracer : RayCasting
    {
        public RayTracer(Scene scene) : base(scene) {}
        public RayTracer() : base() {}

        protected override RGBColor Trace(Ray ray, int depth) {
            
            Intersection intersection;
            RGBColor color = RGBColor.Black;
            if(this.scene.FindIntersection(ray, out intersection)){
                Material material = intersection.HitPrimitive.Material;
                this.scene.Shader = material.CreateShader(this.scene);
                color = this.scene.Shader.Shade(ray, intersection);
                Ray rRay = new Ray();
                if (depth < this.maxDepth)
                {
                    float n1 = this.scene.RefractIndex;
                    float n2 = material.RefractIndex;
                    if(ray.PrevRefractIndex == material.RefractIndex) {
                        float temp = n1;
                        n1 = n2;
                        n2 = temp;
                    }
                    float kSpec = material.KSpec;
                    bool specFromRefract = false;
                    float fresnelFactor = 0;
                    if (material.KTrans > 0)
                    {
                        Vector3D T;
                        if (Vector3D.Refracted(intersection.Normal, ray.Direction, out T, n1, n2))
                        {
                            rRay.Origin = intersection.HitPoint;
                            rRay.Direction = T;
                            rRay.PrevRefractIndex = material.RefractIndex;
                            //RGBColor absorbance = material.DiffuseColor * 0.15f * -intersection.TMin;
                            //RGBColor transparency = new RGBColor((float)Math.Exp(absorbance.R), 
                            //                                     (float)Math.Exp(absorbance.G),
                            //                                     (float)Math.Exp(absorbance.B));
                            fresnelFactor = Vector3D.FresnelBySchlick(intersection.Normal, ray.Direction, n1, n2);
                            float kTrans = material.KTrans - fresnelFactor;
                            kSpec += fresnelFactor;
                            specFromRefract = true;
                            kTrans = kTrans < 0 ? 0 : kTrans;
                            color += this.Trace(rRay, depth + 1) * kTrans;
                        }
                    }
                    
                    if(kSpec > 0){
                        rRay.Origin = intersection.HitPoint;
                        rRay.Direction = Vector3D.Reflected(intersection.Normal, ray.Direction);
                        if(!specFromRefract){
                            fresnelFactor = Vector3D.FresnelBySchlick(intersection.Normal, ray.Direction, n1, n2);
                            kSpec = material.KSpec + fresnelFactor;    
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