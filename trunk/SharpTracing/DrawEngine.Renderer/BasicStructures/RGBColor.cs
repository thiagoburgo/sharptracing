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
using System.Runtime.InteropServices;
using DrawEngine.Renderer.BasicStructures.Design;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.BasicStructures {
    [TypeConverter(typeof (RGBColorConverter)), Editor(typeof (RGBColorEditor), typeof (UITypeEditor)), Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGBColor : IEquatable<RGBColor> {
        public float R;
        public float G;
        public float B;

        public RGBColor(float r, float g, float b) {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public float Luminance {
            get { return (0.2989f * R) + (0.5866f * G) + (0.1145f * B); }
        }

        public float Max {
            get {
                float max = this.R;
                if (max < this.G) {
                    max = this.G;
                }
                if (max < this.B) {
                    max = this.B;
                }
                return max;
            }
        }

        //public override string ToString() {
        //    return "(R=" + r + ", G=" + g + ", B=" + b + ")";
        //    //return ((Color) this).ToString();
        //}
        //public static implicit operator RGBColor(Color color)
        //{
        //    return new RGBColor(color.R, color.G, color.B);
        //}
        //public static implicit operator Color(RGBColor color)
        //{
        //    color.Clamp();
        //    return Color.FromArgb((int)(color.r * 255f), (int)(color.g * 255f), (int)(color.b * 255f));
        //}
        public float Average {
            get { return ((this.R + this.G + this.B) / 3f); }
        }

        public float Sum {
            get { return (this.R + this.G + this.B); }
        }

        public RGBColor Normalized {
            get {
                RGBColor temp = this; //new RGBColor(this.R, this.G, this.B);
                temp.Normalize();
                return temp;
            }
        }

        #region IEquatable<RGBColor> Members

        public bool Equals(RGBColor color) {
            return (this.R.IsEqual(color.R) && this.G.IsEqual(color.G) && this.B.IsEqual(color.B));
        }

        #endregion

        public void Clamp() {
            if (this.R > 1.0f) {
                this.R = 1.0f;
            } else if (this.R < 0.0f) {
                this.R = 0.0f;
            }
            if (this.G > 1.0f) {
                this.G = 1.0f;
            } else if (this.G < 0.0f) {
                this.G = 0.0f;
            }
            if (this.B > 1.0f) {
                this.B = 1.0f;
            } else if (this.B < 0.0f) {
                this.B = 0.0f;
            }
        }

        private const float OneDiv255 = 1f / 255f;
        public void Normalize() {
            if (this.R > 0) {
                this.R *= OneDiv255;
            }
            if (this.G > 0) {
                this.G *= OneDiv255;
            }
            if (this.B > 0) {
                this.B *= OneDiv255;
            }
        }

        public Color ToColor() {
            RGBColor color = this;
            color.Clamp();
            return Color.FromArgb((int) (color.R * 255), (int) (color.G * 255), (int) (color.B * 255));
        }

        public static RGBColor FromColor(Color color) {
            RGBColor rgb = new RGBColor(color.R, color.G, color.B);
            rgb.Normalize();
            return rgb;
        }

        public static bool operator ==(RGBColor rgb1, RGBColor rgb2) {
            return rgb1.R.IsEqual(rgb2.R) && rgb1.G.IsEqual(rgb2.G) && rgb1.B.IsEqual(rgb2.B);
        }

        public static bool operator !=(RGBColor rgb1, RGBColor rgb2) {
            return !rgb1.R.IsEqual(rgb2.R) || !rgb1.G.IsEqual(rgb2.G) || !rgb1.B.IsEqual(rgb2.B);
        }

        public static RGBColor operator +(RGBColor rgb1, RGBColor rgb2) {
            //RGBColor retorno = new RGBColor(rgb1.red + rgb2.red, rgb1.green + rgb2.green, rgb1.blue + rgb2.blue);
            rgb1.R += rgb2.R;
            rgb1.G += rgb2.G;
            rgb1.B += rgb2.B;
            return rgb1;
        }

        public static RGBColor operator -(RGBColor rgb1, RGBColor rgb2) {
            //RGBColor retorno = new RGBColor(rgb1.red - rgb2.red, rgb1.green - rgb2.green, rgb1.blue - rgb2.blue);
            rgb1.R -= rgb2.R;
            rgb1.G -= rgb2.G;
            rgb1.B -= rgb2.B;
            return rgb1;
        }

        public static RGBColor operator *(float escalar, RGBColor rgb) {
            //RGBColor retorno = new RGBColor(escalar * rgb.red, escalar * rgb.green, escalar * rgb.blue);
            //return retorno;
            rgb.R *= escalar;
            rgb.G *= escalar;
            rgb.B *= escalar;
            return rgb;
        }

        public static RGBColor operator *(RGBColor rgb, float escalar) {
            //RGBColor retorno = new RGBColor(escalar * rgb.red, escalar * rgb.green, escalar * rgb.blue);
            //return retorno;
            rgb.R *= escalar;
            rgb.G *= escalar;
            rgb.B *= escalar;
            return rgb;
        }

        public static RGBColor operator *(RGBColor rgb, RGBColor rgb1) {
            //RGBColor retorno = new RGBColor(rgb1.red * rgb.red, rgb1.green * rgb.green, rgb1.blue * rgb.blue);
            //return retorno;
            rgb1.R *= rgb.R;
            rgb1.G *= rgb.G;
            rgb1.B *= rgb.B;
            return rgb1;
        }

        public static RGBColor operator /(RGBColor rgb, float scalar) {
            //RGBColor retorno = new RGBColor(rgb.red * (1f / scalar), rgb.green * (1f / scalar), rgb.blue * (1f / scalar));
            //return retorno;
            float oneDivScalar = (1f / scalar);
            rgb.R *= oneDivScalar;
            rgb.G *= oneDivScalar;
            rgb.B *= oneDivScalar;
            return rgb;
        }

        public override string ToString() {
            return this.ToColor().ToString();
        }

        #region Predefined Colors

        public static RGBColor AliceBlue {
            get { return FromColor(Color.AliceBlue); }
        }

        public static RGBColor AntiqueWhite {
            get { return FromColor(Color.AntiqueWhite); }
        }

        public static RGBColor Aqua {
            get { return FromColor(Color.Aqua); }
        }

        public static RGBColor Aquamarine {
            get { return FromColor(Color.Aquamarine); }
        }

        public static RGBColor Azure {
            get { return FromColor(Color.Azure); }
        }

        public static RGBColor Beige {
            get { return FromColor(Color.Beige); }
        }

        public static RGBColor Bisque {
            get { return FromColor(Color.Bisque); }
        }

        public static RGBColor Black {
            get { return FromColor(Color.Black); }
        }

        public static RGBColor BlanchedAlmond {
            get { return FromColor(Color.BlanchedAlmond); }
        }

        public static RGBColor Blue {
            get { return FromColor(Color.Blue); }
        }

        public static RGBColor BlueViolet {
            get { return FromColor(Color.BlueViolet); }
        }

        public static RGBColor Brown {
            get { return FromColor(Color.Brown); }
        }

        public static RGBColor BurlyWood {
            get { return FromColor(Color.BurlyWood); }
        }

        public static RGBColor CadetBlue {
            get { return FromColor(Color.CadetBlue); }
        }

        public static RGBColor Chartreuse {
            get { return FromColor(Color.Chartreuse); }
        }

        public static RGBColor Chocolate {
            get { return FromColor(Color.Chocolate); }
        }

        public static RGBColor Coral {
            get { return FromColor(Color.Coral); }
        }

        public static RGBColor CornflowerBlue {
            get { return FromColor(Color.CornflowerBlue); }
        }

        public static RGBColor Cornsilk {
            get { return FromColor(Color.Cornsilk); }
        }

        public static RGBColor Crimson {
            get { return FromColor(Color.Crimson); }
        }

        public static RGBColor Cyan {
            get { return FromColor(Color.Cyan); }
        }

        public static RGBColor DarkBlue {
            get { return FromColor(Color.DarkBlue); }
        }

        public static RGBColor DarkCyan {
            get { return FromColor(Color.DarkCyan); }
        }

        public static RGBColor DarkGoldenrod {
            get { return FromColor(Color.DarkGoldenrod); }
        }

        public static RGBColor DarkGray {
            get { return FromColor(Color.DarkGray); }
        }

        public static RGBColor DarkGreen {
            get { return FromColor(Color.DarkGreen); }
        }

        public static RGBColor DarkKhaki {
            get { return FromColor(Color.DarkKhaki); }
        }

        public static RGBColor DarkMagenta {
            get { return FromColor(Color.DarkMagenta); }
        }

        public static RGBColor DarkOliveGreen {
            get { return FromColor(Color.DarkOliveGreen); }
        }

        public static RGBColor DarkOrange {
            get { return FromColor(Color.DarkOrange); }
        }

        public static RGBColor DarkOrchid {
            get { return FromColor(Color.DarkOrchid); }
        }

        public static RGBColor DarkRed {
            get { return FromColor(Color.DarkRed); }
        }

        public static RGBColor DarkSalmon {
            get { return FromColor(Color.DarkSalmon); }
        }

        public static RGBColor DarkSeaGreen {
            get { return FromColor(Color.DarkSeaGreen); }
        }

        public static RGBColor DarkSlateBlue {
            get { return FromColor(Color.DarkSlateBlue); }
        }

        public static RGBColor DarkSlateGray {
            get { return FromColor(Color.DarkSlateGray); }
        }

        public static RGBColor DarkTurquoise {
            get { return FromColor(Color.DarkTurquoise); }
        }

        public static RGBColor DarkViolet {
            get { return FromColor(Color.DarkViolet); }
        }

        public static RGBColor DeepPink {
            get { return FromColor(Color.DeepPink); }
        }

        public static RGBColor DeepSkyBlue {
            get { return FromColor(Color.DeepPink); }
        }

        public static RGBColor DimGray {
            get { return FromColor(Color.DimGray); }
        }

        public static RGBColor DodgerBlue {
            get { return FromColor(Color.DodgerBlue); }
        }

        public static RGBColor Firebrick {
            get { return FromColor(Color.Firebrick); }
        }

        public static RGBColor FloralWhite {
            get { return FromColor(Color.FloralWhite); }
        }

        public static RGBColor ForestGreen {
            get { return FromColor(Color.ForestGreen); }
        }

        public static RGBColor Fuchsia {
            get { return FromColor(Color.Fuchsia); }
        }

        public static RGBColor Gainsboro {
            get { return FromColor(Color.Gainsboro); }
        }

        public static RGBColor GhostWhite {
            get { return FromColor(Color.GhostWhite); }
        }

        public static RGBColor Gold {
            get { return FromColor(Color.Gold); }
        }

        public static RGBColor Goldenrod {
            get { return FromColor(Color.Goldenrod); }
        }

        public static RGBColor Gray {
            get { return FromColor(Color.Gray); }
        }

        public static RGBColor Green {
            get { return FromColor(Color.Green); }
        }

        public static RGBColor GreenYellow {
            get { return FromColor(Color.GreenYellow); }
        }

        public static RGBColor Honeydew {
            get { return FromColor(Color.Honeydew); }
        }

        public static RGBColor HotPink {
            get { return FromColor(Color.HotPink); }
        }

        public static RGBColor IndianRed {
            get { return FromColor(Color.IndianRed); }
        }

        public static RGBColor Indigo {
            get { return FromColor(Color.Indigo); }
        }

        public static RGBColor Ivory {
            get { return FromColor(Color.Ivory); }
        }

        public static RGBColor Khaki {
            get { return FromColor(Color.Khaki); }
        }

        public static RGBColor Lavender {
            get { return FromColor(Color.Lavender); }
        }

        public static RGBColor LavenderBlush {
            get { return FromColor(Color.LavenderBlush); }
        }

        public static RGBColor LawnGreen {
            get { return FromColor(Color.LawnGreen); }
        }

        public static RGBColor LemonChiffon {
            get { return FromColor(Color.LemonChiffon); }
        }

        public static RGBColor LightBlue {
            get { return FromColor(Color.LightBlue); }
        }

        public static RGBColor LightCoral {
            get { return FromColor(Color.LightCoral); }
        }

        public static RGBColor LightCyan {
            get { return FromColor(Color.LightCyan); }
        }

        public static RGBColor LightGoldenrodYellow {
            get { return FromColor(Color.LightGoldenrodYellow); }
        }

        public static RGBColor LightGreen {
            get { return FromColor(Color.LightGreen); }
        }

        public static RGBColor LightGray {
            get { return FromColor(Color.LightGray); }
        }

        public static RGBColor LightPink {
            get { return FromColor(Color.LightPink); }
        }

        public static RGBColor LightSalmon {
            get { return FromColor(Color.LightSalmon); }
        }

        public static RGBColor LightSeaGreen {
            get { return FromColor(Color.LightSeaGreen); }
        }

        public static RGBColor LightSkyBlue {
            get { return FromColor(Color.LightSkyBlue); }
        }

        public static RGBColor LightSlateGray {
            get { return FromColor(Color.LightSlateGray); }
        }

        public static RGBColor LightSteelBlue {
            get { return FromColor(Color.LightSteelBlue); }
        }

        public static RGBColor LightYellow {
            get { return FromColor(Color.LightYellow); }
        }

        public static RGBColor Lime {
            get { return FromColor(Color.Lime); }
        }

        public static RGBColor LimeGreen {
            get { return FromColor(Color.LimeGreen); }
        }

        public static RGBColor Linen {
            get { return FromColor(Color.Linen); }
        }

        public static RGBColor Magenta {
            get { return FromColor(Color.Magenta); }
        }

        public static RGBColor Maroon {
            get { return FromColor(Color.Maroon); }
        }

        public static RGBColor MediumAquamarine {
            get { return FromColor(Color.MediumAquamarine); }
        }

        public static RGBColor MediumBlue {
            get { return FromColor(Color.MediumBlue); }
        }

        public static RGBColor MediumOrchid {
            get { return FromColor(Color.MediumOrchid); }
        }

        public static RGBColor MediumPurple {
            get { return FromColor(Color.MediumPurple); }
        }

        public static RGBColor MediumSeaGreen {
            get { return FromColor(Color.MediumSeaGreen); }
        }

        public static RGBColor MediumSlateBlue {
            get { return FromColor(Color.MediumSlateBlue); }
        }

        public static RGBColor MediumSpringGreen {
            get { return FromColor(Color.MediumSpringGreen); }
        }

        public static RGBColor MediumTurquoise {
            get { return FromColor(Color.MediumTurquoise); }
        }

        public static RGBColor MediumVioletRed {
            get { return FromColor(Color.MediumVioletRed); }
        }

        public static RGBColor MidnightBlue {
            get { return FromColor(Color.MidnightBlue); }
        }

        public static RGBColor MintCream {
            get { return FromColor(Color.MintCream); }
        }

        public static RGBColor MistyRose {
            get { return FromColor(Color.MistyRose); }
        }

        public static RGBColor Moccasin {
            get { return FromColor(Color.Moccasin); }
        }

        public static RGBColor NavajoWhite {
            get { return FromColor(Color.NavajoWhite); }
        }

        public static RGBColor Navy {
            get { return FromColor(Color.Navy); }
        }

        public static RGBColor OldLace {
            get { return FromColor(Color.OldLace); }
        }

        public static RGBColor Olive {
            get { return FromColor(Color.Olive); }
        }

        public static RGBColor OliveDrab {
            get { return FromColor(Color.OliveDrab); }
        }

        public static RGBColor Orange {
            get { return FromColor(Color.Orange); }
        }

        public static RGBColor OrangeRed {
            get { return FromColor(Color.OrangeRed); }
        }

        public static RGBColor Orchid {
            get { return FromColor(Color.Orchid); }
        }

        public static RGBColor PaleGoldenrod {
            get { return FromColor(Color.PaleGoldenrod); }
        }

        public static RGBColor PaleGreen {
            get { return FromColor(Color.PaleGreen); }
        }

        public static RGBColor PaleTurquoise {
            get { return FromColor(Color.PaleTurquoise); }
        }

        public static RGBColor PaleVioletRed {
            get { return FromColor(Color.PaleVioletRed); }
        }

        public static RGBColor PapayaWhip {
            get { return FromColor(Color.PapayaWhip); }
        }

        public static RGBColor PeachPuff {
            get { return FromColor(Color.PeachPuff); }
        }

        public static RGBColor Peru {
            get { return FromColor(Color.Peru); }
        }

        public static RGBColor Pink {
            get { return FromColor(Color.Pink); }
        }

        public static RGBColor Plum {
            get { return FromColor(Color.Plum); }
        }

        public static RGBColor PowderBlue {
            get { return FromColor(Color.PowderBlue); }
        }

        public static RGBColor Purple {
            get { return FromColor(Color.Purple); }
        }

        public static RGBColor Red {
            get { return FromColor(Color.Red); }
        }

        public static RGBColor RosyBrown {
            get { return FromColor(Color.RosyBrown); }
        }

        public static RGBColor RoyalBlue {
            get { return FromColor(Color.RoyalBlue); }
        }

        public static RGBColor SaddleBrown {
            get { return FromColor(Color.SaddleBrown); }
        }

        public static RGBColor Salmon {
            get { return FromColor(Color.Salmon); }
        }

        public static RGBColor SandyBrown {
            get { return FromColor(Color.SandyBrown); }
        }

        public static RGBColor SeaGreen {
            get { return FromColor(Color.SeaGreen); }
        }

        public static RGBColor SeaShell {
            get { return FromColor(Color.SeaShell); }
        }

        public static RGBColor Sienna {
            get { return FromColor(Color.Sienna); }
        }

        public static RGBColor Silver {
            get { return FromColor(Color.Silver); }
        }

        public static RGBColor SkyBlue {
            get { return FromColor(Color.SkyBlue); }
        }

        public static RGBColor SlateBlue {
            get { return FromColor(Color.SlateBlue); }
        }

        public static RGBColor SlateGray {
            get { return FromColor(Color.SlateGray); }
        }

        public static RGBColor Snow {
            get { return FromColor(Color.Snow); }
        }

        public static RGBColor SpringGreen {
            get { return FromColor(Color.SpringGreen); }
        }

        public static RGBColor SteelBlue {
            get { return FromColor(Color.SteelBlue); }
        }

        public static RGBColor Tan {
            get { return FromColor(Color.Tan); }
        }

        public static RGBColor Teal {
            get { return FromColor(Color.Teal); }
        }

        public static RGBColor Thistle {
            get { return FromColor(Color.Thistle); }
        }

        public static RGBColor Tomato {
            get { return FromColor(Color.Tomato); }
        }

        public static RGBColor Turquoise {
            get { return FromColor(Color.Turquoise); }
        }

        public static RGBColor Violet {
            get { return FromColor(Color.Violet); }
        }

        public static RGBColor Wheat {
            get { return FromColor(Color.Wheat); }
        }

        public static RGBColor White {
            get { return FromColor(Color.White); }
        }

        public static RGBColor WhiteSmoke {
            get { return FromColor(Color.WhiteSmoke); }
        }

        public static RGBColor Yellow {
            get { return FromColor(Color.Yellow); }
        }

        public static RGBColor YellowGreen {
            get { return FromColor(Color.YellowGreen); }
        }

        #endregion
    }
}