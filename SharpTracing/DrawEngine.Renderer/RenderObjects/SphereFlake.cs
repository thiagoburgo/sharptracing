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
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.SpatialSubdivision.Acceleration;

namespace DrawEngine.Renderer.RenderObjects
{
    [Serializable]
    public class SphereFlake : Primitive, ITransformable3D
    {
        private float initialRadius;
        private int maxDepth;
        private List<Primitive> sphereFlake;
        private KDTreePrimitiveManager sphereFlakeKDTree;
        public SphereFlake() : this(Point3D.Zero, 20, 1) {}
        public SphereFlake(Point3D center, float initialRadius, int maxDepth)
        {
            this.center = center;
            this.initialRadius = initialRadius;
            this.maxDepth = maxDepth;
            this.sphereFlake = new List<Primitive>(maxDepth * 6);
            this.sphereFlakeKDTree = new KDTreePrimitiveManager(this.sphereFlake);
            this.redoFlake();
        }
        public int SphereCount
        {
            get { return this.sphereFlake.Count; }
        }
        /**
         * Skip format is xyzxyz fo which to skip.
         * Thus 000100 means skip the -x direction.
         * Thus 10000 means skip the +x direction.
         */
        public Point3D Center
        {
            get { return this.center; }
            set
            {
                this.center = value;
                this.redoFlake();
            }
        }
        public float InitialRadius
        {
            get { return this.initialRadius; }
            set
            {
                this.initialRadius = value;
                this.redoFlake();
            }
        }
        public int MaxDepth
        {
            get { return this.maxDepth; }
            set
            {
                this.maxDepth = value;
                this.redoFlake();
            }
        }

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis)
        {
            this.center.Rotate(angle, axis);
            this.redoFlake();
        }
        public void RotateAxisX(float angle)
        {
            this.center.RotateAxisX(angle);
            this.redoFlake();
        }
        public void RotateAxisY(float angle)
        {
            this.center.RotateAxisY(angle);
            this.redoFlake();
        }
        public void RotateAxisZ(float angle)
        {
            this.center.RotateAxisZ(angle);
            this.redoFlake();
        }
        public void Scale(float factor)
        {
            this.initialRadius = this.initialRadius * factor;
            this.redoFlake();
        }
        public void Translate(float tx, float ty, float tz)
        {
            this.center.Translate(tx, ty, tz);
            this.redoFlake();
        }
        public void Translate(Vector3D translateVector)
        {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        private void redoFlake()
        {
            this.sphereFlake.Clear();
            this.doFlake(this.center.X, this.center.Y, this.center.Z, this.initialRadius, 0, "000000");
            this.sphereFlakeKDTree.Optimize();
        }
        private void doFlake(float cX, float cY, float cZ, float radius, int depth, string skip)
        {
            if(depth > this.maxDepth){
                return;
            }
            Sphere sphere = new Sphere(radius, new Point3D(cX, cY, cZ));
            sphere.Material = this.Material;
            this.sphereFlake.Add(sphere);
            float hRadius = radius * 0.5f;
            float slopeRadius = radius + hRadius;
            if(skip != "100000"){
                this.doFlake(cX + slopeRadius, cY, cZ, hRadius, depth + 1, "000100");
            }
            if(skip != "000100"){
                this.doFlake(cX - slopeRadius, cY, cZ, hRadius, depth + 1, "100000");
            }
            if(skip != "010000"){
                this.doFlake(cX, cY + slopeRadius, cZ, hRadius, depth + 1, "000010");
            }
            if(skip != "000010"){
                this.doFlake(cX, cY - slopeRadius, cZ, hRadius, depth + 1, "010000");
            }
            if(skip != "001000"){
                this.doFlake(cX, cY, cZ + slopeRadius, hRadius, depth + 1, "000001");
            }
            if(skip != "000001"){
                this.doFlake(cX, cY, cZ - slopeRadius, hRadius, depth + 1, "001000");
            }
        }
        public override bool FindIntersection(Ray ray, out Intersection intersect)
        {
            bool intersects = this.sphereFlakeKDTree.FindIntersection(ray, out intersect);
            intersect.HitPrimitive = this;
            return intersects;
        }
        public override bool IsInside(Point3D point)
        {
            throw new NotImplementedException();
        }
        public override Vector3D NormalOnPoint(Point3D pointInPrimitive)
        {
            throw new NotImplementedException();
        }
        public override bool IsOverlap(BoundBox boundBox)
        {
            throw new NotImplementedException();
        }
    }
}