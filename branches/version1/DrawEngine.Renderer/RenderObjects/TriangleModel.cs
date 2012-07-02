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
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Xml.Serialization;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Importers;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects.Design;
using DrawEngine.Renderer.SpatialSubdivision.Acceleration;

namespace DrawEngine.Renderer.RenderObjects
{
    public enum ShadeType
    {
        Phong,
        Flat
    }

    [Serializable]
    public class TriangleModel : Primitive, IDisposable, ITransformable3D
    {
        #region Delegates
        public delegate void ElementLoadEventHandler(int percentageOfTotal, ElementMesh element);

        public delegate void EndBuildEventHandler(TimeSpan timeToBuild);

        public delegate void InitBuildEventHandler();
        #endregion

        private AbstractLoaderModel loader;
        private IntersectableAccelerationStructure<Triangle> manager;
        private string path;
        private ShadeType shadeType;
        private Triangle[] triangles;
        public TriangleModel() : this("") {}
        public TriangleModel(string path)
        {
            this.Path = path;
            this.Name = System.IO.Path.GetFileNameWithoutExtension(this.Name) ;
        }
        public int TriangleCount
        {
            get
            {
                if(this.triangles != null){
                    return this.triangles.Length;
                }
                return 0;
            }
        }
        //Muito lento
        [XmlIgnore, Browsable(false)]
        public Triangle[] Triangles
        {
            get { return this.triangles; }
            internal set
            {
                if(value != null){
                    this.triangles = value;
                }
            }
        }
        [Editor(typeof(ModelFileEditor), typeof(UITypeEditor)), RefreshProperties(RefreshProperties.All)]
        public string Path
        {
            get { return this.path; }
            set
            {
                //if (File.Exists(value))
                //{
                this.path = value;
                this.Name = System.IO.Path.GetFileNameWithoutExtension(this.path);
                //}
                //else
                //{
                //    throw new FileNotFoundException(String.Format("Cannot load \"{0}\", file not found!", value) );
                //}
            }
        }
        [DefaultValue(ShadeType.Phong)]
        public ShadeType ShadeType
        {
            get { return this.shadeType; }
            set { this.shadeType = value; }
        }

