using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace TooboxUI.Components {
    partial class Toolbox {
        #region Nested type: TabConverter

        internal class TabConverter : ExpandableObjectConverter {
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
                if ((destinationType != _instanceDescriptorType) || !(value is Tab)) {
                    goto Label_ReturnBase;
                }
                Tab tab = (Tab) value;
                ConstructorInfo ci = typeof (Tab).GetConstructor(new Type[0]);
                return new InstanceDescriptor(ci, new object[0], false);
                Label_ReturnBase:
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        #endregion
    }
}