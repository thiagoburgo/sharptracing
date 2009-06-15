using System;
using System.Windows.Forms;
using DrawEngine.Renderer.Tracers;

namespace DrawEngine.SharpTracingUI
{
    public partial class FullScreenView : Form
    {
        private DistributedRayTracer tracer;
        public FullScreenView()
        {
            this.InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }
        public DistributedRayTracer Tracer
        {
            get { return this.tracer; }
            set { this.tracer = value; }
        }
        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
        private void closeFullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
        private void FullScreenView_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape || e.KeyData == Keys.F11){
                this.Close();
                this.Dispose();
            }
        }
    }
}