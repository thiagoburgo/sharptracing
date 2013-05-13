using System.Collections.Generic;
using DrawEngine.Renderer.BasicStructures;

namespace DrawEngine.Renderer.SpatialSubdivision.Acceleration {
    public class NoAccerelationStructure<T> : IntersectableAccelerationStructure<T> where T : IIntersectable {
        public NoAccerelationStructure(IList<T> accelUnits) : base(accelUnits) {}
        public override void Optimize() {}
    }
}