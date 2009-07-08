using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Cameras;
using DrawEngine.Renderer.Cameras.Design;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.Lights;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Renderers;
using DrawEngine.Renderer.RenderObjects;
using DrawEngine.Renderer.RenderObjects.EnvironmentMaps;
using DrawEngine.Renderer.RenderObjects.EnvironmentMaps.Design;
using DrawEngine.Renderer.Samplers;
using DrawEngine.Renderer.Shaders;
using DrawEngine.Renderer.SpatialSubdivision.Acceleration;
using DrawEngine.Renderer.Util;

namespace DrawEngine.Renderer {
    [XmlRoot("Scene", IsNullable = false), Serializable]
    public class Scene : INameable, IIntersectable {
        private IntersectableAccelerationStructure<Primitive> accelerationStructure;
        private RGBColor backgroundColor = RGBColor.Black;
        private NameableCollection<Camera> cameras;
        private Camera defaultCamera;
        private string defaultCameraName;
        private EnvironmentMap environmentMap = new CubeMap();
        private RGBColor iAmb;
        private bool isShadowActive;
        private NameableCollection<Light> lights;
        private NameableCollection<Material> materials;
        private NameableCollection<Primitive> primitives;
        private float refractIndex;
        private RenderStrategy renderStrategy;
        private Sampler sampler;
        private int glossySamples;
        private Shader shader;
        private bool visible = true;


        #region EVENTOS
        private bool isSoftShadowActive;
        private string name;
        private int softShadowSamples;
        public event NameChangedEventHandler OnNameChanged;
        public event NameChangingEventHandler OnNameChanging;
        public string Name {
            get { return this.name; }
            set {
                if(this.OnNameChanging != null) {
                    CancelNameChageEventArgs cancel = new CancelNameChageEventArgs(value);
                    this.OnNameChanging(this, cancel);
                    if(cancel.Cancel) {
                        throw new ArgumentException("Name changing cancelled!");
                    }
                }
                string oldName = this.name;
                this.name = value;
                if(this.OnNameChanged != null) {
                    this.OnNameChanged(this, oldName);
                }
            }
        }
        #endregion

        public Scene(RGBColor iamb, NameableCollection<Light> lights, Camera defaultCamera, float refractIndice) {
            this.IAmb = iamb;
            this.RefractIndex = refractIndice;
            if(lights != null){
                this.Lights = new NameableCollection<Light>(lights);    
            } else{
                this.Lights = new NameableCollection<Light>();    
            }
            this.primitives = new NameableCollection<Primitive>();
            this.AccelerationStructure = new KDTreePrimitiveManager(this.primitives as IList<Primitive>);
            this.materials = new NameableCollection<Material>();
            this.DefaultCamera = defaultCamera;
            this.Cameras = new NameableCollection<Camera>();
            if(defaultCamera != null) {
                this.Cameras.Add(defaultCamera);
            }
            this.sampler = new RegularGridSampler();
            this.shader = new PhongShader(this);
            this.renderStrategy = new ScanlineRenderStrategy(this);
            this.isShadowActive = false;
            this.softShadowSamples = 8;
            this.glossySamples = 8;
            //this.cameras.CollectionChanged += new NotifyCollectionChangedEventHandler<Camera>(cameras_CollectionChanged);
        }
        public Scene() : this(RGBColor.White, new NameableCollection<Light>(), null, 1.0f) { }
        public Scene(Scene toCopy) {
            if(toCopy != null) {
                this.defaultCamera = toCopy.defaultCamera;
                this.IAmb = toCopy.iAmb;
                this.refractIndex = toCopy.refractIndex;
                this.lights = new NameableCollection<Light>(toCopy.lights);
                this.primitives = new NameableCollection<Primitive>(toCopy.primitives);
                this.materials = new NameableCollection<Material>(toCopy.materials);
                this.cameras = new NameableCollection<Camera>(toCopy.cameras);
                this.isShadowActive = toCopy.isShadowActive;
                this.isSoftShadowActive = toCopy.isSoftShadowActive;
                this.softShadowSamples = toCopy.softShadowSamples;
                this.glossySamples = toCopy.glossySamples;
                this.IsEnvironmentMapped = toCopy.IsEnvironmentMapped;
                this.EnvironmentMap = toCopy.environmentMap;
                this.backgroundColor = toCopy.backgroundColor;
                if(toCopy.sampler != null) {
                    this.sampler = new RegularGridSampler(toCopy.sampler.SamplesX, toCopy.sampler.SamplesY);
                }
            }
        }
        //void cameras_CollectionChanged(object sender, NotifyCollectionChangedEventArgs<Camera> e) {
        //    if(this.cameras.ContainsName(this.defaultCameraName)) {
        //        this.DefaultCamera = this.cameras[this.DefaultCameraName];
        //    }
        //}
        [Category("Appereance")]
        public bool IsEnvironmentMapped { get; set; }
        [Category("Appereance"), DefaultValue(null), TypeConverter(typeof(ExpandableObjectConverter)),
         Editor(typeof(CubeMapUIEditor), typeof(UITypeEditor))]

