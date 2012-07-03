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
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using DrawEngine.Renderer.Importers;
using DrawEngine.Renderer.Mesh;

namespace DrawEngine.Renderer.RenderObjects.Design
{
    
    public partial class LoadingModelDialog : Form
    {
        private TimeSpan inicio;
        private TriangleModel triangleModel;
        public LoadingModelDialog(TriangleModel triangleModel)
        {
            this.InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
            this.triangleModel = triangleModel;
            if (triangleModel.Path != null && File.Exists(triangleModel.Path))
            {
                triangleModel.OnElementLoaded += this.triangleModel_OnElementLoaded;
                triangleModel.OnInitBuild += this.triangleModel_OnInitBuild;
                triangleModel.OnEndBuild += this.triangleModel_OnEndBuild;
            }
            else
            {
                this.Close();
            }
        }
        private void triangleModel_OnEndBuild(TimeSpan timeToBuild)
        {
            this.DialogResult = DialogResult.OK;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(delegate { this.Close(); }));
            }
        }
        private void triangleModel_OnInitBuild()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(delegate
                {
                    this.lblInfo.Text =
                            string.Format("Optimize Model for fast intersections... Wait.. ");
                    this.lblTrianglesInfo.Text = "Triangles: " + this.triangleModel.TriangleCount;
                }));
            }
        }
        private void triangleModel_OnElementLoaded(int percentageOfTotal, ElementMesh element)
        {
            if (this.progressBar.InvokeRequired)
            {
                this.progressBar.Invoke(new Action(delegate { this.progressBar.Value = percentageOfTotal; }));
            }
            string name = "Elements";
            switch (element)
            {
                case ElementMesh.Vertex:
                    name = "Vertices";
                    break;
                case ElementMesh.Triangle:
                    name = "Triangles";
                    break;
                case ElementMesh.VertexNormal:
                    name = "Vertices Normals";
                    break;
            }
            if (this.lblInfo.InvokeRequired)
            {
                this.lblInfo.Invoke(
                        new Action(
                                delegate { this.lblInfo.Text = string.Format("Loading {0}... {1}%", name, percentageOfTotal); }));
            }
        }
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.triangleModel.Load();
        }
        private void LoadingModelDialog_Load(object sender, EventArgs e)
        {
            this.inicio = DateTime.Now.TimeOfDay;
            this.timer1.Start();
            this.backgroundWorker.RunWorkerAsync();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.lblTimeElapsed.InvokeRequired)
            {
                this.lblTimeElapsed.Invoke(
                        new Action(
                                delegate
                                {
                                    this.lblTimeElapsed.Text = String.Format("Elapsed Time: {0:mm}m{0:ss}s",
                                                                             DateTime.Now.Subtract(this.inicio));
                                }));
            }
        }
        private void LoadingModelDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.backgroundWorker.CancelAsync();
            this.backgroundWorker.Dispose();
            this.timer1.Stop();
        }
    }
}