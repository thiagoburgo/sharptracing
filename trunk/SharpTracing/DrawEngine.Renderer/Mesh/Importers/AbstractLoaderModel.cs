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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.Importers {
    public abstract class AbstractLoaderModel : IDisposable {
        #region Delegates/Events

        public delegate void ElementLoadEventHandler(int percentageOfTotal, ElementMesh element);

        public abstract event ElementLoadEventHandler OnElementLoaded;

        #endregion

        protected string path;
        private bool disposed;
        protected Triangle[] triangles;
        public BoundBox BoundBox = BoundBox.Zero;
        private static readonly Dictionary<string, AbstractLoaderModel> s_importers;
        public AbstractLoaderModel() : this("") {}

        public AbstractLoaderModel(String path) {
            this.path = path;
        }

        static AbstractLoaderModel() {
            // get all available importers
            s_importers = new Dictionary<string, AbstractLoaderModel>();
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type tp in ass.GetTypes()) {
                    if (!tp.IsAbstract && tp.IsClass && typeof (AbstractLoaderModel).IsAssignableFrom(tp)) {
                        AbstractLoaderModel importer = Activator.CreateInstance(tp) as AbstractLoaderModel;
                        if (importer == null) {
                            continue;
                        }
                        foreach (String ext in importer.Extensions) {
                            s_importers.Add(ext, importer);
                        }
                    }
                }
            }
        }

        public static AbstractLoaderModel GetLoader(String path) {
            AbstractLoaderModel import;
            String ext = Path.GetExtension(path);
            if (!s_importers.TryGetValue(Path.GetExtension(path), out import)) {
                throw new IOException("Loader not found for this file type. Extension: " + ext);
            }
            if (import != null) {
                import.path = path;
            }
            return import;
        }

        #region IDisposable Members

        public void Dispose() {
            if (!this.disposed) {
                this.triangles = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                this.disposed = true;
            }
        }

        #endregion

        public abstract List<String> Extensions { get; }
        public abstract Triangle[] Load();

        ~AbstractLoaderModel() {
            this.Dispose();
        }
    }
}