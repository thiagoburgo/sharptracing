using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DrawEngine.Renderer.Mesh.Importers;
using DrawEngine.Renderer.SpatialSubdivision.Acceleration;

namespace DrawEngine.Renderer.Mesh {
    public static class MeshModelLoader {
        public delegate void EndBuildEventHandler(TimeSpan timeToBuild);

        public delegate void InitBuildEventHandler(MeshModel mesh);

        private static IntersectableAccelerationStructure<MeshTriangle> manager;

        public static event MeshModel.ElementLoadEventHandler OnElementLoaded {
            add {
                foreach (AbstractMeshImporter import in s_importers.Values) {
                    import.OnElementLoaded += value;
                }
            }
            remove {
                foreach (AbstractMeshImporter import in s_importers.Values) {
                    import.OnElementLoaded -= value;
                }
            }
        }

        public static event InitBuildEventHandler OnInitBuild;
        public static event EndBuildEventHandler OnEndBuild;
        private static readonly Dictionary<string, AbstractMeshImporter> s_importers;

        static MeshModelLoader() {
            // get all available importers
            s_importers = new Dictionary<string, AbstractMeshImporter>();
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type tp in ass.GetTypes()) {
                    if (!tp.IsAbstract && tp.IsClass && typeof (AbstractMeshImporter).IsAssignableFrom(tp)) {
                        AbstractMeshImporter importer = Activator.CreateInstance(tp) as AbstractMeshImporter;
                        if (importer == null) {
                            continue;
                        }
                        foreach (String ext in importer.RegisteredExtensions) {
                            s_importers.Add(ext, importer);
                        }
                    }
                }
            }
        }

        public static void Import(ref MeshModel mesh) {
            AbstractMeshImporter import;
            String ext = Path.GetExtension(mesh.FilePath);
            if (!s_importers.TryGetValue(Path.GetExtension(mesh.FilePath), out import)) {
                throw new IOException("MeshImport not found for this file type. Extension: " + ext);
            }
            import.Import(ref mesh);
            //float len = Math.Abs(mesh.BoundBox.PMax.Y - mesh.BoundBox.PMin.Y);
            //mesh.Scale(50 / len);
            //mesh.Translate(-mesh.BoundBox.Center.X, -mesh.BoundBox.Center.Y, -mesh.BoundBox.Center.Z);

            //manager = new NoAccerelationStructure<MeshTriangle>(mesh.Triangles);
            manager = new Octree<MeshTriangle>(mesh.BoundBox, mesh.Triangles);
            //manager = new TriangleKDTree(new List<MeshTriangle>(mesh.Triangles));
            mesh.AccelerationManager = manager;
            DateTime antes = DateTime.Now;
            if (OnInitBuild != null) {
                OnInitBuild(mesh);
            }
            manager.Optimize();
            if (OnEndBuild != null) {
                OnEndBuild(DateTime.Now.Subtract(antes));
            }
        }
    }
}