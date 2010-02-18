using System.Runtime.InteropServices;
using System;
namespace DrawEngine.Renderer.Importers
{
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PointerToVertex
    {
        public int Vertex1;
        public int Vertex2;
        public int Vertex3;
        public PointerToVertex(int vertex1, int vertex2, int vertex3)
        {
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
            this.Vertex3 = vertex3;
        }
    }
}