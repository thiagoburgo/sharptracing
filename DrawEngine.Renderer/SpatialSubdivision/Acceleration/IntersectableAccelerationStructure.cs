using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DrawEngine.Renderer.BasicStructures;

namespace DrawEngine.Renderer.SpatialSubdivision.Acceleration {
    public abstract class IntersectableAccelerationStructure<T> : AccelerationStructure<T> where T : IIntersectable {
        protected IntersectableAccelerationStructure(IList<T> accelerationUnits) : base(accelerationUnits) {}

        #region IIntersectable Members

        /// <summary>
        /// Implement (if not override) a linear search to intersection accel units
        /// </summary>
        /// <param name="ray"> Ray from camera to scene</param>
        /// <param name="intersection">intersection information</param>
        /// <returns>true if a intersection found</returns>
        public virtual bool FindIntersection(Ray ray, out Intersection intersection) {
            intersection = new Intersection {TMin = float.MaxValue};
            Intersection intersection_comp;
            bool hit = false;
            foreach (T hitPrimitive in this.AccelerationUnits) {
                if (hitPrimitive.Visible && hitPrimitive.FindIntersection(ray, out intersection_comp) &&
                    intersection_comp.TMin < intersection.TMin) {
                    intersection = intersection_comp;
                    hit = true;
                }
            }
            return hit;
        }

        #endregion

    }
}