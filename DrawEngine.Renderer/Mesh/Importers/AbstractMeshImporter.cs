using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.Mesh.Importers
{
    public abstract class AbstractMeshImporter
    {
        public abstract List<String> RegisteredExtensions { get;}
        public abstract void Import(ref MeshModel mesh);
        public abstract event MeshModel.ElementLoadEventHandler OnElementLoaded;
    }
}
