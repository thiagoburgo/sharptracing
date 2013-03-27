namespace DrawEngine.Renderer.Mathematics.QMCRandom {
    /// <summary>
    /// Interface for the Random Number Generators used bu the GACOM
    /// </summary>
    public interface IRandom {
        /// <summary>
        /// Reinitialize generator
        /// </summary>
        /// <param name="seed">seed</param>
        void ResetGenerator(int seed);

        /// <summary>
        /// Return next double
        /// </summary>
        /// <returns>a number between 0 and 1</returns>
        double NextDouble();
    }
}