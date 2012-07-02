using System;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.PhotonMapping;
using System.Windows.Forms;
using Util.IO;
using Util.Conversion;
using System.IO;
using DrawEngine.Renderer.Util.IO;

namespace DrawEngine.ConsoleTests
{
    class Program
    {
       
        /// <summary>
        /// Get the asc line
        /// </summary>
        /// <returns>An string with the line values or null if EOF</returns>
        public static string GetLineInternal(BinaryReader br)
        {
            string line = null;
            sbyte c;

            while (br.PeekChar() != -1)
            {
                c = (sbyte)br.ReadByte();

                if (c == 10 || c == 13) break;

                line += (char)c;
            }

            // if the next character is 10 or 13 don't get it
            c = (sbyte)br.PeekChar();
            if (c == 10 || c == 13) c = (sbyte)br.ReadByte();

            return line;
        }
        
        static void Main(string[] args)
        {
            BinaryReader br = new BinaryReader(File.OpenRead("cube_bin2.ply"));
            String line = GetLineInternal(br);
            while (line != "end_header")
            {
                line = GetLineInternal(br);
            }
            for (int i = 0; i < 8; i++)
			{
                Console.WriteLine("X: " + br.ReadSingle());
                Console.WriteLine("Y: " + br.ReadSingle());
                Console.WriteLine("Z: " + br.ReadSingle());
			}
            
            
//            ebw.Write(@"ply
//format binary_little_endian 1.0
//comment VCGLIB generated
//element vertex 8
//property float x
//property float y
//property float z
//element face 12
//property list uchar int vertex_indices
//end_header");
            //ebw.Write(-1);
            //ebw.Write(-1);
            //ebw.Write(-1);

            //ebw.Write(1);
            //ebw.Write(-1);
            //ebw.Write(-1);

            //ebw.Write(1);
            //ebw.Write(1);
            //ebw.Write(-1);

            //ebw.Write(-1);
            //ebw.Write(1);
            //ebw.Write(-1);

            //ebw.Write(-1);
            //ebw.Write(-1);
            //ebw.Write(1);

            //ebw.Write(1);
            //ebw.Write(-1);
            //ebw.Write(1);

            //ebw.Write(1);
            //ebw.Write(1);
            //ebw.Write(1);

            //ebw.Write(-1);
            //ebw.Write(1);
            //ebw.Write(1);
            //ebw.Flush();
            //ebw.Close();
            

            //FormRandom formRandom = new FormRandom();
            //Application.Run(formRandom);
            //int max_photons = 500000;
            //PhotonMap map = new PhotonMap(max_photons);
            //DateTime antes = DateTime.Now;
            //for(int i = 0; i < max_photons; i++) {
            //    Random rdn = new Random();
            //    Point3D p;
            //    Vector3D dir;
            //    do {
            //        dir.X = (float)(-1 + 2 * rdn.NextDouble());
            //        dir.Y = (float)(-1 + 2 * rdn.NextDouble());
            //        dir.Z = (float)(-1 + 2 * rdn.NextDouble());
            //        p.X = (float)(-1 + 2 * rdn.NextDouble());
            //        p.Y = (float)(-1 + 2 * rdn.NextDouble());
            //        p.Z = (float)(-1 + 2 * rdn.NextDouble());
            //    } while(dir.X * dir.X + dir.Y * dir.Y + dir.Z * dir.Z > 1);
            //    map.Store(new Photon(dir, p, RGBColor.White));
            //}
            //map.Balance();
            //Console.WriteLine(DateTime.Now - antes);
            
            //RGBColor color = map.IrradianceEstimate(new Point3D(1, 0, 0), Vector3D.UnitY, 1.5f, 500);
            //Console.WriteLine(color);
        }
    }
}