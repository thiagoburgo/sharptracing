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

namespace DrawEngine.Renderer.Mathematics.Algebra
{
    public class OrthoNormalBasis
    {
        private Vector3D u, v, w;
        public void FlipU()
        {
            this.u.Flip();
        }
        public void FlipV()
        {
            this.v.Flip();
        }
        public void FlipW()
        {
            this.w.Flip();
        }
        public void SwapUV()
        {
            Vector3D t = this.u;
            this.u = this.v;
            this.v = t;
        }
        public void SwapVW()
        {
            Vector3D t = this.v;
            this.v = this.w;
            this.w = t;
        }
        public void SwapWU()
        {
            Vector3D t = this.w;
            this.w = this.u;
            this.u = t;
        }
        public Vector3D Transform(Vector3D a)
        {
            float x = (a.X * this.u.X) + (a.Y * this.v.X) + (a.Z * this.w.X);
            float y = (a.X * this.u.Y) + (a.Y * this.v.Y) + (a.Z * this.w.Y);
            float z = (a.X * this.u.Z) + (a.Y * this.v.Z) + (a.Z * this.w.Z);
            a.X = x;
            a.Y = y;
            a.Z = z;
            return a;
        }
        public Vector3D UnTransform(Vector3D a)
        {
            float x = (a * this.u);
            float y = (a * this.v);
            float z = (a * this.w);
            a.X = x;
            a.Y = y;
            a.Z = z;
            return a;
        }
        public float UnTransformX(Vector3D a)
        {
            return (a * this.u);
        }
        public float UnTransformY(Vector3D a)
        {
            return (a * this.v);
        }
        public float UnTransformZ(Vector3D a)
        {
            return (a * this.w);
        }
        public static OrthoNormalBasis MakeFromW(Vector3D w)
        {
            OrthoNormalBasis onb = new OrthoNormalBasis();
            w.Normalize();
            onb.w = w;
            if((Math.Abs(onb.w.X) < Math.Abs(onb.w.Y)) && (Math.Abs(onb.w.X) < Math.Abs(onb.w.Z))){
                onb.v.X = 0;
                onb.v.Y = onb.w.Z;
                onb.v.Z = -onb.w.Y;
            } else if(Math.Abs(onb.w.Y) < Math.Abs(onb.w.Z)){
                onb.v.X = onb.w.Z;
                onb.v.Y = 0;
                onb.v.Z = -onb.w.X;
            } else{
                onb.v.X = onb.w.Y;
                onb.v.Y = -onb.w.X;
                onb.v.Z = 0;
            }
            onb.v.Normalize();
            onb.u = onb.v ^ onb.w;
            return onb;
        }
        public static OrthoNormalBasis MakeFromWV(Vector3D w, Vector3D v)
        {
            OrthoNormalBasis onb = new OrthoNormalBasis();
            w.Normalize();
            onb.w = w;
            onb.u = onb.w ^ v;
            onb.u.Normalize();
            onb.v = (onb.w ^ onb.u);
            return onb;
        }
    }
}