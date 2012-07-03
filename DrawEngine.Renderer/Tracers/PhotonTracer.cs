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
using System.Drawing;
using System.Windows.Forms;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.Lights;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.PhotonMapping;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.Tracers {
    public enum EnlightenmentType {
        Direct,
        Indirect,
        Caustics
    }

    public class PhotonTracer : RayCasting {
        private readonly PhotonMap causticsEnlightenment;
        private readonly PhotonMap indirectEnlightenment;
        private readonly int maxPhotons;
        //private int currentRecursions;
        private double irradianceArea = 2.5d;
        private int irradiancePhotonNumber = 500;
        private int maxRecursions = 20;
        private int storedCaustic;
        //private int storedDirect;
        private int storedIndirect;
        private bool buildPhotonMaps;
        public PhotonTracer(Scene scene, int maxPhotons)
            : base(scene) {
            this.scene = scene;
            this.maxPhotons = maxPhotons;
            this.indirectEnlightenment = new PhotonMap(maxPhotons);
            this.causticsEnlightenment = new PhotonMap(maxPhotons);
            this.buildPhotonMaps = true;
            this.scene.Primitives.CollectionChanged += Primitives_CollectionChanged;
            this.scene.Lights.CollectionChanged += Lights_CollectionChanged;
            this.scene.Materials.CollectionChanged += Materials_CollectionChanged;
        }

        void Materials_CollectionChanged(object sender, NotifyCollectionChangedEventArgs<Material> e) {
            this.buildPhotonMaps = true;
        }
        void Lights_CollectionChanged(object sender, NotifyCollectionChangedEventArgs<Light> e) {
            this.buildPhotonMaps = true;
        }
        void Primitives_CollectionChanged(object sender, NotifyCollectionChangedEventArgs<Primitive> e) {
            this.buildPhotonMaps = true;
        }
        public int MaxRecursions {
            get { return this.maxRecursions; }
            set { this.maxRecursions = value; }
        }
        public double IrradianceArea {
            get { return this.irradianceArea; }
            set { this.irradianceArea = value; }
        }
        public int IrradiancePhotonNumber {
            get { return this.irradiancePhotonNumber; }
            set { this.irradiancePhotonNumber = value; }
        }
        public PhotonMap IndirectEnlightenment {
            get { return this.indirectEnlightenment; }
        }
        public PhotonMap CausticsEnlightenment
        {
            get { return this.causticsEnlightenment; }
        }

        public void ScatterPhotons() {
            foreach(Light light in this.scene.Lights) {
                int emittedPhotons = 0;
                foreach(Photon photon in light.GeneratePhotons()) {
                    photon.PrevRefractIndex = this.scene.RefractIndex;
                    this.ShootPhoton(photon, 0, EnlightenmentType.Direct);
                    emittedPhotons++;
                }
                //double inv_emittedPhotons = 1.0d / emittedPhotons;
                //this.causticsEnlightenment.ScalePhotonPower(inv_emittedPhotons);
                //this.IndirectEnlightenment.ScalePhotonPower(inv_emittedPhotons);
            }
            this.causticsEnlightenment.Balance();
            this.IndirectEnlightenment.Balance();
        }
        public override void Render(Graphics g) {
            if(buildPhotonMaps){
                this.ScatterPhotons();
                buildPhotonMaps = false;
            }
            double resX = this.scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            double resY = this.scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;
            double x, y;
            int iterations = 0;
            RayTracer tracer = new RayTracer(this.scene);
            int pCol = 0, pRow = 0, pIteration = 1, pMax = 2;
            SolidBrush brush = new SolidBrush(Color.Black);
            double resTotal = resX * resY;
            while(iterations < resTotal) {
                //Render Pixels Out of Order With Increasing Resolution: 2x2, 4x4, 16x16... 512x512
                if(pCol >= pMax) {
                    pRow++;
                    pCol = 0;
                    if(pRow >= pMax) {
                        pIteration++;
                        pRow = 0;
                        pMax = (int)Math.Pow(2, pIteration);
                    }
                }
                bool pNeedsDrawing = (pIteration == 1 || (pRow % 2 != 0) || (!(pRow % 2 != 0) && (pCol % 2 != 0)));

                x = pCol * (resX / pMax);
                y = pRow * (resY / pMax);
                pCol++;
                if(pNeedsDrawing) {
                    iterations++;
                    Ray ray = this.scene.DefaultCamera.CreateRayFromScreen(x, y);
                    ray.PrevRefractIndex = this.scene.RefractIndex;

                    Intersection intersection;
                    if(this.scene.FindIntersection(ray, out intersection)) {
                        brush.Color = this.indirectEnlightenment.IrradianceEstimate(intersection.HitPoint, intersection.Normal, this.IrradianceArea,  this.IrradiancePhotonNumber).ToColor();
                        g.FillRectangle(brush, (float)x, (float)y, (float)(resX / pMax), (float)(resY / pMax));
                    }
                }
            }
        }
        protected override RGBColor Trace(Ray ray, int depth) {
            Intersection intersection;
            if(this.scene.FindIntersection(ray, out intersection)) {
                Material material = intersection.HitPrimitive.Material;
                this.scene.Shader = material.CreateShader(this.scene);
                RGBColor color = this.scene.Shader.Shade(ray, intersection);
                Ray rRay = new Ray();
                if(depth < 5) {
                    if(material.IsReflective) {
                        rRay.Origin = intersection.HitPoint;
                        rRay.Direction = Vector3D.Reflected(intersection.Normal, ray.Direction);
                        color += this.indirectEnlightenment.IrradianceEstimate(intersection.HitPoint, intersection.Normal, this.IrradianceArea, this.IrradiancePhotonNumber);
                        //color += this.Trace(rRay, depth + 1) * material.KSpec;
                        this.Trace(rRay, depth + 1);
                    }
                    if(material.IsTransparent) {
                        Vector3D T;
                        //double eta = intersection.HitFromInSide
                        //                ? material.RefractIndex * 1 / this.scene.RefractIndex
                        //                : this.scene.RefractIndex * 1 / material.RefractIndex;
                        //double eta = this.scene.RefractIndex * 1 / material.RefractIndex;
                        double eta = (ray.PrevRefractIndex == material.RefractIndex)
                                        ? material.RefractIndex * 1 / this.scene.RefractIndex
                                        : this.scene.RefractIndex * 1 / material.RefractIndex;
                        if(Vector3D.Refracted(intersection.Normal, ray.Direction, out T, eta)) {
                            rRay.Origin = intersection.HitPoint;
                            rRay.Direction = T;
                            rRay.PrevRefractIndex = material.RefractIndex;
                            color += this.indirectEnlightenment.IrradianceEstimate(intersection.HitPoint, intersection.Normal, this.IrradianceArea, this.IrradiancePhotonNumber);
                            this.Trace(rRay, depth + 1); //* material.KTrans
                        }
                    }
                }
                return color;
            }
            return this.scene.IsEnvironmentMapped ? this.scene.EnvironmentMap.GetColor(ray) : this.scene.BackgroundColor;
        }
        private static double Max(double v1, double v2, double v3) {
            return Math.Max(v1, Math.Max(v2, v3));
        }
        private static double Prob(RGBColor pPower, double coeff) {
            RGBColor colorFact = pPower * coeff;
            return Max(colorFact.R, colorFact.G, colorFact.B) / Max(pPower.R, pPower.G, pPower.B);
        }
        public void ShootPhoton(Photon photon, int depth, EnlightenmentType enlightenmentType) {
            Intersection intersection;
            if(this.scene.FindIntersection(photon, out intersection) && (depth < this.MaxRecursions)) {
                Material material = intersection.HitPrimitive.Material;
                //this.scene.Shader = material.CreateShader(this.scene);
                //RGBColor color = this.scene.Shader.Shade(photon, intersection);

                //double avgPower = photon.Power.Average;
                //double probDiff = avgPower * material.KDiff;
                //double probSpec = avgPower * material.KSpec;
                //double avgPower = photon.Power.Average;
                RGBColor mixColor = (photon.Power * material.DiffuseColor);
                double maxColor = Max(mixColor.R, mixColor.G, mixColor.B) / Max(photon.Power.R, photon.Power.G, photon.Power.B);
                double probDiff = maxColor * material.KDiff;
                double probTrans = maxColor * material.KTrans;
                double probSpec = maxColor * material.KSpec;
                Photon rPhoton = new Photon();
                Random rdn = new Random();
                double randomValue = rdn.NextDouble();
                if(randomValue <= probDiff) {

                    photon.Position = intersection.HitPoint;
                    //photon.Power = mixColor / probDiff;
                    this.storePhoton(photon, enlightenmentType);

                    rPhoton.Position = intersection.HitPoint;
                    rPhoton.Power = mixColor / probDiff;
                    rPhoton.Direction = Vector3D.ReflectedDiffuse(intersection.Normal);
                    this.ShootPhoton(rPhoton, depth + 1, EnlightenmentType.Indirect);
                }
                else if(randomValue <= probSpec + probDiff) {
                    rPhoton.Direction = Vector3D.Reflected(intersection.Normal, photon.Direction);
                    //rPhoton.Direction.Normalize();
                    rPhoton.Position = intersection.HitPoint;
                    rPhoton.Power = mixColor / probSpec;
                    this.ShootPhoton(rPhoton, depth + 1, EnlightenmentType.Caustics);
                }else if(randomValue <= probSpec + probDiff + probTrans) {
                    Vector3D T;
                    //double eta = intersection.HitFromInSide
                    //                ? material.RefractIndex * 1 / this.scene.RefractIndex
                    //                : this.scene.RefractIndex * 1 / material.RefractIndex;
                    //double eta = this.scene.RefractIndex * 1 / material.RefractIndex;
                    double eta = (photon.PrevRefractIndex == material.RefractIndex)
                                    ? material.RefractIndex * 1 / this.scene.RefractIndex
                                    : this.scene.RefractIndex * 1 / material.RefractIndex;
                    if(Vector3D.Refracted(intersection.Normal, photon.Direction, out T, eta)) {
                        rPhoton.Position = intersection.HitPoint;
                        rPhoton.Direction = T;
                        rPhoton.Power = mixColor / probTrans;
                        rPhoton.PrevRefractIndex = material.RefractIndex;
                        this.ShootPhoton(rPhoton, depth + 1, EnlightenmentType.Caustics);
                    }
                }
                else {
                    photon.Position = intersection.HitPoint;
                    //Absorb
                    this.storePhoton(photon, enlightenmentType);
                }

            }
        }
        private void storePhoton(Photon photon, EnlightenmentType enlightenmentType) {
            switch(enlightenmentType) {
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
                    this.CausticsEnlightenment.Store(photon);
                    break;
            }
        }
    }
}