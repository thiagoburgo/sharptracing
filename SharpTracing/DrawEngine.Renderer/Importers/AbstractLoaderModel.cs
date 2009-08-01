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