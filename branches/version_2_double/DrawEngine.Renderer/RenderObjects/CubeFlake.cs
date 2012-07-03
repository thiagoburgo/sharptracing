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
using DrawEngine.Renderer.SpatialSubdivision.Acceleration;

namespace DrawEngine.Renderer.RenderObjects
{
    [Serializable]
    public class CubeFlake : Primitive
    {
        private List<Primitive> cubeFlake;
        private KDTreePrimitiveManager cubeFlakeKDTree;
        private float initialLength;
        private int maxDepth;
        public CubeFlake() : this(Point3D.Zero, 20, 1) {}
        public CubeFlake(Point3D center, float initialLength, int maxDepth)
        {
            this.center = center;
            this.initialLength = initialLength;
            this.maxDepth = maxDepth;
            this.cubeFlake = new List<Primitive>(maxDepth * 6);
            this.cubeFlakeKDTree = new KDTreePrimitiveManager(this.cubeFlake);
            this.redoFlake();
        }
        public Point3D Center
        {
            get { return this.center; }
            set
            {
                this.center = value;
                this.redoFlake();
            }
        }
        public float InitialLength
        {
            get { return this.initialLength; }
            set
            {
                this.initialLength = value;
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
        private void redoFlake()
        {
            this.cubeFlake.Clear();
            this.doFlake(this.center.X, this.center.Y, this.center.Z, this.initialLength, 0, "000000");
            this.cubeFlakeKDTree.Optimize();
        }
        /**
         * Skip format is xyzxyz fo which to skip.
         * Thus 000100 means skip the -x direction.
         * Thus 10000 means skip the +x direction.
         */
        private void doFlake(float cX, float cY, float cZ, float radius, int depth, string skip)
        {
            if(depth > this.maxDepth){
                return;
            }
            Box box = new Box(new Point3D(cX, cY, cZ), 2 * radius, 2 * radius, 2 * radius);
            //box.Material = this.Material;
            this.cubeFlake.Add(box);
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
            bool intersects = this.cubeFlakeKDTree.FindIntersection(ray, out intersect);
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