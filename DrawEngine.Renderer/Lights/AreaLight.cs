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
using System.Collections.Generic;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.PhotonMapping;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.Lights
{
    //public enum AreaLightType {
    //    Quandragle,
    //    Sphere
    //}
    [Serializable]
    public class AreaLight : Light
    {
        //private AreaLightType areaLightType = Lights.AreaLightType.Quandragle;
        private readonly Quadrilatero lightShape;
        private Vector3D direction;
        private double height;
        private Point3D towardsAt;
        private double width;
        public AreaLight() : base()
        {
            this.Width = 50;
            this.Height = 50;
            this.TowardsAt = Point3D.Zero;
            this.lightShape = new Quadrilatero(this.position, this.direction, this.width, this.height);
        }
        public AreaLight(RGBColor intensity, Point3D position, Point3D towardsAt, double width, double height)
                : base(intensity, position)
        {
            this.Width = width;
            this.Height = height;
            this.TowardsAt = towardsAt;
            this.lightShape = new Quadrilatero(position, this.direction, this.width, this.height);
        }
        public double Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        public Point3D TowardsAt
        {
            get { return this.towardsAt; }
            set
            {
                this.towardsAt = value;
                this.direction = (this.towardsAt - this.Position);
                //this.direction.Normalize();
            }
        }
        public Vector3D Direction
        {
            get { return this.direction; }
            set
            {
                this.direction = value;
                this.direction.Normalize();
                //this.towardsAt = this.position + this.direction;
            }
        }
        public double Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        public Point3D GetRandomPoint()
        {
            Random rnd = new Random(((int)DateTime.Now.Ticks) ^ 47);
            Vector3D rndVect1 = (this.lightShape.Vertex2 - this.lightShape.Vertex1).Normalized * this.width
                                * rnd.NextDouble();
            Vector3D rndVect2 = (this.lightShape.Vertex4 - this.lightShape.Vertex1).Normalized * this.height
                                * rnd.NextDouble();
            return this.lightShape.Vertex1 + rndVect1 + rndVect2;
        }
        public override double GetColorFactor(Vector3D pointToLight)
        {
            return 1.0d;
        }
        public override IEnumerable<Photon> GeneratePhotons()
        {
            throw new NotImplementedException();
        }

        #region ITransformable members
        public override void Rotate(double angle, Vector3D axis)
        {
            throw new NotImplementedException();
        }
        public override void RotateAxisX(double angle)
        {
            throw new NotImplementedException();
        }
        public override void RotateAxisY(double angle)
        {
            throw new NotImplementedException();
        }
        public override void RotateAxisZ(double angle)
        {
            throw new NotImplementedException();
        }
        public override void Scale(double factor)
        {
            throw new NotImplementedException();
        }
        public override void Translate(double tx, double ty, double tz)
        {
            throw new NotImplementedException();
        }
        public override void Translate(Vector3D translateVector)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}