using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace TooboxUI.Components
{
    partial class Toolbox
    {
        #region Nested type: ItemConverter
        internal class ItemConverter : TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if(destinationType == typeof(InstanceDescriptor)){
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
                if((destinationType != typeof(InstanceDescriptor)) || !(value is Item)){
                    goto Label_ReturnBase;
                }
                Item item = (Item)value;
                ConstructorInfo ci = typeof(Item).GetConstructor(new Type[0]);
                return new InstanceDescriptor(ci, new object[0], false);
                Label_ReturnBase:
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
        #endregion
    }
}