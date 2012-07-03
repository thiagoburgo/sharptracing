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
        private double a;
        protected double b;
        protected double c;
        protected double d;
        protected double e;
        protected double f;
        protected double g;
        protected double h;
        protected double i;
        protected double j;
        public Quadric() : this(36.0d, 9.0d, -4.0d, 0.0d, 0.0d, 0d, 0d, 0.0d, 0.0d, -36.0d) {}
        public Quadric(double a, double b, double c, double d, double e, double f, double g, double h, double i, double j)
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
        public double A
        {
            get { return this.a; }
            set { this.a = value; }
        }
        public double B
        {
            get { return this.b; }
            set { this.b = value; }
        }
        public double C
        {
            get { return this.c; }
            set { this.c = value; }
        }
        public double D
        {
            get { return this.d; }
            set { this.d = value; }
        }
        public double E
        {
            get { return this.e; }
            set { this.e = value; }
        }
        public double F
        {
            get { return this.f; }
            set { this.f = value; }
        }
        public double G
        {
            get { return this.g; }
            set { this.g = value; }
        }
        public double H
        {
            get { return this.h; }
            set { this.h = value; }
        }
        public double I
        {
            get { return this.i; }
            set { this.i = value; }
        }
        public double J
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
            double Aq_1 = this.a * (rayDir.X * rayDir.X);
            double Aq_2 = this.b * (rayDir.Y * rayDir.Y);
            double Aq_3 = this.c * (rayDir.Z * rayDir.Z);
            double Aq_4 = this.d * rayDir.X * rayDir.Y;
            double Aq_5 = this.e * rayDir.X * rayDir.Z;
            double Aq_6 = this.f * rayDir.Y * rayDir.Z;
            double Bq_1 = 2 * this.a * rayPoint.X * rayDir.X;
            double Bq_2 = 2 * this.b * rayPoint.Y * rayDir.Y;
            double Bq_3 = 2 * this.c * rayPoint.Z * rayDir.Z;
            double Bq_4 = this.d * ((rayPoint.X * rayDir.Y) + (rayPoint.Y * rayDir.X));
            double Bq_5 = this.e * rayPoint.X * rayDir.Z;
            double Bq_6 = this.f * ((rayPoint.Y * rayDir.Z) + (rayDir.Y * rayPoint.Z));
            double Bq_7 = this.g * rayDir.X;
            double Bq_8 = this.h * rayDir.Y;
            double Bq_9 = this.i * rayDir.Z;
            double Cq_1 = this.a * (rayPoint.X * rayPoint.X);
            double Cq_2 = this.b * (rayPoint.Y * rayPoint.Y);
            double Cq_3 = this.c * (rayPoint.Z * rayPoint.Z);
            double Cq_4 = this.d * rayPoint.X * rayPoint.Y;
            double Cq_5 = this.e * rayPoint.X * rayPoint.Z;
            double Cq_6 = this.f * rayPoint.Y * rayPoint.Z;
            double Cq_7 = this.g * rayPoint.X;
            double Cq_8 = this.h * rayPoint.Y;
            double Cq_9 = this.i * rayPoint.Z;
            double Aq = Aq_1 + Aq_2 + Aq_3 + Aq_4 + Aq_5 + Aq_6;
            double Bq = Bq_1 + Bq_2 + Bq_3 + Bq_4 + Bq_5 + Bq_6 + Bq_7 + Bq_8 + Bq_9;
            double Cq = Cq_1 + Cq_2 + Cq_3 + Cq_4 + Cq_5 + Cq_6 + Cq_7 + Cq_8 + Cq_9 + this.j;
            //quadratic equation:   Aqt² + Bqt + Cq = 0
            double t, t0, t1, tMin, tMax;
            if(EquationSolver.SolveQuadric(Aq, Bq, Cq, out t0, out t1) == 0){
                return false;
            }
            tMin = Math.Min(t0, t1);
            tMax = Math.Max(t0, t1);
            if(tMin > 0.01d){
                t = tMin;
            } else if(tMax > 0.01d){
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
            double nX1 = 2 * this.a * tempP.X;
            double nX2 = this.d * tempP.Y;
            double nX3 = this.e * tempP.Z;
            double nY1 = 2 * this.b * tempP.Y;
            double nY2 = this.d * tempP.X;
            double nY3 = this.f * tempP.Z;
            double nZ1 = 2 * this.c * tempP.Z;
            double nZ2 = this.e * tempP.X;
            double nZ3 = this.f * tempP.Y;
            double normalX = nX1 + nX2 + nX3 + this.g;
            double normalY = nY1 + nY2 + nY3 + this.h;
            double normalZ = nZ1 + nZ2 + nZ3 + this.i;
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
        //    double t = 0; //t-value wich will lead us to the intersection point
        //    // with formula f(X,Y,Z) = AX^2 + 2BXY + 2CXZ + 2DX + EY^2 + 2FYZ + 2GY + HZ^2 + 2IZ + J
        //    double xd = ray.Direction.X;  // xd :: x . x-component, d . direction
        //    double yd = ray.Direction.Y;
        //    double zd = ray.Direction.Z;
        //    double xo = ray.Origin.X;
        //    double yo = ray.Origin.Y;
        //    double zo = ray.Origin.Z;
        //    double discrim;
        //    // use (xo,yo,zo) + t(xd,yd,zd) in the equation of the quadric
        //    double aq, bq, cq; // transform to equation aq * t^2 + bq * t + cq = 0
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
        //        double sqrtdis = Math.Sqrt(discrim);
        //        double t1, t2;
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
        //    double comp1 = point.X * a2 + point.Y * b2 + point.Z * c2 + d2;
        //    double comp2 = point.X * b2 + point.Y * e2 + point.Z * f2 + g2;
        //    double comp3 = point.X * c2 + point.Y * f2 + point.Z * h2 + i2;
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