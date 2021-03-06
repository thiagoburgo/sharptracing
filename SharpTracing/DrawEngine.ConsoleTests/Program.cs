using System;
using System.IO;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.ConsoleTests {
    internal class Program {
        /// <summary>
        /// Get the asc line
        /// </summary>
        /// <returns>An string with the line values or null if EOF</returns>
        public static string GetLineInternal(BinaryReader br) {
            string line = null;
            sbyte c;

            while (br.PeekChar() != -1) {
                c = (sbyte) br.ReadByte();

                if (c == 10 || c == 13) {
                    break;
                }

                line += (char) c;
            }

            // if the next character is 10 or 13 don't get it
            c = (sbyte) br.PeekChar();
            if (c == 10 || c == 13) {
                c = (sbyte) br.ReadByte();
            }

            return line;
        }

        private static void Main(string[] args) {
            DateTime antes = DateTime.Now;

            Point3D[] large = new Point3D[1024 * 1024];
            for (int i = 0; i < large.Length; i++) {
                large[i] = new Point3D(i, i, i);
            }
            Console.WriteLine(DateTime.Now - antes);

            //RGBColor color = map.IrradianceEstimate(new Point3D(1, 0, 0), Vector3D.UnitY, 1.5f, 500);
            //Console.WriteLine(color);
        }
    }
}