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
using System.Xml.Serialization;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Cameras
{
    [XmlInclude(typeof(PinholeCamera)), XmlInclude(typeof(SphericalCamera)), XmlInclude(typeof(ThinLensCamera)), Serializable]
    public abstract class Camera : ITransformable3D, INameable
    {
        private double aspect;
        protected double au, av;
        protected OrthoNormalBasis basis;
        protected Point3D eye; //LF
        private double fov;
        private Point3D lookAt; //LA
        private string name;
        protected double resX;
        protected double resY;
        private Vector3D up; //VUP
        protected Camera() : this(new Point3D(0, 50, -150), Point3D.Zero, Vector3D.UnitY, 0.5d, 512d, 512d) {}
        protected Camera(Point3D eye, Point3D lookAt, Vector3D up, double fov, double resX, double resY)
        {
            this.ResX = resX;
            this.ResY = resY;
            this.aspect = resX / resY;
            this.eye = eye;
            this.Fov = fov;
            this.up = up;
            this.LookAt = lookAt;
            this.basis = OrthoNormalBasis.MakeFromWV(eye - lookAt, up);
            this.au = Math.Tan(fov * 0.5d);
            this.av = this.au * 1.0d / this.aspect;
        }
        public Vector3D ViewUp
        {
            get { return this.up; }
            set
            {
                this.up = value;
                this.basis = OrthoNormalBasis.MakeFromWV(this.eye - this.lookAt, this.up);
            }
        }
        public double Fov
        {
            get { return this.fov; }
            set
            {
                if(value > 0.0d && value <= Math.PI){
                    this.fov = value;
                    this.au = Math.Tan(this.fov * 0.5d);
                    this.av = this.au * 1.0d / this.aspect;
                }
            }
        }
        public Point3D LookAt
        {
            get { return this.lookAt; }
            set
            {
                this.lookAt = value;
                this.basis = OrthoNormalBasis.MakeFromWV(this.eye - this.lookAt, this.up);
            }
        }
        public Point3D Eye
        {
            get { return this.eye; }
            set
            {
                this.eye = value;
                if(value.Z == 0){
                    value.Z = double.MinValue;
                }
                this.basis = OrthoNormalBasis.MakeFromWV(value - this.lookAt, this.up);
            }
        }
        public double ResX
        {
            get { return this.resX; }
            set
            {
                if(value > 0){
                    this.resX = value;
                    this.aspect = this.resX / this.resY;
                    this.av = this.au * 1.0d / this.aspect;
                } else{
                    throw new ArgumentException("The value must be greater than ZERO!");
                }
            }
        }
        public double ResY
        {
            get { return this.resY; }
            set
            {
                if(value > 0){
                    this.resY = value;
                    this.aspect = this.resX / this.resY;
                    this.av = this.au * 1.0d / this.aspect;
                } else{
                    throw new ArgumentException("The value must be greater than ZERO!");
                }
            }
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

        #region ITransformable3D Members
        public void Rotate(double angle, Vector3D axis)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisX(double angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisY(double angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisZ(double angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void Scale(double factor)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void Translate(double tx, double ty, double tz)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void Translate(Vector3D translateVector)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        public Ray CreateRayFromScreen(PointF pointOnScreen)
        {
            return CreateRayFromScreen(pointOnScreen.X, pointOnScreen.Y);
        }
        public abstract Ray CreateRayFromScreen(double x, double y);
        public override string ToString()
        {
            if(!String.IsNullOrEmpty(this.name)){
                return this.name;
            }
            return this.GetType().Name;
        }
    }
}