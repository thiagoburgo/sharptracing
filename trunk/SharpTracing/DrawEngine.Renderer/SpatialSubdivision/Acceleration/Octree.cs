using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;
using DrawEngine.Renderer.Util;

namespace DrawEngine.Renderer.SpatialSubdivision.Acceleration
{
    public class Octree<T> : IntersectableAccelerationStructure<T>, IComparable<Octree<T>>, ITransformable3D,
                             IDisposable where T : IIntersectable, IBoundBox, ITransformable3D
    {
        private const int maxBuildDepth = 9;
        private readonly int maxUnitsInBox = 4;
        private BoundBox box;
        private bool isLeaf;
        private float ltmax;
        private float ltmin;
        private Octree<T>[] octrees;
        //private Triangle[] triangles;
        public Octree(BoundBox box, T[] acceleationUnits) : base(acceleationUnits)
        {
            this.box = box;
            if(acceleationUnits != null && acceleationUnits.Length < this.maxUnitsInBox){
                this.maxUnitsInBox = acceleationUnits.Length;
            }
        }
        public Octree(BoundBox box) : this(box, null) {}
        public BoundBox BoundBox
        {
            get { return this.box; }
        }
        public Octree<T>[] Octrees
        {
            get { return this.octrees; }
        }

        #region IComparable<Octree<T>> Members
        public int CompareTo(Octree<T> other)
        {
            if(this.ltmin < other.ltmin){
                return -1;
            } else if(this.ltmin > other.ltmin){
                return 1;
            }
            return 0;
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            this.octrees = null;
            this.accelerationUnits = null;
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
            ScaleAllBoundBoxes(this, factor);
        }
        public void Translate(float tx, float ty, float tz)
        {
            TranslateAllBoundBoxes(this, tx, ty, tz);
            this.box.Translate(tx, ty, tz);
        }
        public void Translate(Vector3D translateVector)
        {
            TranslateAllBoundBoxes(this, translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        ~Octree()
        {
            this.octrees = null;
            this.accelerationUnits = null;
        }
        public override void Optimize()
        {
            this.Build(1);
        }
        public void Optimize(T[] units)
        {
            this.accelerationUnits = units;
            this.Build(1);
        }
        //private void Build(T[] trs)
        //{
        //    this.accelerationUnits = trs;
        //    Build(1);
        //}
        private void Build(int depth)
        {
            if(depth > maxBuildDepth || this.accelerationUnits.Count < this.maxUnitsInBox){
                this.isLeaf = true;
                return;
            }
            Point3D p01 = this.box.PMin;
            Point3D p02 = new Point3D(this.box.PMin.X, this.box.PMin.Y, this.box.Center.Z);
            Point3D p03 = new Point3D(this.box.Center.X, this.box.PMin.Y, this.box.Center.Z);
            Point3D p04 = new Point3D(this.box.Center.X, this.box.PMin.Y, this.box.PMin.Z);
            Point3D p05 = new Point3D(this.box.PMin.X, this.box.Center.Y, this.box.PMin.Z);
            Point3D p06 = new Point3D(this.box.PMin.X, this.box.Center.Y, this.box.Center.Z);
            Point3D p07 = new Point3D(this.box.Center.X, this.box.Center.Y, this.box.Center.Z);
            Point3D p08 = new Point3D(this.box.Center.X, this.box.Center.Y, this.box.PMin.Z);
            Point3D p09 = new Point3D(this.box.Center.X, this.box.Center.Y, this.box.PMax.Z);
            Point3D p10 = new Point3D(this.box.PMax.X, this.box.Center.Y, this.box.PMax.Z);
            Point3D p11 = new Point3D(this.box.PMax.X, this.box.Center.Y, this.box.Center.Z);
            Point3D p12 = new Point3D(this.box.Center.X, this.box.PMax.Y, this.box.Center.Z);
            Point3D p13 = new Point3D(this.box.Center.X, this.box.PMax.Y, this.box.PMax.Z);
            Point3D p14 = this.box.PMax;
            Point3D p15 = new Point3D(this.box.PMax.X, this.box.PMax.Y, this.box.Center.Z);
            this.octrees = new Octree<T>[8];
            this.octrees[0] = new Octree<T>(new BoundBox(p01, p07));
            this.octrees[1] = new Octree<T>(new BoundBox(p02, p09));
            this.octrees[2] = new Octree<T>(new BoundBox(p03, p10));
            this.octrees[3] = new Octree<T>(new BoundBox(p04, p11));
            this.octrees[4] = new Octree<T>(new BoundBox(p05, p12));
            this.octrees[5] = new Octree<T>(new BoundBox(p06, p13));
            this.octrees[6] = new Octree<T>(new BoundBox(p07, p14));
            this.octrees[7] = new Octree<T>(new BoundBox(p08, p15));
            List<Octree<T>> list = new List<Octree<T>>();
            foreach(Octree<T> o in this.octrees){
                o.PickAccelUnits(this.accelerationUnits);
                if(o.accelerationUnits.Count != 0){
                    o.Build(depth + 1);
                    list.Add(o);
                }
            }
            //this.accelerationUnits = null;
            this.octrees = list.ToArray();
        }
        private void PickAccelUnits(IList<T> trs)
        {
            List<T> list = new List<T>();
            foreach(T tr in trs){
                if(tr.IsOverlap(this.box)){
                    list.Add(tr);
                }
            }
            this.accelerationUnits = list.ToArray();
        }
        public override bool FindIntersection(Ray ray, out Intersection intersect)
        {
            intersect = new Intersection();
            //if (!this.BoxIntersect(ray))
            //{
            //    return false;
            //}
            intersect.TMin = float.MaxValue;
            bool hit = false;
            if(this.isLeaf){
                Intersection intersect_comp;
                foreach(IIntersectable tri in this.accelerationUnits){
                    if(tri.FindIntersection(ray, out intersect_comp) && intersect_comp.TMin < intersect.TMin){
                        intersect = intersect_comp;
                        hit = true;
                        //if(intersect.Normal == Vector3D.Zero) {
                        //    MessageBox.Show("Test");
                        //}
                    }
                }
                return hit && intersect.TMin <= this.ltmax;
            }
            int counter = 0;
            foreach(Octree<T> o in this.octrees){
                if(o.BoxIntersect(ray)){
                    counter++;
                }
            }
            if(counter > 0){
                this.octrees.InsertionSort();
            }
            for(int i = 0; i < counter; i++){
                if(this.octrees[i].FindIntersection(ray, out intersect)){
                    //if(intersect.Normal == Vector3D.Zero) {
                    //    MessageBox.Show("Test");
                    //}
                    return true;
                }
            }
            return false;
        }
        public bool BoxIntersect(Ray ray)
        {
            this.ltmin = float.MaxValue;
            this.ltmax = float.MaxValue;
            float tmin, tmax, tymin, tymax, tzmin, tzmax;
            if(ray.Inv_Direction.X > 0){
                tmin = (this.box.PMin.X - ray.Origin.X) * ray.Inv_Direction.X;
                tmax = (this.box.PMax.X - ray.Origin.X) * ray.Inv_Direction.X;
            } else{
                tmin = (this.box.PMax.X - ray.Origin.X) * ray.Inv_Direction.X;
                tmax = (this.box.PMin.X - ray.Origin.X) * ray.Inv_Direction.X;
            }
            if(ray.Inv_Direction.Y > 0){
                tymin = (this.box.PMin.Y - ray.Origin.Y) * ray.Inv_Direction.Y;
                tymax = (this.box.PMax.Y - ray.Origin.Y) * ray.Inv_Direction.Y;
            } else{
                tymin = (this.box.PMax.Y - ray.Origin.Y) * ray.Inv_Direction.Y;
                tymax = (this.box.PMin.Y - ray.Origin.Y) * ray.Inv_Direction.Y;
            }
            if((tmin > tymax) || (tymin > tmax)){
                return false;
            }
            if(tymin > tmin){
                tmin = tymin;
            }
            if(tymax < tmax){
                tmax = tymax;
            }
            if(ray.Inv_Direction.Z > 0){
                tzmin = (this.box.PMin.Z - ray.Origin.Z) * ray.Inv_Direction.Z;
                tzmax = (this.box.PMax.Z - ray.Origin.Z) * ray.Inv_Direction.Z;
            } else{
                tzmin = (this.box.PMax.Z - ray.Origin.Z) * ray.Inv_Direction.Z;
                tzmax = (this.box.PMin.Z - ray.Origin.Z) * ray.Inv_Direction.Z;
            }
            if((tmin > tzmax) || (tzmin > tmax)){
                return false;
            }
            if(tzmin > tmin){
                tmin = tzmin;
            }
            if(tzmax < tmax){
                tmax = tzmax;
            }
            if((tmin < this.ltmin) && (tmax > 0)){
                this.ltmin = tmin;
                this.ltmax = tmax;
                return true;
            }
            return false;
        }
        private static void TranslateAllBoundBoxes(Octree<T> oct, float tx, float ty, float tz)
        {
            List<Octree<T>> octs = GetAllOctrees(oct);
            foreach(Octree<T> octree in octs){
                octree.box.Translate(tx, ty, tz);
                //if (octree.isLeaf)
                //{
                //    foreach (ITransformable3D unit in octree.accelerationUnits)
                //    {
                //        unit.Translate(tx, ty, tz);
                //    }
                //}
            }
            foreach(ITransformable3D unit in oct.accelerationUnits){
                unit.Translate(tx, ty, tz);
            }
        }
        private static void ScaleAllBoundBoxes(Octree<T> oct, float factor)
        {
            if(oct != null){
                oct.box.Scale(factor);
                for(int i = 0; oct.accelerationUnits != null && i < oct.accelerationUnits.Count; i++){
                    if(oct.accelerationUnits[i] != null){
                        oct.accelerationUnits[i].Scale(factor);
                    }
                }
                for(int i = 0; oct.octrees != null && i < oct.octrees.Length; i++){
                    ScaleAllBoundBoxes(oct.octrees[i], factor);
                }
            }
        }
        public List<BoundBox> GetAllBoundBoxes()
        {
            return GetAllBoundBoxes(this);
        }
        private static List<BoundBox> GetAllBoundBoxes(Octree<T> oct)
        {
            List<BoundBox> ret = new List<BoundBox>(32);
            if(oct != null){
                ret.Add(oct.box);
                for(int i = 0; !oct.isLeaf && i < oct.octrees.Length; i++){
                    ret.AddRange(GetAllBoundBoxes(oct.octrees[i]));
                }
            }
            return ret;
        }
        public List<Octree<T>> GetAllOctrees()
        {
            return GetAllOctrees(this);
        }
        private static List<Octree<T>> GetAllOctrees(Octree<T> oct)
        {
            List<Octree<T>> ret = new List<Octree<T>>(32);
            if(oct != null){
                ret.Add(oct);
                for(int i = 0; !oct.isLeaf && i < oct.octrees.Length; i++){
                    ret.AddRange(GetAllOctrees(oct.octrees[i]));
                }
            }
            return ret;
        }
    }
}