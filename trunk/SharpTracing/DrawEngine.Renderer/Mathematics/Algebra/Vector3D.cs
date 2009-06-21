using System;
using System.ComponentModel;
using System.Drawing.Design;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.Mathematics.Algebra.Design;
using Component=DrawEngine.Renderer.Algebra.Component;

namespace DrawEngine.Renderer.Mathematics.Algebra
{
    [Editor(typeof(VectorOrPointEditor), typeof(UITypeEditor)), TypeConverter(typeof(VectorOrPointTypeConverter))]
    public struct Vector3D : ITransformable3D, IEquatable<Vector3D> {
        public static readonly Vector3D UnitX = new Vector3D(1, 0, 0);
        public static readonly Vector3D UnitY = new Vector3D(0, 1, 0);
        public static readonly Vector3D UnitZ = new Vector3D(0, 0, 1);
        public static readonly Vector3D Zero = new Vector3D(0, 0, 0);
        public float X, Y, Z;
        public Vector3D(float x, float y, float z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public float Length {
            get {
                double temp = (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
                if(temp > 0) {
                    return (float)Math.Sqrt(temp);
                }
                else {
                    return 0;
                }
            }
        }
        public float Length2 {
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
                float z = Math.Abs(this.Z);
                float y = Math.Abs(this.Y);
                float x = Math.Abs(this.X);
                return (x > y
                                ? (x > z ? new Dominant(Component.X, this.X) : new Dominant(Component.Z, this.Z))
                                : (y > z ? new Dominant(Component.Y, this.Y) : new Dominant(Component.Z, this.Z)));
            }
        }
        public float this[int index] {
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
        public float this[Component index] {
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
        public void Rotate(float angle, Vector3D axis) {
            float sin = (float)Math.Sin(angle);
            float one_cos = (float)(1.0 - Math.Cos(angle));
            axis.Normalize();
            this = this
                   *
                   new Matrix3D(1 + one_cos * ((axis.X * axis.X) - 1), (one_cos * axis.X * axis.Y) - axis.Z * sin,
                                (one_cos * axis.X * axis.Z) + axis.Y * sin, (one_cos * axis.X * axis.Y) + axis.Z * sin,
                                1 + one_cos * ((axis.Y * axis.Y) - 1), (one_cos * axis.Y * axis.Z) - axis.X * sin,
                                (one_cos * axis.X * axis.Z) - axis.Y * sin, (one_cos * axis.Y * axis.Z) + axis.X * sin,
                                1 + one_cos * ((axis.Z * axis.Z) - 1));
        }
        public void RotateAxisX(float angle) {
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);
            this = this * new Matrix3D(1, 0, 0, 0, cos, -sin, 0, sin, cos);
        }
        public void RotateAxisY(float angle) {
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);
            this = this * new Matrix3D(cos, 0, sin, 0, 1, 0, -sin, 0, cos);
        }
        public void RotateAxisZ(float angle) {
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);
            this = this * new Matrix3D(cos, -sin, 0, sin, cos, 0, 0, 0, 1);
        }
        public void Scale(float factor) {
            this = this * factor;
        }
        public void Translate(float tx, float ty, float tz) {
            this.Translate(new Vector3D(tx, ty, tz));
        }
        public void Translate(Vector3D translateVector) {
            this = this + translateVector;
        }
        #endregion

        public static Vector3D CreateRandomVector() {
            System.Random rnd = new System.Random();
            //Random rnd = new Random();
            float r1 = (float)rnd.NextDouble();
            float r2 = (float)rnd.NextDouble();
            float twoPIr1 = (float)(2 * Math.PI * r1);
            float oneMinusR2 = 1 - r2;
            float x = (float)(2 * Math.Cos(twoPIr1) * Math.Sqrt(r2 * oneMinusR2));
            float y = (float)(2 * Math.Sin(twoPIr1) * Math.Sqrt(r2 * oneMinusR2));
            float z = 1 - 2 * r2;
            return new Vector3D(x, y, z);
        }

        public static void SampleDisk(out float u, out float v) {
            bool done = false;
            System.Random rnd = new System.Random();
            double coord1 = 0, coord2 = 0;
            // Keep working until we get a random point inside unit circle
            while(!done) {
                coord1 = rnd.NextDouble();
                coord2 = rnd.NextDouble();
                // Test if u,v are inside unit circle
                // centered at <0.5,0.5> with radius 0.5
                double x = coord1 - 0.5;
                double y = coord2 - 0.5f;
                double dist2 = x * x + y * y;
                if(dist2 <= 0.25) {
                    // Yeah, inside unit circle
                    done = true;
                }
            }

            // Return result
            u = (float)coord1;
            v = (float)coord2;
        }

        public static Vector3D GlossPerturbRay(Vector3D toPertube) {

            Vector3D uDir, vDir;// IN - u-dir on perpendicular disc to ray
            // IN - v-dir on perpendicular disc to ray
            // Get Random Point on unit disc
            float discU, discV;
            SampleDisk(out discU, out discV);
            Vector3D.Orthonormalize(toPertube, out uDir, out vDir);
            const float g_GlossRadius = 0.05f;

            // Scale to our actual disc size
            float len = toPertube.Length;
            discU *= g_GlossRadius * len;
            discV *= g_GlossRadius * len;

            // Calculate new perturbed direction
            Vector3D pDir = toPertube + (discU * uDir) + (discV * vDir);

            // Success
            return pDir;
        }

