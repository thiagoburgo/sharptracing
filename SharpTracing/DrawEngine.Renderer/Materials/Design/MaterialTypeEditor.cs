using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace DrawEngine.Renderer.Materials.Design
{
    public class MaterialSelectorEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if(context != null && context.Instance != null){
                return UITypeEditorEditStyle.DropDown;
            }
            return base.GetEditStyle(context);
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            // Uses the IWindowsFormsEditorService to display a 
            // drop-down UI in the Properties window.
            IWindowsFormsEditorService edSvc =
                    (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if(edSvc != null){
                MaterialTreeViewEditorControl view = new MaterialTreeViewEditorControl();
                // Display an angle selection control and retrieve the value.                
                edSvc.DropDownControl(view);
                if(view.SelectedMaterial != null){
                    return view.SelectedMaterial;
                }
            }
            return value;
        }
    }
}