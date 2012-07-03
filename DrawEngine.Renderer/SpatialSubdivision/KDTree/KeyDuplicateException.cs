namespace DrawEngine.Renderer.SpatialSubdivision.KDTree
{
    /// <summary>
    ///KeyDuplicateException is thrown when the <code>KDTree.Insert</code> method
    ///is invoked on a key already in the KDTree.
    /// </summary>
    public class KeyDuplicateException : KDException
    {
        public KeyDuplicateException() : base("Key already in tree") {}
    }
}