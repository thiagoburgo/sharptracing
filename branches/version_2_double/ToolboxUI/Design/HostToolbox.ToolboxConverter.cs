using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace TooboxUI.Components
{
    partial class HostToolbox
    {
        private static Type _instanceDescriptorType = typeof(InstanceDescriptor);

        #region Nested type: ToolboxConverter
        internal new class ToolboxConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if(destinationType == _instanceDescriptorType){
                    return true;
                }
                return base.CanConvertTo(context, destinationType);
            }
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                             Type destinationType)
            {
                if(destinationType == null){
                    throw new ArgumentNullException("destinationType");
                }
                if((destinationType != _instanceDescriptorType) || !(value is HostToolbox)){
                    goto Label_ReturnBase;
                }
                HostToolbox toolbox = (HostToolbox)value;
                ConstructorInfo ci = toolbox.GetType().GetConstructor(new Type[]{typeof(bool)});
                if(ci != null){
                    bool createGeneral = false;
                    if(toolbox.Site != null && toolbox.Site.DesignMode){
                        createGeneral = toolbox.CreateGeneralCategory;
                    }
                    return new InstanceDescriptor(ci, new object[]{createGeneral}, false);
                } else{
                    ci = toolbox.GetType().GetConstructor(new Type[0]);
                    return new InstanceDescriptor(ci, new object[0]);
                }
                Label_ReturnBase:
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
        #endregion
    }
}