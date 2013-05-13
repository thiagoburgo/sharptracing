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
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DrawEngine.Renderer.RenderObjects.EnvironmentMaps.Design {
    public partial class SphereMapEditorForm : Form {
        public SphereMapEditorForm(SphereMap sphereMap) {
            this.InitializeComponent();
            if (!String.IsNullOrEmpty(sphereMap.ImagePath)) {
                this.txtPathBase.Text = Path.GetDirectoryName(sphereMap.ImagePath);
                this.ddlNamePrefix.SelectedItem = sphereMap.ImagePath;
            }
            this.numericUpDownRadius.Value = Convert.ToDecimal(sphereMap.Radius);
            this.SphereMap = sphereMap;
            var files = Directory.GetFiles(this.txtPathBase.Text);
            this.ddlNamePrefix.Items.AddRange(files.ToArray());
        }

        public SphereMap SphereMap { get; set; }

        private void txtPathBase_TextChanged(object sender, EventArgs e) {
            var files = Directory.GetFiles(this.txtPathBase.Text);
            this.ddlNamePrefix.Items.AddRange(files.ToArray());
        }

        private void btnOk_Click(object sender, EventArgs e) {
            string path = this.ddlNamePrefix.SelectedItem as String;
            if (!String.IsNullOrEmpty(path)) {
                this.SphereMap = new SphereMap(Convert.ToSingle(this.numericUpDownRadius.Value), path);
                this.DialogResult = DialogResult.OK;
                this.Close();
            } else {
                if (
                    MessageBox.Show("The file name is empty! Close dialog anyway?", "Choose a file...",
                                    MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
        }

        private void ddlNamePrefix_SelectedIndexChanged(object sender, EventArgs e) {
            string pattern = this.ddlNamePrefix.SelectedItem as String;
            if (!String.IsNullOrEmpty(pattern)) {
                this.pictureBox1.Image =
                    Image.FromFile(Path.Combine(this.txtPathBase.Text, pattern.Replace("{#}", "_nz")));
            }
        }

        private void btnBasePath_Click(object sender, EventArgs e) {
            if (this.folderDialog.ShowDialog() == DialogResult.OK) {
                this.txtPathBase.Text = this.folderDialog.SelectedPath;
            }
        }
    }
}