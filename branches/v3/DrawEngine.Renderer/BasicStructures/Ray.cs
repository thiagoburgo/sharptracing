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
using System.Runtime.InteropServices;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.BasicStructures {
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Ray : ITransformable3D {
        private Vector3D direction;
        private Vector3D invertedDirection;
        public Point3D Origin;
        public float PrevRefractIndex;
        public IPrimitive PrevPrimitive;

        public Ray(Point3D origin, Vector3D direction) {
            this.direction = direction;
            this.direction.Normalize();
            this.invertedDirection.X = 1.0f / this.direction.X;
            this.invertedDirection.Y = 1.0f / this.direction.Y;
            this.invertedDirection.Z = 1.0f / this.direction.Z;
            this.Origin = origin;
            this.PrevRefractIndex = -1;
            this.PrevPrimitive = null;
        }

        public Vector3D Direction {
            get { return this.direction; }
            set {
                this.direction = value;
                this.direction.Normalize();
                //this.invertedDirection.X = this.direction.X != 0f ? 1.0f / this.direction.X : 1f;
                //this.invertedDirection.Y = this.direction.Y != 0f ? 1.0f / this.direction.Y : 1f;
                //this.invertedDirection.Z = this.direction.Z != 0f ? 1.0f / this.direction.Z : 1f;
                this.invertedDirection.X = 1.0f / this.direction.X;
                this.invertedDirection.Y = 1.0f / this.direction.Y;
                this.invertedDirection.Z = 1.0f / this.direction.Z;
            }
        }

        public Vector3D InvertedDirection {
            get { return this.invertedDirection; }
        }

        #region ITransformable3D Members BUGADO

        public void Rotate(float angle, Vector3D axis) {
            this.Origin.Rotate(angle, axis);
            this.direction.Rotate(angle, axis);
            this.Direction = this.direction;
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
            this.Direction = direction * factor;
        }

        public void Translate(float tx, float ty, float tz) {
            this.Origin.Translate(tx, ty, tz);
            this.direction.Translate(tx, ty, tz);
            this.Direction = this.direction;
        }

        public void Translate(Vector3D translateVector) {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }

        #endregion

        public override string ToString() {
            return "[O: " + this.Origin + " D: " + this.direction + "]";
        }
    }
}