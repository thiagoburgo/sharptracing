//#define Culling
using System;
using System.ComponentModel;
using System.Windows.Forms;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.RenderObjects
{
    [Serializable]
    public class Triangle : Primitive, ITransformable3D
    {
        public BarycentricCoordinate CurrentBarycentricCoordinate;
        private Vector3D edge12;
        private Vector3D edge13;
        private Vector3D edge23;
        private Vector3D normal;
        public Vector3D NormalOnVertex1, NormalOnVertex2, NormalOnVertex3;
        private Point3D vertex1, vertex2, vertex3;
        public Triangle() : this(new Point3D(-20, 0, 0), new Point3D(20, 0, 0), new Point3D(0, 0, 20)) {}
        public Triangle(Point3D vertex1, Point3D vertex2, Point3D vertex3)
        {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.vertex3 = vertex3;
            this.AdjustTriangle();
        }
        public Vector3D Normal
        {
            get { return this.normal; }
        }
        [RefreshProperties(RefreshProperties.All)]
        public Point3D Vertex1
        {
            get { return this.vertex1; }
            set
            {
                this.vertex1 = value;
                this.AdjustTriangle();
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public Point3D Vertex2
        {
            get { return this.vertex2; }
            set
            {
                this.vertex2 = value;
                this.AdjustTriangle();
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public Point3D Vertex3
        {
            get { return this.vertex3; }
            set
            {
                this.vertex3 = value;
                this.AdjustTriangle();
            }
        }
        public Vector3D Edge12
        {
            get { return this.edge12; }
        }
        public Vector3D Edge23
        {
            get { return this.edge23; }
        }
        public Vector3D Edge31
        {
            get { return this.edge13; }
        }

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisX(float angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisY(float angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisZ(float angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void Scale(float factor)
        {
            this.vertex1.Scale(factor);
            this.vertex2.Scale(factor);
            this.vertex3.Scale(factor);
            this.AdjustTriangle();
        }
        public void Translate(float tx, float ty, float tz)
        {
            this.vertex1.Translate(tx, ty, tz);
            this.vertex2.Translate(tx, ty, tz);
            this.vertex3.Translate(tx, ty, tz);
            this.AdjustTriangle();
        }
        public void Translate(Vector3D translateVector)
        {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        private void AdjustTriangle()
        {
            this.edge12 = (this.vertex2 - this.vertex1);
            this.edge23 = (this.vertex3 - this.vertex2);
            this.edge13 = (this.vertex3 - this.vertex1);
            this.normal = this.edge12 ^ this.edge13;
            this.normal.Normalize();
            //if(MathUtil.NearZero(this.normal.X, 1.0e-6) && MathUtil.NearZero(this.normal.Y, 1.0e-6) && MathUtil.NearZero(this.normal.Z, 1.0e-6)) {
            //    MessageBox.Show("FUDEU NORMAL TRIANGULO");
            //}
        }
        public override bool FindIntersection(Ray ray, out Intersection intersect)
        {
            intersect = new Intersection();
            Vector3D tvec, pvec, qvec;
            float det, inv_det, u, v, t;
            /* begin calculating determinant - also used to calculate U parameter */
            pvec = ray.Direction ^ this.edge13;
            /* if determinant is near zero, ray lies in plane of triangle */
            det = this.edge12 * pvec;
#if TEST_CULL           
        /* define TEST_CULL if culling is desired */
           if (det < 0.000001f)
              return false;

           /* calculate distance from vert0 to ray origin */
           tvec = ray.Origin - vertex1;

           /* calculate U parameter and test bounds */
           u = tvec * pvec;
           if (u < 0.0f || u > det)
              return false;

           /* prepare to test V parameter */
           qvec = tvec ^edge12;

            /* calculate V parameter and test bounds */
           v = ray.Direction * qvec;
           if (v < 0.0f || u + v > det)
              return false;

           /* calculate t, scale parameters, ray intersects triangle */
           t = edge13 * qvec;
           inv_det = 1.0f / det;
           t *= inv_det;
           u *= inv_det;
           v *= inv_det;
#else
            /* the non-culling branch */
            if(det > -0.000001f && det < 0.000001f){
                return false;
            }
            inv_det = 1.0f / det;
            /* calculate distance from vert0 to ray origin */
            tvec = ray.Origin - this.vertex1;
            /* calculate U parameter and test bounds */
            u = (tvec * pvec) * inv_det;
            if(u < 0.0f || u > 1.0f){
                return false;
            }
            /* prepare to test V parameter */
            qvec = tvec ^ this.edge12;
            /* calculate V parameter and test bounds */
            v = (ray.Direction * qvec) * inv_det;
            if(v < 0.0f || u + v > 1.0f){
                return false;
            }
            /* calculate t, ray intersects triangle */
            t = (this.edge13 * qvec) * inv_det;
            if(t < 0.01){
                return false;
            }
#endif
            intersect.TMin = t;
            //if (ray.Direction * this.normal > 0)
            //{
            //    this.normal.Flip();
            //    //ray.Direction.Flip();
            //}
            intersect.Normal = this.normal;
            intersect.HitPoint = ray.Origin + (t * ray.Direction);
            this.CurrentBarycentricCoordinate = new BarycentricCoordinate(1 - (u + v), u, v);
            intersect.HitPrimitive = this;
            if(this.material != null && this.material.IsTexturized){
                //int widthTex = this.material.Texture.Width - 1;
                //int heightTex = this.material.Texture.Height - 1;
                //this.material.Color = this.material.Texture.GetPixel((int)(u * widthTex), (int)(v * heightTex));
                this.currentTextureCoordinate.U = u;
                this.currentTextureCoordinate.V = v;
            }
            return true;
        }
        private static bool SameSide(Point3D p1, Point3D p2, Point3D a, Point3D b)
        {
            Vector3D cp1 = (b - a ^ p1 - a);
            Vector3D cp2 = (b - a ^ p2 - a);
            if(cp1 * cp2 >= 0){
                return true;
            } else{
                return false;
            }
        }
        public bool PointInTriangle(Point3D p)
        {
            if(SameSide(p, this.vertex1, this.vertex2, this.vertex3)
               && SameSide(p, this.vertex2, this.vertex1, this.vertex3)
               && SameSide(p, this.vertex3, this.vertex1, this.vertex2)){
                return true;
            } else{
                return false;
            }
        }
        public override Vector3D NormalOnPoint(Point3D pointInPrimitive)
        {
            return this.normal;
        }
        public override bool IsInside(Point3D point)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #region Fast triangle-box overlapping
        //private static Vector3D halfVector;
        //private static Vector3D normal = new Vector3D();
        private static Vector3D e0, e1, e2;
        private static float max;
        private static float min;
        private static float p0, p1, p2, rad;
        private static Vector3D v0, v1, v2;
        private static Vector3D vmax;
        private static Vector3D vmin;
        public override bool IsOverlap(BoundBox bb)
        {
            float d, fex, fey, fez;
            v0 = (this.Vertex1 - bb.Center);
            v1 = (this.Vertex2 - bb.Center);
            v2 = (this.Vertex3 - bb.Center);
            //e0 = (this.Vertex2 - this.Vertex1);
            //e1 = (this.Vertex3 - this.Vertex2);
            //e2 = (this.Vertex1 - this.Vertex3);
            e0 = this.Edge12;
            e1 = this.Edge23;
            e2 = this.Edge31;
            fex = Math.Abs(e0.X);
            fey = Math.Abs(e0.Y);
            fez = Math.Abs(e0.Z);
            if(!AXISTEST_X01(e0.Z, e0.Y, fez, fey, bb.HalfVector)){
                return false;
            }
            if(!AXISTEST_Y02(e0.Z, e0.X, fez, fex, bb.HalfVector)){
                return false;
            }
            if(!AXISTEST_Z12(e0.Y, e0.X, fey, fex, bb.HalfVector)){
                return false;
            }
            fex = Math.Abs(e1.X);
            fey = Math.Abs(e1.Y);
            fez = Math.Abs(e1.Z);
            if(!AXISTEST_X01(e1.Z, e1.Y, fez, fey, bb.HalfVector)){
                return false;
            }
            if(!AXISTEST_Y02(e1.Z, e1.X, fez, fex, bb.HalfVector)){
                return false;
            }
            if(!AXISTEST_Z0(e1.Y, e1.X, fey, fex, bb.HalfVector)){
                return false;
            }
            fex = Math.Abs(e2.X);
            fey = Math.Abs(e2.Y);
            fez = Math.Abs(e2.Z);
            if(!AXISTEST_X2(e2.Z, e2.Y, fez, fey, bb.HalfVector)){
                return false;
            }
            if(!AXISTEST_Y1(e2.Z, e2.X, fez, fex, bb.HalfVector)){
                return false;
            }
            if(!AXISTEST_Z12(e2.Y, e2.X, fey, fex, bb.HalfVector)){
                return false;
            }
            /* test in X-direction */
            FindMinMax(v0.X, v1.X, v2.X);
            if(min > bb.HalfVector.X || max < -bb.HalfVector.X){
                return false;
            }
            /* test in Y-direction */
            FindMinMax(v0.Y, v1.Y, v2.Y);
            if(min > bb.HalfVector.Y || max < -bb.HalfVector.Y){
                return false;
            }
            /* test in Z-direction */
            FindMinMax(v0.Z, v1.Z, v2.Z);
            if(min > bb.HalfVector.Z || max < -bb.HalfVector.Z){
                return false;
            }
            /* test if the box intersects the plane of the triangle */
            //normal = (e0 ^ e1);
            d = -this.Normal * v0;
            return PlaneBoxOverlap(this.Normal, bb.HalfVector, d);
        }
        private static void FindMinMax(float x0, float x1, float x2)
        {
            min = max = x0;
            if(x1 < min){
                min = x1;
            }
            if(x1 > max){
                max = x1;
            }
            if(x2 < min){
                min = x2;
            }
            if(x2 > max){
                max = x2;
            }
        }
        private static bool PlaneBoxOverlap(Vector3D normal, Vector3D halfVector, float d)
        {
            if(normal.X > 0.0f){
                vmin.X = -halfVector.X;
                vmax.X = halfVector.X;
            } else{
                vmin.X = halfVector.X;
                vmax.X = -halfVector.X;
            }
            if(normal.Y > 0.0f){
                vmin.Y = -halfVector.Y;
                vmax.Y = halfVector.Y;
            } else{
                vmin.Y = halfVector.Y;
                vmax.Y = -halfVector.Y;
            }
            if(normal.Z > 0.0f){
                vmin.Z = -halfVector.Z;
                vmax.Z = halfVector.Z;
            } else{
                vmin.Z = halfVector.Z;
                vmax.Z = -halfVector.Z;
            }
            if((normal * vmin) + d > 0.0f){
                return false;
            }
            return ((normal * vmax) + d >= 0.0f);
        }
        /*======================== X-tests ========================*/
        private static bool AXISTEST_X01(float a, float b, float fa, float fb, Vector3D halfVector)
        {
            p0 = a * v0.Y - b * v0.Z;
            p2 = a * v2.Y - b * v2.Z;
            if(p0 < p2){
                min = p0;
                max = p2;
            } else{
                min = p2;
                max = p0;
            }
            rad = fa * halfVector.Y + fb * halfVector.Z;
            return !(min > rad || max < -rad);
        }
        private static bool AXISTEST_X2(float a, float b, float fa, float fb, Vector3D halfVector)
        {
            p0 = a * v0.Y - b * v0.Z;
            p1 = a * v1.Y - b * v1.Z;
            if(p0 < p1){
                min = p0;
                max = p1;
            } else{
                min = p1;
                max = p0;
            }
            rad = fa * halfVector.Y + fb * halfVector.Z;
            return !(min > rad || max < -rad);
        }
        /*======================== Y-tests ========================*/
        private static bool AXISTEST_Y02(float a, float b, float fa, float fb, Vector3D halfVector)
        {
            p0 = -a * v0.X + b * v0.Z;
            p2 = -a * v2.X + b * v2.Z;
            if(p0 < p2){
                min = p0;
                max = p2;
            } else{
                min = p2;
                max = p0;
            }
            rad = fa * halfVector.X + fb * halfVector.Z;
            return !(min > rad || max < -rad);
        }
        private static bool AXISTEST_Y1(float a, float b, float fa, float fb, Vector3D halfVector)
        {
            p0 = -a * v0.X + b * v0.Z;
            p1 = -a * v1.X + b * v1.Z;
            if(p0 < p1){
                min = p0;
                max = p1;
            } else{
                min = p1;
                max = p0;
            }
            rad = fa * halfVector.X + fb * halfVector.Z;
            return !(min > rad || max < -rad);
        }
        /*======================== Z-tests ========================*/
        private static bool AXISTEST_Z12(float a, float b, float fa, float fb, Vector3D halfVector)
        {
            p1 = a * v1.X - b * v1.Y;
            p2 = a * v2.X - b * v2.Y;
            if(p2 < p1){
                min = p2;
                max = p1;
            } else{
                min = p1;
                max = p2;
            }
            rad = fa * halfVector.X + fb * halfVector.Y;
            return !(min > rad || max < -rad);
        }
        private static bool AXISTEST_Z0(float a, float b, float fa, float fb, Vector3D halfVector)
        {
            p0 = a * v0.X - b * v0.Y;
            p1 = a * v1.X - b * v1.Y;
            if(p0 < p1){
                min = p0;
                max = p1;
            } else{
                min = p1;
                max = p0;
            }
            rad = fa * halfVector.X + fb * halfVector.Y;
            return !(min > rad || max < -rad);
        }
        #endregion
    }
}