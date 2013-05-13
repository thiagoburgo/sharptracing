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
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Cameras {
    [XmlInclude(typeof (PinholeCamera)), XmlInclude(typeof (SphericalCamera)), XmlInclude(typeof (ThinLensCamera)),
     Serializable]
    public abstract class Camera : ITransformable3D, INameable {
        private float aspect;
        protected float au, av;
        protected OrthoNormalBasis basis;
        protected Point3D eye; //LF
        private float fov;
        private Point3D lookAt; //LA
        private string name;
        protected float resX;
        protected float resY;
        private Vector3D up; //VUP
        protected Camera() : this(new Point3D(0, 50, -150), Point3D.Zero, Vector3D.UnitY, 0.5f, 512f, 512f) {}

        protected Camera(Point3D eye, Point3D lookAt, Vector3D up, float fov, float resX, float resY) {
            this.ResX = resX;
            this.ResY = resY;
            this.aspect = resX / resY;
            this.eye = eye;
            this.Fov = fov;
            this.up = up;
            this.LookAt = lookAt;
            this.basis = OrthoNormalBasis.MakeFromWV(eye - lookAt, up);
            this.au = (float) Math.Tan(fov * 0.5f);
            this.av = this.au * 1.0f / this.aspect;
        }

        public Vector3D ViewUp {
            get { return this.up; }
            set {
                this.up = value;
                this.basis = OrthoNormalBasis.MakeFromWV(this.eye - this.lookAt, this.up);
            }
        }

        public float Fov {
            get { return this.fov; }
            set {
                if (value > 0.0f && value <= Math.PI) {
                    this.fov = value;
                    this.au = (float) Math.Tan(this.fov * 0.5f);
                    this.av = this.au * 1.0f / this.aspect;
                }
            }
        }

        public Point3D LookAt {
            get { return this.lookAt; }
            set {
                this.lookAt = value;
                this.basis = OrthoNormalBasis.MakeFromWV(this.eye - this.lookAt, this.up);
            }
        }

        public Point3D Eye {
            get { return this.eye; }
            set {
                this.eye = value;
                if (value.Z.NearZero()) {
                    value.Z = float.MinValue;
                }
                this.basis = OrthoNormalBasis.MakeFromWV(value - this.lookAt, this.up);
            }
        }

        public float ResX {
            get { return this.resX; }
            set {
                if (value > 0) {
                    this.resX = value;
                    this.aspect = this.resX / this.resY;
                    this.av = this.au * 1.0f / this.aspect;
                } else {
                    throw new ArgumentException("The value must be greater than ZERO!");
                }
            }
        }

        public float ResY {
            get { return this.resY; }
            set {
                if (value > 0) {
                    this.resY = value;
                    this.aspect = this.resX / this.resY;
                    this.av = this.au * 1.0f / this.aspect;
                } else {
                    throw new ArgumentException("The value must be greater than ZERO!");
                }
            }
        }

        #region INameable Members

        public event NameChangedEventHandler OnNameChanged;
        public event NameChangingEventHandler OnNameChanging;

        public string Name {
            get { return this.name; }
            set {
                if (!String.IsNullOrEmpty(value)) {
                    if (this.OnNameChanging != null) {
                        CancelNameChageEventArgs cancel = new CancelNameChageEventArgs(value);
                        this.OnNameChanging(this, cancel);
                        if (cancel.Cancel) {
                            throw new ArgumentException("Mudança de nome cancelada!");
                        }
                    }
                    string oldName = this.name;
                    this.name = value;
                    if (this.OnNameChanged != null) {
                        this.OnNameChanged(this, oldName);
                    }
                }
            }
        }

        public int Compare(INameable x, INameable y) {
            return x.Name.CompareTo(y.Name);
        }

        #endregion

        #region ITransformable3D Members

        public void Rotate(float angle, Vector3D axis) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RotateAxisX(float angle) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RotateAxisY(float angle) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RotateAxisZ(float angle) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Scale(float factor) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Translate(float tx, float ty, float tz) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Translate(Vector3D translateVector) {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public Ray CreateRayFromScreen(PointF pointOnScreen) {
            return CreateRayFromScreen(pointOnScreen.X, pointOnScreen.Y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract Ray CreateRayFromScreen(float x, float y);

        public override string ToString() {
            if (!String.IsNullOrEmpty(this.name)) {
                return this.name;
            }
            return this.GetType().Name;
        }
    }
}