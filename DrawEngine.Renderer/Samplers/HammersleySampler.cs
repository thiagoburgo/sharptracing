using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Samplers
{
    public class HammersleySampler : Sampler
    {
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
            for (int p = 0; p < this.numberOfSets; p++)
                for (int j = 0; j < numberOfSamples; j++)
                {
                    this.samples.Add(new Point2D(j / (float)numberOfSamples, phi(j)));
                }
        }
    }
}
