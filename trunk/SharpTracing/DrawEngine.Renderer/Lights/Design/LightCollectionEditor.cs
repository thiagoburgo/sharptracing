using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DrawEngine.Renderer.Lights.Design
{
    public class LightCollectionEditor : UITypeEditor
    {
        public LightCollectionEditor() {}
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
            LightCollectionForm form = new LightCollectionForm();
            form.Lights = (LightDictionary)value;
            MessageBox.Show(context.PropertyDescriptor.PropertyType.ReflectedType.Name);
            if(edSvc != null){
                if(form.ShowDialog() == DialogResult.OK){
                    return form.Lights;
                }
            }
            return value;
        }
    }
}