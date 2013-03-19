/**
 * Criado por: Thiago Burgo Belo (thiagoburgo@gmail.com)
 * SharpTracing é um projeto feito inicialmente para disciplina
 * Computação Gráfica da UFPE e depois melhorado nos tempos livres
 * sinta-se a vontade para copiar modificar e mandar correções e 
 * sugestões. Mantenha os créditos!
 * **************************************************************
 * SharpTracing is a project originally created to discipline 
 * Computer Graphics of UFPE and was improved in my free time.
 * Feel free to copy, modify and  give fixes 
 * suggestions. Keep the credits!
 */
 using System;

namespace DrawEngine.Renderer.Mathematics.QMCRandom
{
    /// <summary>
    /// implements the Sobol Quasi Monte Carlo generator algorithm
    ///
    /// All the theory can be found in:
    /// "Simulação de Quase Monte Carlo: quebrando a maldição da dimensionalidade"
    /// and
    /// "Algorithm 659 Implementing Sobol's QuasiRandon Sequence Generator"
    ///
    /// </summary>
    public sealed class QMCSobolRandom : IRandom
    {
        #region Parameters
        /// <summary>
        /// Number of primitives polynomes
        /// </summary>
        private const int numbPolynomes = 100;
        /// <summary>
        /// Randon Number Generator
        /// </summary>
        private readonly IRandom numberGenerator;
        /// <summary>
        /// Direct numbers
        /// </summary>
        private double[] directNumbers;
        /// <summary>
        /// m0
        /// </summary>
        private int[] m0;
        /// <summary>
        /// Current State
        /// </summary>
        private int numSequence;
        /// <summary>
        /// List of primitives polynomes
        /// </summary>
        private int[][] primPolynomes;
        /// <summary>
        /// Seed
        /// </summary>
        private int seedRandom;
        /// <summary>
        /// Primitive polynome used
        /// </summary>
        private int[] usedPolynome;
        /// <summary>
        /// vInt
        /// </summary>
        private double[] vInt;
        /// <summary>
        /// xSobol
        /// </summary>
        private uint xSobol;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicialize the generator
        /// </summary>
        /// <param name="seed">Generator seed</param>
        public QMCSobolRandom(double seed)
        {
            try{
                if((seed < 0) | (seed >= 1)){
                    throw new Exception("seed out of range, it must be [0..1[");
                }
                this.seedRandom = (int)(seed * numbPolynomes);
                this.numberGenerator = new MRG32k3aRandom();
                this.Initialize();
            } catch(Exception e){
                throw new Exception("QMCSobolRandom:" + e.Message);
            }
        }
        /// <summary>
        /// Inicialize the generator
        /// </summary>
        public QMCSobolRandom()
        {
            try{
                int milliSecond = DateTime.Now.Millisecond;
                this.seedRandom = (int)((milliSecond / 1000.0) * numbPolynomes);
                this.numberGenerator = new MRG32k3aRandom();
                this.Initialize();
            } catch(Exception e){
                throw new Exception("QMCSobolRandom:" + e.Message);
            }
        }
        #endregion