        #region
        //public override bool IntersectPoint(out Intersection intersect, Ray ray) {
        //    intersect = new Intersection();
        //    if (!this.boundBox.Intersect(ray)) {
        //        return false;
        //    }
        //    intersect.TMin = float.PositiveInfinity;
        //    Intersection intersection_comp = new Intersection();
        //    Vector3D v1, v2, v3;
        //    bool hit = false;
        //    int less = 0;
        //    BarycentricCoordinate bary = new BarycentricCoordinate();
        //    for (int i = 0; i < this.triangles.Length; i++) {
        //        if (triangles[i].IntersectPoint(out intersection_comp, ray) && intersection_comp.TMin < intersect.TMin) {
        //            intersect = intersection_comp;
        //            bary = triangles[i].CurrentBarycentricCoordinate;
        //            less = i;
        //            hit = true;
        //        }
        //    }
        //    if (hit) {
        //        if (this.shadeType == ShadeType.Phong) {
        //            Triangle t = (Triangle)intersect.HitPrimitive;
        //            bary = t.BarycentricCoordinateOnPoint(intersect.HitPoint);
        //            if (bary.IsValid) {
        //                v1 = bary.Alpha * this.normalsPerVertex[this.pointersToVertex[less].Vertex1];
        //                v2 = bary.Beta * this.normalsPerVertex[this.pointersToVertex[less].Vertex2];
        //                v3 = bary.Gama * this.normalsPerVertex[this.pointersToVertex[less].Vertex3];
        //                intersect.Normal = (v1 + v2 + v3);
        //                intersect.Normal.Normalize();
        //            }
        //        }
        //        intersect.HitPrimitive = this;
        //        intersect.HitPrimitive.Material = this.material;
        //    }
        //    return hit;
        //}
        //public void ProcessNormalsPerVertex() {
        //    Vector3D vertexVectorSum;
        //    int faceCount;
        //    PointerToVertex currentFace;
        //    for (int i = 0; i < this.Vertices.Length; i++) {
        //        // Find faces attached to our current vertex
        //        faceCount = 0;
        //        vertexVectorSum = Vector3D.Zero;
        //        for (int j = 0; j < this.pointersToVertex.Length; j++) {
        //            currentFace = this.pointersToVertex[j];
        //            if ((i == currentFace.Vertex1) || (i == currentFace.Vertex2) || (i == currentFace.Vertex3)) {
        //                faceCount++;
        //                vertexVectorSum = vertexVectorSum + this.triangles[j].Normal;
        //            }
        //        }
        //        // Use sum of face normals to calculate this vertex's normal
        //        this.normalsPerVertex[i] = vertexVectorSum / faceCount;
        //        this.normalsPerVertex[i].Normalize();
        //    }
        //}
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            if(this.loader != null){
                this.loader.Dispose();
            }
            if(this.triangles != null){
                Array.Clear(this.triangles, 0, this.triangles.Length);
            }
            this.triangles = null;
            this.manager = null;
            this.boundBox = new BoundBox();
            this.loader = null;
        }
        #endregion

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisX(float angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisY(float angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisZ(float angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void Scale(float factor)
        {
            foreach(Triangle tri in this.triangles){
                tri.Scale(factor);
            }
            this.boundBox.Scale(factor);
            //this.manager.Scale(factor);
        }
        public void Translate(float tx, float ty, float tz)
        {
            //foreach (Triangle tri in triangles)
            //{
            //    tri.Translate(tx, ty, tz);
            //}
            //this.boundBox.Translate(tx, ty, tz);
            ((ITransformable3D)this.manager).Translate(tx, ty, tz);
            this.boundBox.Translate(tx, ty, tz);
        }
        public void Translate(Vector3D translateVector)
        {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        public event ElementLoadEventHandler OnElementLoaded;
        public event InitBuildEventHandler OnInitBuild;
        public event EndBuildEventHandler OnEndBuild;
        public override bool FindIntersection(Ray ray, out Intersection intersect)
        {
            intersect = new Intersection();
            //if (!this.BoundBox.Intersect(ray)) {
            //    return false;
            //}
            if(this.manager != null && this.manager.FindIntersection(ray, out intersect)){
                if(this.shadeType == ShadeType.Phong){
                    Triangle t = (Triangle)intersect.HitPrimitive;
                    BarycentricCoordinate bary = t.CurrentBarycentricCoordinate;
                    Vector3D v1 = bary.Alpha * t.NormalOnVertex1;
                    Vector3D v2 = bary.Beta * t.NormalOnVertex2;
                    Vector3D v3 = bary.Gama * t.NormalOnVertex3;
                    intersect.Normal = (v1 + v2 + v3);
                    intersect.Normal.Normalize();
                }
                if(intersect.Normal * ray.Direction > 0){
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
            foreach(Triangle tri in this.triangles){
                if(tri.PointInTriangle(pointInPrimitive)){
                    BarycentricCoordinate bary = tri.CurrentBarycentricCoordinate;
                    Vector3D v1 = bary.Alpha * tri.NormalOnVertex1;
                    Vector3D v2 = bary.Beta * tri.NormalOnVertex2;
                    Vector3D v3 = bary.Gama * tri.NormalOnVertex3;
                    return (v1 + v2 + v3);
                }
            }
            return Vector3D.Zero;
        }
        private ModelType ResolverModelType()
        {
            List<string[]> header = new List<string[]>();
            using(StreamReader sr = new StreamReader(this.path)){
                string hLine = sr.ReadLine().Trim();
                string ext = System.IO.Path.GetExtension(this.path).ToLower();
                while(hLine == string.Empty){
                    hLine = sr.ReadLine().Trim();
                }
                if(ext == ".ply" && hLine.ToLower() == "ply"){
                    return ModelType.Ply;
                } else if(ext == ".byu"){
                    return ModelType.Byu;
                } else if(ext == ".obj"){
                    return ModelType.Obj;
                } else if(ext == ".off" || ext == ".noff" || ext == ".cnoff"){
                    return ModelType.Off;
                }
                return ModelType.None;
            }
        }
        public void Load()
        {
            ModelType modelType = this.ResolverModelType();
            this.loader = null;
            switch(modelType){
                case ModelType.Ply:
                    this.loader = new LoaderPlyModel(this.path);
                    break;
                case ModelType.Byu:
                    this.loader = new LoaderByuModel(this.path);
                    break;
                case ModelType.Obj:
                    this.loader = new LoaderObjModel(this.path);
                    break;
                case ModelType.Off:
                    this.loader = new LoaderOffModel(this.path);
                    break;
                case ModelType.None:
                    throw new IOException(String.Format("O Arquivo {0} tem o formato inválido ou está corrompido!",
                                                        this.path));
            }
            if(this.loader != null){
                this.loader.OnElementLoaded += this.TriangleModel_OnElementLoaded;
                this.triangles = this.loader.Load();
                this.loader.Dispose();
                this.boundBox = this.loader.BoundBox;
                float len = Math.Abs(this.boundBox.PMax.Y - this.boundBox.PMin.Y);
                this.boundBox.Scale(50 / len);
                //this.boundBox.Translate(-this.boundBox.Center.ToVector3D());
                foreach(Triangle t in this.triangles){
                    t.Scale(50 / len);
                    //t.Translate(-this.boundBox.Center.ToVector3D());
                }
                //this.manager = new NoAccerelationStructure<Triangle>(this.triangles);
                this.manager = new Octree<Triangle>(this.boundBox, this.triangles);
                //this.manager = new TriangleKDTree(new List<Triangle>(triangles));
                if(this.OnInitBuild != null){
                    this.OnInitBuild();
                }
                DateTime antes = DateTime.Now;
                this.manager.Optimize();
                ((ITransformable3D)this.manager).Translate(-(this.boundBox.Center.X), 0, 0);
                ((ITransformable3D)this.manager).Translate(0, -(this.boundBox.Center.Y), 0);
                ((ITransformable3D)this.manager).Translate(0, 0, -(this.boundBox.Center.Z));
                //this.manager.Optimize();
                if(this.OnEndBuild != null){
                    this.OnEndBuild(DateTime.Now.Subtract(antes));
                }
            }
        }
        private void TriangleModel_OnElementLoaded(int percentageOfTotal, ElementMesh element)
        {
            if(this.OnElementLoaded != null){
                this.OnElementLoaded(percentageOfTotal, element);
            }
        }
        public override bool IsInside(Point3D point)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override bool IsOverlap(BoundBox boundBox)
        {
            throw new NotImplementedException();
        }
    }
}