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
using DrawEngine.Renderer.Mesh.Importers;

namespace DrawEngine.Renderer.Mesh.Design {
    public partial class LoadingMeshModelDialog : Form {
        private TimeSpan inicio;
        private MeshModel meshModel;

        public LoadingMeshModelDialog(MeshModel meshModel) {
            this.InitializeComponent();
            this.DialogResult = DialogResult.Cancel;

            this.DialogResult = DialogResult.Cancel;
            this.meshModel = meshModel;
            if (meshModel.FilePath != null && File.Exists(meshModel.FilePath)) {
                MeshModelLoader.OnElementLoaded += MeshModel_OnElementLoaded;
                MeshModelLoader.OnInitBuild += this.MeshModel_OnInitBuild;
                MeshModelLoader.OnEndBuild += this.MeshModel_OnEndBuild;
            } else {
                this.Close();
            }
        }

        private void MeshModel_OnEndBuild(TimeSpan timeToBuild) {
            this.DialogResult = DialogResult.OK;
            if (this.InvokeRequired) {
                this.Invoke(new Action(delegate {
                                           this.DialogResult = DialogResult.OK;
                                           this.Close();
                                       }));
            }
        }

        private void MeshModel_OnInitBuild(MeshModel mesh) {
            if (this.InvokeRequired) {
                this.Invoke(new Action(delegate {
                                           this.lblInfo.Text =
                                               string.Format("Optimize Model for fast intersections... Wait.. ");
                                           this.lblTrianglesInfo.Text = "Triangles: " + mesh.Triangles.Length;
                                       }));
            }
        }

        private void MeshModel_OnElementLoaded(int percentageOfTotal, ElementMesh element) {
            if (this.progressBar.InvokeRequired) {
                this.progressBar.Invoke(new Action(delegate { this.progressBar.Value = percentageOfTotal; }));
            }
            string name = "Elements";
            switch (element) {
                case ElementMesh.Vertex:
                    name = "Vertices";
                    break;
                case ElementMesh.VextexIndice:
                    name = "Triangles";
                    break;
                case ElementMesh.VertexNormal:
                    name = "Vertices Normals";
                    break;
            }
            if (this.lblInfo.InvokeRequired) {
                this.lblInfo.Invoke(
                    new Action(
                        delegate { this.lblInfo.Text = string.Format("Loading {0}... {1}%", name, percentageOfTotal); }));
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
            MeshModelLoader.Import(ref this.meshModel);
        }

        private void LoadingModelDialog_Load(object sender, EventArgs e) {
            this.inicio = DateTime.Now.TimeOfDay;
            this.timer1.Start();
            this.backgroundWorker.RunWorkerAsync();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (this.lblTimeElapsed.InvokeRequired) {
                this.lblTimeElapsed.Invoke(
                    new Action(
                        delegate {
                            this.lblTimeElapsed.Text = String.Format("Elapsed Time: {0:mm}m{0:ss}s",
                                                                     DateTime.Now.Subtract(this.inicio));
                        }));
            }
        }

        private void LoadingModelDialog_FormClosing(object sender, FormClosingEventArgs e) {
            this.backgroundWorker.CancelAsync();
            this.backgroundWorker.Dispose();
            this.timer1.Stop();
        }
    }
}