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
using System.Collections.Generic;
using System.Drawing;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Tracers {
    public sealed class DistributedRayTracer : RayCasting {
        public DistributedRayTracer(Scene scene) : base(scene) { }
        public DistributedRayTracer() : base() { }
        
        protected override RGBColor Trace(Ray ray, int depth) {
            Intersection intersection;
            RGBColor color = RGBColor.Black;
            if(this.scene.FindIntersection(ray, out intersection)) {
                Material material = intersection.HitPrimitive.Material;
                this.scene.Shader = material.CreateShader(this.scene);
                color = this.scene.Shader.Shade(ray, intersection);
                Ray rRay = new Ray();
                RGBColor medColor = RGBColor.Black;
                if(depth < this.maxDepth) {
                    double n1 = this.scene.RefractIndex;
                    double n2 = material.RefractIndex;
                    if (ray.PrevRefractIndex == material.RefractIndex)
                    {
                        double temp = n1;
                        n1 = n2;
                        n2 = temp;
                    }
                    double kSpec = material.KSpec;
                    bool specFromRefract = false;
                    double fresnelFactor = 0;

                    if (material.KTrans > 0)
                    {
                        Vector3D T;
                        //double eta = intersection.HitFromInSide
                        //                ? material.RefractIndex * 1 / this.scene.RefractIndex
                        //                : this.scene.RefractIndex * 1 / material.RefractIndex;
                        double eta = (ray.PrevRefractIndex == material.RefractIndex)
                                        ? material.RefractIndex * 1 / this.scene.RefractIndex
                                        : this.scene.RefractIndex * 1 / material.RefractIndex;

                        if (Vector3D.Refracted(intersection.Normal, ray.Direction, out T, eta))
                        {
                            rRay.Origin = intersection.HitPoint;
                            rRay.PrevRefractIndex = material.RefractIndex;
                            rRay.Direction = T;

                            fresnelFactor = Vector3D.FresnelBySchlick(intersection.Normal, ray.Direction, n1, n2);
                            double kTrans = material.KTrans - fresnelFactor;
                            kSpec += fresnelFactor;
                            specFromRefract = true;
                            kTrans = kTrans < 0 ? 0 : kTrans;

                            if (material.Glossy > 0 && this.scene.GlossySamples > 0)
                            {   
                                foreach (Vector3D blurry in GetBlurryDirections(T, this.scene.GlossySamples, material.Glossy, intersection.Normal))
                                {
                                    rRay.Direction = blurry;
                                    medColor += this.Trace(rRay, depth + 1) * kTrans;
                                }
                                
                                color += medColor * 1d / (this.scene.GlossySamples * this.scene.GlossySamples);
                            }
                            else
                            {
                                color += this.Trace(rRay, depth + 1) * kTrans;
                            }
                        }
                    }
                    if (kSpec > 0)
                    {
                        Vector3D reflected = Vector3D.Reflected(intersection.Normal, ray.Direction);
                        rRay.Origin = intersection.HitPoint;
                        rRay.Direction = reflected;
                        /********* Must be calculated for each blurry direction, but is slow ***********/
                        if (!specFromRefract)
                        {
                            fresnelFactor = Vector3D.FresnelBySchlick(intersection.Normal, ray.Direction, n1, n2);
                            kSpec = material.KSpec + fresnelFactor;
                        }
                        kSpec = kSpec > 1 ? 1 : kSpec;
                        /********************/
                        if(material.Glossy > 0 && this.scene.GlossySamples > 0) {
                            foreach(Vector3D blurry in GetBlurryDirections(reflected, this.scene.GlossySamples, material.Glossy, intersection.Normal)) {
                                rRay.Direction = blurry;
                                medColor += this.Trace(rRay, depth + 1) * kSpec;
                            }
                            color += medColor * 1d / (this.scene.GlossySamples * this.scene.GlossySamples);
                        }
                        else {
                            color += this.Trace(rRay, depth + 1) * kSpec;
                        }
                    }
                    
                }
                return color;
            }
            return this.scene.IsEnvironmentMapped ? this.scene.EnvironmentMap.GetColor(ray) : color;
        }
        private static List<Vector3D> GetBlurryDirections(Vector3D toPertub, int gridLen, double glossy, Vector3D normal) {
            List<Vector3D> blurryVectors = new List<Vector3D>(gridLen * gridLen);
            double factor = glossy / gridLen;

            Vector3D u = (normal ^ toPertub);//*factor;
            Vector3D v = (u ^ toPertub) * factor;
            u *= factor;
            Vector3D start = (toPertub - u) - v;

            Vector3D sample;
            Random rnd = new Random();
            for(int row = 0; row < gridLen; row++) {
                for(int col = 0; col < gridLen; col++) {
                    sample = start + u * (col + rnd.NextDouble() /*Jitter u*/)
                                   + v * (row + rnd.NextDouble() /*Jitter v*/);
                    blurryVectors.Add(sample);
                }
            }
            return blurryVectors;
        }

        #region Other Blurry
        //private static Vector3D Blurry(Vector3D toPertub, double glossy, Vector3D normal, Point3D hitPoint) {
        //    Random rdn = new Random();
        //    double theta = (glossy * rdn.NextDouble() % (2 * Math.PI)); //random angle between 0 and 2pi
        //    double phi = (glossy * (rdn.NextDouble() % (Math.PI * 0.5d))); //random angle between 0 and (pi/2)*roughness
        //    Vector3D newdir = new Vector3D((Math.Cos(theta) * Math.Sin(phi)),
        //                                   (Math.Sin(theta) * Math.Sin(phi)), 
        //                                   1d - (Math.Cos(phi)));
        //    return toPertub - newdir;
        //}

        //private static Vector3D Blurry(Vector3D toPertub,double glossy) {
        //    Random rdn = new Random();
        //    double x = (-1 + 2 * rdn.NextDouble());
        //    double y = (-1 + 2 * rdn.NextDouble());
        //    double z = (-1 + 2 * rdn.NextDouble());
        //    Vector3D u = new Vector3D(x, y, z);
        //    Vector3D.Orthogonalize(ref u, toPertub);
        //    x = (-1 + 2 * rdn.NextDouble());
        //    y = (-1 + 2 * rdn.NextDouble());
        //    z = (-1 + 2 * rdn.NextDouble());
        //    Vector3D v = new Vector3D(x, y, z);
        //    Vector3D.Orthogonalize(ref v, u);
        //    return toPertub + ((u * glossy) + (v * glossy));
        //}
 
        //private static Vector3D Blurry(Vector3D toPertub, double glossy, Vector3D normal, Point3D hitPoint) {
        //    Random rdn = new Random();
        //    Vector3D u = normal ^ toPertub;
        //    Vector3D v = u ^ toPertub;
        //    Vector3D offSet = (hitPoint + u * (rdn.NextDouble() * glossy)) - (hitPoint + v * (rdn.NextDouble() * glossy));

        //    return toPertub + offSet;
        //}
        #endregion
        
    }
}