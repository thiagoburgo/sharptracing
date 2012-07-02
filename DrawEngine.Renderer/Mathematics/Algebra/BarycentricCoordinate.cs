namespace DrawEngine.Renderer.Algebra
{
    public struct BarycentricCoordinate
    {
        public float Alpha;
        public float Beta;
        public float Gama;
        public BarycentricCoordinate(float alpha, float beta, float gama)
        {
            this.Alpha = alpha;
            this.Beta = beta;
            this.Gama = gama;
        }
        public bool IsValid
        {
            get { return (this.Alpha >= 0 && this.Beta >= 0 && this.Alpha + this.Beta <= 1); }
        }
    }
}