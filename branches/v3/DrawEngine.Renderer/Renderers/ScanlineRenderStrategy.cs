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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.Tracers;
using DrawEngine.Renderer.Util;
using System.Linq;

namespace DrawEngine.Renderer.Renderers
{

    [Serializable]
    public class ScanlineRenderStrategy : RenderStrategy
    {
        private ManualResetEvent cancelHandle = new ManualResetEvent(false);
        private long numberOfWorkers = 0;
        public override void CancelRender()
        {
            if (Interlocked.Read(ref numberOfWorkers) != 0)
            {
                base.CancelRender();
                cancelHandle.WaitOne();
            }
        }
        public override void Render(RayCasting caster, IEnumerable<TiledBitmap.Tile> tiles)
        {
            //float resX = caster.Scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            //float resY = caster.Scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;
            //Parallel.ForEach(tiles, tile => {
            ManualResetEvent fineshedHandle = new ManualResetEvent(false);
            IEnumerable<TiledBitmap.Tile> imgTiles = tiles as IList<TiledBitmap.Tile> ?? tiles.ToList();

            numberOfWorkers = imgTiles.Count();
            foreach (var imgTile in imgTiles)
            {
                caster = caster.Clone();
                Tuple<RayCasting, TiledBitmap.Tile> casterTile = new Tuple<RayCasting, TiledBitmap.Tile>(caster, imgTile);
                //new Thread(state => 
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        Tuple<RayCasting, TiledBitmap.Tile> localCasterAndTile = state as Tuple<RayCasting, TiledBitmap.Tile>;
                        TiledBitmap.Tile tile = localCasterAndTile.Item2;
                        RayCasting caster1 = localCasterAndTile.Item1;
                        //Stopwatch timer = new Stopwatch();
                        //timer.Start();
                        //Console.WriteLine("Started@" + DateTime.Now.ToString("mm:ss.fff tt"));


                        float total = tile.Width * tile.Height;
                        float partial = 0;
                        for (int y = tile.Y; y < tile.Y + tile.Height; y++)
                        {
                            if (cancelRender) {
                                break;
                            }
                            for (int x = tile.X; x < tile.X + tile.Width; x++)
                            {
                                if (cancelRender)
                                {
                                    break;
                                }
                                Ray ray;
                                RGBColor finalColor = RGBColor.Black;
                                if (caster1.Scene.Sampler.SamplesPerPixel > 1)
                                {
                                    foreach (Point2D sample in caster1.Scene.Sampler.GenerateSamples(x, y))
                                    {
                                        ray = caster1.Scene.DefaultCamera.CreateRayFromScreen(sample.X, sample.Y);
                                        ray.PrevRefractIndex = caster1.Scene.RefractIndex;
                                        finalColor += caster1.Trace(ray, 0);
                                    }
                                    finalColor = (finalColor * 1f / caster1.Scene.Sampler.SamplesPerPixel);
                                }
                                else
                                {
                                    ray = caster1.Scene.DefaultCamera.CreateRayFromScreen(x, y);
                                    finalColor = caster1.Trace(ray, 0);
                                }

                                tile.Image.SetPixel(x - tile.X, y - tile.Y, finalColor.ToColor());
                                //unsafe
                                //{
                                //    Bitmap img = tile.Image;
                                //    BitmapData imgData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
                                //    byte bitsPerPixel = Texture.GetBitsPerPixel(imgData.PixelFormat);
                                //    byte* scan0 = (byte*)imgData.Scan0.ToPointer();
                                //    byte* data = scan0 + (x - tile.X) * imgData.Stride + (y - tile.Y) * bitsPerPixel / 8;

                                //    data[0] = (byte)(finalColor.R * 255);
                                //    data[1] = (byte)(finalColor.G * 255);
                                //    data[2] = (byte)(finalColor.B * 255);
                                //    img.UnlockBits(imgData);    
                                //}

                            }
                            partial += tile.Height;
                            if (((partial / total) * 100) >= 20)
                            {
                                partial = 0;
                                tile.CompleteCycle(TiledBitmap.RenderCycleType.Partial);
                            }
                        }
                        tile.CompleteCycle(TiledBitmap.RenderCycleType.Finish);

                        //timer.Stop();
                    }
                    finally
                    {
                        if (Interlocked.Decrement(ref numberOfWorkers) == 0)
                        {
                            fineshedHandle.Set();
                            cancelHandle.Set();
                            cancelRender = false;
                        }
                    }
                }, casterTile);
                //}){IsBackground = false}.Start(casterTile);
            }//);
            cancelHandle.Set();
            fineshedHandle.WaitOne();
        }


    }
}