        public void Normalize() {
            float len = this.Length;
            if(len != 1.0f) {
                if(len == 0.0f) {
                    this.X = 0.0f;
                    this.Y = 0.0f;
                    this.Z = 0.0f;
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
            float div = (v2 * v2);
            if(div != 0) {
                v1 = v1 - (((v1 * v2) * 1 / div) * v2);
            }
        }
        // Returns a righthanded orthonormal basis to complement vector w
        public static void Orthonormalize(Vector3D w, out Vector3D u, out Vector3D v) {
            if(w.X > 0.5f || w.X < -0.5f || w.Y > 0.5f || w.Y < -0.5f) {
                u.X = w.Y;
                u.Y = -w.X;
                u.Z = 0.0f;
            }
            else {
                u.X = 0.0f;
                u.Y = w.Z;
                u.Z = -w.Y;
            }
            u.Normalize();
            v = w ^ u;
            v.Normalize();
        }
        public static void Orthonormalize(Vector3D u, out Vector3D v) {
            if(u.X > 0.5f || u.X < -0.5f || u.Y > 0.5f || u.Y < -0.5f) {
                v.X = u.Y;
                v.Y = -u.X;
                v.Z = 0.0f;
            }
            else {
                v.X = 0.0f;
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
            return new Vector3D((v1.X + v2.X), (v1.Y + v2.Y), (v1.Z + v2.Z));
        }
        /// <summary>
        /// Operador unário que retorna um NOVO vetor com direcao invertida
        /// </summary>
        /// <param name="v1">Vetor para inversao</param>
        /// <returns>o valor do vetor invertido</returns>
        public static Vector3D operator -(Vector3D v1) {
            return new Vector3D(-v1.X, -v1.Y, -v1.Z);
        }
        /// <summary>
        /// Diminue dois vetores
        /// </summary>
        /// <param name="v1">Vetor1</param>
        /// <param name="v2">Vetor2</param>
        /// <returns>Retorno a subtracao entre Vetor1 e Vetor1</returns>
        public static Vector3D operator -(Vector3D v1, Vector3D v2) {
            return new Vector3D((v1.X - v2.X), (v1.Y - v2.Y), (v1.Z - v2.Z));
        }
        /// <summary>
        /// Produto entre um vetor e um escalar
        /// </summary>
        /// <param name="vector">Vetor para aumento da magnitude</param>
        /// <param name="scalar">Escalar que aumenta a magnitude do vetor</param>        
        /// <returns>Vetor com a magnitude aumentada</returns>	
        public static Vector3D operator *(Vector3D vector, float scalar) {
            return new Vector3D((vector.X * scalar), (vector.Y * scalar), (vector.Z * scalar));
        }
        /// <summary>
        /// Produto entre um vetor e um escalar
        /// </summary>
        /// <param name="scalar">Escalar que aumenta a magnitude do vetor</param>
        /// <param name="vector">Vetor para aumento da magnitude</param>
        /// <returns>Vetor com a magnitude aumentada</returns>
        public static Vector3D operator *(float scalar, Vector3D vector) {
            return new Vector3D((vector.X * scalar), (vector.Y * scalar), (vector.Z * scalar));
        }
        /// <summary>
        /// Divisão de um vetor por um escalar
        /// </summary>
        /// <param name="vector">Vetor que terá sua magnitude será dividida</param>
        /// <param name="scalar">Divisor da magnitude do vetor</param>
        /// <returns>Vetor dividido</returns>
        public static Vector3D operator /(Vector3D vector, float scalar) {
            return new Vector3D((vector.X / scalar), (vector.Y / scalar), (vector.Z / scalar));
        }
        /// <summary>
        /// Calcula o produto escalar entre dois vetores
        /// </summary>
        /// <param name="v1">Vetor1</param>
        /// <param name="v2">Vetor2</param>
        /// <returns>Retorna o tamalho da projecao do Vetor1 sobre o Vetor2</returns>
        public static float operator *(Vector3D v1, Vector3D v2) {
            return ((v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z));
        }
        /// <summary>
        /// Produto vetorial entre dois vetores
        /// </summary>
        /// <param name="v1">Vetor1</param>
        /// <param name="v2">Vetor2</param>
        /// <returns>Retorna um vetor que é perpendicular aos outros dois da entrada</returns>
        public static Vector3D operator ^(Vector3D v1, Vector3D v2) {
            return new Vector3D((v1.Y * v2.Z) - (v1.Z * v2.Y), (v1.Z * v2.X) - (v1.X * v2.Z),
                                (v1.X * v2.Y) - (v1.Y * v2.X));
        }
        public static implicit operator float[](Vector3D vec) {
            return new float[] { vec.X, vec.Y, vec.Z };
        }
        public static implicit operator Vector3D(float[] vec) {
            return new Vector3D(vec[0], vec[1], vec[2]);
        }
        public Point3D ToPoint3D() {
            return new Point3D(this.X, this.Y, this.Z);
        }
        public override string ToString() {
            return this.X + "; " + this.Y + "; " + this.Z;
        }
        /// <summary>
        /// Calculates the surface normal of a surface that includes the three specified points in counter-clockwise order
        /// </summary>
        /// <param name="v1">First Vertex</param>
        /// <param name="v2">Second Vertex</param>
        /// <param name="v3">Third Vertex</param>
        /// <returns>A vector representing the surface normal</returns>
        public static Vector3D Normal(Point3D p1, Point3D p2, Point3D p3) {
            Vector3D v12 = p2 - p1;
            Vector3D v23 = p3 - p2;
            /*v12.Cross(v23);
			v12.Normalize();
			return v12;*/
            //cross product is in left-handed coordinates, but we want CCW points, which is right-handed
            //so return opposite of usual order
            v23 = v23 ^ v12;
            v23.Normalize();
            return v23;
        }
    }
}