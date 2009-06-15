using System;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.Importers
{
    public abstract class AbstractLoaderModel : IDisposable
    {
        #region Delegates
        public delegate void ElementLoadEventHandler(int percentageOfTotal, ElementMesh element);
        #endregion

        protected BoundBox boundBox;
        private bool disposed;
        protected Triangle[] triangles;
        public BoundBox BoundBox
        {
            get { return this.boundBox; }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if(!this.disposed){
                this.triangles = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                this.disposed = true;
            }
        }
        #endregion

        public abstract Triangle[] Load();
        public abstract event ElementLoadEventHandler OnElementLoaded;
        ~AbstractLoaderModel()
        {
            this.Dispose();
        }
        //public Point3D[] Vertices {
        //    get { return vertices; }
        //}
        //public Triangle[] Triangles {
        //    get { return triangles; }
        //}
    }
}