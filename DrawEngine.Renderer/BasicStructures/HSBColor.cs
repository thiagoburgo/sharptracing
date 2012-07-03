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
using System.Drawing;
using System.Runtime.InteropServices;

namespace DrawEngine.Renderer.BasicStructures
{
    /// <summary>
    /// Provides Round-trip conversion from RGB to HSB and back
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HSBColor
    {
        int a;
        double b;
        double h;
        double s;
        public HSBColor(double h, double s, double b)
        {
            this.a = 0xff;
            this.h = Math.Min(Math.Max(h, 0), 255);
            this.s = Math.Min(Math.Max(s, 0), 255);
            this.b = Math.Min(Math.Max(b, 0), 255);
        }
        public HSBColor(int a, double h, double s, double b)
        {
            this.a = a;
            this.h = Math.Min(Math.Max(h, 0), 255);
            this.s = Math.Min(Math.Max(s, 0), 255);
            this.b = Math.Min(Math.Max(b, 0), 255);
        }
        public HSBColor(Color color)
        {
            HSBColor temp = FromColor(color);
            this.a = temp.a;
            this.h = temp.h;
            this.s = temp.s;
            this.b = temp.b;
        }
        public HSBColor(RGBColor color)
        {
            HSBColor temp = FromColor(color);
            this.a = temp.a;
            this.h = temp.h;
            this.s = temp.s;
            this.b = temp.b;
        }
        public double H
        {
            get { return this.h; }
            set { this.h = value; }
        }
        public double S
        {
            get { return this.s; }
            set { this.s = value; }
        }
        public double B
        {
            get { return this.b; }
            set { this.b = value; }
        }
        public int A
        {
            get { return this.a; }
            set { this.a = value; }
        }
        public static HSBColor operator +(HSBColor hsb1, HSBColor hsb2)
        {
            HSBColor retorno = new HSBColor(hsb1.h + hsb2.h, hsb1.s + hsb2.s, hsb1.b + hsb2.b);
            return retorno;
        }
        public static HSBColor operator -(HSBColor hsb1, HSBColor hsb2)
        {
            HSBColor retorno = new HSBColor(hsb1.h - hsb2.h, hsb1.s - hsb2.s, hsb1.b - hsb2.b);
            return retorno;
        }
        public static HSBColor operator *(double escalar, HSBColor hsb)
        {
            HSBColor retorno = new HSBColor(escalar * hsb.h, escalar * hsb.s, escalar * hsb.b);
            return retorno;
        }
        public static HSBColor operator *(HSBColor hsb, double escalar)
        {
            HSBColor retorno = new HSBColor(escalar * hsb.h, escalar * hsb.s, escalar * hsb.b);
            return retorno;
        }
        public static HSBColor operator *(HSBColor hsb, HSBColor hsb1)
        {
            HSBColor retorno = new HSBColor(hsb1.h * hsb.h, hsb1.s * hsb.s, hsb1.b * hsb.b);
            return retorno;
        }
        public static HSBColor operator /(HSBColor hsb, double scalar)
        {
            HSBColor retorno = new HSBColor(hsb.h * (1 / scalar), hsb.s * (1 / scalar), hsb.b * (1 / scalar));
            return retorno;
        }
        public static Color ShiftHue(Color c, double hueDelta)
        {
            HSBColor hsb = FromColor(c);
            hsb.h += hueDelta;
            hsb.h = Math.Min(Math.Max(hsb.h, 0), 255);
            return ToColor(hsb);
        }
        public static Color ShiftSaturation(Color c, double saturationDelta)
        {
            HSBColor hsb = FromColor(c);
            hsb.s += saturationDelta;
            hsb.s = Math.Min(Math.Max(hsb.s, 0), 255);
            return ToColor(hsb);
        }
        public static Color ShiftBrighness(Color c, double brightnessDelta)
        {
            HSBColor hsb = FromColor(c);
            hsb.b += brightnessDelta;
            hsb.b = Math.Min(Math.Max(hsb.b, 0), 255);
            return ToColor(hsb);
        }
        public static RGBColor ToRGBColor(HSBColor hsbColor)
        {
            return RGBColor.FromColor(ToColor(hsbColor));
        }
        public static Color ToColor(HSBColor hsbColor)
        {
            double r = hsbColor.b;
            double g = hsbColor.b;
            double b = hsbColor.b;
            if(hsbColor.s != 0){
                double max = hsbColor.b;
                double dif = hsbColor.b * hsbColor.s / 255d;
                double min = hsbColor.b - dif;
                double h = hsbColor.h * 360d / 255d;
                if(h < 60d){
                    r = max;
                    g = h * dif / 60d + min;
                    b = min;
                } else if(h < 120d){
                    r = -(h - 120d) * dif / 60d + min;
                    g = max;
                    b = min;
                } else if(h < 180d){
                    r = min;
                    g = max;
                    b = (h - 120d) * dif / 60d + min;
                } else if(h < 240d){
                    r = min;
                    g = -(h - 240d) * dif / 60d + min;
                    b = max;
                } else if(h < 300d){
                    r = (h - 240d) * dif / 60d + min;
                    g = min;
                    b = max;
                } else if(h <= 360d){
                    r = max;
                    g = min;
                    b = -(h - 360d) * dif / 60 + min;
                } else{
                    r = 0;
                    g = 0;
                    b = 0;
                }
            }
            return Color.FromArgb(hsbColor.a, (int)Math.Round(Math.Min(Math.Max(r, 0), 255)),
                                  (int)Math.Round(Math.Min(Math.Max(g, 0), 255)),
                                  (int)Math.Round(Math.Min(Math.Max(b, 0), 255)));
        }
        public static HSBColor FromColor(Color color)
        {
            HSBColor ret = new HSBColor(0d, 0d, 0d);
            ret.a = color.A;
            double r = color.R;
            double g = color.G;
            double b = color.B;
            double max = Math.Max(r, Math.Max(g, b));
            if(max <= 0){
                return ret;
            }
            double min = Math.Min(r, Math.Min(g, b));
            double dif = max - min;
            if(max > min){
                if(g == max){
                    ret.h = (b - r) / dif * 60d + 120d;
                } else if(b == max){
                    ret.h = (r - g) / dif * 60d + 240d;
                } else if(b > g){
                    ret.h = (g - b) / dif * 60d + 360d;
                } else{
                    ret.h = (g - b) / dif * 60d;
                }
                if(ret.h < 0){
                    ret.h = ret.h + 360d;
                }
            } else{
                ret.h = 0;
            }
            ret.h *= 255d / 360d;
            ret.s = (dif / max) * 255d;
            ret.b = max;
            return ret;
        }
        public static HSBColor FromColor(RGBColor rgbColor)
        {
            return FromColor(rgbColor.ToColor());
        }
    }
}