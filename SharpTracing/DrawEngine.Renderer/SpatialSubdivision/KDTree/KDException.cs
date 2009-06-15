using System;

namespace DrawEngine.Renderer.SpatialSubdivision.KDTree
{
    public class KDException : Exception
    {
        public KDException(String message) : base(message) {}
    }
}