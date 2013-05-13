using System;
using System.Collections.Generic;

namespace DrawEngine.Renderer.SpatialSubdivision.Acceleration {
    public abstract class AccelerationStructure<T> : IDisposable, IAccelerationStructure
    {
        protected IList<T> accelerationUnits;

        protected AccelerationStructure(IList<T> accelerationUnits) {
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

      
        #region IDisposable Members

        public virtual void Dispose() {
            this.accelerationUnits = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        #endregion

        public abstract void Optimize();
    }
}