using System;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.PhotonMapping;
using System.Windows.Forms;

namespace DrawEngine.ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            //FormRandom formRandom = new FormRandom();
            //Application.Run(formRandom);
            int max_photons = 500000;
            PhotonMap map = new PhotonMap(max_photons);
            DateTime antes = DateTime.Now;
            for(int i = 0; i < max_photons; i++) {
                Random rdn = new Random();
                Point3D p;
                Vector3D dir;
                do {
                    dir.X = (float)(-1 + 2 * rdn.NextDouble());
                    dir.Y = (float)(-1 + 2 * rdn.NextDouble());
                    dir.Z = (float)(-1 + 2 * rdn.NextDouble());
                    p.X = (float)(-1 + 2 * rdn.NextDouble());
                    p.Y = (float)(-1 + 2 * rdn.NextDouble());
                    p.Z = (float)(-1 + 2 * rdn.NextDouble());
                } while(dir.X * dir.X + dir.Y * dir.Y + dir.Z * dir.Z > 1);
                map.Store(new Photon(dir, p, RGBColor.White));
            }
            map.Balance();
            Console.WriteLine(DateTime.Now - antes);
            
            //RGBColor color = map.IrradianceEstimate(new Point3D(1, 0, 0), Vector3D.UnitY, 1.5f, 500);
            //Console.WriteLine(color);
        }
    }
}