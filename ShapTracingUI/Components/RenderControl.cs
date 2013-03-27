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
using System.Drawing;
using System.Windows.Forms;

namespace DrawEngine.SharpTracingUI {
    public partial class RenderControl : UserControl {
        private static readonly object lockObject = new Object();
        private volatile Image image;

        public RenderControl() {
            this.InitializeComponent();
            this.image = new Bitmap(this.Width, this.Height);
            this.AdjustFormScrollbars(true);
        }

        public Image Image {
            get {
                lock (lockObject) {
                    return this.image;
                }
            }
            set {
                lock (lockObject) {
                    this.image = value;
                    if (!this.InvokeRequired) {
                        this.Refresh();
                    }
                }
            }
        }

        protected override void OnInvalidated(InvalidateEventArgs e) {
            base.OnInvalidated(e);
        }

        protected override void OnPaint(PaintEventArgs e) {
            lock (lockObject) {
                e.Graphics.DrawImageUnscaled(this.image, this.Location);
            }
            base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);
        }

        public override void Refresh() {
            base.Refresh();
        }
    }
}