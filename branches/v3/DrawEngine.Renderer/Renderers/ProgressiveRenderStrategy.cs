using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.Tracers;
using DrawEngine.Renderer.Util;

namespace DrawEngine.Renderer.Renderers
{
    public class ProgressiveRenderStrategy : RenderStrategy
    {
        public override void Render(RayCasting caster, IEnumerable<TiledBitmap.Tile> tiles)
        {
            #region Progressive Render from http://www.cc.gatech.edu/~phlosoft/photon/
            //float resX = caster.Scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            //float resY = caster.Scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;
            ManualResetEvent fineshedHandle = new ManualResetEvent(false);
            IEnumerable<TiledBitmap.Tile> imgTiles = tiles as IList<TiledBitmap.Tile> ?? tiles.ToList();

            int numberOfWorkers = imgTiles.Count();
            foreach (var imgTile in imgTiles)
            {
                
                caster = caster.Clone();
                Tuple<RayCasting, TiledBitmap.Tile> casterTile = new Tuple<RayCasting, TiledBitmap.Tile>(caster, imgTile);
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        Tuple<RayCasting, TiledBitmap.Tile> localCasterAndTile = state as Tuple<RayCasting, TiledBitmap.Tile>;
                        TiledBitmap.Tile tile = localCasterAndTile.Item2;
                        RayCasting caster1 = localCasterAndTile.Item1;

                        tile.CompleteCycle(TiledBitmap.RenderCycleType.Start);

                        int iterations = 0;
                        int pCol = 0, pRow = 0, pIteration = 1, pMax = 2;

                        float resTotal = tile.Width * tile.Height;
                        while (iterations < resTotal)
                        {
                            //Render Pixels Out of Order With Increasing Resolution: 2x2, 4x4, 16x16... 512x512
                            if (pCol >= pMax)
                            {
                                pRow++;
                                pCol = 0;
                                if (pRow >= pMax)
                                {
                                    pIteration++;
                                    pRow = 0;
                                    pMax <<= 1; //Equals: pMax = (int)Math.Pow(2, pIteration);
                                }
                            }
                            bool pNeedsDrawing = (pIteration == 1 || (pRow % 2 != 0) || (pRow % 2 == 0 && (pCol % 2 != 0)));
                            float x = pCol * (tile.Width / pMax) + tile.X;
                            float y = pRow * (tile.Height / pMax) + tile.Y;
                            pCol++;
                            if (pNeedsDrawing)
                            {
                                iterations++;
                                Ray ray;
                                RGBColor finalColor = RGBColor.Black;
                                if (caster1.Scene.Sampler.SamplesPerPixel > 1)
                                {

                                    foreach (Point2D sample in caster1.Scene.Sampler.GenerateSamples(x, y))
                                    {
                                        //ray = this.scene.DefaultCamera.CreateRayFromScreen(x + sample.X, y + sample.Y);
                                        ray = caster1.Scene.DefaultCamera.CreateRayFromScreen(sample.X , sample.Y);
                                        ray.PrevRefractIndex = caster1.Scene.RefractIndex;
                                        finalColor += caster1.Trace(ray, 0);
                                    }
                                    //brush.Color = (finalColor * (1f / this.scene.Sampler.SamplesPerPixel)).ToColor();
                                    finalColor = (finalColor / caster1.Scene.Sampler.SamplesPerPixel);


                                    //Graphics graphics = Graphics.FromImage(tile.Image);
                                    tile.Graphics.FillRectangle(new SolidBrush(finalColor.ToColor()), x - tile.X, y - tile.Y, (tile.Width / pMax), (tile.Height / pMax));
                                    //graphics.Flush();
                                    //graphics.Dispose();

                                }
                                else
                                {
                                    ray = caster1.Scene.DefaultCamera.CreateRayFromScreen(x , y );
                                    finalColor = caster1.Trace(ray, 0);

                                    // Debug.WriteLine("Graphics: " + tile.Graphics.GetHashCode());

                                    tile.Graphics.FillRectangle(new SolidBrush(finalColor.ToColor()), x - tile.X, y - tile.Y, (tile.Width / pMax), (tile.Height / pMax));

                                    //tile.Graphics.Flush(FlushIntention.Sync);
                                }
                                tile.CompleteCycle(TiledBitmap.RenderCycleType.Partial);
                            }
                        }
                        tile.CompleteCycle(TiledBitmap.RenderCycleType.Finish);
                    }
                    finally
                    {
                        if (Interlocked.Decrement(ref numberOfWorkers) == 0)
                        {
                            fineshedHandle.Set();
                        }
                    }

                }, casterTile);
            }//);
            fineshedHandle.WaitOne();
            #endregion
        }


    }
}
