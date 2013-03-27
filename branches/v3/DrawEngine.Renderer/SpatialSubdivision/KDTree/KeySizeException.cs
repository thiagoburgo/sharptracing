namespace DrawEngine.Renderer.SpatialSubdivision.KDTree {
    /// <summary>
    /// KeySizeException is thrown when a KDTree method is invoked on a
    ///key whose size (array length) mismatches the one used in the that
    ///KDTree's constructor.
    /// </summary>
    public class KeySizeException : KDException {
        public KeySizeException() : base("Key size mismatch") {}
    }
}