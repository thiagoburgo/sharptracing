using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.RenderObjects;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.Materials;

namespace DrawEngine.Renderer.Mesh
{
    public struct MeshTriangle : ITransformable3D, IPrimitive
    {
        public MeshVertex Vertex1;
        public MeshVertex Vertex2;
        public MeshVertex Vertex3;
        private Material material;
        private bool visible;
        public BarycentricCoordinate CurrentBarycentricCoordinate;

        public MeshTriangle(MeshVertex vertex1, MeshVertex vertex2, MeshVertex vertex3)
        {
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
            this.Vertex3 = vertex3;
            this.visible = true;
            this.CurrentBarycentricCoordinate = BarycentricCoordinate.Zero;
            this.material = null;
        }

        public Vector3D Normal { 
            get { return Vector3D.Normal(this.Vertex1.Position, this.Vertex2.Position, this.Vertex3.Position); } 
        }
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
            this.Vertex1.Position.Scale(factor);
            this.Vertex2.Position.Scale(factor);
            this.Vertex3.Position.Scale(factor);
        }
        public void Translate(double tx, double ty, double tz)
        {
            this.Vertex1.Position.Translate(tx, ty, tz);
            this.Vertex2.Position.Translate(tx, ty, tz);
            this.Vertex3.Position.Translate(tx, ty, tz);
        }
        public void Translate(Vector3D translateVector)
        {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        #region IIntersectable Members

        //From http://jgt.akpeters.com/papers/GuigueDevillers03/ray_triangle_intersection.html
        public bool FindIntersection(Ray ray, out Intersection intersect)
        {
            intersect = new Intersection();
            Vector3D vect0, vect1, nvect;
            double det, inv_det;

            vect0 = this.Vertex2.Position - this.Vertex1.Position;
            vect1 = this.Vertex3.Position - this.Vertex1.Position;
            Vector3D normalNonNormalized = vect0 ^ vect1;

            /* orientation of the ray with respect to the triangle's normal, 
               also used to calculate output parameters*/
            det = -(ray.Direction * normalNonNormalized);
#if TEST_CULL
            /* define TEST_CULL if culling is desired */
            if (det < 0.000001d) return false;

            /* calculate vector from ray origin to this.vertex1 */
            vect0 = this.Vertex1.Position - ray.Origin;
            /* vector used to calculate u and v parameters */
            nvect = ray.Direction ^ vect0;

            /* calculate vector from ray origin to this.vertex2*/
            vect1 = this.Vertex2.Position - ray.Origin;
            /* calculate unnormalized v parameter and test bounds */
            double v = -(vect1 * nvect);

            if (v < 0.0 || v > det) return false;

            /* calculate vector from ray origin to this.vertex3*/
            vect1 = this.Vertex3.Position - ray.Origin;
            /* calculate unnormalized v parameter and test bounds */
            double u = vect1 * nvect;

            if (u < 0.0 || u + v > det) return false;

            /* calculate unormalized t parameter */
            double t = -(vect0 * normalNonNormalized);

            inv_det = 1.0d / det;
            /* calculate u v t, ray intersects triangle */
            u = u * inv_det;
            v = v * inv_det;
            t = t * inv_det;

#else
            /* the non-culling branch */

            /* if determinant is near zero, ray is parallel to the plane of triangle */
            if (det > -0.000001d && det < 0.000001d) return false;

            /* calculate vector from ray origin to this.vertex1 */
            vect0 = this.Vertex1.Position - ray.Origin;

            /* normal vector used to calculate u and v parameters */
            nvect = ray.Direction ^ vect0;

            inv_det = 1.0d / det;
            /* calculate vector from ray origin to this.vertex2*/
            vect1 = this.Vertex2.Position - ray.Origin;

            /* calculate v parameter and test bounds */
            double v = -(vect1 * nvect) * inv_det;

            if (v < 0.0d || v > 1.0d) return false;

            /* calculate vector from ray origin to this.vertex3*/
            vect1 = this.Vertex3.Position - ray.Origin;
            /* calculate v parameter and test bounds */
            double u = (vect1 * nvect) * inv_det;

            if (u < 0.0d || u + v > 1.0d) return false;

            /* calculate t, ray intersects triangle */
            double t = -(vect0 * normalNonNormalized) * inv_det;
#endif

            //if (t < 100)
            //    return false;
            // return 1;

            //if (t < 100) //FIXME: the correct is t < 0, but dont work, why? tell me you! =P
            //{
            //    return false;
            //}
            if (t >= 0)
            {
                intersect.TMin = t;
                intersect.Normal = Vector3D.Normal(this.Vertex1.Position, this.Vertex2.Position, this.Vertex3.Position);
                intersect.HitPoint = ray.Origin + (t * ray.Direction);
                this.CurrentBarycentricCoordinate = new BarycentricCoordinate(1.0d - (u + v), u, v);
                intersect.HitPrimitive = this;
                return true;
            }
            return false;
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        #endregion

        #region IBoundBox Members
        public BoundBox BoundBox
        {
            get { return BoundBox.Zero; }
            set { }
        }
        public bool IsOverlap(BoundBox bb)
        {
            double d, fex, fey, fez;
            v0 = (this.Vertex1.Position - bb.Center);
            v1 = (this.Vertex2.Position - bb.Center);
            v2 = (this.Vertex3.Position - bb.Center);
            e0 = (this.Vertex2.Position - this.Vertex1.Position);
            e1 = (this.Vertex3.Position - this.Vertex2.Position);
            e2 = (this.Vertex1.Position - this.Vertex3.Position);
            //e0 = this.Edge1;
            //e1 = this.Edge3;
            //e2 = this.Edge2;
            fex = Math.Abs(e0.X);
            fey = Math.Abs(e0.Y);
            fez = Math.Abs(e0.Z);
            if (!AXISTEST_X01(e0.Z, e0.Y, fez, fey, bb.HalfVector))
            {
                return false;
            }
            if (!AXISTEST_Y02(e0.Z, e0.X, fez, fex, bb.HalfVector))
            {
                return false;
            }
            if (!AXISTEST_Z12(e0.Y, e0.X, fey, fex, bb.HalfVector))
            {
                return false;
            }
            fex = Math.Abs(e1.X);
            fey = Math.Abs(e1.Y);
            fez = Math.Abs(e1.Z);
            if (!AXISTEST_X01(e1.Z, e1.Y, fez, fey, bb.HalfVector))
            {
                return false;
            }
            if (!AXISTEST_Y02(e1.Z, e1.X, fez, fex, bb.HalfVector))
            {
                return false;
            }
            if (!AXISTEST_Z0(e1.Y, e1.X, fey, fex, bb.HalfVector))
            {
                return false;
            }
            fex = Math.Abs(e2.X);
            fey = Math.Abs(e2.Y);
            fez = Math.Abs(e2.Z);
            if (!AXISTEST_X2(e2.Z, e2.Y, fez, fey, bb.HalfVector))
            {
                return false;
            }
            if (!AXISTEST_Y1(e2.Z, e2.X, fez, fex, bb.HalfVector))
            {
                return false;
            }
            if (!AXISTEST_Z12(e2.Y, e2.X, fey, fex, bb.HalfVector))
            {
                return false;
            }
            /* test in X-direction */
            FindMinMax(v0.X, v1.X, v2.X);
            if (min > bb.HalfVector.X || max < -bb.HalfVector.X)
            {
                return false;
            }
            /* test in Y-direction */
            FindMinMax(v0.Y, v1.Y, v2.Y);
            if (min > bb.HalfVector.Y || max < -bb.HalfVector.Y)
            {
                return false;
            }
            /* test in Z-direction */
            FindMinMax(v0.Z, v1.Z, v2.Z);
            if (min > bb.HalfVector.Z || max < -bb.HalfVector.Z)
            {
                return false;
            }
            /* test if the box intersects the plane of the triangle */
            //normal = (e0 ^ e1);
            d = -this.Normal * v0;
            return PlaneBoxOverlap(this.Normal, bb.HalfVector, d);
        }
        #endregion

        #region Fast triangle-box overlapping
        private static Vector3D e0, e1, e2;
        private static double max;
        private static double min;
        private static double p0, p1, p2, rad;
        private static Vector3D v0, v1, v2;
        private static Vector3D vmax;
        private static Vector3D vmin;

        private static void FindMinMax(double x0, double x1, double x2)
        {
            min = max = x0;
            if (x1 < min)
            {
                min = x1;
            }
            if (x1 > max)
            {
                max = x1;
            }
            if (x2 < min)
            {
                min = x2;
            }
            if (x2 > max)
            {
                max = x2;
            }
        }
        private static bool PlaneBoxOverlap(Vector3D normal, Vector3D halfVector, double d)
        {
            if (normal.X > 0.0d)
            {
                vmin.X = -halfVector.X;
                vmax.X = halfVector.X;
            }
            else
            {
                vmin.X = halfVector.X;
                vmax.X = -halfVector.X;
            }
            if (normal.Y > 0.0d)
            {
                vmin.Y = -halfVector.Y;
                vmax.Y = halfVector.Y;
            }
            else
            {
                vmin.Y = halfVector.Y;
                vmax.Y = -halfVector.Y;
            }
            if (normal.Z > 0.0d)
            {
                vmin.Z = -halfVector.Z;
                vmax.Z = halfVector.Z;
            }
            else
            {
                vmin.Z = halfVector.Z;
                vmax.Z = -halfVector.Z;
            }
            if ((normal * vmin) + d > 0.0d)
            {
                return false;
            }
            return ((normal * vmax) + d >= 0.0d);
        }
        /*======================== X-tests ========================*/
        private static bool AXISTEST_X01(double a, double b, double fa, double fb, Vector3D halfVector)
        {
            p0 = a * v0.Y - b * v0.Z;
            p2 = a * v2.Y - b * v2.Z;
            if (p0 < p2)
            {
                min = p0;
                max = p2;
            }
            else
            {
                min = p2;
                max = p0;
            }
            rad = fa * halfVector.Y + fb * halfVector.Z;
            return !(min > rad || max < -rad);
        }
        private static bool AXISTEST_X2(double a, double b, double fa, double fb, Vector3D halfVector)
        {
            p0 = a * v0.Y - b * v0.Z;
            p1 = a * v1.Y - b * v1.Z;
            if (p0 < p1)
            {
                min = p0;
                max = p1;
            }
            else
            {
                min = p1;
                max = p0;
            }
            rad = fa * halfVector.Y + fb * halfVector.Z;
            return !(min > rad || max < -rad);
        }
        /*======================== Y-tests ========================*/
        private static bool AXISTEST_Y02(double a, double b, double fa, double fb, Vector3D halfVector)
        {
            p0 = -a * v0.X + b * v0.Z;
            p2 = -a * v2.X + b * v2.Z;
            if (p0 < p2)
            {
                min = p0;
                max = p2;
            }
            else
            {
                min = p2;
                max = p0;
            }
            rad = fa * halfVector.X + fb * halfVector.Z;
            return !(min > rad || max < -rad);
        }
        private static bool AXISTEST_Y1(double a, double b, double fa, double fb, Vector3D halfVector)
        {
            p0 = -a * v0.X + b * v0.Z;
            p1 = -a * v1.X + b * v1.Z;
            if (p0 < p1)
            {
                min = p0;
                max = p1;
            }
            else
            {
                min = p1;
                max = p0;
            }
            rad = fa * halfVector.X + fb * halfVector.Z;
            return !(min > rad || max < -rad);
        }
        /*======================== Z-tests ========================*/
        private static bool AXISTEST_Z12(double a, double b, double fa, double fb, Vector3D halfVector)
        {
            p1 = a * v1.X - b * v1.Y;
            p2 = a * v2.X - b * v2.Y;
            if (p2 < p1)
            {
                min = p2;
                max = p1;
            }
            else
            {
                min = p1;
                max = p2;
            }
            rad = fa * halfVector.X + fb * halfVector.Y;
            return !(min > rad || max < -rad);
        }
        private static bool AXISTEST_Z0(double a, double b, double fa, double fb, Vector3D halfVector)
        {
            p0 = a * v0.X - b * v0.Y;
            p1 = a * v1.X - b * v1.Y;
            if (p0 < p1)
            {
                min = p0;
                max = p1;
            }
            else
            {
                min = p1;
                max = p0;
            }
            rad = fa * halfVector.X + fb * halfVector.Y;
            return !(min > rad || max < -rad);
        }
        #endregion

        #region IPrimitive Members



        public Material Material
        {
            get { return material; }
            set { material = value; }
        }

        #endregion

      
    }
}
