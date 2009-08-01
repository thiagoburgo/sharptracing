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
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.RenderObjects
{
    /// <summary>
    /// Ax2 + 2Bxy + 2Cxz + 2Dx + Ey2 +2Fyz +26y + Hz2 + 2Iz +J = 0
    /// </summary>
    [Serializable]
    public class Quadric : Primitive
    {
        private float a;
        protected float b;
        protected float c;
        protected float d;
        protected float e;
        protected float f;
        protected float g;
        protected float h;
        protected float i;
        protected float j;
        public Quadric() : this(36.0f, 9.0f, -4.0f, 0.0f, 0.0f, 0f, 0f, 0.0f, 0.0f, -36.0f) {}
        public Quadric(float a, float b, float c, float d, float e, float f, float g, float h, float i, float j)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
            this.g = g;
            this.h = h;
            this.i = i;
            this.j = j;
            this.boundBox = new BoundBox(new Point3D(-20, -20, -20), new Point3D(-20, -20, -20));
        }
        public float A
        {
            get { return this.a; }
            set { this.a = value; }
        }
        public float B
        {
            get { return this.b; }
            set { this.b = value; }
        }
        public float C
        {
            get { return this.c; }
            set { this.c = value; }
        }
        public float D
        {
            get { return this.d; }
            set { this.d = value; }
        }
        public float E
        {
            get { return this.e; }
            set { this.e = value; }
        }
        public float F
        {
            get { return this.f; }
            set { this.f = value; }
        }
        public float G
        {
            get { return this.g; }
            set { this.g = value; }
        }
        public float H
        {
            get { return this.h; }
            set { this.h = value; }
        }
        public float I
        {
            get { return this.i; }
            set { this.i = value; }
        }
        public float J
        {
            get { return this.j; }
            set { this.j = value; }
        }
        public override bool FindIntersection(Ray ray, out Intersection intersect)
        {
            intersect = new Intersection();
            Vector3D rayDir = ray.Direction;
            Point3D rayPoint = ray.Origin;
            //this->boundingBox->intersect(line, point, normal, u, v);
            //if(!(*point))
            //{
            //    *point = 0;
            //    *normal = 0;
            //    return;
            //}
            //@see: http://www.siggraph.org/education/materials/HyperGraph/raytrace/rtinter4.htm
            float Aq_1 = this.a * (rayDir.X * rayDir.X);
            float Aq_2 = this.b * (rayDir.Y * rayDir.Y);
            float Aq_3 = this.c * (rayDir.Z * rayDir.Z);
            float Aq_4 = this.d * rayDir.X * rayDir.Y;
            float Aq_5 = this.e * rayDir.X * rayDir.Z;
            float Aq_6 = this.f * rayDir.Y * rayDir.Z;
            float Bq_1 = 2 * this.a * rayPoint.X * rayDir.X;
            float Bq_2 = 2 * this.b * rayPoint.Y * rayDir.Y;
            float Bq_3 = 2 * this.c * rayPoint.Z * rayDir.Z;
            float Bq_4 = this.d * ((rayPoint.X * rayDir.Y) + (rayPoint.Y * rayDir.X));
            float Bq_5 = this.e * rayPoint.X * rayDir.Z;
            float Bq_6 = this.f * ((rayPoint.Y * rayDir.Z) + (rayDir.Y * rayPoint.Z));
            float Bq_7 = this.g * rayDir.X;
            float Bq_8 = this.h * rayDir.Y;
            float Bq_9 = this.i * rayDir.Z;
            float Cq_1 = this.a * (rayPoint.X * rayPoint.X);
            float Cq_2 = this.b * (rayPoint.Y * rayPoint.Y);
            float Cq_3 = this.c * (rayPoint.Z * rayPoint.Z);
            float Cq_4 = this.d * rayPoint.X * rayPoint.Y;
            float Cq_5 = this.e * rayPoint.X * rayPoint.Z;
            float Cq_6 = this.f * rayPoint.Y * rayPoint.Z;
            float Cq_7 = this.g * rayPoint.X;
            float Cq_8 = this.h * rayPoint.Y;
            float Cq_9 = this.i * rayPoint.Z;
            float Aq = Aq_1 + Aq_2 + Aq_3 + Aq_4 + Aq_5 + Aq_6;
            float Bq = Bq_1 + Bq_2 + Bq_3 + Bq_4 + Bq_5 + Bq_6 + Bq_7 + Bq_8 + Bq_9;
            float Cq = Cq_1 + Cq_2 + Cq_3 + Cq_4 + Cq_5 + Cq_6 + Cq_7 + Cq_8 + Cq_9 + this.j;
            //quadratic equation:   Aqt² + Bqt + Cq = 0
            float t, t0, t1, tMin, tMax;
            if(EquationSolver.SolveQuadric(Aq, Bq, Cq, out t0, out t1) == 0){
                return false;
            }
            tMin = (float)Math.Min(t0, t1);
            tMax = (float)Math.Max(t0, t1);
            if(tMin > 0.01f){
                t = tMin;
            } else if(tMax > 0.01f){
                t = tMax;
            } else{
                return false;
            }
            Point3D tempP = rayPoint + rayDir * t;
            //if (!this.boundBox.IsInside(tempP) && tMax > 0)
            //{
            //    tempP = rayPoint + rayDir * tMax;
            //}
            //bool isInsideBox = this->boundingBox->checkCollision(Box(tempP.getX(), tempP.getY(), tempP.getZ(),
            //														 tempP.getX(), tempP.getY(), tempP.getZ()));
            //if(!isInsideBox && t == tMin && tMax > 0)
            //{
            //    tempP = (*rayPoint) + ((*rayDir) * tMax);
            //    isInsideBox = this->boundingBox->checkCollision(Box(tempP.getX(), tempP.getY(), tempP.getZ(),
            //                                                         tempP.getX(), tempP.getY(), tempP.getZ()));
            //}
            //if(isInsideBox)
            //{
            //*point = new Point(tempP.getX(), tempP.getY(), tempP.getZ());
            //calculating normal
            float nX1 = 2 * this.a * tempP.X;
            float nX2 = this.d * tempP.Y;
            float nX3 = this.e * tempP.Z;
            float nY1 = 2 * this.b * tempP.Y;
            float nY2 = this.d * tempP.X;
            float nY3 = this.f * tempP.Z;
            float nZ1 = 2 * this.c * tempP.Z;
            float nZ2 = this.e * tempP.X;
            float nZ3 = this.f * tempP.Y;
            float normalX = nX1 + nX2 + nX3 + this.g;
            float normalY = nY1 + nY2 + nY3 + this.h;
            float normalZ = nZ1 + nZ2 + nZ3 + this.i;
            intersect.Normal = new Vector3D(normalX, normalY, normalZ);
            intersect.Normal.Normalize();
            intersect.HitPoint = tempP;
            intersect.HitPrimitive = this;
            intersect.TMin = t;
            return true;
            //}
        }
        //public override bool FindIntersection(out DrawEngine.Renderer.Algebra.Intersection intersect, DrawEngine.Renderer.Algebra.Ray ray)
        //{
        //    intersect = new Intersection();
        //    float t = 0; //t-value wich will lead us to the intersection point
        //    // with formula f(X,Y,Z) = AX^2 + 2BXY + 2CXZ + 2DX + EY^2 + 2FYZ + 2GY + HZ^2 + 2IZ + J
        //    float xd = ray.Direction.X;  // xd :: x . x-component, d . direction
        //    float yd = ray.Direction.Y;
        //    float zd = ray.Direction.Z;
        //    float xo = ray.Origin.X;
        //    float yo = ray.Origin.Y;
        //    float zo = ray.Origin.Z;
        //    float discrim;
        //    // use (xo,yo,zo) + t(xd,yd,zd) in the equation of the quadric
        //    float aq, bq, cq; // transform to equation aq * t^2 + bq * t + cq = 0
        //    aq = xd * (a * xd + b2 * yd + c2 * zd) + yd * (e * yd + f2 * zd) + zd * zd * h;
        //    bq = xd * (a2 * xo + b2 * yo + c2 * zo + d2) + yd * (e2 * yo + f2 * zo + b2 * xo + g2) + zd * (c2 * xo + f2 * yo + h2 * zo + i2);
        //    cq = xo * (a * xo + b2 * yo + c2 * zo + d2) + yo * (e * yo + f2 * zo + g2) + zo * (h * zo + i2) + j;
        //    discrim = bq * bq - 4 * aq * cq;
        //    if (discrim < 0)
        //        return false;
        //    if (discrim == 0)
        //        t = -bq / (2 * aq);
        //    else
        //    {
        //        float sqrtdis = (float)Math.Sqrt(discrim);
        //        float t1, t2;
        //        t1 = (-bq + sqrtdis) / (2 * aq);
        //        t2 = (-bq - sqrtdis) / (2 * aq);
        //        if (t1 < t2 && t1 > 0)
        //            t = t1;
        //        else if (t2 > 0)
        //            t = t2;
        //        else
        //            return false;
        //    }
        //    Point3D point = ray.Origin + ray.Direction * t;
        //    // normal on point == calculate by using partial derivatives
        //    float comp1 = point.X * a2 + point.Y * b2 + point.Z * c2 + d2;
        //    float comp2 = point.X * b2 + point.Y * e2 + point.Z * f2 + g2;
        //    float comp3 = point.X * c2 + point.Y * f2 + point.Z * h2 + i2;
        //    Vector3D norm = new Vector3D(comp1, comp2, comp3);
        //    //hitpoint.p = point;
        //    //hitpoint.normal = norm;
        //    //hitpoint.normal.Normalize();
        //    //hitpoint.time = t;
        //    intersect.HitPrimitive = this;
        //    intersect.HitPoint = point;
        //    intersect.Normal = norm;
        //    intersect.Normal.Normalize();
        //    intersect.TMin = t;
        //    return true;
        //}
        public override bool IsInside(Point3D point)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override Vector3D NormalOnPoint(Point3D pointInPrimitive)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override bool IsOverlap(BoundBox boundBox)
        {
            throw new NotImplementedException();
        }
    }
}