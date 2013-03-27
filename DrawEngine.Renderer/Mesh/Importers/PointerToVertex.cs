using System;
using System.Runtime.InteropServices;

namespace DrawEngine.Renderer.Importers {
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PointerToVertex {
        public int Vertex1;
        public int Vertex2;
        public int Vertex3;

        public PointerToVertex(int vertex1, int vertex2, int vertex3) {
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
            this.Vertex3 = vertex3;
        }

        /// <summary>
        /// Zero Based
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int this[int index] {
            get {
                if (index > 2) {
                    index = index % 3;
                }
                if (index == 0) {
                    return this.Vertex1;
                } else if (index == 1) {
                    return this.Vertex2;
                } else {
                    return this.Vertex3;
                }
            }
            set {
                if (index > 2) {
                    index = index % 3;
                }
                if (index == 0) {
                    this.Vertex1 = value;
                } else if (index == 1) {
                    this.Vertex2 = value;
                } else {
                    this.Vertex3 = value;
                }
            }
        }
    }
}