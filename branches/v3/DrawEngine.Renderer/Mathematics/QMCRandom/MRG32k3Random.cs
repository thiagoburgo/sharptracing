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
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Mathematics.QMCRandom {
    /// <summary>
    /// class TMRG32k3a : implements Multiple Recursive Generators developed by
    /// Pierre L'ecuyer [http://www.iro.umontreal.ca/~lecuyer/] in his papers
    /// titled "Good Parameter and Implementation For Combined Recursive Number
    /// Generators", Shorter version in Operations Research, 47, 1 (1999), 159--164
    ///
    /// period of MRG32k3a = 2^191  diferent values
    /// </summary>
    public sealed class MRG32k3aRandom : IRandom {
        #region Parameters

        /// <summary>
        /// Maximum int value
        /// </summary>
        private const int maxLong = int.MaxValue;

        private readonly double m1; // modules of component 1
        private readonly double m2; // modules of component 2
        private readonly double norm; // normalization factor in order to samples ~ [0..1[
        private readonly double a12; // coefficient for component 1
        private readonly double a13n; // coefficient for component 1
        private readonly double a21; // coefficient for component 2
        private readonly double a23n; // coefficient for component 2
        private int k; // seed for component 2
        private double p1; // auxiliar variable for combination
        private double p2; // auxiliar variable for combination
        private double s10; // seed for component 1
        private double s11; // seed for component 1
        private double s12; // seed for component 1
        private double s20; // seed for component 2
        private double s21; // seed for component 2
        private double s22; // seed for component 2

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize the generator
        /// </summary>
        /// <param name="seed">Generator seed</param>
        public MRG32k3aRandom(double seed) {
            try {
                if ((seed < 0) | (seed >= 1)) {
                    throw new Exception("seed out of range, it must be [0..1[");
                }
                // Initialize the paramethers
                this.m1 = 4294967087;
                this.m2 = 4294944443;
                this.a12 = 1403580;
                this.a13n = 810728;
                this.a21 = 527612;
                this.a23n = 1370589;
                this.norm = 2.328306549295728e-10;
                if (!seed.NearZero()) {
                    this.s10 = this.s11 = this.s12 = this.s20 = this.s21 = this.s22 = (seed * maxLong);
                } else {
                    this.s10 = this.s11 = this.s12 = this.s20 = this.s21 = this.s22 = 12345;
                }
            } catch (Exception e) {
                throw new Exception("MRG32k3aRandom:" + e.Message);
            }
        }

        /// <summary>
        /// Inicialize the generator
        /// </summary>
        public MRG32k3aRandom() {
            try {
                // Initialize the paramethers
                this.m1 = 4294967087;
                this.m2 = 4294944443;
                this.a12 = 1403580;
                this.a13n = 810728;
                this.a21 = 527612;
                this.a23n = 1370589;
                this.norm = 2.328306549295728e-10;
                int milliSecond = DateTime.Now.Millisecond;
                double seed = ((milliSecond / 1000.0) * maxLong);
                if (!seed.NearZero()) {
                    this.s10 = this.s11 = this.s12 = this.s20 = this.s21 = this.s22 = seed;
                } else {
                    this.s10 = this.s11 = this.s12 = this.s20 = this.s21 = this.s22 = 12345;
                }
            } catch (Exception e) {
                throw new Exception("MRG32k3aRandom:" + e.Message);
            }
        }

        #endregion

        #region IRandom Members

        /// <summary>
        /// Reinicialize the generator
        /// </summary>
        /// <param name="seed">Generator seed</param>
        public void ResetGenerator(int seed) {
            try {
                if ((seed < 0) | (seed >= 1)) {
                    throw new Exception("seed out of range, it must be [0..1[");
                }
                if (seed != 0) {
                    this.s10 = this.s11 = this.s12 = this.s20 = this.s21 = this.s22 = (double) (seed * maxLong);
                } else {
                    this.s10 = this.s11 = this.s12 = this.s20 = this.s21 = this.s22 = 12345;
                }
            } catch (Exception e) {
                throw new Exception("MRG32k3aRandom:" + e.Message);
            }
        }

        /// <summary>
        /// Retorns a random double
        /// </summary>
        /// <returns>valor entre 0 e 1</returns>
        public double NextDouble() {
            double result;
            double resultAux;
            /* Component 1 */
            this.p1 = this.a12 * this.s11 - this.a13n * this.s10;
            this.k = (int) (this.p1 / this.m1);
            this.p1 -= this.k * this.m1;
            if (this.p1 < 0.0) {
                this.p1 += this.m1;
            }
            this.s10 = this.s11;
            this.s11 = this.s12;
            this.s12 = this.p1;
            /* Component 2 */
            this.p2 = this.a21 * this.s22 - this.a23n * this.s20;
            this.k = (int) (this.p2 / this.m2);
            this.p2 -= this.k * this.m2;
            if (this.p2 < 0.0) {
                this.p2 += this.m2;
            }
            this.s20 = this.s21;
            this.s21 = this.s22;
            this.s22 = this.p2;
            /* Combination */
            if (this.p1 <= this.p2) {
                resultAux = (double) ((this.p1 - this.p2 + this.m1) * this.norm);
            } else {
                resultAux = (double) ((this.p1 - this.p2) * this.norm);
            }
            result = resultAux;
            return (result);
        }

        #endregion
    }
}