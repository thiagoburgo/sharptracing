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