using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.PhotonMapping;

namespace DrawEngine.Renderer.Lights
{
    [XmlInclude(typeof(SpotLight)), XmlInclude(typeof(PointLight)), XmlInclude(typeof(AreaLight)), Serializable]
    public abstract class Light : ITransformable3D, INameable, IPhotonSource
    {
        protected RGBColor color;
        private int maxPhotons = 500000;
        private string name;
        protected Point3D position;
        protected Light()
        {
            this.color = RGBColor.White;
            this.position = new Point3D(0, 100, 0);
        }
        protected Light(RGBColor intensity, Point3D position)
        {
            this.color = intensity;
            this.position = position;
        }
        public RGBColor Color
        {
            get { return this.color; }
            set { this.color = value; }
        }
        public Point3D Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

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
        public int Compare(INameable x, INameable y)
        {
            return x.Name.CompareTo(y.Name);
        }
        #endregion

        #region IPhotonSource Members
        public abstract IEnumerable<Photon> GeneratePhotons();
        public int MaxPhotons
        {
            get { return this.maxPhotons; }
            set { this.maxPhotons = value; }
        }
        #endregion

        #region ITransformable3D Members
        public abstract void Rotate(float angle, Vector3D axis);
        public abstract void RotateAxisX(float angle);
        public abstract void RotateAxisY(float angle);
        public abstract void RotateAxisZ(float angle);
        public abstract void Scale(float factor);
        public abstract void Translate(float tx, float ty, float tz);
        public abstract void Translate(Vector3D translateVector);
        #endregion

        public abstract float GetColorFactor(Vector3D pointToLight);
        public override string ToString()
        {
            return this.name + "[Position: " + this.position + ", Color: " + this.color + "]";
        }
    }
}