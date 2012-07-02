// *
// * Copyright (C) 2008 Roger Alsing : http://www.RogerAlsing.com
// *
// * This library is free software; you can redistribute it and/or modify it
// * under the terms of the GNU Lesser General Public License 2.1 or later, as
// * published by the Free Software Foundation. See the included license.txt
// * or http://www.gnu.org/copyleft/lesser.html for details.
// *
// *
using System;
using Alsing.Windows;

namespace Alsing.Drawing.GDI
{
    public class GDIFont : GDIObject
    {
        public bool Bold;
        public byte Charset;
        public string FontName;
        public IntPtr hFont;
        public bool Italic;
        public float Size;
        public bool Strikethrough;
        public bool Underline;
        public GDIFont()
        {
            this.Create();
        }
        public GDIFont(string fontname, float size)
        {
            this.Init(fontname, size, false, false, false, false);
            this.Create();
        }
        public GDIFont(string fontname, float size, bool bold, bool italic, bool underline, bool strikethrough)
        {
            this.Init(fontname, size, bold, italic, underline, strikethrough);
            this.Create();
        }
        protected void Init(string fontname, float size, bool bold, bool italic, bool underline, bool strikethrough)
        {
            this.FontName = fontname;
            this.Size = size;
            this.Bold = bold;
            this.Italic = italic;
            this.Underline = underline;
            this.Strikethrough = strikethrough;
            var tFont = new LogFont{
                                           lfItalic = ((byte)(this.Italic ? 1 : 0)),
                                           lfStrikeOut = ((byte)(this.Strikethrough ? 1 : 0)),
                                           lfUnderline = ((byte)(this.Underline ? 1 : 0)),
                                           lfWeight = (this.Bold ? 700 : 400),
                                           lfWidth = 0,
                                           lfHeight = ((int)(-this.Size * 1.3333333333333)),
                                           lfCharSet = 1,
                                           lfFaceName = this.FontName
                                   };
            this.hFont = NativeMethods.CreateFontIndirect(tFont);
        }
        ~GDIFont()
        {
            this.Destroy();
        }
        protected override void Destroy()
        {
            if(this.hFont != (IntPtr)0){
                NativeMethods.DeleteObject(this.hFont);
            }
            base.Destroy();
            this.hFont = (IntPtr)0;
        }
    }
}