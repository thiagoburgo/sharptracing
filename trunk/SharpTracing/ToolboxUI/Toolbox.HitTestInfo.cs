using System.Diagnostics;
using System.Drawing;

namespace TooboxUI.Components {
    partial class Toolbox {
        #region Nested type: HitTestInfo

        /// <summary>
        /// Contains information about an area of a <see cref="Toolbox"/> control. 
        /// </summary>
        public sealed class HitTestInfo {
            private HitArea _hitArea;
            private readonly Point _hitPoint;
            private IToolboxObject _hitTool;
            internal HitTestInfo(Point location) : this(location, HitArea.None, null) {}

            internal HitTestInfo(Point location, HitArea area, IToolboxObject tool) {
                this._hitPoint = location;
                this._hitArea = area;
                this._hitTool = tool;
            }

            /// <summary>
            /// Gets the <see cref="Toolbox.HitArea"/> that represents the area of the toolbox evaluated by the hit-test operation.
            /// </summary>
            public HitArea HitArea {
                [DebuggerStepThrough] get { return this._hitArea; }
                [DebuggerStepThrough] internal set { this._hitArea = value; }
            }

            /// <summary>
            /// Gets the point that was hit-tested.
            /// </summary>
            public Point Location {
                [DebuggerStepThrough] get { return this._hitPoint; }
            }

            /// <summary>
            /// Gets the <see cref="Tab"/> or the <see cref="Item"/> object that was hit if any.
            /// </summary>
            public IToolboxObject Tool {
                [DebuggerStepThrough] get { return this._hitTool; }
                [DebuggerStepThrough] internal set { this._hitTool = value; }
            }
        }

        #endregion
    }
}