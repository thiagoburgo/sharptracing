using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawEngine.Renderer.Util
{
    public static class RandomUtil
    {
        /// <summary>
        /// Shuffle the array. Fisher-Yates Shuffle.
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="array">Array to shuffle.</param>
        public static void Shuffle<T>(this T[] array)
        {
            Random random = new Random();
            for (int i = array.Length; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                // Swap.
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }
        /// <summary>
        /// Shuffle the array. Fisher-Yates Shuffle.
        /// </summary>
        /// <typeparam name="T">IList element type.</typeparam>
        /// <param name="array">IList to shuffle.</param>
        public static void Shuffle<T>(this IList<T> array)
        {
            Random random = new Random();
            for (int i = array.Count; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                // Swap.
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }
    }
}
