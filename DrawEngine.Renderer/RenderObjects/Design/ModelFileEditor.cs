/**
 * Criado por: Thiago Burgo Belo (thiagoburgo@gmail.com)
 * SharpTracing � um projeto feito inicialmente para disciplina
 * Computa��o Gr�fica da UFPE e depois melhorado nos tempos livres
 * sinta-se a vontade para copiar modificar e mandar corre��es e 
 * sugest�es. Mantenha os cr�ditos!
 * **************************************************************
 * SharpTracing is a project originally created to discipline 
 * Computer Graphics of UFPE and was improved in my free time.
 * Feel free to copy, modify and  give fixes 
 * suggestions. Keep the credits!
 */
 using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DrawEngine.Renderer.RenderObjects.Design
{
    public class ModelFileEditor : UITypeEditor
    {
        private OpenFileDialog view = new OpenFileDialog();
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            // Uses the IWindowsFormsEditorService to display a 
            // drop-down UI in the Properties window.
            IWindowsFormsEditorService edSvc =
                    (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if(edSvc != null){
                // Display an angle selection control and retrieve the value.                
                //view.DefaultExt = "ply";
                this.view.Filter =
                        "All Know Files|*.ply;*.byu;*.obj;*.off;*.noff;*.cnoff|Ply Files|*.ply|Byu Files|*.byu|Wave Obj Files|*.obj|Off Files|*.off;*.noff;*.cnoff";
                if(this.view.ShowDialog() == DialogResult.OK){
                    TriangleModel model = context.Instance as TriangleModel;
                    if(model != null){
                        model.Path = this.view.FileName;
                        LoadingModelDialog modelDlg = new LoadingModelDialog(model);
                        if(modelDlg.ShowDialog() == DialogResult.OK){
                            return this.view.FileName;
                        } else{
                            model = new TriangleModel();
                        }
                    }
                }
            }
            return "";
        }
        
        
    }
}