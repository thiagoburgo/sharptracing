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
using System.Drawing;
using System.Windows.Forms;
using Alsing.Windows;

namespace Alsing.Drawing.GDI
{
    public class GDISurface : GDIObject
    {
        private WeakReference _Control;
        protected IntPtr _OldBmp = IntPtr.Zero;
        protected IntPtr _OldBrush = IntPtr.Zero;
        protected IntPtr _OldFont = IntPtr.Zero;
        protected IntPtr _OldPen = IntPtr.Zero;
        protected IntPtr mhBMP;
        protected IntPtr mhDC;
        protected int mHeight;
        protected int mTabSize = 4;
        protected int mWidth;
        public GDISurface(IntPtr hDC)
        {
            this.mhDC = hDC;
        }
        public GDISurface(int width, int height, IntPtr hdc)
        {
            this.Init(width, height, hdc);
            this.Create();
        }
        public GDISurface(int width, int height, GDISurface surface)
        {
            this.Init(width, height, surface.hDC);
            this.Create();
        }
        public GDISurface(int width, int height, Control CompatibleControl, bool BindControl)
        {
            IntPtr hDCControk = NativeMethods.ControlDC(CompatibleControl);
            this.Init(width, height, hDCControk);
            NativeMethods.ReleaseDC(CompatibleControl.Handle, hDCControk);
            if(BindControl){
                this.Control = CompatibleControl;
            }
            this.Create();
        }
        private Control Control
        {
            get
            {
                if(this._Control != null){
                    return (Control)this._Control.Target;
                }
                return null;
            }
            set { this._Control = new WeakReference(value); }
        }
        public IntPtr hDC
        {
            get { return this.mhDC; }
        }
        public IntPtr hBMP
        {
            get { return this.mhBMP; }
        }
        public Color TextForeColor
        {
            //map get,settextcolor
            get { return NativeMethods.IntToColor(NativeMethods.GetTextColor(this.mhDC)); }
            set { NativeMethods.SetTextColor(this.mhDC, NativeMethods.ColorToInt(value)); }
        }
        public Color TextBackColor
        {
            //map get,setbkcolor
            get { return NativeMethods.IntToColor(NativeMethods.GetBkColor(this.mhDC)); }
            set { NativeMethods.SetBkColor(this.mhDC, NativeMethods.ColorToInt(value)); }
        }
        public bool FontTransparent
        {
            //map get,setbkmode
            //1=transparent , 2=solid
            get { return NativeMethods.GetBkMode(this.mhDC) < 2; }
            set { NativeMethods.SetBkMode(this.mhDC, value ? 1 : 2); }
        }
        public GDIFont Font
        {
            get
            {
                var tm = new GDITextMetric();
                var fontname = new string(' ', 48);
                NativeMethods.GetTextMetrics(this.mhDC, ref tm);
                NativeMethods.GetTextFace(this.mhDC, 79, fontname);
                var gf = new GDIFont{
                                            FontName = fontname,
                                            Bold = (tm.tmWeight > 400),
                                            Italic = (tm.tmItalic != 0),
                                            Underline = (tm.tmUnderlined != 0),
                                            Strikethrough = (tm.tmStruckOut != 0),
                                            Size = ((int)(((tm.tmMemoryHeight) / (double)tm.tmDigitizedAspectY) * 72))
                                    };
                return gf;
            }
            set
            {
                IntPtr res = NativeMethods.SelectObject(this.mhDC, value.hFont);
                if(this._OldFont == IntPtr.Zero){
                    this._OldFont = res;
                }
            }
        }
        protected void Init(int width, int height, IntPtr hdc)
        {
            this.mWidth = width;
            this.mHeight = height;
            this.mhDC = NativeMethods.CreateCompatibleDC(hdc);
            this.mhBMP = NativeMethods.CreateCompatibleBitmap(hdc, width, height);
            IntPtr ret = NativeMethods.SelectObject(this.mhDC, this.mhBMP);
            this._OldBmp = ret;
            if(this.mhDC == (IntPtr)0){
                throw new OutOfMemoryException("hDC creation FAILED!!");
            }
            if(this.mhDC == (IntPtr)0){
                throw new OutOfMemoryException("hBMP creation FAILED!!");
            }
        }
        public Size MeasureString(string Text)
        {
            //map GetTabbedTextExtent
            //to be implemented
            return new Size(0, 0);
        }
        public Size MeasureTabbedString(string Text, int tabsize)
        {
            int ret = NativeMethods.GetTabbedTextExtent(this.mhDC, Text, Text.Length, 1, ref tabsize);
            return new Size(ret & 0xFFFF, (ret >> 16) & 0xFFFF);
        }
        public void DrawString(string Text, int x, int y, int width, int height)
        {
            //to be implemented
            //map DrawText
        }
        public Size DrawTabbedString(string Text, int x, int y, int taborigin, int tabsize)
        {
            int ret = NativeMethods.TabbedTextOut(this.mhDC, x, y, Text, Text.Length, 1, ref tabsize, taborigin);
            return new Size(ret & 0xFFFF, (ret >> 16) & 0xFFFF);
        }
        //---------------------------------------
        //render methods , 
        //render to dc ,
        //render to control
        //render to gdisurface
        public void RenderTo(IntPtr hdc, int x, int y)
        {
            //map bitblt
            NativeMethods.BitBlt(hdc, x, y, this.mWidth, this.mHeight, this.mhDC, 0, 0, (int)GDIRop.SrcCopy);
        }
        public void RenderTo(GDISurface target, int x, int y)
        {
            RenderTo(target.hDC, x, y);
        }
        public void RenderTo(GDISurface target, int SourceX, int SourceY, int Width, int Height, int DestX, int DestY)
        {
            NativeMethods.BitBlt(target.hDC, DestX, DestY, Width, Height, this.hDC, SourceX, SourceY,
                                 (int)GDIRop.SrcCopy);
        }
        public void RenderToControl(int x, int y)
        {
            IntPtr hdc = NativeMethods.ControlDC(this.Control);
            RenderTo(hdc, x, y);
            NativeMethods.ReleaseDC(this.Control.Handle, hdc);
        }
        //---------------------------------------
        public Graphics CreateGraphics()
        {
            return Graphics.FromHdc(this.mhDC);
        }
        //---------------------------------------
        public void FillRect(GDIBrush brush, int x, int y, int width, int height)
        {
            APIRect gr;
            gr.top = y;
            gr.left = x;
            gr.right = width + x;
            gr.bottom = height + y;
            NativeMethods.FillRect(this.mhDC, ref gr, brush.hBrush);
        }
        public void DrawFocusRect(int x, int y, int width, int height)
        {
            APIRect gr;
            gr.top = y;
            gr.left = x;
            gr.right = width + x;
            gr.bottom = height + y;
            NativeMethods.DrawFocusRect(this.mhDC, ref gr);
        }
        public void FillRect(Color color, int x, int y, int width, int height)
        {
            var b = new GDIBrush(color);
            FillRect(b, x, y, width, height);
            b.Dispose();
        }
        public void InvertRect(int x, int y, int width, int height)
        {
            APIRect gr;
            gr.top = y;
            gr.left = x;
            gr.right = width + x;
            gr.bottom = height + y;
            NativeMethods.InvertRect(this.mhDC, ref gr);
        }
        public void DrawLine(GDIPen pen, Point p1, Point p2)
        {
            IntPtr oldpen = NativeMethods.SelectObject(this.mhDC, pen.hPen);
            APIPoint gp;
            gp.x = 0;
            gp.y = 0;
            NativeMethods.MoveToEx(this.mhDC, p1.X, p1.Y, ref gp);
            NativeMethods.LineTo(this.mhDC, p2.X, p2.Y);
            NativeMethods.SelectObject(this.mhDC, oldpen);
        }
        public void DrawLine(Color color, Point p1, Point p2)
        {
            var p = new GDIPen(color, 1);
            DrawLine(p, p1, p2);
            p.Dispose();
        }
        public void DrawRect(Color color, int left, int top, int width, int height)
        {
            var p = new GDIPen(color, 1);
            DrawRect(p, left, top, width, height);
            p.Dispose();
        }
        public void DrawRect(GDIPen pen, int left, int top, int width, int height)
        {
            DrawLine(pen, new Point(left, top), new Point(left + width, top));
            DrawLine(pen, new Point(left, top + height), new Point(left + width, top + height));
            DrawLine(pen, new Point(left, top), new Point(left, top + height));
            DrawLine(pen, new Point(left + width, top), new Point(left + width, top + height + 1));
        }
        public void Clear(Color color)
        {
            var b = new GDIBrush(color);
            Clear(b);
            b.Dispose();
        }
        public void Clear(GDIBrush brush)
        {
            FillRect(brush, 0, 0, this.mWidth, this.mHeight);
        }
        public void Flush()
        {
            NativeMethods.GdiFlush();
        }
        protected override void Destroy()
        {
            if(this._OldBmp != IntPtr.Zero){
                NativeMethods.SelectObject(this.hDC, this._OldBmp);
            }
            if(this._OldFont != IntPtr.Zero){
                NativeMethods.SelectObject(this.hDC, this._OldFont);
            }
            if(this._OldPen != IntPtr.Zero){
                NativeMethods.SelectObject(this.hDC, this._OldPen);
            }
            if(this._OldBrush != IntPtr.Zero){
                NativeMethods.SelectObject(this.hDC, this._OldBrush);
            }
            if(this.mhBMP != (IntPtr)0){
                NativeMethods.DeleteObject(this.mhBMP);
            }
            if(this.mhDC != (IntPtr)0){
                NativeMethods.DeleteDC(this.mhDC);
            }
            this.mhBMP = (IntPtr)0;
            this.mhDC = (IntPtr)0;
            base.Destroy();
        }
        public void SetBrushOrg(int x, int y)
        {
            APIPoint p;
            p.x = 0;
            p.y = 0;
            NativeMethods.SetBrushOrgEx(this.mhDC, x, y, ref p);
        }
    }
}