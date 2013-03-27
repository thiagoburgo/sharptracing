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
using System.Windows.Forms.Design;

namespace DrawEngine.Renderer.Cameras.Design {
    public class DefaultCameraEditor : UITypeEditor {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            if (context != null && context.Instance != null) {
                return UITypeEditorEditStyle.DropDown;
            }
            return base.GetEditStyle(context);
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            // Uses the IWindowsFormsEditorService to display a 
            // drop-down UI in the Properties window.
            IWindowsFormsEditorService edSvc =
                (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));
            ListBoxDefaultCameraControl dccontrol = new ListBoxDefaultCameraControl();
            Camera[] cameras = new Camera[UnifiedScenesRepository.CurrentEditingScene.Cameras.Count];
            UnifiedScenesRepository.CurrentEditingScene.Cameras.CopyTo(cameras, 0);
            //ListBox.ObjectCollection oc = new ListBox.ObjectCollection(list, cameras);
            //list.Items.AddRange(oc);
            dccontrol.ListBoxCameras.Items.AddRange(cameras);
            if (edSvc != null) {
                // Display an angle selection control and retrieve the value.                
                //edSvc.DropDownControl(list);                
                edSvc.DropDownControl(dccontrol);
                if (dccontrol.ListBoxCameras.SelectedItem != null) {
                    return dccontrol.ListBoxCameras.SelectedItem;
                }
            }
            return value;
        }
    }
}