        public EnvironmentMap EnvironmentMap {
            get { return this.environmentMap; }
            set { this.environmentMap = value; }
        }
        [Category("Appereance")]
        public RGBColor BackgroundColor {
            get { return this.backgroundColor; }
            set { this.backgroundColor = value; }
        }
        [Category("Shadow")]
        public bool IsShadowActive {
            get { return this.isShadowActive; }
            set { this.isShadowActive = value; }
        }
        [Category("Shadow")]
        public bool IsSoftShadowActive {
            get { return this.isSoftShadowActive; }
            set { this.isSoftShadowActive = value; }
        }
        [Category("Shadow")]
        public int SoftShadowSamples {
            get { return this.softShadowSamples; }
            set { this.softShadowSamples = value; }
        }
        [Category("Viewer"), TypeConverter(typeof(ExpandableObjectConverter))]
        public Sampler Sampler {
            get { return this.sampler; }
            set { this.sampler = value; }
        }
        [Category("Viewer")]
        public int GlossySamples
        {
            get { return this.glossySamples; }
            set { this.glossySamples = value; }
        }
        [XmlIgnore, Category("Viewer")]
        public Shader Shader {
            get { return this.shader; }
            set { this.shader = value; }
        }
        [XmlIgnore, Category("Viewer")]
        public RenderStrategy RenderStrategy {
            get { return this.renderStrategy; }
            set { this.renderStrategy = value; }
        }
        [Category("Appereance")]
        public RGBColor IAmb {
            get { return this.iAmb; }
            set { this.iAmb = value; }
        }
        [Category("Collections")]
        public NameableCollection<Light> Lights {
            get { return this.lights; }
            set {
                if(value != null) {
                    this.lights = value;
                }
            }
        }
        [Category("Collections")]
        public NameableCollection<Material> Materials {
            get { return this.materials; }
            set {
                if(value != null) {
                    this.materials = value;
                }
            }
        }
        [XmlIgnore, Category("Camera"), TypeConverter(typeof(ExpandableObjectConverter)),
         Editor(typeof(DefaultCameraEditor), typeof(UITypeEditor))]
        public Camera DefaultCamera {
            get {
                if(!String.IsNullOrEmpty(this.defaultCameraName) && this.cameras.ContainsName(this.defaultCameraName) ){
                    return this.cameras[this.defaultCameraName];
                } else{
                    PinholeCamera camera = new PinholeCamera();
                    this.cameras.Add(camera);
                    this.DefaultCameraName = camera.Name;
                    return camera;
                }
            }
            set {
                if(value != null) {
                    this.defaultCameraName = value.Name;
                }
            }
        }
        //public Camera DefaultCamera {
        //    get {
        //        if(this.defaultCamera == null && this.defaultCameraName == null) {
        //            PinholeCamera camera = new PinholeCamera();
        //            this.cameras.Add(camera);
        //            this.DefaultCamera = camera;
        //        }
        //        return this.defaultCamera;
        //    }
        //    set {
        //        if(value != null) {
        //            this.defaultCamera = value;
        //            this.defaultCameraName = value.Name;
        //        }
        //    }
        //}
        [Browsable(false), Category("Camera"), XmlElement("DefaultCamera")]
        public string DefaultCameraName {
            get { return this.defaultCameraName; }
            set
            {
                this.defaultCameraName = value;
                //if(!String.IsNullOrEmpty(this.defaultCameraName) && this.cameras != null && this.cameras.ContainsName(this.defaultCameraName)){
                //    this.defaultCamera = this.cameras[this.defaultCameraName];
                //}
            }
        }
        [Category("Collections")]
        public NameableCollection<Camera> Cameras {
            get { return this.cameras; }
            set {
                if(value != null) {
                    this.cameras = value;
                }
            }
        }
        [Category("Collections")]
        public NameableCollection<Primitive> Primitives {
            get { return this.primitives; }
            set {
                if(value != null) {
                    this.primitives = value;
                    this.AccelerationStructure = new KDTreePrimitiveManager(this.primitives);
                }
            }
        }
        [XmlIgnore]
        public IntersectableAccelerationStructure<Primitive> AccelerationStructure {
            get { return this.accelerationStructure; }
            set {
                this.accelerationStructure = value;
                this.accelerationStructure.AccelerationUnits = this.primitives;
            }
        }
        [Category("Appereance"), DefaultValue(1.0f)]
        public float RefractIndex {
            get { return this.refractIndex; }
            set { this.refractIndex = value > 0.0f ? value : 1; }
        }

        #region IIntersectable Members
        public bool FindIntersection(Ray ray, out Intersection intersection) {
            //this.AccelerationStructure.Optimize();
            return this.AccelerationStructure.FindIntersection(ray, out intersection);
        }
        public bool Visible {
            get { return this.visible; }
            set { this.visible = value; }
        }
        #endregion

        #region INameable Members
        public int Compare(INameable x, INameable y) {
            return x.Name.CompareTo(y.Name);
        }
        #endregion

        public void Save(string sceneName) {
            ObjectXMLSerializer<Scene>.Save(this, sceneName, SerializedFormats.Document);
        }
        public static Scene Load(string scenePath) {
            return ObjectXMLSerializer<Scene>.Load(scenePath, SerializedFormats.Document);
        }
    }
}