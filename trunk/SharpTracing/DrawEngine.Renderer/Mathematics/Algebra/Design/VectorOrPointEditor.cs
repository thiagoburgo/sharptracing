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
using System.Security.Permissions;
using System.Windows.Forms.Design;

namespace DrawEngine.Renderer.Mathematics.Algebra.Design {
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class VectorOrPointEditor : UITypeEditor {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            // Return the value if the value is not of type Int32, Double and Single.
            bool isVector;
            if (value == null) {
                object[] objs = context.Instance as object[];
                if (objs != null) {
                    value =
                        objs[objs.Length - 1].GetType()
                                             .GetProperty(context.PropertyDescriptor.Name)
                                             .GetValue(objs[objs.Length - 1], null);
                }
            }
            if (value.GetType() != typeof (Vector3D) && value.GetType() != typeof (Point3D)) {
                throw new Exception("Invalid value! \r\n The value must be a Point3D or Vector3D");
            } else if (value.GetType() == typeof (Vector3D)) {
                isVector = true;
            } else {
                isVector = false;
            }
            // Uses the IWindowsFormsEditorService to display a 
            // drop-down UI in the Properties window.
            IWindowsFormsEditorService edSvc =
                (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));
            if (edSvc != null) {
                // Display an angle selection control and retrieve the value.
                VectorPoint3DControl pointVecCrtl = new VectorPoint3DControl();
                if (isVector) {
                    pointVecCrtl.Vector3D = (Vector3D) value;
                    edSvc.DropDownControl(pointVecCrtl);
                    return pointVecCrtl.Vector3D;
                } else {
                    pointVecCrtl.Point3D = (Point3D) value;
                    edSvc.DropDownControl(pointVecCrtl);
                    return pointVecCrtl.Point3D;
                }
            }
            return value;
        }
    }
}