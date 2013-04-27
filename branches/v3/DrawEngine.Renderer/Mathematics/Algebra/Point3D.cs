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
using System.Runtime.InteropServices;
using DrawEngine.Renderer.Mathematics.Algebra.Design;

namespace DrawEngine.Renderer.Mathematics.Algebra {
    /// <summary>
    /// This class describe the point entity in three dimensional space 
    /// </summary>
    [Editor(typeof (VectorOrPointEditor), typeof (UITypeEditor)), TypeConverter(typeof (VectorOrPointTypeConverter))]
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Point3D : ITransformable3D, IEquatable<Point3D> {
        public float X;
        public float Y;
        public float Z;
        public static readonly Point3D Zero;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        public Point3D(float x, float y, float z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Retorna a Dominant <seealso cref="Dominant"/> component of this instance
        /// </summary>
        [Browsable(false)]
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

        /// <summary>
        /// Indexer for a component, case the indexer greater than 3, the new indexer
        /// to access a componet will be calculate(index % 3).
        /// </summary>
        /// <param name="index">Index of a component</param>
        /// <returns>Value of component indexed</returns>
        public float this[int index] {
            get {
                if (index > 2) {
                    index = index % 3;
                }
                if (index == 0) {
                    return this.X;
                } else if (index == 1) {
                    return this.Y;
                } else {
                    return this.Z;
                }
            }
            set {
                if (index > 2) {
                    index = index % 3;
                }
                if (index == 0) {
                    this.X = value;
                } else if (index == 1) {
                    this.Y = value;
                } else {
                    this.Z = value;
                }
            }
        }

        /// <summary>
        /// Indexer for a component, case the indexer greater than 3, the new indexer
        /// to access a componet will be calculate(index % 3).
        /// </summary>
        /// <param name="index">Index of a component</param>
        /// <returns>Value of component indexed</returns>
        public float this[Component index] {
            get {
                switch (index) {
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
                switch (index) {
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

        #region IEquatable<Point3D> Members

        public bool Equals(Point3D other) {
            return other.X.IsEqual(this.X) && other.Y.IsEqual(this.Y) && other.Z.IsEqual(this.Z);
        }

        #endregion

        #region ITransformable3D Members

        public void Rotate(float angle, Vector3D axis) {
            float sin = (float) Math.Sin(angle);
            float oneMinusCos = (float) (1.0 - Math.Cos(angle));
            axis.Normalize();
            Point3D result = this *
                   new Matrix3D(1 + oneMinusCos * ((axis.X * axis.X) - 1),
                                (oneMinusCos * axis.X * axis.Y) - axis.Z * sin,
                                (oneMinusCos * axis.X * axis.Z) + axis.Y * sin,
                                (oneMinusCos * axis.X * axis.Y) + axis.Z * sin,
                                1 + oneMinusCos * ((axis.Y * axis.Y) - 1),
                                (oneMinusCos * axis.Y * axis.Z) - axis.X * sin,
                                (oneMinusCos * axis.X * axis.Z) - axis.Y * sin,
                                (oneMinusCos * axis.Y * axis.Z) + axis.X * sin,
                                1 + oneMinusCos * ((axis.Z * axis.Z) - 1));
            this.X = result.X;
            this.Y = result.Y;
            this.Z = result.Z;
        }

        public void RotateAxisX(float angle) {
            float sin = (float) Math.Sin(angle);
            float cos = (float) Math.Cos(angle);
            Point3D result = this * new Matrix3D(1, 0, 0, 0, cos, -sin, 0, sin, cos);
            this.X = result.X;
            this.Y = result.Y;
            this.Z = result.Z;
        }

        public void RotateAxisY(float angle) {
            float sin = (float) Math.Sin(angle);
            float cos = (float) Math.Cos(angle);
            Point3D result = this * new Matrix3D(cos, 0, sin, 0, 1, 0, -sin, 0, cos);
            this.X = result.X;
            this.Y = result.Y;
            this.Z = result.Z;
        }

        public void RotateAxisZ(float angle) {
            float sin = (float) Math.Sin(angle);
            float cos = (float) Math.Cos(angle);
            Point3D result = this * new Matrix3D(cos, -sin, 0, sin, cos, 0, 0, 0, 1);
            this.X = result.X;
            this.Y = result.Y;
            this.Z = result.Z;
        }

        public void Scale(float factor) {
            this.X *= factor;
            this.Y *= factor;
            this.Z *= factor;
            //this = this * factor;
        }

        public void Translate(float tx, float ty, float tz) {
            this.Translate(new Vector3D(tx, ty, tz));
        }

        public void Translate(Vector3D translateVector) {
            Point3D result = this + translateVector;
            this.X = result.X;
            this.Y = result.Y;
            this.Z = result.Z;
        }

        #endregion

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public static bool operator ==(Point3D p1, Point3D p2) {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point3D p1, Point3D p2) {
            return !p1.Equals(p2);
        }

        public static Point3D operator +(Point3D p1, Point3D p2) {
            p1.X += p2.X;
            p1.Y += p2.Y;
            p1.Z += p2.Z;
            return p1;
            //return new Point3D((p1.X + p2.X), (p1.Y + p2.Y), (p1.Z + p2.Z));
        }

        public static Point3D operator +(Point3D p1, Vector3D v2) {
            p1.X += v2.X;
            p1.Y += v2.Y;
            p1.Z += v2.Z;
            return p1;
            //return new Point3D((p1.X + v2.X), (p1.Y + v2.Y), (p1.Z + v2.Z));
        }

        public static Point3D operator +(Vector3D v2, Point3D p1) {
            p1.X += v2.X;
            p1.Y += v2.Y;
            p1.Z += v2.Z;
            return p1;
            //return new Point3D((p1.X + v2.X), (p1.Y + v2.Y), (p1.Z + v2.Z));
        }

        /// <summary>
        /// P1 - P2 returns an Vector3D from P1 to P2
        /// </summary>
        /// <param name="p1">First Point</param>
        /// <param name="p2">Second Point</param>
        /// <returns>An Vector3D from P1 to P2</returns>
        public static Vector3D operator -(Point3D p1, Point3D p2) {
            return new Vector3D((p1.X - p2.X), (p1.Y - p2.Y), (p1.Z - p2.Z));
        }

        public static Point3D operator -(Point3D p1, Vector3D v2) {
            p1.X -= v2.X;
            p1.Y -= v2.Y;
            p1.Z -= v2.Z;
            return p1;
            //return new Point3D((p1.X - v2.X), (p1.Y - v2.Y), (p1.Z - v2.Z));
        }

        public static Point3D operator -(Vector3D v2, Point3D p1) {
            p1.X = v2.X - p1.X;
            p1.Y = v2.Y - p1.Y;
            p1.Z = v2.Z - p1.Z;
            return p1;
        }

        public static Point3D operator -(Point3D p1) {
            p1.X = -p1.X;
            p1.Y = -p1.Y;
            p1.Z = -p1.Z;
            return p1;
        }

        public static Point3D operator *(Point3D p1, float scalar) {
            p1.X *= scalar;
            p1.Y *= scalar;
            p1.Z *= scalar;
            return p1;
            //return new Point3D((point3D.X * scalar), (point3D.Y * scalar), (point3D.Z * scalar));
        }

        public static Point3D operator *(float scalar, Point3D p1) {
            p1.X *= scalar;
            p1.Y *= scalar;
            p1.Z *= scalar;
            return p1;
            //return new Point3D((point3D.X * scalar), (point3D.Y * scalar), (point3D.Z * scalar));
        }

        public static Point3D operator /(Point3D p1, float scalar) {
            p1.X /= scalar;
            p1.Y /= scalar;
            p1.Z /= scalar;
            return p1;
            //return new Point3D((point3D.X * 1 / scalar), (point3D.Y * 1 / scalar), (point3D.Z * 1 / scalar));
        }

        public static implicit operator float[](Point3D point) {
            return new[] {point.X, point.Y, point.Z};
        }

        public static implicit operator Point3D(float[] point) {
            return new Point3D(point[0], point[1], point[2]);
        }

        //TODO Verificar se isso é certo
        public static float operator *(Point3D p1, Point3D p2) {
            return ((p1.X * p2.X) + (p1.Y * p2.Y) + (p1.Z * p2.Z));
        }

        public static float operator *(Point3D p, Vector3D v) {
            return ((p.X * v.X) + (p.Y * v.Y) + (p.Z * v.Z));
        }

        public static float operator *(Vector3D v, Point3D p) {
            return ((p.X * v.X) + (p.Y * v.Y) + (p.Z * v.Z));
        }

        /// <summary>
        /// Create a new instance of a Vector3D with a copy components X, Y and Z this Point3D instance
        /// </summary>
        /// <returns></returns>
        public Vector3D ToVector3D() {
            return new Vector3D(this.X, this.Y, this.Z);
        }

        public override string ToString() {
            return this.X + "; " + this.Y + "; " + this.Z;
        }
    }
}