// Key-size mismatch exception supporting KDTree class
namespace DrawEngine.Renderer.SpatialSubdivision.KDTree
{
    public class KeyMissingException : KDException
    {
        /* made public by MSL */
        public KeyMissingException() : base("Key not found") {}
    }
}