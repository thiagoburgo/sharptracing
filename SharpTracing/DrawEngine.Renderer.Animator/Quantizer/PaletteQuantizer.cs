/////////////////////////////////////////////////////////////////////////////////
// Paint.NET
// Copyright (C) Rick Brewster, Chris Crosetto, Dennis Dietrich, Tom Jackson, 
//               Michael Kelsey, Brandon Ortiz, Craig Taylor, Chris Trevino, 
//               and Luke Walker
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.
// See src/setup/License.rtf for complete licensing and attribution information.
/////////////////////////////////////////////////////////////////////////////////
// Based on: http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnaspp/html/colorquant.asp

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace DrawEngine.Renderer.Animator.Quantizer {
    /// <summary>
    /// Summary description for PaletteQuantizer.
    /// </summary>
    internal unsafe class PaletteQuantizer : Quantizer {
        /// <summary>
        /// Lookup table for colors
        /// </summary>
        private readonly Dictionary<uint, byte> _colorMap;

        /// <summary>
        /// List of all colors in the palette
        /// </summary>
        private readonly Color[] _colors;

        /// <summary>
        /// Construct the palette quantizer
        /// </summary>
        /// <param name="palette">The color palette to quantize to</param>
        /// <remarks>
        /// Palette quantization only requires a single quantization step
        /// </remarks>
        public PaletteQuantizer(List<Color> palette) : base(true) {
            this._colorMap = new Dictionary<uint, byte>();
            this._colors = new Color[palette.Count];
            palette.CopyTo(this._colors);
        }

        /// <summary>
        /// Override this to process the pixel in the second pass of the algorithm
        /// </summary>
        /// <param name="pixel">The pixel to quantize</param>
        /// <returns>The quantized value</returns>
        protected override byte QuantizePixel(ColorBgra* pixel) {
            byte colorIndex = 0;
            uint colorHash = pixel->Bgra;
            // Check if the color is in the lookup table
            if (this._colorMap.ContainsKey(colorHash)) {
                colorIndex = this._colorMap[colorHash];
            } else {
                // Not found - loop through the palette and find the nearest match.
                // Firstly check the alpha value - if 0, lookup the transparent color
                if (0 == pixel->A) {
                    // Transparent. Lookup the first color with an alpha value of 0
                    for (int index = 0; index < this._colors.Length; index++) {
                        if (0 == this._colors[index].A) {
                            colorIndex = (byte) index;
                            break;
                        }
                    }
                } else {
                    // Not transparent...
                    int leastDistance = int.MaxValue;
                    int red = pixel->R;
                    int green = pixel->G;
                    int blue = pixel->B;
                    // Loop through the entire palette, looking for the closest color match
                    for (int index = 0; index < this._colors.Length; index++) {
                        Color paletteColor = this._colors[index];
                        int redDistance = paletteColor.R - red;
                        int greenDistance = paletteColor.G - green;
                        int blueDistance = paletteColor.B - blue;
                        int distance = (redDistance * redDistance) + (greenDistance * greenDistance) +
                                       (blueDistance * blueDistance);
                        if (distance < leastDistance) {
                            colorIndex = (byte) index;
                            leastDistance = distance;
                            // And if it's an exact match, exit the loop
                            if (0 == distance) {
                                break;
                            }
                        }
                    }
                }
                // Now I have the color, pop it into the hashtable for next time
                this._colorMap.Add(colorHash, colorIndex);
            }
            return colorIndex;
        }

        /// <summary>
        /// Retrieve the palette for the quantized image
        /// </summary>
        /// <param name="palette">Any old palette, this is overrwritten</param>
        /// <returns>The new color palette</returns>
        protected override ColorPalette GetPalette(ColorPalette palette) {
            for (int index = 0; index < this._colors.Length; index++) {
                palette.Entries[index] = this._colors[index];
            }
            return palette;
        }
    }
}