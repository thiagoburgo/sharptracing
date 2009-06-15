using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DrawEngine.Renderer.RenderObjects.EnvironmentMaps.Design
{
    public class SphereMapEditor : UITypeEditor
    {
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
                SphereMapEditorForm view = new SphereMapEditorForm(value as SphereMap);
                if(view.ShowDialog() == DialogResult.OK){
                    return view.SphereMap;
                }
            }
            return value;
        }
    }
}