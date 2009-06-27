/////////////////////////////////////////////////////////////////////////////////
// Paint.NET
// Copyright (C) Rick Brewster, Chris Crosetto, Dennis Dietrich, Tom Jackson, 
//               Michael Kelsey, Brandon Ortiz, Craig Taylor, Chris Trevino, 
//               and Luke Walker
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.
// See src/setup/License.rtf for complete licensing and attribution information.
/////////////////////////////////////////////////////////////////////////////////
// Based on: http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnaspp/html/colorquant.asp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace DrawEngine.Renderer.Animator.Quantizer
{
    /// <summary>
    /// Quantize using an Octree
    /// </summary>
    internal unsafe class OctreeQuantizer : Quantizer
    {
        /// <summary>
        /// Maximum allowed color depth
        /// </summary>
        private int _maxColors;
        /// <summary>
        /// Stores the tree
        /// </summary>
        private Octree _octree;
        /// <summary>
        /// Construct the octree quantizer
        /// </summary>
        /// <remarks>
        /// The Octree quantizer is a two pass algorithm. The initial pass sets up the octree,
        /// the second pass quantizes a color based on the nodes in the tree
        /// </remarks>
        /// <param name="maxColors">The maximum number of colors to return</param>
        /// <param name="maxColorBits">The number of significant bits</param>
        public OctreeQuantizer(int maxColors, int maxColorBits) : base(false)
        {
            if(maxColors > 255){
                throw new ArgumentOutOfRangeException("maxColors", maxColors,
                                                      "The number of colors should be less than 256");
            }
            if((maxColorBits < 1) | (maxColorBits > 8)){
                throw new ArgumentOutOfRangeException("maxColorBits", maxColorBits, "This should be between 1 and 8");
            }
            this._octree = new Octree(maxColorBits);
            this._maxColors = maxColors;
        }
        /// <summary>
        /// Process the pixel in the first pass of the algorithm
        /// </summary>
        /// <param name="pixel">The pixel to quantize</param>
        /// <remarks>
        /// This function need only be overridden if your quantize algorithm needs two passes,
        /// such as an Octree quantizer.
        /// </remarks>
        protected override void InitialQuantizePixel(ColorBgra* pixel)
        {
            this._octree.AddColor(pixel);
        }
        /// <summary>
        /// Override this to process the pixel in the second pass of the algorithm
        /// </summary>
        /// <param name="pixel">The pixel to quantize</param>
        /// <returns>The quantized value</returns>
        protected override byte QuantizePixel(ColorBgra* pixel)
        {
            byte paletteIndex = (byte)this._maxColors; // The color at [_maxColors] is set to transparent
            // Get the palette index if this non-transparent
            if(pixel->A > 0){
                paletteIndex = (byte)this._octree.GetPaletteIndex(pixel);
            }
            return paletteIndex;
        }
        /// <summary>
        /// Retrieve the palette for the quantized image
        /// </summary>
        /// <param name="original">Any old palette, this is overrwritten</param>
        /// <returns>The new color palette</returns>
        protected override ColorPalette GetPalette(ColorPalette original)
        {
            // First off convert the octree to _maxColors colors
            List<Color> palette = this._octree.Palletize(this._maxColors - 1);
            // Then convert the palette based on those colors
            for(int index = 0; index < palette.Count; index++){
                original.Entries[index] = palette[index];
            }
            for(int i = palette.Count; i < original.Entries.Length; ++i){
                original.Entries[i] = Color.FromArgb(255, 0, 0, 0);
            }
            // Add the transparent color
            original.Entries[this._maxColors] = Color.FromArgb(0, 0, 0, 0);
            return original;
        }

        #region Nested type: Octree
        /// <summary>
        /// Class which does the actual quantization
        /// </summary>
        private class Octree
        {
            /// <summary>
            /// Mask used when getting the appropriate pixels for a given node
            /// </summary>
            private static int[] mask = new int[8]{0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01};
            /// <summary>
            /// Number of leaves in the tree
            /// </summary>
            private int _leafCount;
            /// <summary>
            /// Maximum number of significant bits in the image
            /// </summary>
            private int _maxColorBits;
            private Color[] _palette;
            /// <summary>
            /// Cache the previous color quantized
            /// </summary>
            private uint _previousColor;
            /// <summary>
            /// Store the last node quantized
            /// </summary>
            private OctreeNode _previousNode;
            /// <summary>
            /// Array of reducible nodes
            /// </summary>
            private OctreeNode[] _reducibleNodes;
            /// <summary>
            /// The root of the octree
            /// </summary>
            private OctreeNode _root;
            private PaletteTable paletteTable;
            /// <summary>
            /// Construct the octree
            /// </summary>
            /// <param name="maxColorBits">The maximum number of significant bits in the image</param>
            public Octree(int maxColorBits)
            {
                this._maxColorBits = maxColorBits;
                this._leafCount = 0;
                this._reducibleNodes = new OctreeNode[9];
                this._root = new OctreeNode(0, this._maxColorBits, this);
                this._previousColor = 0;
                this._previousNode = null;
            }
            /// <summary>
            /// Get/Set the number of leaves in the tree
            /// </summary>
            public int Leaves
            {
                get { return this._leafCount; }
                set { this._leafCount = value; }
            }
            /// <summary>
            /// Return the array of reducible nodes
            /// </summary>
            protected OctreeNode[] ReducibleNodes
            {
                get { return this._reducibleNodes; }
            }
            /// <summary>
            /// Add a given color value to the octree
            /// </summary>
            /// <param name="pixel"></param>
            public void AddColor(ColorBgra* pixel)
            {
                // Check if this request is for the same color as the last
                if(this._previousColor == pixel->Bgra){
                    // If so, check if I have a previous node setup. This will only ocurr if the first color in the image
                    // happens to be black, with an alpha component of zero.
                    if(null == this._previousNode){
                        this._previousColor = pixel->Bgra;
                        this._root.AddColor(pixel, this._maxColorBits, 0, this);
                    } else{
                        // Just update the previous node
                        this._previousNode.Increment(pixel);
                    }
                } else{
                    this._previousColor = pixel->Bgra;
                    this._root.AddColor(pixel, this._maxColorBits, 0, this);
                }
            }
            /// <summary>
            /// Reduce the depth of the tree
            /// </summary>
            public void Reduce()
            {
                int index;
                // Find the deepest level containing at least one reducible node
                for(index = this._maxColorBits - 1; (index > 0) && (null == this._reducibleNodes[index]); index--){
                    // intentionally blank
                }
                // Reduce the node most recently added to the list at level 'index'
                OctreeNode node = this._reducibleNodes[index];
                this._reducibleNodes[index] = node.NextReducible;
                // Decrement the leaf count after reducing the node
                this._leafCount -= node.Reduce();
                // And just in case I've reduced the last color to be added, and the next color to
                // be added is the same, invalidate the previousNode...
                this._previousNode = null;
            }
            /// <summary>
            /// Keep track of the previous node that was quantized
            /// </summary>
            /// <param name="node">The node last quantized</param>
            protected void TrackPrevious(OctreeNode node)
            {
                this._previousNode = node;
            }
            /// <summary>
            /// Convert the nodes in the octree to a palette with a maximum of colorCount colors
            /// </summary>
            /// <param name="colorCount">The maximum number of colors</param>
            /// <returns>A list with the palettized colors</returns>
            public List<Color> Palletize(int colorCount)
            {
                while(this.Leaves > colorCount){
                    this.Reduce();
                }
                // Now palettize the nodes
                List<Color> palette = new List<Color>(this.Leaves);
                int paletteIndex = 0;
                this._root.ConstructPalette(palette, ref paletteIndex);
                // And return the palette
                this._palette = palette.ToArray();
                this.paletteTable = null;
                return palette;
            }
            /// <summary>
            /// Get the palette index for the passed color
            /// </summary>
            /// <param name="pixel"></param>
            /// <returns></returns>
            public int GetPaletteIndex(ColorBgra* pixel)
            {
                int ret = -1;
                ret = this._root.GetPaletteIndex(pixel, 0);
                if(ret < 0){
                    if(this.paletteTable == null){
                        this.paletteTable = new PaletteTable(this._palette);
                    }
                    ret = this.paletteTable.FindClosestPaletteIndex(pixel->ToColor());
                }
                return ret;
            }

            #region Nested type: OctreeNode
            /// <summary>
            /// Class which encapsulates each node in the tree
            /// </summary>
            protected class OctreeNode
            {
                /// <summary>
                /// Blue component
                /// </summary>
                private int _blue;
                /// <summary>
                /// Pointers to any child nodes
                /// </summary>
                private OctreeNode[] _children;
                /// <summary>
                /// Green Component
                /// </summary>
                private int _green;
                /// <summary>
                /// Flag indicating that this is a leaf node
                /// </summary>
                private bool _leaf;
                /// <summary>
                /// Pointer to next reducible node
                /// </summary>
                private OctreeNode _nextReducible;
                /// <summary>
                /// The index of this node in the palette
                /// </summary>
                private int _paletteIndex;
                /// <summary>
                /// Number of pixels in this node
                /// </summary>
                private int _pixelCount;
                /// <summary>
                /// Red component
                /// </summary>
                private int _red;
                /// <summary>
                /// Construct the node
                /// </summary>
                /// <param name="level">The level in the tree = 0 - 7</param>
                /// <param name="colorBits">The number of significant color bits in the image</param>
                /// <param name="octree">The tree to which this node belongs</param>
                public OctreeNode(int level, int colorBits, Octree octree)
                {
                    // Construct the new node
                    this._leaf = (level == colorBits);
                    this._red = 0;
                    this._green = 0;
                    this._blue = 0;
                    this._pixelCount = 0;
                    // If a leaf, increment the leaf count
                    if(this._leaf){
                        octree.Leaves++;
                        this._nextReducible = null;
                        this._children = null;
                    } else{
                        // Otherwise add this to the reducible nodes
                        this._nextReducible = octree.ReducibleNodes[level];
                        octree.ReducibleNodes[level] = this;
                        this._children = new OctreeNode[8];
                    }
                }
                /// <summary>
                /// Get/Set the next reducible node
                /// </summary>
                public OctreeNode NextReducible
                {
                    get { return this._nextReducible; }
                    set { this._nextReducible = value; }
                }
                /// <summary>
                /// Return the child nodes
                /// </summary>
                public OctreeNode[] Children
                {
                    get { return this._children; }
                }
                /// <summary>
                /// Add a color into the tree
                /// </summary>
                /// <param name="pixel">The color</param>
                /// <param name="colorBits">The number of significant color bits</param>
                /// <param name="level">The level in the tree</param>
                /// <param name="octree">The tree to which this node belongs</param>
                public void AddColor(ColorBgra* pixel, int colorBits, int level, Octree octree)
                {
                    // Update the color information if this is a leaf
                    if(this._leaf){
                        this.Increment(pixel);
                        // Setup the previous node
                        octree.TrackPrevious(this);
                    } else{
                        // Go to the next level down in the tree
                        int shift = 7 - level;
                        int index = ((pixel->R & mask[level]) >> (shift - 2))
                                    | ((pixel->G & mask[level]) >> (shift - 1)) | ((pixel->B & mask[level]) >> (shift));
                        OctreeNode child = this._children[index];
                        if(null == child){
                            // Create a new child node & store in the array
                            child = new OctreeNode(level + 1, colorBits, octree);
                            this._children[index] = child;
                        }
                        // Add the color to the child node
                        child.AddColor(pixel, colorBits, level + 1, octree);
                    }
                }
                /// <summary>
                /// Reduce this node by removing all of its children
                /// </summary>
                /// <returns>The number of leaves removed</returns>
                public int Reduce()
                {
                    int children = 0;
                    this._red = 0;
                    this._green = 0;
                    this._blue = 0;
                    // Loop through all children and add their information to this node
                    for(int index = 0; index < 8; index++){
                        if(null != this._children[index]){
                            this._red += this._children[index]._red;
                            this._green += this._children[index]._green;
                            this._blue += this._children[index]._blue;
                            this._pixelCount += this._children[index]._pixelCount;
                            ++children;
                            this._children[index] = null;
                        }
                    }
                    // Now change this to a leaf node
                    this._leaf = true;
                    // Return the number of nodes to decrement the leaf count by
                    return (children - 1);
                }
                /// <summary>
                /// Traverse the tree, building up the color palette
                /// </summary>
                /// <param name="palette">The palette</param>
                /// <param name="paletteIndex">The current palette index</param>
                public void ConstructPalette(List<Color> palette, ref int paletteIndex)
                {
                    if(this._leaf){
                        // Consume the next palette index
                        this._paletteIndex = paletteIndex++;
                        // And set the color of the palette entry
                        int r = this._red / this._pixelCount;
                        int g = this._green / this._pixelCount;
                        int b = this._blue / this._pixelCount;
                        palette.Add(Color.FromArgb(r, g, b));
                    } else{
                        // Loop through children looking for leaves
                        for(int index = 0; index < 8; index++){
                            if(null != this._children[index]){
                                this._children[index].ConstructPalette(palette, ref paletteIndex);
                            }
                        }
                    }
                }
                /// <summary>
                /// Return the palette index for the passed color
                /// </summary>
                public int GetPaletteIndex(ColorBgra* pixel, int level)
                {
                    int paletteIndex = this._paletteIndex;
                    if(!this._leaf){
                        int shift = 7 - level;
                        int index = ((pixel->R & mask[level]) >> (shift - 2))
                                    | ((pixel->G & mask[level]) >> (shift - 1)) | ((pixel->B & mask[level]) >> (shift));
                        if(null != this._children[index]){
                            paletteIndex = this._children[index].GetPaletteIndex(pixel, level + 1);
                        } else{
                            paletteIndex = -1;
                        }
                    }
                    return paletteIndex;
                }
                /// <summary>
                /// Increment the pixel count and add to the color information
                /// </summary>
                public void Increment(ColorBgra* pixel)
                {
                    ++this._pixelCount;
                    this._red += pixel->R;
                    this._green += pixel->G;
                    this._blue += pixel->B;
                }
            }
            #endregion
        }
        #endregion
    }
}