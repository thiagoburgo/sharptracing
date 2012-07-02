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
using System.Xml.Serialization;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Materials.Design;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects.CSG;
using DrawEngine.Renderer.SpatialSubdivision.Acceleration;

namespace DrawEngine.Renderer.RenderObjects
{

    #region Includes for generate Scene XML representation
    [XmlInclude(typeof(Torus)), XmlInclude(typeof(Triangle)), XmlInclude(typeof(Box)), XmlInclude(typeof(Cone)),
     XmlInclude(typeof(Cylinder)), XmlInclude(typeof(Disc)), XmlInclude(typeof(Ellipsoid)), XmlInclude(typeof(Plane)),
     XmlInclude(typeof(Quadrilatero)), XmlInclude(typeof(Sphere)), XmlInclude(typeof(TriangleModel)),
     XmlInclude(typeof(Polygon)), XmlInclude(typeof(RegularPolygon)), XmlInclude(typeof(SphereFlake)),
     XmlInclude(typeof(CubeFlake)), XmlInclude(typeof(CornellBox)), Serializable]
    #endregion

    public abstract class Primitive : IPrimitive,INameable
    {
        protected BoundBox boundBox;
        protected Point3D center;
        [XmlIgnore, NonSerialized]
        protected Material material;
        private String name;
        private bool visible = true;
        protected Primitive()
        {
            this.material = new PhongMaterial();
        }
        
        
        [Editor(typeof(MaterialSelectorEditor), typeof(UITypeEditor)), DefaultValue(null),
         TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual Material Material
        {
            get { return this.material; }
            set { this.material = value; }
        }
      
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        #region IBoundBox Members
        public virtual BoundBox BoundBox
        {
            get { return this.boundBox; }
            set { this.boundBox = value; }
        }
        /// <summary>
        /// Verify if this primitive is inside in a bound box. Used in
        /// Octree for accerelation <see cref="Octree{T}<T>"/>
        /// </summary>
        /// <param name="boundBox">BoundBox for inside test</param>
        /// <returns>if inside of a box.</returns>
        public abstract bool IsOverlap(BoundBox boundBox);
        #endregion

        #region IIntersectable Members
        public abstract bool FindIntersection(Ray ray, out Intersection intersect);
        #endregion

        #region INameable Members
        public event NameChangedEventHandler OnNameChanged;
        public event NameChangingEventHandler OnNameChanging;
        public string Name
        {
            get { return this.name; }
            set
            {
                if(!String.IsNullOrEmpty(value)){
                    if(this.OnNameChanging != null){
                        CancelNameChageEventArgs cancel = new CancelNameChageEventArgs(value);
                        this.OnNameChanging(this, cancel);
                        if(cancel.Cancel){
                            throw new ArgumentException("Mudança de nome cancelada!");
                        }
                    }
                    string oldName = this.name;
                    this.name = value;
                    if(this.OnNameChanged != null){
                        this.OnNameChanged(this, oldName);
                    }
                }
            }
        }
        public int Compare(INameable x, INameable y)
        {
            return x.Name.CompareTo(y.Name);
        }
        #endregion

        public abstract bool IsInside(Point3D point);
        public abstract Vector3D NormalOnPoint(Point3D pointInPrimitive);
        public static DifferencePrimitive operator -(Primitive prim1, Primitive prim2)
        {
            DifferencePrimitive diff = new DifferencePrimitive(prim1, prim2);
            return diff;
        }
        public static UnionPrimitive operator +(Primitive prim1, Primitive prim2)
        {
            return null;
        }
        public static IntersectionPrimitive operator &(Primitive prim1, Primitive prim2)
        {
            IntersectionPrimitive interPri = new IntersectionPrimitive(prim1, prim2);
            return interPri;
        }
        public override string ToString()
        {
            return this.Name;
        }
        public override bool Equals(object obj)
        {
            Primitive prim = obj as Primitive;
            if(prim != null && !String.IsNullOrEmpty(prim.name) && !String.IsNullOrEmpty(this.name)){
                return (prim.name == this.name);
            }
            return base.Equals(obj);
        }
        //#region ITransformable3D Members
        //public abstract void Rotate(float angle, Vector3D axis);
        //public abstract void RotateAxisX(float angle);
        //public abstract void RotateAxisY(float angle);
        //public abstract void RotateAxisZ(float angle);
        //public abstract void Scale(float factor);
        //public abstract void Translate(float tx, float ty, float tz);
        //public abstract void Translate(Vector3D translateVector);
        //#endregion
    }
}