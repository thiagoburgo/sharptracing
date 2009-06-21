using System;
using System.ComponentModel;
using System.Globalization;

namespace DrawEngine.Renderer.Mathematics.Algebra.Design
{
    public class VectorOrPointTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) ? true : base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string valor = value as string;
            float x, y, z;
            if(valor != null){
                string[] coord = valor.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries);
                x = Convert.ToSingle(coord[0]);
                y = Convert.ToSingle(coord[1]);
                z = Convert.ToSingle(coord[2]);
                if(context.PropertyDescriptor.PropertyType == typeof(Vector3D)){
                    return new Vector3D(x, y, z);
                } else if(context.PropertyDescriptor.PropertyType == typeof(Point3D)){
                    return new Point3D(x, y, z);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) ? true : base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if(value != null && (value is Point3D || value is Vector3D)){
                return value.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}