using System;
using System.Drawing;
using System.IO;

#region .NET Disclaimer/Info
//===============================================================================
//
// gOODiDEA, uland.com
//===============================================================================
//
// $Header :		$  
// $Author :		$
// $Date   :		$
// $Revision:		$
// $History:		$  
//  
//===============================================================================
#endregion

namespace DrawEngine.Renderer.Animator
{
    public class GifEncoder
    {
        protected bool closeStream = false; // close stream when finished
        protected int colorDepth; // number of bit planes
        protected byte[] colorTab; // RGB palette
        protected int delay = 0; // frame delay (hundredths)
        protected int dispose = -1; // disposal code (-1 = use default)
        protected bool firstFrame = true;
        protected int height;
        protected Image image; // current frame
        protected byte[] indexedPixels; // converted frame indexed to palette
        protected Stream os; //output stream
        protected int palSize = 7; // color table size (bits-1)
        protected byte[] pixels; // BGR byte array from frame
        protected int repeat = -1; // no repeat
        protected int sample = 10; // default sample interval for quantizer
        protected bool sizeSet = false; // if false, get size from first frame
        protected bool started = false; // ready to output frames
        protected int transIndex; // transparent index in color table
        protected Color transparent = Color.Empty; // transparent color if given
        protected bool[] usedEntry = new bool[256]; // active palette entries
        protected int width;
        public GifEncoder()
        {
            this.Start();
        }
        public GifEncoder(Stream stream)
        {
            this.Start(stream);
        }
        public void SetDelay(int ms)
        {
            this.delay = (int)Math.Round(ms / 10.0f);
        }
        /**
		 * Sets the GIF frame disposal code for the last added frame
		 * and any subsequent frames.  Default is 0 if no transparent
		 * color has been set, otherwise 2.
		 * @param code int disposal code.
		 */
        public void SetDispose(int code)
        {
            if(code >= 0){
                this.dispose = code;
            }
        }
        /**
		 * Sets the number of times the set of GIF frames
		 * should be played.  Default is 1; 0 means play
		 * indefinitely.  Must be invoked before the first
		 * image is added.
		 *
		 * @param iter int number of iterations.
		 * @return
		 */
        public void SetRepeat(int iter)
        {
            if(iter >= 0){
                this.repeat = iter;
            }
        }
        /**
		 * Sets the transparent color for the last added frame
		 * and any subsequent frames.
		 * Since all colors are subject to modification
		 * in the quantization process, the color in the final
		 * palette for each frame closest to the given color
		 * becomes the transparent color for that frame.
		 * May be set to null to indicate no transparent color.
		 *
		 * @param c Color to be treated as transparent on display.
		 */
        public void SetTransparent(Color c)
        {
            this.transparent = c;
        }
        private void Start()
        {
            this.Start(new MemoryStream());
        }
        private void Start(Stream stream)
        {
            this.os = stream;
            this.WriteString("GIF89a");
        }
        public Stream Finish()
        {
            this.os.WriteByte(0x3b); // gif trailer
            this.os.Flush();
            return this.os;
        }
        public void AddFrame(Image image)
        {
            this.GetImagePixels();
            this.AnalyzePixels(); // build color table & map pixels
            if(this.firstFrame){
                this.WriteLSD(); // logical screen descriptior
                this.WritePalette(); // global color table
                if(this.repeat >= 0){
                    // use NS app extension to indicate reps
                    this.WriteNetscapeExt();
                }
            }
            this.WriteGraphicCtrlExt(); // write graphic control extension
            this.WriteImageDesc(); // image descriptor
            if(!this.firstFrame){
                this.WritePalette(); // local color table
            }
            this.WritePixels(); // encode and write pixel data
        }
        protected void GetImagePixels()
        {
            int w = this.image.Width;
            int h = this.image.Height;
            //		int type = image.GetType().;
            if((w != this.width) || (h != this.height)){
                // create new image with right size/format
                Image temp = new Bitmap(this.width, this.height);
                Graphics g = Graphics.FromImage(temp);
                g.DrawImage(this.image, 0, 0);
                this.image = temp;
                g.Dispose();
            }
            this.pixels = new Byte[3 * this.image.Width * this.image.Height];
            int count = 0;
            Bitmap tempBitmap = new Bitmap(this.image);
            for(int th = 0; th < this.image.Height; th++){
                for(int tw = 0; tw < this.image.Width; tw++){
                    Color color = tempBitmap.GetPixel(tw, th);
                    this.pixels[count] = color.R;
                    count++;
                    this.pixels[count] = color.G;
                    count++;
                    this.pixels[count] = color.B;
                    count++;
                }
            }
        }
        protected Byte[] GetImagePixels(Image image)
        {
            Byte[] pixels = new Byte[3 * image.Width * image.Height];
            int count = 0;
            Bitmap tempBitmap = new Bitmap(image);
            for(int th = 0; th < image.Height; th++){
                for(int tw = 0; tw < image.Width; tw++){
                    Color color = tempBitmap.GetPixel(tw, th);
                    pixels[count] = color.R;
                    count++;
                    pixels[count] = color.G;
                    count++;
                    pixels[count] = color.B;
                    count++;
                }
            }
            tempBitmap.Dispose();
            return pixels;
        }
        protected void AnalyzePixels()
        {
            int len = this.pixels.Length;
            int nPix = len / 3;
            this.indexedPixels = new byte[nPix];
            NeuQuant nq = new NeuQuant(this.pixels, len, this.sample);
            // initialize quantizer
            this.colorTab = nq.Process(); // create reduced palette
            int k = 0;
            for(int i = 0; i < nPix; i++){
                int index = nq.Map(this.pixels[k++] & 0xff, this.pixels[k++] & 0xff, this.pixels[k++] & 0xff);
                this.usedEntry[index] = true;
                this.indexedPixels[i] = (byte)index;
            }
            this.pixels = null;
            this.colorDepth = 8;
            this.palSize = 7;
            // get closest match to transparent color if specified
            if(this.transparent != Color.Empty){
                this.transIndex = this.FindClosest(this.transparent);
            }
        }
        protected int FindClosest(Color color)
        {
            if(this.colorTab == null){
                return -1;
            }
            int r = color.R;
            int g = color.G;
            int b = color.B;
            int minpos = 0;
            int dmin = 256 * 256 * 256;
            int len = this.colorTab.Length;
            for(int i = 0; i < len;){
                int dr = r - (this.colorTab[i++] & 0xff);
                int dg = g - (this.colorTab[i++] & 0xff);
                int db = b - (this.colorTab[i] & 0xff);
                int d = dr * dr + dg * dg + db * db;
                int index = i / 3;
                if(this.usedEntry[index] && (d < dmin)){
                    dmin = d;
                    minpos = index;
                }
                i++;
            }
            return minpos;
        }
        /**
		 * Writes Logical Screen Descriptor
		 */
        protected void WriteLSD()
        {
            // logical screen size
            this.WriteShort(this.width);
            this.WriteShort(this.height);
            // packed fields
            this.os.WriteByte(Convert.ToByte(0x80 | // 1   : global color table flag = 1 (gct used)
                                             0x70 | // 2-4 : color resolution = 7
                                             0x00 | // 5   : gct sort flag = 0
                                             this.palSize)); // 6-8 : gct size
            this.os.WriteByte(0); // background color index
            this.os.WriteByte(0); // pixel aspect ratio - assume 1:1
        }
        /**
		 * Writes color table
		 */
        protected void WritePalette()
        {
            this.os.Write(this.colorTab, 0, this.colorTab.Length);
            int n = (3 * 256) - this.colorTab.Length;
            for(int i = 0; i < n; i++){
                this.os.WriteByte(0);
            }
        }
        /**
		 * Writes Netscape application extension to define
		 * repeat count.
		 */
        protected void WriteNetscapeExt()
        {
            this.os.WriteByte(0x21); // extension introducer
            this.os.WriteByte(0xff); // app extension label
            this.os.WriteByte(11); // block size
            this.WriteString("NETSCAPE" + "2.0"); // app id + auth code
            this.os.WriteByte(3); // sub-block size
            this.os.WriteByte(1); // loop sub-block id
            this.WriteShort(this.repeat); // loop count (extra iterations, 0=repeat forever)
            this.os.WriteByte(0); // block terminator
        }
        protected void WriteGraphicCtrlExt()
        {
            this.os.WriteByte(0x21); // extension introducer
            this.os.WriteByte(0xf9); // GCE label
            this.os.WriteByte(4); // data block size
            int transp, disp;
            if(this.transparent == Color.Empty){
                transp = 0;
                disp = 0; // dispose = no action
            } else{
                transp = 1;
                disp = 2; // force clear if using transparent color
            }
            if(this.dispose >= 0){
                disp = this.dispose & 7; // user override
            }
            disp <<= 2;
            // packed fields
            this.os.WriteByte(Convert.ToByte(0 | // 1:3 reserved
                                             disp | // 4:6 disposal
                                             0 | // 7   user input - 0 = none
                                             transp)); // 8   transparency flag
            this.WriteShort(this.delay); // delay x 1/100 sec
            this.os.WriteByte(Convert.ToByte(this.transIndex)); // transparent color index
            this.os.WriteByte(0); // block terminator
        }
        /**
		 * Writes Image Descriptor
		 */
        protected void WriteImageDesc()
        {
            this.os.WriteByte(0x2c); // image separator
            this.WriteShort(0); // image position x,y = 0,0
            this.WriteShort(0);
            this.WriteShort(this.width); // image size
            this.WriteShort(this.height);
            // packed fields
            if(this.firstFrame){
                // no LCT  - GCT is used for first (or only) frame
                this.os.WriteByte(0);
            } else{
                // specify normal LCT
                this.os.WriteByte(Convert.ToByte(0x80 | // 1 local color table  1=yes
                                                 0 | // 2 interlace - 0=no
                                                 0 | // 3 sorted - 0=no
                                                 0 | // 4-5 reserved
                                                 this.palSize)); // 6-8 size of color table
            }
        }
        protected void WritePixels()
        {
            LZWEncoder encoder = new LZWEncoder(this.width, this.height, this.indexedPixels, this.colorDepth);
            encoder.Encode(this.os);
        }
        protected void WriteShort(int value)
        {
            this.os.WriteByte(Convert.ToByte(value & 0xff));
            this.os.WriteByte(Convert.ToByte((value >> 8) & 0xff));
        }
        protected void WriteString(string text)
        {
            char[] chars = text.ToCharArray();
            for(int i = 0; i < chars.Length; i++){
                this.os.WriteByte((byte)chars[i]);
            }
        }
    }
}