using System.Collections.Generic;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.SpatialSubdivision.Acceleration
{
    public class KDTreePrimitiveManager : KDTreeIntersectableManager<Primitive>
    {
        public KDTreePrimitiveManager() : base()
        {
            this.MaxDesiredObjectsPerLeafCount = 2;
        }
        public KDTreePrimitiveManager(IList<Primitive> content) : base(content)
        {
            this.MaxDesiredObjectsPerLeafCount = 2;
        }
        protected override void SplitOnPlane(IList<Primitive> splitContent, Axis axis, Point3D position,
                                             out IList<Primitive> leftContent, out IList<Primitive> rightContent)
        {
            leftContent = new List<Primitive>();
            rightContent = new List<Primitive>();
            Primitive geoObj;
            switch(axis){
                case Axis.X:
                    for(int i = 0; i < splitContent.Count; i++){
                        geoObj = splitContent[i];
                        if(geoObj.BoundBox.Center.X - geoObj.BoundBox.HalfVector.Length <= position.X){
                            leftContent.Add(geoObj);
                        }
                        if(geoObj.BoundBox.Center.X + geoObj.BoundBox.HalfVector.Length >= position.X){
                            rightContent.Add(geoObj);
                        }
                    }
                    break;
                case Axis.Y:
                    for(int i = 0; i < splitContent.Count; i++){
                        geoObj = splitContent[i];
                        if(geoObj.BoundBox.Center.Y - geoObj.BoundBox.HalfVector.Length <= position.Y){
                            leftContent.Add(geoObj);
                        }
                        if(geoObj.BoundBox.Center.Y + geoObj.BoundBox.HalfVector.Length >= position.Y){
                            rightContent.Add(geoObj);
                        }
                    }
                    break;
                case Axis.Z:
                    for(int i = 0; i < splitContent.Count; i++){
                        geoObj = splitContent[i];
                        if(geoObj.BoundBox.Center.Z - geoObj.BoundBox.HalfVector.Length <= position.Z){
                            leftContent.Add(geoObj);
                        }
                        if(geoObj.BoundBox.Center.Z + geoObj.BoundBox.HalfVector.Length >= position.Z){
                            rightContent.Add(geoObj);
                        }
                    }
                    break;
            }
        }
        protected override Point3D CalculateMid(IList<Primitive> content)
        {
            Primitive currentObj = content[0];
            Point3D mid = currentObj.BoundBox.Center;
            for(int i = 1; i < content.Count; i++){
                currentObj = content[i];
                mid = (i / (i + 1f)) * mid + (1f / (i + 1f)) * currentObj.BoundBox.Center;
            }
            return mid;
        }
    }
}