        #region Auxiliar Methods
        /// <summary>
        /// Initialize the parameters
        /// </summary>
        private void Initialize()
        {
            this.numSequence = 1;
            this.xSobol = 0;
            this.GetPolynomes();
            this.GetInit();
            this.DirectSobol(53);
        }
        /// <summary>
        /// Set the dimension to be used
        /// </summary>
        /// <param name="seedUsed">The seed used</param>
        public void SetDimension(int seedUsed)
        {
            this.seedRandom = (int)(seedUsed * numbPolynomes);
        }
        /// <summary>
        /// Get the polynomes from a file and creates an array
        /// </summary>
        private void GetPolynomes()
        {
            this.primPolynomes = new int[numbPolynomes][];
            this.primPolynomes[0] = new int[]{1, 1};
            this.primPolynomes[1] = new int[]{1, 1, 1};
            this.primPolynomes[2] = new int[]{1, 1, 0, 1};
            this.primPolynomes[3] = new int[]{1, 0, 1, 1};
            this.primPolynomes[4] = new int[]{1, 1, 0, 0, 1};
            this.primPolynomes[5] = new int[]{1, 0, 0, 1, 1};
            this.primPolynomes[6] = new int[]{1, 0, 1, 0, 0, 1};
            this.primPolynomes[7] = new int[]{1, 0, 0, 1, 0, 1};
            this.primPolynomes[8] = new int[]{1, 1, 1, 1, 0, 1};
            this.primPolynomes[9] = new int[]{1, 1, 1, 0, 1, 1};
            this.primPolynomes[10] = new int[]{1, 1, 0, 1, 1, 1};
            this.primPolynomes[11] = new int[]{1, 0, 1, 1, 1, 1};
            this.primPolynomes[12] = new int[]{1, 1, 0, 0, 0, 0, 1};
            this.primPolynomes[13] = new int[]{1, 0, 0, 0, 0, 1, 1};
            this.primPolynomes[14] = new int[]{1, 1, 0, 1, 1, 0, 1};
            this.primPolynomes[15] = new int[]{1, 1, 1, 0, 0, 1, 1};
            this.primPolynomes[16] = new int[]{1, 0, 1, 1, 0, 1, 1};
            this.primPolynomes[17] = new int[]{1, 1, 0, 0, 1, 1, 1};
            this.primPolynomes[18] = new int[]{1, 1, 0, 0, 0, 0, 0, 1};
            this.primPolynomes[19] = new int[]{1, 0, 0, 1, 0, 0, 0, 1};
            this.primPolynomes[20] = new int[]{1, 0, 0, 0, 1, 0, 0, 1};
            this.primPolynomes[21] = new int[]{1, 0, 0, 0, 0, 0, 1, 1};
            this.primPolynomes[22] = new int[]{1, 1, 1, 1, 0, 0, 0, 1};
            this.primPolynomes[23] = new int[]{1, 0, 1, 1, 1, 0, 0, 1};
            this.primPolynomes[24] = new int[]{1, 1, 1, 0, 0, 1, 0, 1};
            this.primPolynomes[25] = new int[]{1, 1, 0, 1, 0, 1, 0, 1};
            this.primPolynomes[26] = new int[]{1, 0, 0, 1, 1, 1, 0, 1};
            this.primPolynomes[27] = new int[]{1, 1, 0, 1, 0, 0, 1, 1};
            this.primPolynomes[28] = new int[]{1, 1, 0, 0, 1, 0, 1, 1};
            this.primPolynomes[29] = new int[]{1, 0, 1, 0, 1, 0, 1, 1};
            this.primPolynomes[30] = new int[]{1, 0, 1, 0, 0, 1, 1, 1};
            this.primPolynomes[31] = new int[]{1, 0, 0, 0, 1, 1, 1, 1};
            this.primPolynomes[32] = new int[]{1, 1, 1, 1, 1, 1, 0, 1};
            this.primPolynomes[33] = new int[]{1, 1, 1, 1, 0, 1, 1, 1};
            this.primPolynomes[34] = new int[]{1, 1, 1, 0, 1, 1, 1, 1};
            this.primPolynomes[35] = new int[]{1, 0, 1, 1, 1, 1, 1, 1};
            this.primPolynomes[36] = new int[]{1, 0, 1, 1, 1, 0, 0, 0, 1};
            this.primPolynomes[37] = new int[]{1, 1, 0, 1, 0, 1, 0, 0, 1};
            this.primPolynomes[38] = new int[]{1, 0, 1, 1, 0, 1, 0, 0, 1};
            this.primPolynomes[39] = new int[]{1, 0, 1, 1, 0, 0, 1, 0, 1};
            this.primPolynomes[40] = new int[]{1, 1, 0, 0, 0, 1, 1, 0, 1};
            this.primPolynomes[41] = new int[]{1, 0, 1, 0, 0, 1, 1, 0, 1};
            this.primPolynomes[42] = new int[]{1, 0, 0, 1, 0, 1, 1, 0, 1};
            this.primPolynomes[43] = new int[]{1, 0, 0, 0, 1, 1, 1, 0, 1};
            this.primPolynomes[44] = new int[]{1, 1, 1, 0, 0, 0, 0, 1, 1};
            this.primPolynomes[45] = new int[]{1, 0, 1, 1, 0, 0, 0, 1, 1};
            this.primPolynomes[46] = new int[]{1, 0, 0, 1, 0, 1, 0, 1, 1};
            this.primPolynomes[47] = new int[]{1, 1, 0, 0, 0, 0, 1, 1, 1};
            this.primPolynomes[48] = new int[]{1, 1, 1, 1, 1, 0, 1, 0, 1};
            this.primPolynomes[49] = new int[]{1, 1, 1, 1, 0, 0, 1, 1, 1};
            this.primPolynomes[50] = new int[]{1, 1, 1, 0, 0, 1, 1, 1, 1};
            this.primPolynomes[51] = new int[]{1, 0, 1, 0, 1, 1, 1, 1, 1};
            this.primPolynomes[52] = new int[]{1, 0, 0, 0, 1, 0, 0, 0, 0, 1};
            this.primPolynomes[53] = new int[]{1, 0, 0, 0, 0, 1, 0, 0, 0, 1};
            this.primPolynomes[54] = new int[]{1, 1, 0, 1, 1, 0, 0, 0, 0, 1};
            this.primPolynomes[55] = new int[]{1, 0, 1, 1, 0, 1, 0, 0, 0, 1};
            this.primPolynomes[56] = new int[]{1, 1, 0, 0, 1, 1, 0, 0, 0, 1};
            this.primPolynomes[57] = new int[]{1, 0, 0, 1, 1, 0, 1, 0, 0, 1};
            this.primPolynomes[58] = new int[]{1, 0, 0, 1, 0, 1, 1, 0, 0, 1};
            this.primPolynomes[59] = new int[]{1, 1, 1, 0, 0, 0, 0, 1, 0, 1};
            this.primPolynomes[60] = new int[]{1, 0, 1, 0, 1, 0, 0, 1, 0, 1};
            this.primPolynomes[61] = new int[]{1, 1, 0, 0, 0, 1, 0, 1, 0, 1};
            this.primPolynomes[62] = new int[]{1, 0, 1, 0, 0, 1, 0, 1, 0, 1};
            this.primPolynomes[63] = new int[]{1, 0, 0, 0, 1, 0, 1, 1, 0, 1};
            this.primPolynomes[64] = new int[]{1, 1, 0, 0, 1, 0, 0, 0, 1, 1};
            this.primPolynomes[65] = new int[]{1, 0, 1, 0, 1, 0, 0, 0, 1, 1};
            this.primPolynomes[66] = new int[]{1, 1, 0, 0, 0, 1, 0, 0, 1, 1};
            this.primPolynomes[67] = new int[]{1, 0, 0, 0, 1, 1, 0, 0, 1, 1};
            this.primPolynomes[68] = new int[]{1, 0, 0, 0, 0, 1, 1, 0, 1, 1};
            this.primPolynomes[69] = new int[]{1, 0, 1, 0, 0, 0, 0, 1, 1, 1};
            this.primPolynomes[70] = new int[]{1, 1, 1, 1, 1, 0, 1, 0, 0, 1};
            this.primPolynomes[71] = new int[]{1, 1, 1, 1, 0, 1, 1, 0, 0, 1};
            this.primPolynomes[72] = new int[]{1, 1, 1, 0, 1, 1, 1, 0, 0, 1};
            this.primPolynomes[73] = new int[]{1, 0, 1, 1, 1, 1, 1, 0, 0, 1};
            this.primPolynomes[74] = new int[]{1, 1, 1, 1, 0, 1, 0, 1, 0, 1};
            this.primPolynomes[75] = new int[]{1, 1, 1, 0, 1, 1, 0, 1, 0, 1};
            this.primPolynomes[76] = new int[]{1, 0, 1, 1, 1, 1, 0, 1, 0, 1};
            this.primPolynomes[77] = new int[]{1, 1, 1, 1, 0, 0, 1, 1, 0, 1};
            this.primPolynomes[78] = new int[]{1, 1, 0, 1, 1, 0, 1, 1, 0, 1};
            this.primPolynomes[79] = new int[]{1, 0, 1, 0, 1, 1, 1, 1, 0, 1};
            this.primPolynomes[80] = new int[]{1, 0, 0, 1, 1, 1, 1, 1, 0, 1};
            this.primPolynomes[81] = new int[]{1, 1, 1, 1, 1, 0, 0, 0, 1, 1};
            this.primPolynomes[82] = new int[]{1, 1, 0, 1, 1, 1, 0, 0, 1, 1};
            this.primPolynomes[83] = new int[]{1, 1, 1, 1, 0, 0, 1, 0, 1, 1};
            this.primPolynomes[84] = new int[]{1, 1, 0, 1, 1, 0, 1, 0, 1, 1};
            this.primPolynomes[85] = new int[]{1, 1, 0, 1, 0, 1, 1, 0, 1, 1};
            this.primPolynomes[86] = new int[]{1, 0, 1, 1, 0, 1, 1, 0, 1, 1};
            this.primPolynomes[87] = new int[]{1, 1, 0, 0, 1, 1, 1, 0, 1, 1};
            this.primPolynomes[88] = new int[]{1, 1, 1, 1, 0, 0, 0, 1, 1, 1};
            this.primPolynomes[89] = new int[]{1, 0, 1, 0, 1, 1, 0, 1, 1, 1};
            this.primPolynomes[90] = new int[]{1, 0, 0, 1, 1, 1, 0, 1, 1, 1};
            this.primPolynomes[91] = new int[]{1, 1, 1, 0, 0, 0, 1, 1, 1, 1};
            this.primPolynomes[92] = new int[]{1, 1, 0, 1, 0, 0, 1, 1, 1, 1};
            this.primPolynomes[93] = new int[]{1, 0, 1, 1, 0, 0, 1, 1, 1, 1};
            this.primPolynomes[94] = new int[]{1, 0, 1, 0, 1, 0, 1, 1, 1, 1};
            this.primPolynomes[95] = new int[]{1, 0, 0, 1, 1, 0, 1, 1, 1, 1};
            this.primPolynomes[96] = new int[]{1, 1, 0, 0, 0, 1, 1, 1, 1, 1};
            this.primPolynomes[97] = new int[]{1, 0, 0, 1, 0, 1, 1, 1, 1, 1};
            this.primPolynomes[98] = new int[]{1, 1, 1, 1, 1, 1, 1, 0, 1, 1};
            this.primPolynomes[99] = new int[]{1, 1, 0, 1, 1, 1, 1, 1, 1, 1};
        }
        /// <summary>
        /// Calculates the first values
        /// </summary>
        private void GetInit()
        {
            int degree;
            degree = this.primPolynomes[this.seedRandom - 1].Length;
            this.usedPolynome = new int[degree];
            this.usedPolynome = this.primPolynomes[this.seedRandom - 1];
            this.m0 = new int[degree - 1];
            for(int i = 0; i < this.m0.Length; i++){
                do{
                    this.m0[i] = (int)Math.Round(this.numberGenerator.NextDouble() * Math.Pow(2, i));
                } while((this.m0[i] % 2) == 0);
            }
        }
        /// <summary>
        /// Calculate all direct numbers
        /// </summary>
        private void DirectSobol(int nd)
        {
            int d = this.usedPolynome.Length - 1;
            int[] m = new int[nd];
            this.directNumbers = new double[nd];
            for(int i = 0; i < this.m0.Length; i++){
                m[i] = this.m0[i];
            }
            // compute direct numbers...
            for(int i = 0; i < d; i++){
                this.directNumbers[i] = (double)(this.m0[i]) / Math.Pow(2, (i + 1));
            }
            for(int i = d; i < nd; i++){
                int tmp = (int)(m[i - d] * Math.Pow(2, d));
                m[i] = m[i - d] ^ tmp;
                for(int j = d - 2; j >= 0; j--){
                    int tmp2 = this.usedPolynome[j + 1] * m[i - j - 1] * (int)Math.Pow(2, j + 1);
                    m[i] ^= tmp2;
                }
                this.directNumbers[i] = (double)m[i] / Math.Pow(2, i + 1);
            }
            this.vInt = new double[nd];
            double pot = Math.Pow(2.0, 32);
            for(int i = 0; i < this.vInt.Length; i++){
                this.vInt[i] = this.directNumbers[i] * pot;
            }
        }
        #endregion

