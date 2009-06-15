using System;
using System.Drawing;

namespace DrawEngine.Renderer.BasicStructures
{
    /// <summary>
    /// Provides Round-trip conversion from RGB to HSB and back
    /// </summary>
    public struct HSBColor
    {
        int a;
        float b;
        float h;
        float s;
        public HSBColor(float h, float s, float b)
        {
            this.a = 0xff;
            this.h = Math.Min(Math.Max(h, 0), 255);
            this.s = Math.Min(Math.Max(s, 0), 255);
            this.b = Math.Min(Math.Max(b, 0), 255);
        }
        public HSBColor(int a, float h, float s, float b)
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
        public float H
        {
            get { return this.h; }
            set { this.h = value; }
        }
        public float S
        {
            get { return this.s; }
            set { this.s = value; }
        }
        public float B
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
        public static HSBColor operator *(float escalar, HSBColor hsb)
        {
            HSBColor retorno = new HSBColor(escalar * hsb.h, escalar * hsb.s, escalar * hsb.b);
            return retorno;
        }
        public static HSBColor operator *(HSBColor hsb, float escalar)
        {
            HSBColor retorno = new HSBColor(escalar * hsb.h, escalar * hsb.s, escalar * hsb.b);
            return retorno;
        }
        public static HSBColor operator *(HSBColor hsb, HSBColor hsb1)
        {
            HSBColor retorno = new HSBColor(hsb1.h * hsb.h, hsb1.s * hsb.s, hsb1.b * hsb.b);
            return retorno;
        }
        public static HSBColor operator /(HSBColor hsb, float scalar)
        {
            HSBColor retorno = new HSBColor(hsb.h * (1 / scalar), hsb.s * (1 / scalar), hsb.b * (1 / scalar));
            return retorno;
        }
        public static Color ShiftHue(Color c, float hueDelta)
        {
            HSBColor hsb = FromColor(c);
            hsb.h += hueDelta;
            hsb.h = Math.Min(Math.Max(hsb.h, 0), 255);
            return ToColor(hsb);
        }
        public static Color ShiftSaturation(Color c, float saturationDelta)
        {
            HSBColor hsb = FromColor(c);
            hsb.s += saturationDelta;
            hsb.s = Math.Min(Math.Max(hsb.s, 0), 255);
            return ToColor(hsb);
        }
        public static Color ShiftBrighness(Color c, float brightnessDelta)
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
            float r = hsbColor.b;
            float g = hsbColor.b;
            float b = hsbColor.b;
            if(hsbColor.s != 0){
                float max = hsbColor.b;
                float dif = hsbColor.b * hsbColor.s / 255f;
                float min = hsbColor.b - dif;
                float h = hsbColor.h * 360f / 255f;
                if(h < 60f){
                    r = max;
                    g = h * dif / 60f + min;
                    b = min;
                } else if(h < 120f){
                    r = -(h - 120f) * dif / 60f + min;
                    g = max;
                    b = min;
                } else if(h < 180f){
                    r = min;
                    g = max;
                    b = (h - 120f) * dif / 60f + min;
                } else if(h < 240f){
                    r = min;
                    g = -(h - 240f) * dif / 60f + min;
                    b = max;
                } else if(h < 300f){
                    r = (h - 240f) * dif / 60f + min;
                    g = min;
                    b = max;
                } else if(h <= 360f){
                    r = max;
                    g = min;
                    b = -(h - 360f) * dif / 60 + min;
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
            HSBColor ret = new HSBColor(0f, 0f, 0f);
            ret.a = color.A;
            float r = color.R;
            float g = color.G;
            float b = color.B;
            float max = Math.Max(r, Math.Max(g, b));
            if(max <= 0){
                return ret;
            }
            float min = Math.Min(r, Math.Min(g, b));
            float dif = max - min;
            if(max > min){
                if(g == max){
                    ret.h = (b - r) / dif * 60f + 120f;
                } else if(b == max){
                    ret.h = (r - g) / dif * 60f + 240f;
                } else if(b > g){
                    ret.h = (g - b) / dif * 60f + 360f;
                } else{
                    ret.h = (g - b) / dif * 60f;
                }
                if(ret.h < 0){
                    ret.h = ret.h + 360f;
                }
            } else{
                ret.h = 0;
            }
            ret.h *= 255f / 360f;
            ret.s = (dif / max) * 255f;
            ret.b = max;
            return ret;
        }
        public static HSBColor FromColor(RGBColor rgbColor)
        {
            return FromColor(rgbColor.ToColor());
        }
    }
}