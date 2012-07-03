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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DrawEngine.Renderer.Animator;

namespace DrawEngine.SharpTracingUI.Components
{
    public partial class FrameView : UserControl
    {
        private int frameHeight = 250;
        private int framePadding = 10;
        private List<Bitmap> images = new List<Bitmap>();
        private Frame selectedFrame = null;
        private int selectedIndex = -1;
        public FrameView()
        {
            this.InitializeComponent();
        }
        public int SelectedIndex
        {
            get { return this.selectedIndex; }
            set { this.selectedIndex = value; }
        }
        public Frame SelectedFrame
        {
            get { return this.selectedFrame; }
        }
        public int FramePadding
        {
            get { return this.framePadding; }
            set { this.framePadding = value; }
        }
        public int FrameHeight
        {
            get { return this.frameHeight; }
            set { this.frameHeight = value; }
        }
        public void AddFrame(Bitmap backImage)
        {
            if(backImage != null){
                Bitmap newBackImage = new Bitmap(backImage);
                Frame frame = new Frame();
                frame.Location = this.CalculateNextLocation();
                frame.ContextMenu = this.ContextMenu;
                int discount = 2 * this.framePadding;
                if(this.VScroll){
                    discount += SystemInformation.VerticalScrollBarWidth;
                }
                frame.Size = new Size(this.Width - discount, this.frameHeight);
                this.images.Add(backImage);
                frame.BackgroundImage = backImage;
                frame.MouseDoubleClick += new MouseEventHandler(this.Frame_MouseDoubleClick);
                frame.MouseClick += new MouseEventHandler(this.Frame_MouseClick);
                frame.MouseDown += new MouseEventHandler(this.Frame_MouseClick);
                //frame.SizeChanged += new EventHandler(frame_SizeChanged);
                this.Controls.Add(frame);
            }
        }
        void Frame_MouseClick(object sender, MouseEventArgs e)
        {
            this.selectedFrame = (Frame)sender;
            this.selectedIndex = this.Controls.GetChildIndex(this.selectedFrame, false);
        }
        void Frame_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.selectedFrame = (Frame)sender;
            this.selectedIndex = this.Controls.GetChildIndex(this.selectedFrame, false);
        }
        public void RemoveFrameAt(int index)
        {
            if(index >= 0 && index < this.Controls.Count){
                this.Controls.RemoveAt(index);
                this.images.RemoveAt(index);
                this.ReadjustLocationBeforeRemove(index);
            }
        }
        public void RemoveFrame(Frame frameToRemove)
        {
            int index = this.Controls.GetChildIndex(frameToRemove, false);
            this.RemoveFrameAt(index);
        }
        public void RemoveAllFrames()
        {
            this.Controls.Clear();
            this.images.Clear();
        }
        private Point CalculateNextLocation()
        {
            if(this.Controls.Count > 0){
                Control ctrl = this.Controls[this.Controls.Count - 1];
                return new Point(this.framePadding, ctrl.Location.Y + ctrl.Height + this.framePadding);
            } else{
                return new Point(this.framePadding, this.framePadding);
            }
        }
        private void ReadjustLocationBeforeRemove(int indexRemoved)
        {
            for(int i = indexRemoved; i < this.Controls.Count; i++){
                this.Controls[i].Location = new Point(this.Controls[i].Location.X,
                                                      this.Controls[i].Location.Y - this.Controls[i].Height
                                                      - this.framePadding);
            }
        }
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.selectedIndex > -1){
                this.RemoveFrameAt(this.selectedIndex);
            }
        }
        private void removeAlltoolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.RemoveAllFrames();
        }
        private void animatedGifToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Thread tSaveAsGif = new Thread(new ThreadStart(this.SaveAsAnimatedGif));
            //tSaveAsGif.Start();
            this.SaveAsAnimatedGif();
        }
        private void SaveAsAnimatedGif()
        {
            //if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    AnimatedGifWriter writer = new AnimatedGifWriter(this.saveFileDialog.FileName);
            //    writer.Frames = this.images;
            //    writer.Save();
            //}
            AnimatedGifEncoder animation = new AnimatedGifEncoder();
            if(this.saveFileDialog.ShowDialog() == DialogResult.OK){
                animation.Start(this.saveFileDialog.FileName);
            }
            animation.Delay = 200;
            animation.Quality = 1;
            //-1:no repeat,0:always repeat
            animation.RepeatTimes = 0;
            foreach(Image frame in this.images){
                animation.AddFrame(frame);
            }
            animation.Finish();
        }
        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.createFolderProject.ShowDialog() == DialogResult.OK){
                for(int i = 0; i < this.images.Count; i++){
                    this.images[i].Save(this.createFolderProject.SelectedPath + "\\frame" + i + ".png", ImageFormat.Png);
                }
            }
        }
        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnimationViewForm.Instance.Show(MainForm.Instance.DockPanel);
            AnimationViewForm.Instance.RunAnimation(this.images);
        }
    }
}