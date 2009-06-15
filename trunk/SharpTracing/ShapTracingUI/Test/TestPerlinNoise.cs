using System;
using System.Windows.Forms;
using DrawEngine.Renderer.Filters;

namespace DrawEngine.SharpTracingUI.Test
{
    public partial class TestPerlinNoise : Form
    {
        public TestPerlinNoise()
        {
            this.InitializeComponent();
        }
        private void TestPerlinNoise_DoubleClick(object sender, EventArgs e)
        {
            this.textBox1.Text = PerlinNoiseFilter.Noise(50, 50, 50).ToString();
        }
    }
}