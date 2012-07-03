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
using System.Drawing.Design;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.Mathematics.Algebra.Design;
using Component=DrawEngine.Renderer.Algebra.Component;
using System.Runtime.InteropServices;

namespace DrawEngine.Renderer.Mathematics.Algebra
{
    [Editor(typeof(VectorOrPointEditor), typeof(UITypeEditor)), TypeConverter(typeof(VectorOrPointTypeConverter))]
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector3D : ITransformable3D, IEquatable<Vector3D> {
        public static readonly Vector3D UnitX = new Vector3D(1, 0, 0);
        public static readonly Vector3D UnitY = new Vector3D(0, 1, 0);
        public static readonly Vector3D UnitZ = new Vector3D(0, 0, 1);
        public static readonly Vector3D Zero = new Vector3D(0, 0, 0);
        public double X, Y, Z;
        public Vector3D(double x, double y, double z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public double Length {
            get {
                double temp = (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
                if(temp > 0) {
                    return Math.Sqrt(temp);
                }
                else {
                    return 0;
                }
            }
        }
        public double Length2 {
            get { return (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z); }
        }
        public Vector3D Normalized {
            get {
                Vector3D v = this;
                v.Normalize();
                return v;
            }
        }
        public Vector3D Inverted {
            get { return -this; }
        }
        public Dominant Dominant {
            get {
                double z = Math.Abs(this.Z);
                double y = Math.Abs(this.Y);
                double x = Math.Abs(this.X);
                return (x > y
                                ? (x > z ? new Dominant(Component.X, this.X) : new Dominant(Component.Z, this.Z))
                                : (y > z ? new Dominant(Component.Y, this.Y) : new Dominant(Component.Z, this.Z)));
            }
        }
        public double this[int index] {
            get {
                int indMod3 = index % 3;
                if(indMod3 == 0) {
                    return this.X;
                }
                else if(indMod3 == 1) {
                    return this.Y;
                }
                else {
                    return this.Z;
                }
            }
            set {
                int indMod3 = index % 3;
                if(indMod3 == 0) {
                    this.X = value;
                }
                else if(indMod3 == 1) {
                    this.Y = value;
                }
                else {
                    this.Z = value;
                }
            }
        }
        public double this[Component index] {
            get {
                switch(index) {
                    case Component.X:
                        return this.X;
                    case Component.Y:
                        return this.Y;
                    case Component.Z:
                        return this.Z;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set {
                switch(index) {
                    case Component.X:
                        this.X = value;
                        break;
                    case Component.Y:
                        this.Y = value;
                        break;
                    case Component.Z:
                        this.Z = value;
                        break;
                }
            }
        }

        #region IEquatable<Vector3D> Members
        public bool Equals(Vector3D other) {
            return other.X == this.X && other.Y == this.Y && other.Z == this.Z;
        }
        #endregion

        #region ITransformable3D Members
        public void Rotate(double angle, Vector3D axis) {
            double sin = Math.Sin(angle);
            double one_cos = (1.0 - Math.Cos(angle));
            axis.Normalize();
            this = this
                   *
                   new Matrix3D(1 + one_cos * ((axis.X * axis.X) - 1), (one_cos * axis.X * axis.Y) - axis.Z * sin,
                                (one_cos * axis.X * axis.Z) + axis.Y * sin, (one_cos * axis.X * axis.Y) + axis.Z * sin,
                                1 + one_cos * ((axis.Y * axis.Y) - 1), (one_cos * axis.Y * axis.Z) - axis.X * sin,
                                (one_cos * axis.X * axis.Z) - axis.Y * sin, (one_cos * axis.Y * axis.Z) + axis.X * sin,
                                1 + one_cos * ((axis.Z * axis.Z) - 1));
        }
        public void RotateAxisX(double angle) {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            this = this * new Matrix3D(1, 0, 0, 0, cos, -sin, 0, sin, cos);
        }
        public void RotateAxisY(double angle) {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            this = this * new Matrix3D(cos, 0, sin, 0, 1, 0, -sin, 0, cos);
        }
        public void RotateAxisZ(double angle) {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            this = this * new Matrix3D(cos, -sin, 0, sin, cos, 0, 0, 0, 1);
        }
        public void Scale(double factor) {
            this = this * factor;
        }
        public void Translate(double tx, double ty, double tz) {
            this.Translate(new Vector3D(tx, ty, tz));
        }
        public void Translate(Vector3D translateVector) {
            this = this + translateVector;
        }
        #endregion

        public static Vector3D CreateRandomVector() {
            System.Random rnd = new System.Random();
            //Random rnd = new Random();
            double r1 = rnd.NextDouble();
            double r2 = rnd.NextDouble();
            double twoPIr1 = (2 * Math.PI * r1);
            double oneMinusR2 = 1 - r2;
            double x = (2 * Math.Cos(twoPIr1) * Math.Sqrt(r2 * oneMinusR2));
            double y = (2 * Math.Sin(twoPIr1) * Math.Sqrt(r2 * oneMinusR2));
            double z = 1 - 2 * r2;
            return new Vector3D(x, y, z);
        }
        public void Normalize() {
            double len = this.Length;
            if(len != 1.0d) {
                if(len == 0.0d) {
                    this.X = 0.0d;
                    this.Y = 0.0d;
                    this.Z = 0.0d;
                }
                else {
                    this.X = this.X * 1 / len;
                    this.Y = this.Y * 1 / len;
                    this.Z = this.Z * 1 / len;
                }
            }
        }
        public void Flip() {
            this.X = (-this.X);
            this.Y = (-this.Y);
            this.Z = (-this.Z);
        }
        public static bool operator ==(Vector3D v1, Vector3D v2) {
            return v1.Equals(v2);
        }
        public static bool operator !=(Vector3D v1, Vector3D v2) {
            return !v1.Equals(v2);
        }
        public static void Orthogonalize(ref Vector3D v1, Vector3D v2) {
            double div = (v2 * v2);
            if(div != 0) {
                v1 = v1 - (((v1 * v2) * 1 / div) * v2);
            }
        }
        // Returns a righthanded orthonormal basis to complement vector w
        public static void Orthonormalize(Vector3D w, out Vector3D u, out Vector3D v) {
            if(w.X > 0.5d || w.X < -0.5d || w.Y > 0.5d || w.Y < -0.5d) {
                u.X = w.Y;
                u.Y = -w.X;
                u.Z = 0.0d;
            }
            else {
                u.X = 0.0d;
                u.Y = w.Z;
                u.Z = -w.Y;
            }
            u.Normalize();
            v = w ^ u;
            v.Normalize();
        }
        public static void Orthonormalize(Vector3D u, out Vector3D v) {
            if(u.X > 0.5d || u.X < -0.5d || u.Y > 0.5d || u.Y < -0.5d) {
                v.X = u.Y;
                v.Y = -u.X;
                v.Z = 0.0d;
            }
            else {
                v.X = 0.0d;
                v.Y = u.Z;
                v.Z = -u.Y;
            }
            v.Normalize();
        }
        public static Vector3D operator |(Vector3D v1, Vector3D v2) {
            Orthogonalize(ref v1, v2);
            return v1;
        }
        /// <summary>
        /// Soma dois vetores
        /// </summary>
        /// <param name="v1">Vetor1</param>
        /// <param name="v2">Vetor2</param>
        /// <returns>Retorna um vetor que representa a soma de outros dois</returns>
        public static Vector3D operator +(Vector3D v1, Vector3D v2) {
            v1.X += v2.X;
            v1.Y += v2.Y;
            v1.Z += v2.Z;
            return v1;
            //return new Vector3D((v1.X + v2.X), (v1.Y + v2.Y), (v1.Z + v2.Z));
        }
        /// <summary>
        /// Operador unário que retorna um NOVO vetor com direcao invertida
        /// </summary>
        /// <param name="v1">Vetor para inversao</param>
        /// <returns>o valor do vetor invertido</returns>
        public static Vector3D operator -(Vector3D v1) {
            v1.X = -v1.X;
            v1.Y = -v1.Y;
            v1.Z = -v1.Z;
            return v1;
            //return new Vector3D(-v1.X, -v1.Y, -v1.Z);
        }
        /// <summary>
        /// Diminue dois vetores
        /// </summary>
        /// <param name="v1">Vetor1</param>
        /// <param name="v2">Vetor2</param>
        /// <returns>Retorno a subtracao entre Vetor1 e Vetor1</returns>
        public static Vector3D operator -(Vector3D v1, Vector3D v2) {
            v1.X -= v2.X;
            v1.Y -= v2.Y;
            v1.Z -= v2.Z;
            return v1;
            //return new Vector3D((v1.X - v2.X), (v1.Y - v2.Y), (v1.Z - v2.Z));
        }
        /// <summary>
        /// Produto entre um vetor e um escalar
        /// </summary>
        /// <param name="v1">Vetor para aumento da magnitude</param>
        /// <param name="scalar">Escalar que aumenta a magnitude do vetor</param>        
        /// <returns>Vetor com a magnitude aumentada</returns>	
        public static Vector3D operator *(Vector3D v1, double scalar) {
            v1.X *= scalar;
            v1.Y *= scalar;
            v1.Z *= scalar;
            return v1;
            //return new Vector3D((vector.X * scalar), (vector.Y * scalar), (vector.Z * scalar));
        }
        /// <summary>
        /// Produto entre um vetor e um escalar
        /// </summary>
        /// <param name="scalar">Escalar que aumenta a magnitude do vetor</param>
        /// <param name="v1">Vetor para aumento da magnitude</param>
        /// <returns>Vetor com a magnitude aumentada</returns>
        public static Vector3D operator *(double scalar, Vector3D v1) {
            v1.X *= scalar;
            v1.Y *= scalar;
            v1.Z *= scalar;
            return v1;
            //return new Vector3D((vector.X * scalar), (vector.Y * scalar), (vector.Z * scalar));
        }
        /// <summary>
        /// Divisão de um vetor por um escalar
        /// </summary>
        /// <param name="v1">Vetor que terá sua magnitude será dividida</param>
        /// <param name="scalar">Divisor da magnitude do vetor</param>
        /// <returns>Vetor dividido</returns>
        public static Vector3D operator /(Vector3D v1, double scalar) {
            v1.X /= scalar;
            v1.Y /= scalar;
            v1.Z /= scalar;
            return v1;
            //return new Vector3D((vector.X / scalar), (vector.Y / scalar), (vector.Z / scalar));
        }
        /// <summary>
        /// Calcula o produto escalar entre dois vetores
        /// </summary>
        /// <param name="v1">Vetor1</param>
        /// <param name="v2">Vetor2</param>
        /// <returns>Retorna o tamalho da projecao do Vetor1 sobre o Vetor2</returns>
        public static double operator *(Vector3D v1, Vector3D v2) {
            return ((v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z));
        }
        /// <summary>
        /// Produto vetorial entre dois vetores
        /// </summary>
        /// <param name="v1">Vetor1</param>
        /// <param name="v2">Vetor2</param>
        /// <returns>Retorna um vetor que é perpendicular aos outros dois da entrada</returns>
        public static Vector3D operator ^(Vector3D v1, Vector3D v2) {
            return new Vector3D((v1.Y * v2.Z) - (v1.Z * v2.Y),
                                (v1.Z * v2.X) - (v1.X * v2.Z),
                                (v1.X * v2.Y) - (v1.Y * v2.X));
        }
        public static implicit operator double[](Vector3D vec) {
            return new double[] { vec.X, vec.Y, vec.Z };
        }
        public static implicit operator Vector3D(double[] vec) {
            return new Vector3D(vec[0], vec[1], vec[2]);
        }
        public Point3D ToPoint3D() {
            return new Point3D(this.X, this.Y, this.Z);
        }
        public override string ToString() {
            return this.X + "; " + this.Y + "; " + this.Z;
        }
        /// <summary>
        /// Calculates the surface normal of a surface that includes the three specified points in 
        /// counter-clockwise order
        /// </summary>
        /// <param name="v1">First Vertex</param>
        /// <param name="v2">Second Vertex</param>
        /// <param name="v3">Third Vertex</param>
        /// <returns>A vector representing the surface normal</returns>
        public static Vector3D Normal(Point3D p1, Point3D p2, Point3D p3) {
            return Normal(p1, p2, p3, true);
        }
        /// <summary>
        /// Calculates the surface normal of a surface that includes the three specified points in 
        /// counter-clockwise order
        /// </summary>
        /// <param name="v1">First Vertex</param>
        /// <param name="v2">Second Vertex</param>
        /// <param name="v3">Third Vertex</param>
        /// <returns>A vector representing the surface normal</returns>
        public static Vector3D Normal(Point3D p1, Point3D p2, Point3D p3, bool normalized)
        {
            Vector3D v12 = p2 - p1;
            Vector3D v23 = p3 - p2;
            //cross product is in left-handed coordinates, but we want CCW points, which is right-handed
            //so return opposite of usual order
            v23 = v23 ^ v12;
            if (normalized)
            {
                v23.Normalize();
            }
            return v23;
        }

     
        /// <summary>
        /// Calculate the refracted vector 
        /// </summary>
        /// <param name="N">Normal vector.</param>
        /// <param name="I">Incoming vector.</param>
        /// <param name="T">Outcoming Refracted vector.</param>
        /// <param name="eta">eta = (Scene Index)/(Material Index) if ray generate outside material</param>
        /// <returns></returns>
        public static bool Refracted(Vector3D N, Vector3D I, out Vector3D T, double n1, double n2) {
            //double cosI = -(I * N);
            //double eta = n1 / n2;
            //double sinT2 = eta * eta * (1.0d - (cosI * cosI));
            //if(sinT2 > 1) {
            //    T = Vector3D.Zero;
            //    return false;
            //}
            //double cosT = Math.Sqrt(1.0 - sinT2);
            //T = (eta * I) + (eta * cosI - cosT) * N;
            ////T = ((eta * cosI - Math.Sqrt(cosT2)) * N) - (eta * I);
            //return true;
            
            double n = n1 / n2;
            double cosI = -(N*I);
            double sinT2 = n * n * (1.0d - cosI * cosI);
            if(sinT2 > 1.0){
                T = Vector3D.Zero;
                return false;
            }
            double cosT = Math.Sqrt(1.0 - sinT2);
            T = n * I + (n * cosI - cosT) * N;
            return true;
        }
        public static bool Refracted(Vector3D N, Vector3D I, out Vector3D T, double eta) {
            double cosI = -(I * N);
            double sinT2 = eta * eta * (1.0d - (cosI * cosI));
            if(sinT2 > 1) {
                T = Vector3D.Zero;
                return false;
            }
            double cosT = Math.Sqrt(1.0 - sinT2);
            T = (eta * I) + (eta * cosI - cosT) * N;
            //T = ((eta * cosI - Math.Sqrt(cosT2)) * N) - (eta * I);
            return true;
        }
        public static double FresnelBySchlick(Vector3D N, Vector3D I, double n1, double n2)
        {
            double r0 = (n1 - n2)/(n1 + n2);
            r0 *= r0;
            double cosI = -(N * I);
            if(n1 > n2){
                double eta = n1 / n2;
                double sinT2 = eta * eta * (1.0d - cosI * cosI);
                if(sinT2 > 1){
                    return 1;
                }
                cosI = Math.Sqrt(1.0 - sinT2);
            }
            double i = 1.0d - cosI;
            return r0 + (1.0d - r0) * i * i * i * i * i;
        }
        public static Vector3D Reflected(Vector3D N, Vector3D I) {
            double NI = N * -I;
            return (2 * (NI) * N) + I;
        }
        public static Vector3D ReflectedDiffuse(Vector3D N) {
            Random random = new Random(((int)DateTime.Now.Ticks) ^ 47);
            Vector3D randDir = new Vector3D(-1 + 2 * random.NextDouble(), -1 + 2 * random.NextDouble(),
                                            -1 + 2 * random.NextDouble());
            double dot = randDir * N;
            if(dot < 0.0d) {
                randDir.Flip();
            }
            return randDir;
        }
    }
}