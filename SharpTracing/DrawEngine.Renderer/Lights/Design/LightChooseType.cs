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
using System.Reflection;
using System.Windows.Forms;

namespace DrawEngine.Renderer.Lights.Design {
    public partial class LightChooseType : Form {
        private Type selectedType;

        public LightChooseType() {
            this.InitializeComponent();
        }

        public Type SelectedType {
            get { return this.selectedType; }
            set { this.selectedType = value; }
        }

        private void ChooseObjectType_Load(object sender, EventArgs e) {
            Type type = typeof (Light);
            Assembly ass = Assembly.GetAssembly(type);
            foreach (Type typeTemp in ass.GetExportedTypes()) {
                if (!typeTemp.IsAbstract) {
                    if (typeTemp.IsSubclassOf(type)) {
                        this.ddlLightTypes.Items.Add(typeTemp);
                    }
                }
            }
            this.ddlLightTypes.SelectedIndex = 0;
        }

        private void btnOk_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.selectedType = this.ddlLightTypes.SelectedItem as Type;
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.selectedType = null;
        }
    }
}