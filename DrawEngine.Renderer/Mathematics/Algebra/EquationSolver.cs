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

namespace DrawEngine.Renderer.Algebra
{
    public static class EquationSolver
    {
        public static int SolveLinear(float a, float b, out float root)
        {
            root = float.NaN;
            if(a == 0.0f){
                return 0;
            }
            if(b == 0.0f){
                root = 0.0f;
                return 1;
            }
            root = -b * 1.0f / a;
            return 1;
        }
        public static int SolveQuadric(float a, float b, float c, out float root1, out float root2)
        {
            float p, q, D;
            root1 = float.NaN;
            root2 = float.NaN;
            // make sure we have a d2 equation
            if(MathUtil.NearZero(a)){
                return SolveLinear(b, c, out root1);
            }
            // normal for: x^2 + px + q
            p = b * 1.0f / (a + a);
            q = c * 1.0f / a;
            D = p * p - q;
            if(MathUtil.NearZero(D)){
                // one double root
                root1 = root2 = -p;
                return 1;
            }
            if(D < 0.0f){
                // no real root
                return 0;
            } else{
                // two real roots
                float sqrt_D = (float)Math.Sqrt(D);
                root1 = sqrt_D - p;
                root2 = -sqrt_D - p;
                //ordena as raizes
                if(root1 > root2){
                    float temp = root1;
                    root1 = root2;
                    root2 = temp;
                }
                return 2;
            }
        }
        public static int SolveCubic(float a, float b, float c, float d, out float root1, out float root2,
                                     out float root3)
        {
            root1 = root2 = root3 = float.NaN;
            int num;
            double sub, A, B, C, sq_A, p, q, cb_p, D;
            // normalize the equation:x ^ 3 + Ax ^ 2 + Bx  + C = 0
            double inv_a = 1.0 / a;
            A = b * inv_a;
            B = c * inv_a;
            C = d * inv_a;
            // substitute x = y - A / 3 to eliminate the quadric term: x^3 + px + q = 0
            sq_A = A * A;
            //double one_3 = 0.33333333333333333333333333333333333333333333333333;
            double one_3 = 1 / 3;
            p = (float)(one_3 * (-one_3 * sq_A + B));
            q = (float)(0.5 * (2.0 / 27.0 * A * sq_A - one_3 * A * B + C));
            // use Cardano's formula
            cb_p = p * p * p;
            D = q * q + cb_p;
            if(MathUtil.NearZero(D)){
                if(MathUtil.NearZero(q)){
                    // one triple solution
                    root1 = 0.0f;
                    num = 1;
                } else{
                    // one single and one double solution
                    float u = (float)Math.Pow(-q, one_3);
                    root1 = u + u;
                    root2 = -u;
                    //if (root1 > root2) {
                    //    float temp = root1;
                    //    root1 = root2;
                    //    root2 = temp;
                    //}
                    num = 2;
                }
            } else if(D < 0.0f){
                // casus irreductibilis: three real solutions
                double phi = (one_3 * Math.Acos((-q / Math.Sqrt(-cb_p))));
                double t = (2.0 * Math.Sqrt(-p));
                root1 = (float)(t * Math.Cos(phi));
                root2 = (float)(-t * Math.Cos(phi + Math.PI * one_3));
                root3 = (float)(-t * Math.Cos(phi - Math.PI * one_3));
                //if (root2 > root3) {
                //    float temp = root2;
                //    root2 = root3;
                //    root3 = temp;
                //    if (root1 > root2) {
                //        float temp2 = root1;
                //        root1 = root2;
                //        root2 = temp2;
                //    }
                //}
                num = 3;
            } else{
                // one real solution
                double sqrt_D = Math.Sqrt(D);
                double u = Math.Pow(sqrt_D + Math.Abs(q), one_3);
                if(q > 0.0f){
                    root1 = (float)(-u + p * 1.0 / u);
                } else{
                    root1 = (float)(u - p * 1.0 / u);
                }
                num = 1;
            }
            // resubstitute
            sub = (one_3 * A);
            root1 -= (float)sub;
            root2 -= (float)sub;
            root3 -= (float)sub;
            return num;
        }
        public static int SolveQuartic(float a, float b, float c, float d, float e, out float root1, out float root2,
                                       out float root3, out float root4)
        {
            root1 = root2 = root3 = root4 = float.NaN;
            if(a == 0.0){
                throw new Exception(
                        "The coefficient of the fourth power of x is 0. Please use the utility for a THIRD degree quadratic. No further action  taken.");
            } //End if a == 0
            if(e == 0.0){
                //throw new Exception(
                //    "One root is 0. Now divide through by x and use the utility for a THIRD degree quadratic to solve the resulting   equation for the other three roots. No further action taken.");
            } //End if e == 0
            float inv_a = 1.0f / a;
            if(a != 1.0f){
                b *= inv_a;
                c *= inv_a;
                d *= inv_a;
                e *= inv_a;
            }
            double cb, cc, cd; // Coefficients for use with cubic solver
            double discrim, q, r, RRe, RIm, DRe, DIm, dum1, ERe, EIm, s, t, term1, r13, sqR, y1, z1Re, z1Im, z2Re;
            cb = -c;
            cc = -4.0 * e + d * b;
            cd = -(b * b * e + d * d) + 4.0 * c * e;
            //if (cd == 0) alert("cd = 0.");
            // Solve the resolvant cubic for y1
            q = (3.0 * cc - (cb * cb)) * (1.0 / 9.0);
            r = -(27.0 * cd) + cb * (9.0 * cc - 2.0 * (cb * cb));
            r *= 1.0 / 54.0;
            discrim = q * q * q + r * r;
            double one_3 = 0.333333333333333333333333333333333333;
            term1 = (cb * one_3);
            if(discrim > 0.0){
                // one root real, two are complex
                s = r + Math.Sqrt(discrim);
                s = ((s < 0.0) ? -Math.Pow(-s, one_3) : Math.Pow(s, one_3));
                t = r - Math.Sqrt(discrim);
                t = ((t < 0.0) ? -Math.Pow(-t, one_3) : Math.Pow(t, one_3));
                y1 = -term1 + s + t;
            } // End if (discrim > 0)
            else{
                if(discrim == 0.0){
                    r13 = ((r < 0.0) ? -Math.Pow(-r, one_3) : Math.Pow(r, one_3));
                    y1 = -term1 + 2.0 * r13;
                } // End if (discrim == 0)
                else{
                    // else discrim < 0
                    q = -q;
                    dum1 = q * q * q;
                    dum1 = Math.Acos((r * 1.0 / Math.Sqrt(dum1)));
                    r13 = 2.0 * Math.Sqrt(q);
                    y1 = -term1 + r13 * Math.Cos(dum1 * one_3);
                } // End discrim < 0
            } // End else discrim <= 0
            // At this point, we have determined y1, a real root of the resolvent cubic.
            // Carry on to solve the original quartic equation
            term1 = b * 0.25;
            sqR = -c + term1 * b + y1; // R-squared
            RRe = RIm = DRe = DIm = ERe = EIm = z1Re = z1Im = z2Re = 0.0;
            if(sqR >= 0){
                if(MathUtil.NearZero(sqR)){
                    dum1 = -(4.0 * e) + y1 * y1;
                    if(dum1 < 0) //D and E will be complex
                    {
                        z1Im = 2.0 * Math.Sqrt(-dum1);
                    } else{
                        //else (dum1 >= 0)
                        z1Re = 2.0 * Math.Sqrt(dum1);
                        z2Re = -z1Re;
                    } //End else (dum1 >= 0)
                } //End if (sqR == 0)
                else{
                    //(sqR > 0)
                    RRe = Math.Sqrt(sqR);
                    z1Re = -(8.0 * d + b * b * b) * 0.25 + b * c;
                    z1Re /= RRe;
                    z2Re = -z1Re;
                } // End else (sqR > 0)
            } //end if (sqR >= 0)
            else{
                //else (sqR < 0)
                RIm = Math.Sqrt(-sqR);
                z1Im = -(8.0 * d + b * b * b) / 4.0 + b * c;
                z1Im /= RIm;
                z1Im = -z1Im;
            } // End else (sqR < 0)
            z1Re += -(2.0 * c + sqR) + 3.0 * b * term1;
            z2Re += -(2.0 * c + sqR) + 3.0 * b * term1;
            //At this point, z1 and z2 should be the terms under the square root for D and E
            if(MathUtil.NearZero(z1Im)){
                // Both z1 and z2 real
                if(z1Re >= 0.0){
                    DRe = Math.Sqrt(z1Re);
                } else{
                    DIm = Math.Sqrt(-z1Re);
                }
                if(z2Re >= 0.0){
                    ERe = Math.Sqrt(z2Re);
                } else{
                    EIm = Math.Sqrt(-z2Re);
                }
            } // End if (zIm == 0)
            else{
                //else (zIm != 0); calculate root of a complex number********
                r = Math.Sqrt(z1Re * z1Re + z1Im * z1Im); // Calculate r, the magnitude
                r = Math.Sqrt(r);
                dum1 = Math.Atan2(z1Im, z1Re); // Calculate the angle between the two vectors
                dum1 = dum1 * 0.5; //Divide this angle by 2
                ERe = DRe = r * Math.Cos(dum1); //Form the new complex value
                DIm = r * Math.Sin(dum1);
                EIm = -DIm;
            } // End else (z1Im != 0)
            int numRoots = 0;
            double imgRoot = -(RIm + EIm) * 0.5;
            if(imgRoot == 0.0){
                root1 = (float)(-(term1 + (RRe + ERe) * 0.5));
                numRoots++;
                imgRoot = (-RIm + EIm) * 0.5;
                if(imgRoot == 0.0){
                    root2 = (float)(-(term1 + (RRe * 0.5)) + ERe * 0.5);
                    numRoots++;
                }
            } else{
                imgRoot = (-DIm + RIm) * 0.5;
                if(imgRoot == 0.0){
                    root1 = (float)(-(term1 + (DRe * 0.5)) + RRe * 0.5);
                    numRoots++;
                    imgRoot = (RIm + DIm) * 0.5;
                    if(imgRoot == 0.0){
                        root2 = (float)(-term1 + (RRe + DRe) * 0.5);
                        numRoots++;
                    }
                }
            }
            return numRoots;
        }
        /// <summary> Solves the equation ax^2+bx+c=0. Solutions are returned in a sorted array
        /// if they exist.
        /// 
        /// </summary>
        /// <param name="a">coefficient of x^2
        /// </param>
        /// <param name="b">coefficient of x^1
        /// </param>
        /// <param name="c">coefficient of x^0
        /// </param>
        /// <returns> an array containing the two real roots, or <code>null</code> if
        /// no real solutions exist
        /// </returns>
        public static float[] SolveQuadric(float a, float b, float c)
        {
            float disc = b * b - 4 * a * c;
            if(disc < 0){
                return null;
            }
            disc = (float)Math.Sqrt(disc);
            float q = ((b < 0) ? (-0.5f) * (b - disc) : (-0.5f) * (b + disc));
            float t0 = q / a;
            float t1 = c / q;
            // return sorted array
            return (t0 > t1) ? new float[]{t1, t0} : new float[]{t0, t1};
        }
        /// <summary> Solve a quartic equation of the form ax^4+bx^3+cx^2+cx^1+d=0. The roots
        /// are returned in a sorted array of floats in increasing order.
        /// 
        /// </summary>
        /// <param name="a">coefficient of x^4
        /// </param>
        /// <param name="b">coefficient of x^3
        /// </param>
        /// <param name="c">coefficient of x^2
        /// </param>
        /// <param name="d">coefficient of x^1
        /// </param>
        /// <param name="e">coefficient of x^0
        /// </param>
        /// <returns> a sorted array of roots, or <code>null</code> if no solutions
        /// exist
        /// </returns>
        public static float[] SolveQuartic(float a, float b, float c, float d, float e)
        {
            float inva = 1f / a;
            float c1 = b * inva;
            float c2 = c * inva;
            float c3 = d * inva;
            float c4 = e * inva;
            // cubic resolvant
            float c12 = c1 * c1;
            float p = (float)((-0.375) * c12 + c2);
            float q = (float)(0.125 * c12 * c1 - 0.5 * c1 * c2 + c3);
            float r = (float)((-0.01171875) * c12 * c12 + 0.0625 * c12 * c2 - 0.25 * c1 * c3 + c4);
            float z = SolveCubicForQuartic((-0.5f) * p, -r, 0.5f * r * p - 0.125f * q * q);
            float d1 = 2.0f * z - p;
            if(d1 < 0){
                if(d1 > 1.0e-10){
                    d1 = 0;
                } else{
                    return null;
                }
            }
            float d2;
            if(d1 < 1.0e-10){
                d2 = z * z - r;
                if(d2 < 0){
                    return null;
                }
                d2 = (float)Math.Sqrt(d2);
            } else{
                d1 = (float)Math.Sqrt(d1);
                d2 = 0.5f * q / d1;
            }
            // setup usefull values for the quadratic factors
            float q1 = d1 * d1;
            float q2 = (-0.25f) * c1;
            float pm = q1 - 4 * (z - d2);
            float pp = q1 - 4 * (z + d2);
            if(pm >= 0 && pp >= 0){
                // 4 roots (!)
                pm = (float)Math.Sqrt(pm);
                pp = (float)Math.Sqrt(pp);
                float[] results = new float[4];
                results[0] = (-0.5f) * (d1 + pm) + q2;
                results[1] = (-0.5f) * (d1 - pm) + q2;
                results[2] = 0.5f * (d1 + pp) + q2;
                results[3] = 0.5f * (d1 - pp) + q2;
                // tiny insertion sort
                for(int i = 1; i < 4; i++){
                    for(int j = i; j > 0 && results[j - 1] > results[j]; j--){
                        float t = results[j];
                        results[j] = results[j - 1];
                        results[j - 1] = t;
                    }
                }
                return results;
            } else if(pm >= 0){
                pm = (float)Math.Sqrt(pm);
                float[] results = new float[2];
                results[0] = (-0.5f) * (d1 + pm) + q2;
                results[1] = (-0.5f) * (d1 - pm) + q2;
                return results;
            } else if(pp >= 0){
                pp = (float)Math.Sqrt(pp);
                float[] results = new float[2];
                results[0] = 0.5f * (d1 - pp) + q2;
                results[1] = 0.5f * (d1 + pp) + q2;
                return results;
            }
            return null;
        }
        /// <summary> Return only one root for the specified cubic equation. This routine is
        /// only meant to be called by the quartic solver. It assumes the cubic is of
        /// the form: x^3+px^2+qx+r.
        /// 
        /// </summary>
        /// <param name="p">
        /// </param>
        /// <param name="q">
        /// </param>
        /// <param name="r">
        /// </param>
        /// <returns>
        /// </returns>
        private static float SolveCubicForQuartic(float p, float q, float r)
        {
            float A2 = p * p;
            float Q = (A2 - 3.0f * q) / 9.0f;
            float R = (p * (A2 - 4.5f * q) + 13.5f * r) / 27.0f;
            float Q3 = Q * Q * Q;
            float R2 = R * R;
            float d = Q3 - R2;
            float an = p / 3.0f;
            if(d >= 0){
                d = R / (float)Math.Sqrt(Q3);
                float theta = (float)Math.Acos(d) / 3.0f;
                float sQ = (-2.0f) * (float)Math.Sqrt(Q);
                return sQ * (float)Math.Cos(theta) - an;
            } else{
                float sQ = (float)Math.Pow(Math.Sqrt(R2 - Q3) + Math.Abs(R), 1.0 / 3.0);
                if(R < 0){
                    return (sQ + Q / sQ) - an;
                } else{
                    return -(sQ + Q / sQ) - an;
                }
            }
        }
    }
}