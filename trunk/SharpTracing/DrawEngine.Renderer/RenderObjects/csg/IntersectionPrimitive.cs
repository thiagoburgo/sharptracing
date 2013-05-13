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
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.RenderObjects.CSG {
    [Serializable]
    public class IntersectionPrimitive : Primitive, IConstrutive {
        private Primitive basePrimitive;
        private Primitive operPrimitive;

        public IntersectionPrimitive(Primitive priBase, Primitive priDiff) {
            this.BasePrimitive = priBase;
            this.OperandPrimitive = priDiff;
            this.Name = "[" + priBase.Name + "] & [" + priDiff.Name + "]";
        }

        #region IConstrutive Members

        public Primitive BasePrimitive {
            get { return this.basePrimitive; }
            set {
                if (value != null) {
                    this.basePrimitive = value;
                } else {
                    throw new ArgumentNullException();
                }
            }
        }

        public Primitive OperandPrimitive {
            get { return this.operPrimitive; }
            set {
                if (value != null) {
                    this.operPrimitive = value;
                } else {
                    throw new ArgumentNullException();
                }
            }
        }

        #endregion

        public override bool FindIntersection(Ray ray, out Intersection intersect) {
            Intersection intersectTmp;
            if (this.basePrimitive.FindIntersection(ray, out intersect)) {
                if (this.operPrimitive.FindIntersection(ray, out intersectTmp)) {
                    if (intersect.TMin < intersectTmp.TMin) {
                        intersect = intersectTmp;
                    }
                }
                return this.basePrimitive.IsInside(intersect.HitPoint) &&
                       this.operPrimitive.IsInside(intersect.HitPoint);
            }
            return false;
        }

        public override bool IsInside(Point3D point) {
            return this.basePrimitive.IsInside(point) && this.operPrimitive.IsInside(point);
        }

        public override Vector3D NormalOnPoint(Point3D pointInPrimitive) {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool IsOverlap(BoundBox boundBox) {
            throw new NotImplementedException();
        }
    }
}