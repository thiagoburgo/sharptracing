using System;
using System.Windows.Forms;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.Mathematics.Random;
using System.Drawing;

namespace DrawEngine.ConsoleTests {
    public partial class FormRandom : Form {
        public FormRandom() {
            InitializeComponent();
        }

        private void btnHalton_Click(object sender, System.EventArgs e) {
            Image img = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(img);

            QMCHaltonRandom haltonRandom = new QMCHaltonRandom();
            haltonRandom.ResetGenerator(20);
            for(int i = 0; i < this.numericUpDownSamples.Value; i++){
                g.FillRectangle(Brushes.Black, (float)(haltonRandom.NextDouble() * this.Width), (float)(haltonRandom.NextDouble() * this.Height), 2, 2);
                
            }
            this.resultPicture.Image = img;    

        }

        private void btnSobol_Click(object sender, EventArgs e) {
            Image img = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(img);

            QMCSobolRandom sobolRandom = new QMCSobolRandom();
            sobolRandom.SetDimension(12);
            for(int i = 0; i < this.numericUpDownSamples.Value; i++) {
                g.FillRectangle(Brushes.Black, (float)(sobolRandom.NextDouble() * this.Width), (float)(sobolRandom.NextDouble() * this.Height), 2, 2);

            }
            this.resultPicture.Image = img;
        }

        private void FormRandom_Load(object sender, EventArgs e) {
            this.propertyGrid1.SelectedObject = new Point3D(10, 11, 30);
        }
    }
}
