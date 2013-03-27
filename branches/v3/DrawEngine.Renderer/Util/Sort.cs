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

namespace DrawEngine.Renderer.Util {
    public static class Sort {
        #region QuickSort

        public static void QuickSort<T>(this T[] array) where T : IComparable<T> {
            Sorting(array, 0, array.Length - 1);
        }

        private static int GetPivotPoint<T>(T[] input, int begPoint, int endPoint) where T : IComparable<T> {
            int pivot = begPoint;
            int m = begPoint + 1;
            int n = endPoint;
            while ((m < endPoint) && (input[pivot].CompareTo(input[m]) >= 0)) {
                m++;
            }
            while ((n > begPoint) && (input[pivot].CompareTo(input[n]) <= 0)) {
                n--;
            }
            while (m < n) {
                T temp = input[m];
                input[m] = input[n];
                input[n] = temp;
                while ((m < endPoint) && (input[pivot].CompareTo(input[m]) >= 0)) {
                    m++;
                }
                while ((n > begPoint) && (input[pivot].CompareTo(input[n]) <= 0)) {
                    n--;
                }
            }
            if (pivot != n) {
                T temp2 = input[n];
                input[n] = input[pivot];
                input[pivot] = temp2;
            }
            return n;
        }

        public static void Sorting<T>(T[] input, int beg, int end) where T : IComparable<T> {
            if (end == beg) {
                return;
            } else {
                int pivot = GetPivotPoint(input, beg, end);
                if (pivot > beg) {
                    Sorting(input, beg, pivot - 1);
                }
                if (pivot < end) {
                    Sorting(input, pivot + 1, end);
                }
            }
        }

        #endregion

        #region InsertionSort

        public static void InsertionSort<T>(this T[] a) where T : IComparable<T> {
            int i;
            int j;
            T val;
            for (i = 1; i < a.Length; i++) {
                val = a[i];
                j = i;
                while ((j > 0) && (a[j - 1].CompareTo(val) > 0)) {
                    a[j] = a[j - 1];
                    j = j - 1;
                }
                a[j] = val;
            }
        }

        #endregion
    }
}