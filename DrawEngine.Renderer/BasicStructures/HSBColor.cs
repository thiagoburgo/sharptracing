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
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.BasicStructures {
    /// <summary>
    /// Provides Round-trip conversion from RGB to HSB and back
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HSBColor {
        public int A;
        public float B;
        public float H;
        public float S;

        public HSBColor(float h, float s, float b) {
            this.A = 0xff;
            this.H = Math.Min(Math.Max(h, 0), 255);
            this.S = Math.Min(Math.Max(s, 0), 255);
            this.B = Math.Min(Math.Max(b, 0), 255);
        }

        public HSBColor(int a, float h, float s, float b) {
            this.A = a;
            this.H = Math.Min(Math.Max(h, 0), 255);
            this.S = Math.Min(Math.Max(s, 0), 255);
            this.B = Math.Min(Math.Max(b, 0), 255);
        }

        public HSBColor(Color color) {
            HSBColor temp = FromColor(color);
            this.A = temp.A;
            this.H = temp.H;
            this.S = temp.S;
            this.B = temp.B;
        }

        public HSBColor(RGBColor color) {
            HSBColor temp = FromColor(color);
            this.A = temp.A;
            this.H = temp.H;
            this.S = temp.S;
            this.B = temp.B;
        }

        public static HSBColor operator +(HSBColor hsb1, HSBColor hsb2) {
            HSBColor retorno = new HSBColor(hsb1.H + hsb2.H, hsb1.S + hsb2.S, hsb1.B + hsb2.B);
            return retorno;
        }

        public static HSBColor operator -(HSBColor hsb1, HSBColor hsb2) {
            HSBColor retorno = new HSBColor(hsb1.H - hsb2.H, hsb1.S - hsb2.S, hsb1.B - hsb2.B);
            return retorno;
        }

        public static HSBColor operator *(float escalar, HSBColor hsb) {
            HSBColor retorno = new HSBColor(escalar * hsb.H, escalar * hsb.S, escalar * hsb.B);
            return retorno;
        }

        public static HSBColor operator *(HSBColor hsb, float escalar) {
            HSBColor retorno = new HSBColor(escalar * hsb.H, escalar * hsb.S, escalar * hsb.B);
            return retorno;
        }

        public static HSBColor operator *(HSBColor hsb, HSBColor hsb1) {
            HSBColor retorno = new HSBColor(hsb1.H * hsb.H, hsb1.S * hsb.S, hsb1.B * hsb.B);
            return retorno;
        }

        public static HSBColor operator /(HSBColor hsb, float scalar) {
            HSBColor retorno = new HSBColor(hsb.H * (1 / scalar), hsb.S * (1 / scalar), hsb.B * (1 / scalar));
            return retorno;
        }

        public static Color ShiftHue(Color c, float hueDelta) {
            HSBColor hsb = FromColor(c);
            hsb.H += hueDelta;
            hsb.H = Math.Min(Math.Max(hsb.H, 0), 255);
            return ToColor(hsb);
        }

        public static Color ShiftSaturation(Color c, float saturationDelta) {
            HSBColor hsb = FromColor(c);
            hsb.S += saturationDelta;
            hsb.S = Math.Min(Math.Max(hsb.S, 0), 255);
            return ToColor(hsb);
        }

        public static Color ShiftBrighness(Color c, float brightnessDelta) {
            HSBColor hsb = FromColor(c);
            hsb.B += brightnessDelta;
            hsb.B = Math.Min(Math.Max(hsb.B, 0), 255);
            return ToColor(hsb);
        }

        public static RGBColor ToRGBColor(HSBColor hsbColor) {
            return RGBColor.FromColor(ToColor(hsbColor));
        }

        public static Color ToColor(HSBColor hsbColor) {
            float r = hsbColor.B;
            float g = hsbColor.B;
            float b = hsbColor.B;
            if (!hsbColor.S.NearZero()) {
                float max = hsbColor.B;
                float dif = hsbColor.B * hsbColor.S / 255f;
                float min = hsbColor.B - dif;
                float h = hsbColor.H * 360f / 255f;
                if (h < 60f) {
                    r = max;
                    g = h * dif / 60f + min;
                    b = min;
                } else if (h < 120f) {
                    r = -(h - 120f) * dif / 60f + min;
                    g = max;
                    b = min;
                } else if (h < 180f) {
                    r = min;
                    g = max;
                    b = (h - 120f) * dif / 60f + min;
                } else if (h < 240f) {
                    r = min;
                    g = -(h - 240f) * dif / 60f + min;
                    b = max;
                } else if (h < 300f) {
                    r = (h - 240f) * dif / 60f + min;
                    g = min;
                    b = max;
                } else if (h <= 360f) {
                    r = max;
                    g = min;
                    b = -(h - 360f) * dif / 60 + min;
                } else {
                    r = 0;
                    g = 0;
                    b = 0;
                }
            }
            return Color.FromArgb(hsbColor.A, (int) Math.Round(Math.Min(Math.Max(r, 0), 255)),
                                  (int) Math.Round(Math.Min(Math.Max(g, 0), 255)),
                                  (int) Math.Round(Math.Min(Math.Max(b, 0), 255)));
        }

        public static HSBColor FromColor(Color color) {
            HSBColor ret = new HSBColor(0f, 0f, 0f);
            ret.A = color.A;
            float r = color.R;
            float g = color.G;
            float b = color.B;
            float max = Math.Max(r, Math.Max(g, b));
            if (max <= 0) {
                return ret;
            }
            float min = Math.Min(r, Math.Min(g, b));
            float dif = max - min;
            if (max > min) {
                if (g.IsEqual(max)) {
                    ret.H = (b - r) / dif * 60f + 120f;
                } else if (b.IsEqual(max)) {
                    ret.H = (r - g) / dif * 60f + 240f;
                } else if (b > g) {
                    ret.H = (g - b) / dif * 60f + 360f;
                } else {
                    ret.H = (g - b) / dif * 60f;
                }
                if (ret.H < 0) {
                    ret.H = ret.H + 360f;
                }
            } else {
                ret.H = 0;
            }
            ret.H *= 255f / 360f;
            ret.S = (dif / max) * 255f;
            ret.B = max;
            return ret;
        }

        public static HSBColor FromColor(RGBColor rgbColor) {
            return FromColor(rgbColor.ToColor());
        }
    }
}