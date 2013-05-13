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
using System.Drawing;
using System.Drawing.Design;

namespace DrawEngine.Renderer.BasicStructures.Design {
    public class RGBColorEditor : ColorEditor {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            if (value is RGBColor) {
                return RGBColor.FromColor((Color) base.EditValue(context, provider, ((RGBColor) value).ToColor()));
            }
            return value;
        }

        public override void PaintValue(PaintValueEventArgs e) {
            Color color;
            if (e.Value is RGBColor) {
                color = ((RGBColor) e.Value).ToColor();
            } else {
                color = (Color) e.Value;
            }
            using (SolidBrush brush = new SolidBrush(color)) {
                e.Graphics.FillRectangle(brush,
                                         new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 1));
            }
            base.PaintValue(e);
        }
    }
}