using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.Collections.Design;
using DrawEngine.Renderer.Mathematics.Algebra;
using System.Runtime.Serialization;

namespace DrawEngine.Renderer.RenderObjects {
    /// <summary>
    /// Describes a polygon primitive
    /// </summary>
    /// <remarks>The polygon must be convex.</remarks>
    [Serializable]
    public class Polygon : Primitive, IPreprocess, ITransformable3D {
        private const double maxAngle = ((2.0 * Math.PI) * 0.9999);
        protected float d;
        protected Vector3D normal;
        protected NotifyList<Point3D> vertices = new NotifyList<Point3D>();
        //Equação do plano ax+by+cz+d=0        


        public Polygon()
            : this(new List<Point3D>{new Point3D(-50, 0, 50),new Point3D(-50, 0, -50),
                                     new Point3D(50, 0, -50),new Point3D(50, 0, 50)}) { }
        public Polygon(ICollection<Point3D> vertices) : this(vertices, null) { }
        public Polygon(ICollection<Point3D> vertices, string name) {
            this.Name = name;
            this.Vertices = new NotifyList<Point3D>(vertices);
        }

        #region IPreprocess Members
        public virtual void Preprocess() {
            if(this.vertices == null || this.vertices.Count < 3) {
                throw new ArgumentException("Polygons must have at least 3 vertices.");
            }
            if(!this.IsCoplanar) {
                throw new ArgumentException("All vertices in a polygon must be coplanar");
            }
            this.normal = Vector3D.Normal(this.vertices[0], this.vertices[1], this.vertices[2]);
            this.d = -(this.normal.X * this.vertices[0].X) - (this.normal.Y * this.vertices[0].Y)
                     - (this.normal.Z * this.vertices[0].Z);
            if(this.normal.Length == 0.0f) {
                throw new ArgumentException("Polygon has a zero-length normal");
            }
            #region calculate bounding box
            //float
            //    minX = vertices[0].X,
            //    minY = vertices[0].Y,
            //    minZ = vertices[0].Z;
            //float
            //    maxX = vertices[0].X,
            //    maxY = vertices[0].Y,
            //    maxZ = vertices[0].Z;
            //for(int i = 0; i < vertices.Count; i++) {
            //    if(vertices[i].X < minX) {
            //        minX = vertices[i].X;
            //    }else if(vertices[i].X > maxX) {
            //        maxX = vertices[i].X;
            //    }
            //    if(vertices[i].Y < minY) {
            //        minY = vertices[i].Y;
            //    }
            //    else if(vertices[i].Y > maxY) {
            //        maxY = vertices[i].Y;
            //    }
            //    if(vertices[i].Z < minZ) {
            //        minZ = vertices[i].Z;
            //    }
            //    else if(vertices[i].Z > maxZ) {
            //        maxZ = vertices[i].Z;
            //    }
            //}
            //this.boundBox = new BoundBox(new Point3D(minX, minY, minZ), new Point3D(maxX, maxY, maxZ)); 
            #endregion
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the set of vertices used by this polygon
        /// </summary>
        [Description("Vertices in counter-clockwise order that make up polygon"),
         Editor(typeof(CustomCollectionEditor), typeof(UITypeEditor))]
        public NotifyList<Point3D> Vertices {
            get { return this.vertices; }
            set {
                if(value != null) {
                    if(value.Count < 3) {
                        throw new ArgumentOutOfRangeException("Vertices collection cannot be <= 3!");
                    } else {
                        this.vertices = value;
                        this.vertices.CollectionChanged += this.vertices_CollectionChanged;
                        //TODO Verificar quando um item for adicionado para poder recalcular o BBox
                        this.Preprocess();
                    }
                }
                else {
                    throw new ArgumentNullException("Vertices collection cannot be empty or <= 3!");
                }
            }
        }
        /// <summary>
        /// Gets the number of vertices in this polygon
        /// </summary>
        [Description("Number of vertices in polygon"), ReadOnly(true)]
        public int VerticesCount {
            get {
                if(this.vertices == null) {
                    return 0;
                }
                return this.vertices.Count;
            }
        }
        /// <summary>
        /// Determines if the polygon is planar
        /// </summary>
        protected virtual bool IsCoplanar {
            get {
                if(this.vertices == null) {
                    return false;
                }
                if(this.vertices.Count <= 3) {
                    return true;
                }
                Vector3D n0 = Vector3D.Normal(this.vertices[0], this.vertices[1], this.vertices[2]);
                for(int i = 1; i < this.vertices.Count - 2; i++) {
                    Vector3D ni = Vector3D.Normal(this.vertices[i], this.vertices[i + 1], this.vertices[i + 2]);
                    if((ni.X != n0.X) || (ni.Y != n0.Y) || (ni.Z != n0.Z)) {
                        return false;
                    }
                }
                return true;
            }
        }
        void vertices_CollectionChanged(object sender, NotifyCollectionChangedEventArgs<Point3D> e) {
            switch(e.Action) {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    this.Preprocess();
                    break;
            }
        }
        #endregion

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis) {
            this.vertices.NotificationsEnabled = false;
            for(int i = 0; i < this.vertices.Count; i++) {
                Point3D vertex = this.vertices[i];
                vertex.Rotate(angle, axis);
                this.vertices[i] = vertex;
            }
            this.vertices.NotificationsEnabled = true;
            this.Preprocess();
        }
        public void RotateAxisX(float angle) {
            this.Rotate(angle, Vector3D.UnitX);
        }
        public void RotateAxisY(float angle) {
            this.Rotate(angle, Vector3D.UnitY);
        }
        public void RotateAxisZ(float angle) {
            this.Rotate(angle, Vector3D.UnitZ);
        }
        public void Scale(float factor) {
            throw new NotImplementedException();
        }
        public void Translate(float tx, float ty, float tz) {
            this.vertices.NotificationsEnabled = false;
            for(int i = 0; i < this.vertices.Count; i++) {
                Point3D vertex = this.vertices[i];
                vertex.Translate(tx, ty, tz);
                this.vertices[i] = vertex;
            }
            this.vertices.NotificationsEnabled = true;
            this.Preprocess();
        }
        public void Translate(Vector3D translateVector) {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        public override bool IsOverlap(BoundBox boundBox) {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Calculates the distance from a ray to the polygon.
        /// </summary>
        /// <param name="ray">The ray to trace</param>
        /// <param name="intersect"></param>
        /// <returns>The distance from the ray to the polygon, or negative if no intersection</returns>
        public override bool FindIntersection(Ray ray, out Intersection intersect) {
            intersect = new Intersection();
            //t = -(N • Ro + D) / (N • Rd)	
            //Vector3D origin = ray.Origin.ToVector3D();
            float NRd = this.normal * ray.Direction;
            if(NRd == 0.0f) {
                return false;
            }
            float t = -(this.normal * ray.Origin.ToVector3D() + this.d) / NRd;
            if(t < 0.01f) {
                return false;
            }
            intersect.Normal = this.normal;
            intersect.HitPoint = ray.Origin + (t * ray.Direction);
            intersect.HitPrimitive = this;
            intersect.TMin = t;
            return this.PointIsInPolygon(intersect.HitPoint);
        }
        /// <summary>
        /// Returns the surface normal of the polygon
        /// </summary>
        /// <param name="point">Point at which to calculate the normal</param>
        /// <returns>Surface normal of the polygon</returns>
        public override Vector3D NormalOnPoint(Point3D point) {
            if((point * this.normal) < 0.0f) {
                return -this.normal;
            }
            return this.normal;
        }
        /// <summary>
        /// Determines if a point is in the polygon
        /// </summary>
        /// <param name="point">The point to test, in object coordinates</param>
        /// <returns>True if point is inside, false otherwise</returns>
        public bool PointIsInPolygon(Point3D point) {
            double angle = 0.0;
            Vector3D a, b;
            for(int i = 0; i < this.vertices.Count; i++) {
                a = this.vertices[i] - point;
                b = this.vertices[(i + 1) % this.vertices.Count] - point;
                a.Normalize();
                b.Normalize();
                double acos = Math.Acos(a * b);
                angle += Double.IsNaN(acos) ? 0 : acos;
            }
            if(angle >= maxAngle) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Determines if the point is inside the polygon
        /// </summary>
        /// <param name="point">The point to test</param>
        /// <returns>Returns true if the point is inside the polygon, false otherwise.</returns>
        /// <remarks>Polygon.IsInside() always returns false</remarks>
        public override bool IsInside(Point3D point) {
            return false;
        }
    }
}