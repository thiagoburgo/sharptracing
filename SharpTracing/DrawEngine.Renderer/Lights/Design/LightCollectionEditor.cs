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
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DrawEngine.Renderer.Lights.Design {
    public class LightCollectionEditor : UITypeEditor {
        public LightCollectionEditor() {}

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            // Uses the IWindowsFormsEditorService to display a 
            // drop-down UI in the Properties window.
            IWindowsFormsEditorService edSvc =
                (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));
            LightCollectionForm form = new LightCollectionForm();
            form.Lights = (LightDictionary) value;
            MessageBox.Show(context.PropertyDescriptor.PropertyType.ReflectedType.Name);
            if (edSvc != null) {
                if (form.ShowDialog() == DialogResult.OK) {
                    return form.Lights;
                }
            }
            return value;
        }
    }
}