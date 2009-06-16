using System;
using System.Drawing;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Lights;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.PhotonMapping;

namespace DrawEngine.Renderer.Tracers
{
    public enum EnlightenmentType
    {
        Direct, 
        Indirect,
        Caustics
    }

    public class PhotonTracer : RayCasting
    {
        private readonly PhotonMap causticsEnlightenment;
        private readonly PhotonMap indirectEnlightenment;
        //private readonly int maxPhotons;
        //private int currentRecursions;
        private float irradianceArea = 1.5f;
        private int irradiancePhotonNumber = 1000000;
        private int maxRecursions = 20;
        private int storedCaustic;
        //private int storedDirect;
        private int storedIndirect;
        public PhotonTracer(Scene scene, int maxPhotons) : base(scene)
        {
            this.scene = scene;
            //this.maxPhotons = maxPhotons;
            this.indirectEnlightenment = new PhotonMap(maxPhotons);
            this.causticsEnlightenment = new PhotonMap(maxPhotons);
        }
        public int MaxRecursions
        {
            get { return this.maxRecursions; }
            set { this.maxRecursions = value; }
        }
        public float IrradianceArea
        {
            get { return this.irradianceArea; }
            set { this.irradianceArea = value; }
        }
        public int IrradiancePhotonNumber
        {
            get { return this.irradiancePhotonNumber; }
            set { this.irradiancePhotonNumber = value; }
        }
        public PhotonMap IndirectEnlightenment
        {
            get { return this.indirectEnlightenment; }
        }
        private void ScatterPhotons()
        {
            foreach(Light light in this.scene.Lights){
                int emittedPhotons = 0;
                foreach(Photon photon in light.GeneratePhotons()){
                    this.TracePhoton(photon, 0, EnlightenmentType.Direct);
                    emittedPhotons++;
                }
                //float inv_emittedPhotons = 1.0f / emittedPhotons;
                //this.causticsEnlightenment.ScalePhotonPower(inv_emittedPhotons);
                //this.IndirectEnlightenment.ScalePhotonPower(inv_emittedPhotons);
            }
            //this.causticsEnlightenment.Balance();
            //this.IndirectEnlightenment.Balance();
        }
        public override void Render(Graphics g)
        {
            this.ScatterPhotons();

            #region Progressive Render
            //float resX = this.scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            //float resY = this.scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;
            //float x, y;
            //int iterations = 0;
            //RGBColor[] colors = new RGBColor[this.scene.Sampler.SamplesPerPixel + 1];
            //int pCol = 0, pRow = 0, pIteration = 1, pMax = 2;
            //SolidBrush brush = new SolidBrush(Color.Black);
            //float resTotal = resX * resY * this.scene.Sampler.SamplesPerPixel;
            //while (iterations <= resTotal)
            //{
            //    //Render Pixels Out of Order With Increasing Resolution: 2x2, 4x4, 16x16... 512x512
            //    if (pCol >= pMax)
            //    {
            //        pRow++;
            //        pCol = 0;
            //        if (pRow >= pMax)
            //        {
            //            pIteration++;
            //            pRow = 0;
            //            pMax = (int)Math.Pow(2, pIteration);
            //        }
            //    }
            //    bool pNeedsDrawing = (pIteration == 1 || (pRow % 2 != 0) || (!(pRow % 2 != 0) && (pCol % 2 != 0)));
            //    x = pCol * (resX / pMax);
            //    y = pRow * (resY / pMax);
            //    pCol++;
            //    if (pNeedsDrawing)
            //    {
            //        iterations++;
            //        Ray ray;
            //        if (this.scene.Sampler.SamplesPerPixel > 1)
            //        {
            //            int i = 0;
            //            foreach (Point2D sample in this.scene.Sampler.GenerateSamples(x, y))
            //            {
            //                //ray = this.scene.DefaultCamera.CreateRayFromScreen(x + sample.X, y + sample.Y);
            //                ray = this.scene.DefaultCamera.CreateRayFromScreen(sample.X, sample.Y);
            //                ray.PrevRefractIndex = this.scene.RefractIndex;
            //                colors[i++] = this.Trace(ray, 0);
            //            }
            //            brush.Color = AverageColors(colors).ToColor();
            //        }
            //        else
            //        {
            //            ray = this.scene.DefaultCamera.CreateRayFromScreen(x, y);
            //            brush.Color = this.Trace(ray, 0).ToColor();
            //        }
            //        g.FillRectangle(brush, x, y, (resX / pMax), (resY / pMax));
            //    }
            //}
            #endregion

            #region Linear Render
            //float resX = this.scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            //float resY = this.scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;
            //SolidBrush brush = new SolidBrush(Color.Black);
            //RGBColor[] colors = new RGBColor[this.scene.Sampler.SamplesPerPixel];
            //for(int y = 0; y < resY; y++) {
            //    for(int x = 0; x < resX; x++) {
            //        Ray ray;
            //        if(this.scene.Sampler.SamplesPerPixel > 1) {
            //            int i = 0;
            //            foreach(Point2D sample in this.scene.Sampler.GenerateSamples(x, y)) {
            //                ray = this.scene.DefaultCamera.CreateRayFromScreen(sample.X, sample.Y);
            //                ray.PrevRefractIndex = this.scene.RefractIndex;
            //                colors[i++] = this.TracePhoton(ray, 0);
            //            }
            //            brush.Color = AverageColors(colors).ToColor();
            //        }
            //        else {
            //            ray = this.scene.DefaultCamera.CreateRayFromScreen(x, y);
            //            brush.Color = this.TracePhoton(ray, 0, ).ToColor();
            //        }
            //        g.FillRectangle(brush, x, y, 1, 1);
            //    }
            //}
            #endregion
        }
        private static float Max(float v1, float v2, float v3)
        {
            return Math.Max(v1, Math.Max(v2, v3));
        }
        private static float Prob(RGBColor pPower, float coeff)
        {
            RGBColor colorFact = pPower * coeff;
            return Max(colorFact.R, colorFact.G, colorFact.B) / Max(pPower.R, pPower.G, pPower.B);
        }
        public void TracePhoton(Photon photon, int depth, EnlightenmentType enlightenmentType)
        {
            Intersection intersection;
            if(this.scene.FindIntersection(photon, out intersection) && (depth < this.MaxRecursions)){
                Material material = intersection.HitPrimitive.Material;
                this.scene.Shader = material.CreateShader(this.scene);
                RGBColor color = this.scene.Shader.Shade(photon, intersection);

                //float avgPower = photon.Power.Average;
                //float probDiff = avgPower * material.KDiff;
                //float probSpec = avgPower * material.KSpec;
                //float avgPower = photon.Power.Average;
                RGBColor mixColor = (photon.Power * material.DiffuseColor);
                float maxColor = Max(mixColor.R, mixColor.G, mixColor.B);
                float probDiff = maxColor * material.KDiff;
                float probSpec = maxColor * material.KSpec;
                Photon rPhoton = new Photon();
                Random rdn = new Random(((int)DateTime.Now.Ticks) ^ 47);
                double randomValue = rdn.NextDouble();
                if(randomValue <= probDiff) {
                    this.storePhoton(photon, enlightenmentType);
                    rPhoton.Direction = ReflectedDiffuse(intersection.Normal).Normalized;
                    rPhoton.Position = intersection.HitPoint;
                    rPhoton.Power = (material.DiffuseColor * photon.Power) / probDiff;
                    this.TracePhoton(rPhoton, depth + 1, EnlightenmentType.Indirect);
                }
                else if(randomValue <= probSpec + probDiff) {

                } else{
                    //Absorb
                    this.storePhoton(photon, enlightenmentType);
                } 
               
            }
        }
        private void storePhoton(Photon photon, EnlightenmentType enlightenmentType)
        {
            switch(enlightenmentType){
                //case EnlightenmentType.Direct:
                //    //Do nothing - Classic RayTracing
                //    ;
                //break;
                case EnlightenmentType.Indirect:
                    this.storedIndirect++;
                    this.indirectEnlightenment.Store(photon);
                    break;
                case EnlightenmentType.Caustics:
                    this.storedCaustic++;
                    this.causticsEnlightenment.Store(photon);
                    break;
            }
        }
    }
}