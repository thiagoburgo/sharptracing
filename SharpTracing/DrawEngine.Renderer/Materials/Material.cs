using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.Serialization;
using System.Windows.Forms.Design;
using System.Xml.Serialization;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.Shaders;

namespace DrawEngine.Renderer.Materials
{
    [XmlInclude(typeof(PhongMaterial)), XmlInclude(typeof(CookTorranceMaterial)), Serializable]
    public abstract class Material : INameable, IDeserializationCallback
    {
        private RGBColor diffuseColor;
        private bool isTexturized;
        private float kAmb; /*Ambiental coefficient [0..1]*/
        private float kDiff; /*Diffuse coefficient [0..1]*/
        private float kSpec; /*Specular coefficient [0..1]*/
        private float kTrans; /*Coeficiente de transmissao*/
        private string name;
        private float glossy; /*Glossy factor*/
        private float refractIndex; /*Index of refraction*/
        private float absorptivity;
        private float shiness; //For phong
        private RGBColor specularColor;
        private Texture texture;
        protected Material()
        {
            this.glossy = 0.1f;
            this.kDiff = 0.5f;
            this.kSpec = 0.5f;
            this.kTrans = 0.0f;
            this.kAmb = 0.1f;
            this.diffuseColor = RGBColor.White;
            this.specularColor = RGBColor.White;
            this.refractIndex = 1.51f;
            this.shiness = 64;
            this.isTexturized = false;
        }
        protected Material(float kdiff, float kspec, float kamb, float refractIndex, float ktrans, float glossy, float shiness,
                           RGBColor color) : this(kdiff, kspec, kamb, refractIndex, ktrans, glossy, shiness, new Texture())
        {
            this.diffuseColor = color;
        }
        protected Material(float kdiff, float kspec, float kamb, float refractIndex, float ktrans, float glossy, float shiness,
                           Texture texture)
        {
            this.KDiff = kdiff;
            this.KSpec = kspec;
            this.KTrans = ktrans;
            this.KAmb = kamb;
            this.diffuseColor = RGBColor.White;
            this.specularColor = RGBColor.White;
            this.RefractIndex = refractIndex;
            this.Glossy = glossy;
            this.Shiness = shiness;
            if(!texture.IsLoaded){
                this.isTexturized = false;
            } else{
                this.texture = texture;
                this.isTexturized = true;
            }
        }
        public bool IsTexturized
        {
            get { return this.isTexturized; }
            set { this.isTexturized = value; }
        }
        public bool IsTransparent
        {
            get
            {
                if(this.kTrans > 0.0f){
                    return true;
                }
                return false;
            }
        }
        public bool IsReflective
        {
            get
            {
                if(this.kSpec > 0.0f){
                    return true;
                }
                return false;
            }
        }
        //[Browsable(false)]        
        [ReadOnly(true), XmlIgnore]
        public Texture Texture
        {
            get { return this.texture; }
            set { this.texture = value; }
        }
        [Browsable(true), Editor(typeof(FileNameEditor), typeof(UITypeEditor)), RefreshProperties(RefreshProperties.All)
        ]
        public String TexturePath
        {
            get { return this.texture.TexturePath; }
            set { this.texture = new Texture(value); }
        }
        // Serializes the 'Picture' Bitmap to XML.
        //[XmlElement("Texture")]
        //[Browsable(false)]
        //public byte[] TextureByteArray
        //{
        //    get
        //    {
        //        if (texture.TextureImage != null)
        //        {
        //            TypeConverter BitmapConverter =
        //                TypeDescriptor.GetConverter(texture.TextureImage.GetType());
        //            return (byte[])
        //                   BitmapConverter.ConvertTo(texture.TextureImage, typeof(byte[]));
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    set
        //    {
        //        if (value != null && value.Length > 0)
        //        {
        //            texture.TextureImage = new Bitmap(new MemoryStream(value));
        //        }
        //        else
        //        {
        //            texture.TextureImage = null;
        //        }
        //    }
        //}        
        public float KDiff
        {
            get { return this.kDiff; }
            set { this.kDiff = value; }
        }
        public float KSpec
        {
            get { return this.kSpec; }
            set { this.kSpec = value; }
        }
        public float KAmb
        {
            get { return this.kAmb; }
            set { this.kAmb = value; }
        }
        public float KTrans
        {
            get { return this.kTrans; }
            set { this.kTrans = value; }
        }
        public float RefractIndex
        {
            get { return this.refractIndex; }
            set { this.refractIndex = value; }
        }
        public float Shiness
        {
            get { return this.shiness; }
            set { this.shiness = value; }
        }
        public RGBColor DiffuseColor
        {
            get { return this.diffuseColor; }
            set { this.diffuseColor = value; }
        }
        public RGBColor SpecularColor
        {
            get { return this.specularColor; }
            set { this.specularColor = value; }
        }

        #region IDeserializationCallback Members
        public void OnDeserialization(object sender)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region INameable Members
        public event NameChangedEventHandler OnNameChanged;
        public event NameChangingEventHandler OnNameChanging;
        public string Name
        {
            get { return this.name; }
            set
            {
                if(!String.IsNullOrEmpty(value)){
                    if(this.OnNameChanging != null){
                        CancelNameChageEventArgs cancel = new CancelNameChageEventArgs(value);
                        this.OnNameChanging(this, cancel);
                        if(cancel.Cancel){
                            throw new ArgumentException("Mudança de nome cancelada!");
                        }
                    }
                    string oldName = this.name;
                    this.name = value;
                    if(this.OnNameChanged != null){
                        this.OnNameChanged(this, oldName);
                    }
                }
            }
        }
        public float Glossy
        {
            get { return this.glossy; }
            set { this.glossy = value; }
        }
        public float Absorptivity
        {
            get { return this.absorptivity; }
            set { this.absorptivity = value; }
        }
        public int Compare(INameable x, INameable y)
        {
            return x.Name.CompareTo(y.Name);
        }
        #endregion

        public abstract Shader CreateShader(Scene scene);
        public override string ToString()
        {
            return this.Name;
        }
        public Material Copy()
        {
            Material ret = (Material)this.MemberwiseClone();
            if(!String.IsNullOrEmpty(this.texture.TexturePath)){
                ret.texture.TexturePath = this.texture.TexturePath;
            }
            return ret;
        }
    }
}