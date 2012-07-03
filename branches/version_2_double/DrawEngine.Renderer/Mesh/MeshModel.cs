using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrawEngine.Renderer.RenderObjects;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.SpatialSubdivision.Acceleration;
using DrawEngine.Renderer.Importers;
using System.Xml.Serialization;
using System.ComponentModel;
using DrawEngine.Renderer.RenderObjects.Design;
using System.Drawing.Design;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.BasicStructures;
using System.IO;
using DrawEngine.Renderer.Mesh.Importers;
using System.Runtime.InteropServices;

namespace DrawEngine.Renderer.Mesh
{
    public enum ShadeType
    {
        Phong,
        Flat
    }

    [Serializable]
    public class MeshModel : Primitive, IDisposable, ITransformable3D
    {
        public MeshTriangle[] Triangles;
        private IntersectableAccelerationStructure<MeshTriangle> accelerationManager;

        private ShadeType shadeType;
        private string filePath;
        #region Delegates
        public delegate void ElementLoadEventHandler(int percentageOfTotal, DrawEngine.Renderer.Mesh.Importers.ElementMesh element);
        #endregion
        public MeshModel(){}
        public MeshModel(int numTriangles)
        {
            Triangles = new MeshTriangle[numTriangles];
        }
        public IntersectableAccelerationStructure<MeshTriangle> AccelerationManager
        {
            get { return accelerationManager; }
            set { accelerationManager = value; }
        }
        [Editor(typeof(ModelFileEditor), typeof(UITypeEditor)), RefreshProperties(RefreshProperties.All)]
        public String FilePath {
            get {
                return this.filePath;
            }
            set {
                this.filePath = value;
                this.Name = System.IO.Path.GetFileNameWithoutExtension(value);
            }
        }
        
        [DefaultValue(ShadeType.Phong)]
        public ShadeType ShadeType
        {
            get { return this.shadeType; }
            set { this.shadeType = value; }
        }
        #region IDisposable Members
        public void Dispose()
        {
            if (this.Triangles != null)
            {  
                this.Triangles = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        #endregion

        #region ITransformable3D Members
        public void Rotate(double angle, Vector3D axis)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisX(double angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisY(double angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisZ(double angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void Scale(double factor)
        {

            if (this.accelerationManager == null)
            {
                foreach (MeshTriangle tri in this.Triangles)
                {
                    tri.Scale(factor);
                }
                this.boundBox.Scale(factor);
            }
            else
            {
                ((ITransformable3D)this.accelerationManager).Scale(factor);
            }
            
        }
        public void Translate(double tx, double ty, double tz)
        {   
            if (this.accelerationManager == null)
            {
                foreach (MeshTriangle tri in Triangles)
                {
                    tri.Translate(tx, ty, tz);
                }
                this.boundBox.Translate(tx, ty, tz);
            }
            else
            {
                ((ITransformable3D)this.accelerationManager).Translate(tx, ty, tz);
            }
        }
        public void Translate(Vector3D translateVector)
        {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        public override bool FindIntersection(Ray ray, out Intersection intersect)
        {
            intersect = new Intersection();
            if (this.accelerationManager != null && this.accelerationManager.FindIntersection(ray, out intersect))
            {
                if (this.shadeType == ShadeType.Phong)
                {
                    MeshTriangle t = (MeshTriangle)intersect.HitPrimitive;
                    BarycentricCoordinate bary = t.CurrentBarycentricCoordinate;
                    Vector3D v1 = bary.Alpha * t.Vertex1.Normal;
                    Vector3D v2 = bary.Beta * t.Vertex2.Normal;
                    Vector3D v3 = bary.Gama * t.Vertex3.Normal;
                    intersect.Normal = (v1 + v2 + v3);
                    intersect.Normal.Normalize();
                }
                if (intersect.Normal * ray.Direction > 0)
                {
                    intersect.Normal.Flip();
                }
                intersect.HitPrimitive = this;
                intersect.HitPrimitive.Material = this.material;
                return true;
            }
            return false;
        }
        public override Vector3D NormalOnPoint(Point3D pointInPrimitive)
        {
            //foreach (Triangle tri in this.triangles)
            //{
            //    if (tri.PointInTriangle(pointInPrimitive))
            //    {
            //        BarycentricCoordinate bary = tri.CurrentBarycentricCoordinate;
            //        Vector3D v1 = bary.Alpha * tri.NormalOnVertex1;
            //        Vector3D v2 = bary.Beta * tri.NormalOnVertex2;
            //        Vector3D v3 = bary.Gama * tri.NormalOnVertex3;
            //        return (v1 + v2 + v3);
            //    }
            //}
            return Vector3D.Zero;
        }

        //public void Load()
        //{
        //    ModelType modelType = this.ResolverModelType();
        //    this.loader = null;
        //    switch (modelType)
        //    {
        //        case ModelType.Ply:
        //            this.loader = new LoaderPlyModel(this.path);
        //            break;
        //        case ModelType.Byu:
        //            this.loader = new LoaderByuModel(this.path);
        //            break;
        //        case ModelType.Obj:
        //            this.loader = new LoaderObjModel(this.path);
        //            break;
        //        case ModelType.Off:
        //            this.loader = new LoaderOffModel(this.path);
        //            break;
        //        case ModelType.None:
        //            throw new IOException(String.Format("O Arquivo {0} tem o formato inválido ou está corrompido!",
        //                                                this.path));
        //    }
        //    if (this.loader != null)
        //    {
        //        this.loader.OnElementLoaded += this.TriangleModel_OnElementLoaded;
        //        this.triangles = this.loader.Load();
        //        this.loader.Dispose();
        //        this.boundBox = this.loader.BoundBox;
        //        double len = Math.Abs(this.boundBox.PMax.Y - this.boundBox.PMin.Y);
        //        this.boundBox.Scale(50 / len);
        //        //this.boundBox.Translate(-this.boundBox.Center.ToVector3D());
        //        foreach (Triangle t in this.triangles)
        //        {
        //            t.Scale(50 / len);
        //            //t.Translate(-this.boundBox.Center.ToVector3D());
        //        }
        //        //this.manager = new NoAccerelationStructure<Triangle>(this.triangles);
        //        this.manager = new Octree<Triangle>(this.boundBox, this.triangles);
        //        //this.manager = new TriangleKDTree(new List<Triangle>(triangles));
        //        if (this.OnInitBuild != null)
        //        {
        //            this.OnInitBuild();
        //        }
        //        DateTime antes = DateTime.Now;
        //        this.manager.Optimize();
        //        ((ITransformable3D)this.manager).Translate(-(this.boundBox.Center.X), 0, 0);
        //        ((ITransformable3D)this.manager).Translate(0, -(this.boundBox.Center.Y), 0);
        //        ((ITransformable3D)this.manager).Translate(0, 0, -(this.boundBox.Center.Z));
        //        //this.manager.Optimize();
        //        if (this.OnEndBuild != null)
        //        {
        //            this.OnEndBuild(DateTime.Now.Subtract(antes));
        //        }
        //    }
        //}
        //private void TriangleModel_OnElementLoaded(int percentageOfTotal, ElementMesh element)
        //{
        //    if (this.OnElementLoaded != null)
        //    {
        //        this.OnElementLoaded(percentageOfTotal, element);
        //    }
        //}
        public override bool IsInside(Point3D point)
        {
            throw new NotImplementedException();
        }
        public override bool IsOverlap(BoundBox boundBox)
        {
            throw new NotImplementedException();
        }
    }
}
