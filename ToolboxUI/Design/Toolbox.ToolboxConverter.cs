using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace TooboxUI.Components {
    partial class Toolbox {
        private static readonly Type _instanceDescriptorType = typeof (InstanceDescriptor);

        #region Nested type: ToolboxConverter

        internal class ToolboxConverter : TypeConverter {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
                if (destinationType == _instanceDescriptorType) {
                    return true;
                }
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                             Type destinationType) {
                if (destinationType == null) {
                    throw new ArgumentNullException("destinationType");
                }
                if ((destinationType != _instanceDescriptorType) || !(value is Toolbox)) {
                    goto Label_ReturnBase;
                }
                Toolbox toolbox = (Toolbox) value;
                Type toolboxType = toolbox.GetType();
                ConstructorInfo ci = toolboxType.GetConstructor(new Type[] {typeof (bool)});
                if (ci != null) {
                    bool createGeneral = false;
                    if (toolbox.Site != null && toolbox.Site.DesignMode) {
                        createGeneral = toolbox.CreateGeneralCategory;
                    }
                    return new InstanceDescriptor(ci, new object[] {createGeneral}, false);
                } else {
                    ci = toolboxType.GetConstructor(new Type[0]);
                    return new InstanceDescriptor(ci, new object[0]);
                }
                Label_ReturnBase:
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        #endregion
    }
}