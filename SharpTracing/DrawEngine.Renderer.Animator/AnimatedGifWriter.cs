using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DrawEngine.Renderer.Animator.Quantizer;

namespace DrawEngine.Renderer.Animator {
    public class AnimatedGifWriter : IDisposable {
        private int delay;
        private string filePath;
        private readonly FileStream fileStream;
        private List<Image> imageList;
        private readonly MemoryStream memoryStream;

        public AnimatedGifWriter(string filePath) {
            this.filePath = filePath;
            this.imageList = new List<Image>();
            this.delay = 20;
            this.memoryStream = new MemoryStream();
            this.fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
        }

        public List<Image> Frames {
            get { return this.imageList; }
            set {
                if (value != null) {
                    this.imageList = value;
                }
            }
        }

        public int Delay {
            get { return this.delay; }
            set {
                if (this.delay > 0) {
                    this.delay = value / 1000;
                }
            }
        }

        #region IDisposable Members

        public void Dispose() {
            foreach (Image image in this.imageList) {
                image.Dispose();
            }
            this.imageList.Clear();
            this.imageList = null;
        }

        #endregion

        public void AddFrame(Image frame) {
            this.imageList.Add(frame);
        }

        public void Save() {
            Byte[] buf1;
            Byte[] buf2;
            Byte[] buf3;
            //Variable declaration
            //memoryStream = new MemoryStream();
            buf2 = new Byte[19];
            buf3 = new Byte[8];
            buf2[0] = 33; //extension introducer
            buf2[1] = 255; //application extension
            buf2[2] = 11; //size of block
            buf2[3] = 78; //N
            buf2[4] = 69; //E
            buf2[5] = 84; //T
            buf2[6] = 83; //S
            buf2[7] = 67; //C
            buf2[8] = 65; //A
            buf2[9] = 80; //P
            buf2[10] = 69; //E
            buf2[11] = 50; //2
            buf2[12] = 46; //.
            buf2[13] = 48; //0
            buf2[14] = 3; //Size of block
            buf2[15] = 1; //
            buf2[16] = 0; //
            buf2[17] = 0; //
            buf2[18] = 0; //Block terminator
            buf3[0] = 33; //Extension introducer
            buf3[1] = 249; //Graphic control extension
            buf3[2] = 4; //Size of block
            buf3[3] = 9; //Flags: reserved, disposal method, user input, transparent color
            buf3[4] = Convert.ToByte(this.delay & 0xff);
            buf3[5] = Convert.ToByte((this.delay >> 8) & 0xff);
            //buf3[4] = 10;  //Delay time low byte
            //buf3[5] = 3;   //Delay time high byte
            buf3[6] = 255; //Transparent color index
            buf3[7] = 0; //Block terminator
            bool firstTime = true;
            foreach (Image image in this.imageList) {
                OctreeQuantizer quantizer = new OctreeQuantizer(255, 8);
                using (Bitmap quantized = quantizer.Quantize(image)) {
                    quantized.Save(this.memoryStream, ImageFormat.Gif);
                    image.Dispose();
                }
                //image.Save(memoryStream, ici, parameters);
                buf1 = this.memoryStream.ToArray();
                if (firstTime) {
                    //only write these the first time....                    
                    this.fileStream.Write(buf1, 0, 781); //Header & global color table
                    this.fileStream.Write(buf2, 0, 19); //Application extension
                    firstTime = false;
                }
                this.fileStream.Write(buf3, 0, 8); //Graphic extension
                this.fileStream.Write(buf1, 781, buf1.Length - 782); //Image Data
                this.memoryStream.SetLength(0);
            }
            this.fileStream.WriteByte((Byte) 0x3B);
            this.fileStream.Flush();
            this.fileStream.Close();
        }
    }
}