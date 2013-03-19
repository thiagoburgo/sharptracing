/////////////////////////////////////////////////////////////////////////////////
// Paint.NET
// Copyright (C) Rick Brewster, Chris Crosetto, Dennis Dietrich, Tom Jackson, 
//               Michael Kelsey, Brandon Ortiz, Craig Taylor, Chris Trevino, 
//               and Luke Walker
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.
// See src/setup/License.rtf for complete licensing and attribution information.
/////////////////////////////////////////////////////////////////////////////////
using System.Drawing;

namespace DrawEngine.Renderer.Animator.Quantizer
{
    public sealed class PaletteTable
    {
        private Color[] palette;
        public PaletteTable(Color[] palette)
        {
            this.palette = (Color[])palette.Clone();
        }
        public Color this[int index]
        {
            get { return this.palette[index]; }
            set { this.palette[index] = value; }
        }
        private int GetDistanceSquared(Color a, Color b)
        {
            int dsq = 0; // delta squared
            int v;
            v = a.B - b.B;
            dsq += v * v;
            v = a.G - b.G;
            dsq += v * v;
            v = a.R - b.R;
            dsq += v * v;
            return dsq;
        }
        public int FindClosestPaletteIndex(Color pixel)
        {
            int dsqBest = int.MaxValue;
            int ret = 0;
            for(int i = 0; i < this.palette.Length; ++i){
                int dsq = this.GetDistanceSquared(this.palette[i], pixel);
                if(dsq < dsqBest){
                    dsqBest = dsq;
                    ret = i;
                }
            }
            return ret;
        }
    }
}