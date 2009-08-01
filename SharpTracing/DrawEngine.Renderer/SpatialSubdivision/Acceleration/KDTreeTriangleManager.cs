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
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.SpatialSubdivision.Acceleration
{
    public class KDTreeTriangleManager : KDTreeIntersectableManager<Triangle>, ITransformable3D
    {
        public KDTreeTriangleManager() : base() {}
        public KDTreeTriangleManager(List<Triangle> content) : base(content) {}

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis)
        {
            throw new NotImplementedException();
        }
        public void RotateAxisX(float angle)
        {
            throw new NotImplementedException();
        }
        public void RotateAxisY(float angle)
        {
            throw new NotImplementedException();
        }
        public void RotateAxisZ(float angle)
        {
            throw new NotImplementedException();
        }
        public void Scale(float factor)
        {
            throw new NotImplementedException();
        }
        public void Translate(float tx, float ty, float tz)
        {
            foreach(Triangle triangle in this.AccelerationUnits){
                triangle.Translate(tx, ty, tz);
            }
            this.Optimize();
        }
        public void Translate(Vector3D translateVector)
        {
            throw new NotImplementedException();
        }
        #endregion

        protected override Point3D CalculateMid(IList<Triangle> content)
        {
            Triangle currentTriangle = content[0];
            Point3D mid = (1f / 3f) * (currentTriangle.Vertex1 + currentTriangle.Vertex2 + currentTriangle.Vertex3);
            for(int i = 1; i < content.Count; i++){
                currentTriangle = content[i];
                mid = (i / (i + 1f)) * mid
                      +
                      (1f / (i + 1f)) * (1f / 3f)
                      * (currentTriangle.Vertex1 + currentTriangle.Vertex2 + currentTriangle.Vertex3);
            }
            return mid;
        }
        protected override void SplitOnPlane(IList<Triangle> splitContent, Axis axis, Point3D position,
                                             out IList<Triangle> leftContent, out IList<Triangle> rightContent)
        {
            leftContent = new List<Triangle>();
            rightContent = new List<Triangle>();
            switch(axis){
                case Axis.X:
                    foreach(Triangle triangle in splitContent){
                        if(triangle.Vertex1.X <= position.X || triangle.Vertex2.X <= position.X
                           || triangle.Vertex3.X <= position.X){
                            leftContent.Add(triangle);
                        }
                        if(triangle.Vertex1.X > position.X || triangle.Vertex2.X > position.X
                           || triangle.Vertex3.X > position.X){
                            rightContent.Add(triangle);
                        }
                    }
                    break;
                case Axis.Y:
                    foreach(Triangle triangle in splitContent){
                        if(triangle.Vertex1.Y <= position.Y || triangle.Vertex2.Y <= position.Y
                           || triangle.Vertex3.Y <= position.Y){
                            leftContent.Add(triangle);
                        }
                        if(triangle.Vertex1.Y > position.Y || triangle.Vertex2.Y > position.Y
                           || triangle.Vertex3.Y > position.Y){
                            rightContent.Add(triangle);
                        }
                    }
                    break;
                case Axis.Z:
                    foreach(Triangle triangle in splitContent){
                        if(triangle.Vertex1.Z <= position.Z || triangle.Vertex2.Z <= position.Z
                           || triangle.Vertex3.Z <= position.Z){
                            leftContent.Add(triangle);
                        }
                        if(triangle.Vertex1.Z > position.Z || triangle.Vertex2.Z > position.Z
                           || triangle.Vertex3.Z > position.Z){
                            rightContent.Add(triangle);
                        }
                    }
                    break;
            }
        }
    }
}