using System;
using System.Collections.Generic;

namespace DrawEngine.Renderer.SpatialSubdivision.Acceleration {
    public abstract class AccelerationStructure<T> : IDisposable {
        protected IList<T> accelerationUnits;

        public AccelerationStructure(IList<T> accelerationUnits) {
            this.AccelerationUnits = accelerationUnits;
        }

        public IList<T> AccelerationUnits {
            get { return this.accelerationUnits; }
            set {
                //if (value != null)
                //{
                this.accelerationUnits = value;
                //}
                //else
                //{
                //    throw new ArgumentNullException("AccelerationUnits", "Collection of AccelerationUnits cannot be null!");
                //}
            }
        }

        public abstract void Optimize();

        #region IDisposable Members

        public virtual void Dispose() {
            this.accelerationUnits = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        #endregion
    }
}