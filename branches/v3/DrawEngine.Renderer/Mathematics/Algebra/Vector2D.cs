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

namespace DrawEngine.Renderer.Algebra
{
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector2D
    {
        public float X, Y;
        public Vector2D(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public float Length
        {
            get { return (float)Math.Sqrt((this.X * this.X) + (this.Y * this.Y)); }
        }
        public Vector2D Normalized
        {
            get
            {
                Vector2D v = this;
                v.Normalize();
                return v;
            }
        }
        public Vector2D Inverted
        {
            get { return -this; }
        }
        public void Normalize()
        {
            float len = this.Length;
            if(len == 0.0f){
                this.X = 0.0f;
                this.Y = 0.0f;
            } else{
                this.X = this.X * 1 / len;
                this.Y = this.Y * 1 / len;
            }
        }
        public void Inverse()
        {
            this.X = (-this.X);
            this.Y = (-this.Y);
        }
        public static bool operator ==(Vector2D v1, Vector2D v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(Vector2D v1, Vector2D v2)
        {
            return !v1.Equals(v2);
        }
        public static void Orthogonalize(ref Vector2D v1, Vector2D v2)
        {
            float div = (v2 * v2);
            if(div != 0){
                v1 = v1 - (((v1 * v2) * 1 / div) * v2);
            }
        }
        public static Vector2D operator |(Vector2D v1, Vector2D v2)
        {
            Orthogonalize(ref v1, v2);
            return v1;
        }
        /// <summary>
        /// Soma dois vetores
        /// </summary>
        /// <param name="v1">Vetor1</param>
        /// <param name="v2">Vetor2</param>
        /// <returns>Retorna um vetor que representa a soma de outros dois</returns>
        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D((v1.X + v2.X), (v1.Y + v2.Y));
        }
        /// <summary>
        /// Operador unário que retorna um NOVO vetor com direcao invertida
        /// </summary>
        /// <param name="v1">Vetor para inversao</param>
        /// <returns>o valor do vetor invertido</returns>
        public static Vector2D operator -(Vector2D v1)
        {
            return new Vector2D(-v1.X, -v1.Y);
        }
        /// <summary>
        /// Diminue dois vetores
        /// </summary>
        /// <param name="v1">Vetor1</param>
        /// <param name="v2">Vetor2</param>
        /// <returns>Retorno a subtracao entre Vetor1 e Vetor1</returns>
        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return new Vector2D((v1.X - v2.X), (v1.Y - v2.Y));
        }
        /// <summary>
        /// Produto entre um vetor e um escalar
        /// </summary>
        /// <param name="vector">Vetor para aumento da magnitude</param>
        /// <param name="scalar">Escalar que aumenta a magnitude do vetor</param>        
        /// <returns>Vetor com a magnitude aumentada</returns>	
        public static Vector2D operator *(Vector2D vector, float scalar)
        {
            return new Vector2D((vector.X * scalar), (vector.Y * scalar));
        }
        /// <summary>
        /// Produto entre um vetor e um escalar
        /// </summary>
        /// <param name="scalar">Escalar que aumenta a magnitude do vetor</param>
        /// <param name="vector">Vetor para aumento da magnitude</param>
        /// <returns>Vetor com a magnitude aumentada</returns>
        public static Vector2D operator *(float scalar, Vector2D vector)
        {
            return new Vector2D((vector.X * scalar), (vector.Y * scalar));
        }
        /// <summary>
        /// Divisão de um vetor por um escalar
        /// </summary>
        /// <param name="vector">Vetor que terá sua magnitude será dividida</param>
        /// <param name="scalar">Divisor da magnitude do vetor</param>
        /// <returns>Vetor dividido</returns>
        public static Vector2D operator /(Vector2D vector, float scalar)
        {
            return new Vector2D((vector.X / scalar), (vector.Y / scalar));
        }
        /// <summary>
        /// Calcula o produto escalar entre dois vetores
        /// </summary>
        /// <param name="v1">Vetor1</param>
        /// <param name="v2">Vetor2</param>
        /// <returns>Retorna o tamalho da projecao do Vetor1 sobre o Vetor2</returns>
        public static float operator *(Vector2D v1, Vector2D v2)
        {
            return ((v1.X * v2.X) + (v1.Y * v2.Y));
        }
        public Point2D ToPoint2D()
        {
            return new Point2D(this.X, this.Y);
        }
        public override string ToString()
        {
            return "(X=" + this.X + ", Y=" + this.Y + ")";
        }
    }
}