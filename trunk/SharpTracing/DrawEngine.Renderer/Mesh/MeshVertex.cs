using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.Algebra;
using System.Runtime.InteropServices;

namespace DrawEngine.Renderer.Mesh
{
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MeshVertex
    {
        public Point3D Position;
        public Vector3D Normal;

        public MeshVertex(Point3D position, Vector3D normal)
            : this(position)
        {
            Normal = normal;
        }
        public MeshVertex(Point3D position)
        {
            Position = position;
            Normal = Vector3D.Zero;
        }

        public override int GetHashCode()
        {
            long hc = GetLongHashCode();
            return (int)((int)hc ^ (int)(hc >> 32));
        }
        public long GetLongHashCode()
        {
            long hash  = Position.X.GetHashCode();
            hash = hash << 5;
            hash = hash ^ Normal.X.GetHashCode();
            hash = hash << 5;
            hash = hash ^ Position.Y.GetHashCode();
            hash = hash << 5;
            hash = hash ^ Position.Z.GetHashCode();
            hash = hash << 5;
            hash = hash ^ Normal.Z.GetHashCode();
            hash = hash << 5;
            return hash;
        }
        public static bool operator ==(MeshVertex vertex, MeshVertex other)
        {
            return other.Equals(vertex);
        }
        public static bool operator !=(MeshVertex vertex, MeshVertex other)
        {
            return !other.Equals(vertex);
        }
    }
}
