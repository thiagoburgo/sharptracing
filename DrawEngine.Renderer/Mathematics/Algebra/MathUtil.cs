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
namespace DrawEngine.Renderer.Mathematics.Algebra {
    public static class MathUtil {
        public const double Epsilon = 0.5e-5;

        public static bool IsEqual(this double x, double y) {
            return (x - y).NearZero();
        }

        public static bool IsEqual(this double x, float y) {
            return (x - y).NearZero();
        }

        public static bool IsEqual(this float x, float y) {
            return (x - y).NearZero();
        }

        public static bool IsEqual(this float x, double y) {
            return (x - y).NearZero();
        }

        public static bool NearZero(this double x) {
            return (x >= -Epsilon && x <= Epsilon);
        }

        public static bool NearZero(this double x, double tolerance) {
            return (x >= -tolerance && x <= tolerance);
        }

        public static bool NearZero(this float x) {
            return (x >= -Epsilon && x <= Epsilon);
        }

        public static bool NearZero(this float x, double tolerance) {
            return (x >= -tolerance && x <= tolerance);
        }

        public static float ConvertDegreeToRadians(this float degree) {
            return (float) (Math.PI * 1.0 / 180.0) * degree;
        }

        public static float ConvertRadiansToDegree(this float rad) {
            return (float) (180.0 / Math.PI) * rad;
        }

        public static float Lerp(float t, float v1, float v2) {
            return (1.0f - t) * v1 + t * v2;
        }

        public static float Clamp(float val, float low, float high) {
            if (val < low) {
                return low;
            }
            if (val > high) {
                return high;
            }
            return val;
        }

        public static int Clamp(int val, int low, int high) {
            if (val < low) {
                return low;
            }
            if (val > high) {
                return high;
            }
            return val;
        }

        public static int Mod(int a, int b) {
            int n = a / b;
            a -= n * b;
            if (a < 0) {
                a += b;
            }
            return a;
        }
    }
}