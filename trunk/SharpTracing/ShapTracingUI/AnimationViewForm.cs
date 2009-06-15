using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AviFile;
using WeifenLuo.WinFormsUI.Docking;

namespace DrawEngine.SharpTracingUI
{
    public partial class AnimationViewForm : DockContent
    {
        private static AnimationViewForm instance;
        private Queue animationImages = new Queue();
        private Thread tAnimation;
        private AnimationViewForm()
        {
            this.InitializeComponent();
        }
        public static AnimationViewForm Instance
        {
            get
            {
                if(instance == null){
                    instance = new AnimationViewForm();
                }
                return instance;
            }
        }
        public void RunAnimation(List<Bitmap> images)
        {
            this.animationImages = new Queue(images);
            this.animationImages = Queue.Synchronized(this.animationImages);
            if(this.tAnimation != null){
                this.tAnimation.Abort();
            }
            this.tAnimation = new Thread(new ThreadStart(delegate(){
                                                             if(this.folderBrowserDialog1.ShowDialog()
                                                                == DialogResult.OK){
                                                                 DirectoryInfo di =
                                                                         new DirectoryInfo(
                                                                                 this.folderBrowserDialog1.SelectedPath);
                                                                 FileInfo[] fis = di.GetFiles("*.png");
                                                                 Array.Sort<FileInfo>(fis,
                                                                                      new Comparison<FileInfo>(
                                                                                              delegate(FileInfo fi1,
                                                                                                       FileInfo fi2){
                                                                                                  return
                                                                                                          fi1.
                                                                                                                  CreationTime
                                                                                                                  .
                                                                                                                  CompareTo
                                                                                                                  (fi2.
                                                                                                                           CreationTime);
                                                                                              }));
                                                                 foreach(FileInfo fi in fis){
                                                                     Graphics g = this.pictureBox1.CreateGraphics();
                                                                     g.SmoothingMode = SmoothingMode.HighQuality;
                                                                     g.InterpolationMode =
                                                                             InterpolationMode.HighQualityBicubic;
                                                                     g.CompositingQuality =
                                                                             CompositingQuality.HighQuality;
                                                                     Bitmap bmp = Bitmap.FromFile(fi.FullName) as Bitmap;
                                                                     //this.pictureBox1.Image = bmp;
                                                                     g.DrawImage(bmp,
                                                                                 g.VisibleClipBounds.Width / 2f
                                                                                 - bmp.Width / 2f,
                                                                                 g.VisibleClipBounds.Height / 2f
                                                                                 - bmp.Height / 2f);
                                                                     Thread.Sleep(1000 / 24);
                                                                     bmp.Dispose();
                                                                 }
                                                             }
                                                         }));
            this.tAnimation.SetApartmentState(ApartmentState.STA);
            this.tAnimation.Start();
        }
        //public void RunAnimation(List<Bitmap> images)
        //{
        //    //if (images != null && images.Count > 0)
        //    //{
        //    String tempFileName = System.IO.Path.GetTempFileName() + ".avi";
        //    AviManager tempFile = new AviManager(tempFileName, false);
        //    //tempFile.AddVideoStream(false, 10, images[0].Clone() as Bitmap);
        //    tempFile.AddVideoStream(false, 24, Bitmap.FromFile(@"C:\temp\frames\frame_0.png") as Bitmap);
        //    VideoStream stream = tempFile.GetVideoStream();
        //    for (int i = 1; i < 719; i++)
        //    {
        //        Bitmap bmp = Bitmap.FromFile(@"C:\temp\frames\frame_" + i + ".png") as Bitmap;
        //        stream.AddFrame(bmp);
        //        bmp.Dispose();
        //    }
        //    //for (int n = 1; n < images.Count; n++)
        //    //{
        //    //    stream.AddFrame(images[n].Clone() as Bitmap);
        //    //}
        //    AviPlayer player = new AviPlayer(new EditableVideoStream(stream), this.pictureBox1, null);
        //    player.Start();
        //    tempFile.Close();
        //    try { File.Delete(tempFileName); }
        //    catch (IOException) { }
        //    //}
        //}
        public void RunAnimation(VideoStream video) {}
        //protected override void OnVisibleChanged(EventArgs e)
        //{
        //    if (!this.Visible)
        //    {
        //    }
        //    base.OnVisibleChanged(e);
        //}
    }
}