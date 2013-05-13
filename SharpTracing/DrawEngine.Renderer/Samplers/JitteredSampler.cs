using System;
using System.Collections.Generic;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Samplers
{
    public sealed class JitteredSampler : RegularGridSampler
    {
        public JitteredSampler() : base() { }
        public JitteredSampler(int numberOfSamples, int numberOfSets) : base(numberOfSamples, numberOfSets) { }

        //protected override void GenerateSamples()
        //{
        //    //samples.Clear();
        //    //int n = (int)Math.Sqrt(numberOfSamples);
        //    //Random random = new Random();
        //    //for (int p = 0; p < numberOfSets; p++)
        //    //    for (int j = 0; j < n; j++)
        //    //        for (int k = 0; k < n; k++)
        //    //        {
        //    //            samples.Add(new Point2D((k + (float)random.NextDouble()) / n, (j + (float)random.NextDouble()) / n));
        //    //        }

        //}

        private static float phi(int j)
        {
            float x = 0.0f;
            float f = 0.5f;
            while (j > 0)
            {
                x += f * (j % 2);
                j /= 2;
                f *= 0.5f;
            }
            return x;
        }

        protected override void GenerateSamples()
        {
            this.samples.Clear();
            for (int p = 0; p < this.numberOfSets; p++)
                for (int j = 0; j < numberOfSamples; j++)
                {
                    this.samples.Add(new Point2D(j / (float)numberOfSamples, phi(j)));
                }
        }
      
    }
}
