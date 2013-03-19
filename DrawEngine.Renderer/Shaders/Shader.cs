/**
 * Criado por: Thiago Burgo Belo (thiagoburgo@gmail.com)
 * SharpTracing � um projeto feito inicialmente para disciplina
 * Computa��o Gr�fica da UFPE e depois melhorado nos tempos livres
 * sinta-se a vontade para copiar modificar e mandar corre��es e 
 * sugest�es. Mantenha os cr�ditos!
 * **************************************************************
 * SharpTracing is a project originally created to discipline 
 * Computer Graphics of UFPE and was improved in my free time.
 * Feel free to copy, modify and  give fixes 
 * suggestions. Keep the credits!
 */
 using System;
using System.Xml.Serialization;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Lights;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Shaders
{
    [XmlInclude(typeof(PhongShader)), XmlInclude(typeof(CookTorranceShader)), XmlInclude(typeof(PerlinNoiseShader)),
     Serializable]
    public abstract class Shader
    {
        private Scene scene;
        protected Shader() : this(new Scene()) {}
        protected Shader(Scene scene)
        {
            this.scene = scene;
        }
        public Scene Scene
        {
            get { return this.scene; }
            set
            {
                if(value != null){
                    this.scene = value;
                } else{
                    throw new ArgumentNullException("Scene");
                }
            }
        }
        public abstract RGBColor Shade(Ray ray, Intersection intersection);
        //protected virtual bool InShadow(Intersection intersect, Vector3D L) {
        //    if(this.scene.IsShadowActive) {
        //        float len = L.Length;
        //        Ray shadowRay = new Ray(intersect.HitPoint, L);
        //        //foreach(Primitive primitive in this.scene.Primitives) {
        //        if(this.scene.FindIntersection(shadowRay, out intersect)
        //             && intersect.TMin < len) {
        //            return true; //Total Shadow
        //        }
        //        //}   
        //    }
        //    return false;
        //}
        protected virtual float ShadowFactor(Intersection intersect, Vector3D L, Light light)
        {
            float shadowFactor = 1; //[0..1] 1 = not in shadow, 0 = total shadow
            if(this.scene.IsShadowActive){
                Ray shadowRay;
                if(!this.scene.IsSoftShadowActive){
                    float len = L.Length;
                    shadowRay = new Ray(intersect.HitPoint, L);
                    if(this.scene.FindIntersection(shadowRay, out intersect) && intersect.TMin < len){
                        shadowFactor = 0; //Total Shadow
                    }
                } else{
                    AreaLight areaLight = light as AreaLight;
                    if(areaLight != null){
                        Vector3D toRndPointInLight;
                        float reductFactor = 1.0f / this.scene.SoftShadowSamples;
                        for(int i = 0; i < this.scene.SoftShadowSamples; i++){
                            toRndPointInLight = areaLight.GetRandomPoint() - intersect.HitPoint;
                            shadowRay = new Ray(intersect.HitPoint, toRndPointInLight);
                            if((intersect.HitPrimitive != null
                                && intersect.HitPrimitive.FindIntersection(shadowRay, out intersect))
                               ||
                               this.scene.FindIntersection(shadowRay, out intersect)
                               && intersect.TMin < toRndPointInLight.Length){
                                shadowFactor -= reductFactor;
                            }
                        }
                    }
                }
            }
            return shadowFactor;
        }
    }
}