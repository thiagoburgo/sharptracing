using System.Collections.Generic;
using System.Linq;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.SpatialSubdivision.Acceleration
{
    public abstract class IntersectableAccelerationStructure<T> : AccelerationStructure<T>
            where T : IIntersectable
    {
        protected IntersectableAccelerationStructure(IList<T> accelerationUnits) : base(accelerationUnits) {}

        #region IIntersectable Members
        /// <summary>
        /// Implement (if not override) a linear search to intersection accel units
        /// </summary>
        /// <param name="ray"> Ray from camera to scene</param>
        /// <param name="intersection">intersection information</param>
        /// <returns>true if a intersection found</returns>
        public virtual bool FindIntersection(Ray ray, out Intersection intersection)
        {
            intersection = new Intersection();
            intersection.TMin = double.MaxValue;
            Intersection intersection_comp;
            bool hit = false;
            foreach(T hitPrimitive in this.AccelerationUnits){
                if(hitPrimitive.Visible && hitPrimitive.FindIntersection(ray, out intersection_comp)
                   && intersection_comp.TMin < intersection.TMin){
                    intersection = intersection_comp;
                    hit = true;
                }
            }
            return hit;
        }
        #endregion

        public override abstract void Optimize();
    }
}