        #region IRandom Members
        /// <summary>
        /// Reinicialize the generator
        /// </summary>
        /// <param name="seed">Generator seed</param>
        public void ResetGenerator(int seed)
        {
            try{
                if((seed < 0) | (seed >= 1)){
                    throw new Exception("seed out of range, it must be [0..1[");
                }
                this.seedRandom = (int)(seed * numbPolynomes);
                this.Initialize();
            } catch(Exception e){
                throw new Exception("QMCSobolRandom:" + e.Message);
            }
        }
        /// <summary>
        /// Returns a random double
        /// </summary>
        /// <returns>value between 0 and 1</returns>
        public double NextDouble()
        {
            if(this.numSequence == 0){
                this.numSequence++;
                this.xSobol = (uint)Math.Floor(0 * Math.Pow(2, 32));
                return this.xSobol / Math.Pow(2.0, 32);
            } else{
                int mask = 0x001;
                int c;
                int aux = this.numSequence - 1;
                int aux2 = (mask) & (aux);
                for(c = 0; aux2 > 0; c++){
                    mask *= 2;
                    aux2 = (mask) & (aux);
                }
                this.xSobol ^= (uint)this.vInt[c];
                this.numSequence++;
                return this.xSobol / Math.Pow(2, 32);
            }
        }
        #endregion
    }
}