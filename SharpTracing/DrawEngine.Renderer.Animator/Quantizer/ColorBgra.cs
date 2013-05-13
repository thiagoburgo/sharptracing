/////////////////////////////////////////////////////////////////////////////////
// Paint.NET
// Copyright (C) Rick Brewster, Chris Crosetto, Dennis Dietrich, Tom Jackson, 
//               Michael Kelsey, Brandon Ortiz, Craig Taylor, Chris Trevino, 
//               and Luke Walker
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.
// See src/setup/License.rtf for complete licensing and attribution information.
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DrawEngine.Renderer.Animator.Quantizer {
    /// <summary>
    /// This is our pixel format that we will work with. It is always 32-bits / 4-bytes and is
    /// always laid out in BGRA order.
    /// Generally used with the Surface class.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct ColorBgra {
        [FieldOffset(0)] public byte B;
        [FieldOffset(1)] public byte G;
        [FieldOffset(2)] public byte R;
        [FieldOffset(3)] public byte A;

        /// <summary>
        /// Lets you change B, G, R, and A at the same time.
        /// </summary>
        [FieldOffset(0)] public uint Bgra;

        public const int BlueChannel = 0;
        public const int GreenChannel = 1;
        public const int RedChannel = 2;
        public const int AlphaChannel = 3;
        public const int SizeOf = 4;

        /// <summary>
        /// Gets or sets the byte value of the specified color channel.
        /// </summary>
        public unsafe byte this[int channel] {
            get {
                if (channel < 0 || channel > 3) {
                    throw new ArgumentOutOfRangeException("channel", channel, "valid range is [0,3]");
                }
                fixed (byte* p = &this.B) {
                    return p[channel];
                }
            }
            set {
                if (channel < 0 || channel > 3) {
                    throw new ArgumentOutOfRangeException("channel", channel, "valid range is [0,3]");
                }
                fixed (byte* p = &this.B) {
                    p[channel] = value;
                }
            }
        }

        /// <summary>
        /// Compares two ColorBgra instance to determine if they are equal.
        /// </summary>
        public static bool operator ==(ColorBgra lhs, ColorBgra rhs) {
            return lhs.Bgra == rhs.Bgra;
        }

        /// <summary>
        /// Compares two ColorBgra instance to determine if they are not equal.
        /// </summary>
        public static bool operator !=(ColorBgra lhs, ColorBgra rhs) {
            return lhs.Bgra != rhs.Bgra;
        }

        /// <summary>
        /// Compares two ColorBgra instance to determine if they are equal.
        /// </summary>
        public override bool Equals(object obj) {
            if (obj != null && obj is ColorBgra && ((ColorBgra) obj).Bgra == this.Bgra) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this color value.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            unchecked {
                return (int) this.Bgra;
            }
        }

        public static ColorBgra Clamped(int b, int g, int r, int a) {
            return Clamped((double) b, (double) g, (double) r, (double) a);
        }

        public static ColorBgra Clamped(float b, float g, float r, float a) {
            return Clamped((double) b, (double) g, (double) r, (double) a);
        }

        public static ColorBgra Clamped(double b, double g, double r, double a) {
            ColorBgra color = new ColorBgra();
            if (r > 255) {
                color.R = 255;
            } else if (r < 0) {
                color.R = 0;
            } else {
                color.R = (byte) r;
            }
            if (g > 255) {
                color.G = 255;
            } else if (g < 0) {
                color.G = 0;
            } else {
                color.G = (byte) g;
            }
            if (b > 255) {
                color.B = 255;
            } else if (b < 0) {
                color.B = 0;
            } else {
                color.B = (byte) b;
            }
            if (a > 255) {
                color.A = 255;
            } else if (a < 0) {
                color.A = 0;
            } else {
                color.A = (byte) a;
            }
            return color;
        }

        /// <summary>
        /// Creates a new ColorBgra instance with the given color and alpha values.
        /// </summary>
        public static ColorBgra FromRgba(byte r, byte g, byte b, byte a) {
            ColorBgra color = new ColorBgra();
            color.R = r;
            color.G = g;
            color.B = b;
            color.A = a;
            return color;
        }

        /// <summary>
        /// Creates a new ColorBgra instance with the given color values, and 255 for alpha.
        /// </summary>
        public static ColorBgra FromRgb(byte r, byte g, byte b) {
            return FromRgba(r, g, b, 255);
        }

        /// <summary>
        /// Creates a new ColorBgra instance with the given color and alpha values.
        /// </summary>
        public static ColorBgra FromBgra(byte b, byte g, byte r, byte a) {
            ColorBgra color = new ColorBgra();
            color.Bgra = BgraToUInt32(b, g, r, a);
            return color;
        }

        /// <summary>
        /// Packs color and alpha values into a 32-bit integer.
        /// </summary>
        public static UInt32 BgraToUInt32(byte b, byte g, byte r, byte a) {
            return (uint) b + ((uint) g << 8) + ((uint) r << 16) + ((uint) a << 24);
        }

        /// <summary>
        /// Packs color and alpha values into a 32-bit integer.
        /// </summary>
        public static UInt32 BgraToUInt32(int b, int g, int r, int a) {
            return (uint) b + ((uint) g << 8) + ((uint) r << 16) + ((uint) a << 24);
        }

        /// <summary>
        /// Creates a new ColorBgra instance with the given color values, and 255 for alpha.
        /// </summary>
        public static ColorBgra FromBgr(byte b, byte g, byte r) {
            return FromRgb(r, g, b);
        }

        /// <summary>
        /// Constructs a new ColorBgra instance with the given 32-bit value.
        /// </summary>
        public static ColorBgra FromUInt32(UInt32 bgra) {
            ColorBgra color = new ColorBgra();
            color.Bgra = bgra;
            return color;
        }

        /// <summary>
        /// Constructs a new ColorBgra instance from the values in the given Color instance.
        /// </summary>
        public static ColorBgra FromColor(Color c) {
            return FromRgba(c.R, c.G, c.B, c.A);
        }

        /// <summary>
        /// Converts this ColorBgra instance to a Color instance.
        /// </summary>
        public Color ToColor() {
            return Color.FromArgb(this.A, this.R, this.G, this.B);
        }

        public override string ToString() {
            return "B: " + this.B + ", G: " + this.G + ", R: " + this.R + ", A: " + this.A;
        }

        /// <summary>
        /// Casts a ColorBgra to a UInt32.
        /// </summary>
        public static explicit operator UInt32(ColorBgra color) {
            return color.Bgra;
        }

        /// <summary>
        /// Casts a UInt32 to a ColorBgra.
        /// </summary>
        public static explicit operator ColorBgra(UInt32 uint32) {
            return FromUInt32(uint32);
        }
    }
}