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
using System.ComponentModel;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.RenderObjects {
    [Serializable]
    public class Quadrilatero : Polygon, ITransformable3D {
        private bool calcDimensions;
        private float height;
        private float width;
        public Quadrilatero() : this(Point3D.Zero, Vector3D.UnitY, 100, 100) { }
        public Quadrilatero(Point3D center, Vector3D normal, float width, float height) {
            this.calcDimensions = false;
            this.width = width;
            this.height = height;
            this.center = center;
            this.Normal = normal;
        }
        public Quadrilatero(Point3D vertex1, Point3D vertex2, Point3D vertex3, Point3D vertex4) {
            this.calcDimensions = true;
            this.vertices = new NotifyList<Point3D> { vertex1, vertex2, vertex3, vertex4 };
        }

        protected override bool IsCoplanar {
            get { return true; }
        }
        public Point3D Center {
            get { return base.center; }
            set {
                base.center = value;
                this.Preprocess();
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public float Height {
            get { return this.height; }
            set {
                if(value > 0) {
                    this.height = value;
                    this.Preprocess();
                }
                else {
                    throw new ArgumentOutOfRangeException("Height", "The parameter must be great than Zero!");
                }
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public float Width {
            get { return this.width; }
            set {
                if(value > 0) {
                    this.width = value;
                    this.Preprocess();
                }
                else {
                    throw new ArgumentOutOfRangeException("Width", "The parameter must be great than Zero!");
                }
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public Point3D Vertex1 {
            get { return this.vertices[0]; }
            set {
                this.vertices[0] = value;
                this.Preprocess();
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public Point3D Vertex2 {
            get { return this.vertices[1]; }
            set {
                this.vertices[1] = value;
                this.Preprocess();
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public Point3D Vertex3 {
            get { return this.vertices[2]; }
            set {
                this.vertices[2] = value;
                this.Preprocess();
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public Point3D Vertex4 {
            get { return this.vertices[3]; }
            set {
                this.vertices[3] = value;
                this.Preprocess();
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public Vector3D Normal {
            get { return this.normal; }
            set {
                this.normal = value;
                this.normal.Normalize();
                this.Preprocess();
            }
        }

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis) {
            this.center.Rotate(angle, axis);
            this.Preprocess();
        }
        public void RotateAxisX(float angle) {
            this.Rotate(angle, Vector3D.UnitX);
        }
        public void RotateAxisY(float angle) {
            this.Rotate(angle, Vector3D.UnitY);
        }
        public void RotateAxisZ(float angle) {
            this.Rotate(angle, Vector3D.UnitZ);
        }
        public void Scale(float factor) {
            this.width = this.width * factor;
            this.height = this.height * factor;
            this.Preprocess();
        }
        public void Translate(float tx, float ty, float tz) {
            this.center.Translate(tx, ty, tz);
            this.Preprocess();
        }
        public void Translate(Vector3D translateVector) {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        public override void Preprocess() {
            this.vertices.NotificationsEnabled = false;
            Vector3D edge12;
            Vector3D edge23;
            if(this.calcDimensions) {
                edge12 = this.Vertex2 - this.Vertex1;
                edge23 = this.Vertex3 - this.Vertex2;
                this.width = edge12.Length;
                this.height = edge23.Length;
            }
            else {
                Vector3D.Orthonormalize(this.normal, out edge23, out edge12);
                if(this.vertices.Count == 0) {
                    this.vertices = new NotifyList<Point3D> { Point3D.Zero, Point3D.Zero, Point3D.Zero, Point3D.Zero };
                }

                this.vertices[0] = this.center + edge12 * this.width * 0.5f + edge23 * -this.height * 0.5f;
                this.vertices[1] = this.center + edge12 * this.width * 0.5f + edge23 * this.height * 0.5f;
                this.vertices[2] = this.center + edge12 * -this.width * 0.5f + edge23 * this.height * 0.5f;
                this.vertices[3] = this.center + edge12 * -this.width * 0.5f + edge23 * -this.height * 0.5f;
            }
            this.d = -(this.normal.X * this.vertices[0].X) - (this.normal.Y * this.vertices[0].Y)
                     - (this.normal.Z * this.vertices[0].Z);
        }
        //public bool FindIntersection(Ray ray) {
        //    float NRd = (this.normal * ray.Direction);
        //    if(NRd == 0.0f) {
        //        return false;
        //    }
        //    float tMin = (ray.Origin * this.normal) - this.coefQuadrangle;
        //    tMin = -tMin * 1.0f / NRd;
        //    if(tMin < 0.0001f) {
        //        return false;
        //    }
        //    Point3D hitPoint = ray.Origin + (tMin * ray.Direction);
        //    //FIM INTERSECCAO COM O PLANO
        //    float hitDotNorm12 = (hitPoint * this.normal12);
        //    if(hitDotNorm12 < this.coef12) {
        //        return false;
        //    }
        //    float hitDotNorm23 = (hitPoint * this.normal23);
        //    if(hitDotNorm23 < this.coef23) {
        //        return false;
        //    }
        //    float hitDotNorm34 = (hitPoint * this.normal34);
        //    if(hitDotNorm34 < this.coef34) {
        //        return false;
        //    }
        //    float hitDotNorm41 = (hitPoint * this.normal41);
        //    if(hitDotNorm41 < this.coef41) {
        //        return false;
        //    }
        //    return true;
        //}
    }
}