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

namespace DrawEngine.Renderer.RenderObjects.EnvironmentMaps.Design
{
    public partial class CubeMapEditorForm : Form
    {
        private CubeMap cubeMap;
        public CubeMapEditorForm(CubeMap cubeMap)
        {
            this.InitializeComponent();
            this.cubeMap = cubeMap;
            if(!String.IsNullOrEmpty(cubeMap.BasePath)){
                this.txtPathBase.Text = cubeMap.BasePath;
            }
            if(!String.IsNullOrEmpty(cubeMap.FileNamePattern)){
                this.ddlNamePrefix.SelectedItem = cubeMap.FileNamePattern;
            }
            this.numericUpDown1.Value = Convert.ToDecimal(cubeMap.Width);
            this.numericUpDown2.Value = Convert.ToDecimal(cubeMap.Height);
            this.numericUpDown3.Value = Convert.ToDecimal(cubeMap.Depth);
        }
        public CubeMap CubeMap
        {
            get { return this.cubeMap; }
            set { this.cubeMap = value; }
        }
        private void btnBasePath_Click(object sender, EventArgs e)
        {
            if(this.folderDialog.ShowDialog() == DialogResult.OK){
                this.txtPathBase.Text = this.folderDialog.SelectedPath;
            }
        }
        private void txtPathBase_TextChanged(object sender, EventArgs e)
        {
            var files = (from file in Directory.GetFiles(this.txtPathBase.Text)
                         where
                                 Path.GetFileNameWithoutExtension(file).ToLower().EndsWith("_nx")
                                 || Path.GetFileNameWithoutExtension(file).ToLower().EndsWith("_ny")
                                 || Path.GetFileNameWithoutExtension(file).ToLower().EndsWith("_nz")
                                 || Path.GetFileNameWithoutExtension(file).ToLower().EndsWith("_px")
                                 || Path.GetFileNameWithoutExtension(file).ToLower().EndsWith("_py")
                                 || Path.GetFileNameWithoutExtension(file).ToLower().EndsWith("_pz")
                         select
                                 Path.GetFileName(file).ToLower().Replace("_nx", "{#}").Replace("_ny", "{#}").Replace(
                                 "_nz", "{#}").Replace("_px", "{#}").Replace("_py", "{#}").Replace("_pz", "{#}")).
                    Distinct(StringComparer.CurrentCultureIgnoreCase);
            this.ddlNamePrefix.Items.AddRange(files.ToArray());
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            string pattern = this.ddlNamePrefix.SelectedItem as String;
            if(!String.IsNullOrEmpty(pattern)){
                this.cubeMap = new CubeMap(Convert.ToSingle(this.numericUpDown1.Value),
                                           Convert.ToSingle(this.numericUpDown2.Value),
                                           Convert.ToSingle(this.numericUpDown3.Value), this.txtPathBase.Text, pattern);
                this.DialogResult = DialogResult.OK;
                this.Close();
            } else{
                if(
                        MessageBox.Show("The file name pattern is empty! Close dialog anyway?", "Choose a pattern...",
                                        MessageBoxButtons.YesNo) == DialogResult.Yes){
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
        }
        private void ddlNamePrefix_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pattern = this.ddlNamePrefix.SelectedItem as String;
            if(!String.IsNullOrEmpty(pattern)){
                this.pictureBox1.Image =
                        Image.FromFile(Path.Combine(this.txtPathBase.Text, pattern.Replace("{#}", "_nz")));
            }
        }
    }
}