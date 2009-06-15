using System;
using System.Drawing;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;

namespace DrawEngine.Renderer.Tracers
{
    public delegate void SceneRenderingEventHandler(Bitmap partOfImage, int percentageProcessed);

    public abstract class RayCasting
    {
        protected Scene scene;
        protected RayCasting(Scene scene)
        {
            this.scene = scene;
        }
        protected RayCasting()
        {
            this.scene = new Scene();
        }
        public virtual Scene Scene
        {
            get { return this.scene; }
            set
            {
                if(value != null){
                    this.scene = value;
                } else{
                    throw new ArgumentNullException("value:scene");
                }
            }
        }
        public abstract void Render(Graphics g);
        protected static RGBColor AverageColors(params RGBColor[] colors)
        {
            float r = 0, g = 0, b = 0;
            float len_inv = 1f / colors.Length;
            foreach(RGBColor color in colors){
                r = (r + color.R);
                g = (g + color.G);
                b = (b + color.B);
            }
            return new RGBColor((r * len_inv), (g * len_inv), (b * len_inv));
        }
        /// <summary>
        /// Calculate the refracted vector 
        /// </summary>
        /// <param name="N">Normal vector.</param>
        /// <param name="I">Incoming vector.</param>
        /// <param name="T">Outcoming Refracted vector.</param>
        /// <param name="eta">eta = (Scene Index)/(Material Index) if ray generate outside material</param>
        /// <returns></returns>
        protected static bool Refracted(Vector3D N, Vector3D I, out Vector3D T, float eta)
        {
            #region Formula
            //formula T = (n*(N.I) - sqrt(1-n²(1-(N.I)²)).N  - n.I
            //nMeio = nMeio != 0.0f ? nMeio : 1.0f;
            //nObjeto = nObjeto != 0.0f ? nObjeto : 1.0f;
            //float n = nMeio * 1.0f / nObjeto;
            //I.Inverse();
            #endregion

            float cosI = I * N;
            if(cosI < 0){
                I.Flip();
                cosI = I * N;
            }
            float cosT2 = 1.0f - eta * eta * (1.0f - (cosI * cosI));
            if(cosT2 <= 0.0f){
                T = Vector3D.Zero;
                return false;
            }
            T = ((eta * cosI - (float)Math.Sqrt(cosT2)) * N) - (eta * I);
            return true;
        }
        protected static Vector3D Reflected(Vector3D N, Vector3D I)
        {
            float NI = N * -I;
            return (2 * (NI) * N) + I;
        }
        protected static Vector3D ReflectedDiffuse(Vector3D N)
        {
            Random random = new Random(((int)DateTime.Now.Ticks) ^ 47);
            Vector3D randDir = new Vector3D(-1 + 2 * (float)random.NextDouble(), -1 + 2 * (float)random.NextDouble(),
                                            -1 + 2 * (float)random.NextDouble());
            float dot = randDir * N;
            if(dot < 0.0f){
                randDir.Flip();
            }
            return randDir;
        }
    }
}