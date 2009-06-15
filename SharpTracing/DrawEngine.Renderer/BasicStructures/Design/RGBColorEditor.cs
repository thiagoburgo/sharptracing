using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace DrawEngine.Renderer.BasicStructures.Design
{
    public class RGBColorEditor : ColorEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if(value is RGBColor){
                return RGBColor.FromColor((Color)base.EditValue(context, provider, ((RGBColor)value).ToColor()));
            }
            return value;
        }
        public override void PaintValue(PaintValueEventArgs e)
        {
            Color color;
            if(e.Value is RGBColor){
                color = ((RGBColor)e.Value).ToColor();
            } else{
                color = (Color)e.Value;
            }
            using(SolidBrush brush = new SolidBrush(color)){
                e.Graphics.FillRectangle(brush,
                                         new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 1));
            }
            base.PaintValue(e);
        }
    }
}