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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Alsing.Windows;

namespace Alsing.Drawing.GDI
{
    public class FontList : UITypeEditor
    {
        private IWindowsFormsEditorService edSvc;
        private ListBox FontListbox;
        private bool handleLostfocus;
        private void LB_DrawItem(object sender, DrawItemEventArgs e)
        {
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            if(e.Index == -1){
                return;
            }
            object li = this.FontListbox.Items[e.Index];
            string text = li.ToString();
            Brush fg = selected ? SystemBrushes.HighlightText : SystemBrushes.WindowText;
            if(selected){
                const int ofs = 37;
                e.Graphics.FillRectangle(SystemBrushes.Window,
                                         new Rectangle(ofs, e.Bounds.Top, e.Bounds.Width - ofs,
                                                       this.FontListbox.ItemHeight));
                e.Graphics.FillRectangle(SystemBrushes.Highlight,
                                         new Rectangle(ofs + 1, e.Bounds.Top + 1, e.Bounds.Width - ofs - 2,
                                                       this.FontListbox.ItemHeight - 2));
                ControlPaint.DrawFocusRectangle(e.Graphics,
                                                new Rectangle(ofs, e.Bounds.Top, e.Bounds.Width - ofs,
                                                              this.FontListbox.ItemHeight));
            } else{
                e.Graphics.FillRectangle(SystemBrushes.Window, 0, e.Bounds.Top, e.Bounds.Width,
                                         this.FontListbox.ItemHeight);
            }
            e.Graphics.DrawString(text, e.Font, fg, 38, e.Bounds.Top + 4);
            e.Graphics.SetClip(new Rectangle(1, e.Bounds.Top + 2, 34, this.FontListbox.ItemHeight - 4));
            e.Graphics.FillRectangle(SystemBrushes.Highlight,
                                     new Rectangle(1, e.Bounds.Top + 2, 34, this.FontListbox.ItemHeight - 4));
            IntPtr hdc = e.Graphics.GetHdc();
            var gf = new GDIFont(text, 9);
            int a = 0;
            IntPtr res = NativeMethods.SelectObject(hdc, gf.hFont);
            NativeMethods.SetTextColor(hdc, ColorTranslator.ToWin32(SystemColors.Window));
            NativeMethods.SetBkMode(hdc, 0);
            NativeMethods.TabbedTextOut(hdc, 3, e.Bounds.Top + 5, "abc", 3, 0, ref a, 0);
            NativeMethods.SelectObject(hdc, res);
            gf.Dispose();
            e.Graphics.ReleaseHdc(hdc);
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(1, e.Bounds.Top + 2, 34, this.FontListbox.ItemHeight - 4));
            e.Graphics.ResetClip();
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if(context != null && context.Instance != null && provider != null){
                this.edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if(this.edSvc != null){
                    // Create a CheckedListBox and populate it with all the enum values
                    this.FontListbox = new ListBox{
                                                          DrawMode = DrawMode.OwnerDrawFixed,
                                                          BorderStyle = BorderStyle.None,
                                                          Sorted = true
                                                  };
                    this.FontListbox.MouseDown += this.OnMouseDown;
                    this.FontListbox.DoubleClick += this.ValueChanged;
                    this.FontListbox.DrawItem += this.LB_DrawItem;
                    this.FontListbox.ItemHeight = 20;
                    this.FontListbox.Height = 200;
                    this.FontListbox.Width = 180;
                    ICollection fonts = new FontEnum().EnumFonts();
                    foreach(string font in fonts){
                        this.FontListbox.Items.Add(font);
                    }
                    this.edSvc.DropDownControl(this.FontListbox);
                    if(this.FontListbox.SelectedItem != null){
                        return this.FontListbox.SelectedItem.ToString();
                    }
                }
            }
            return value;
        }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if(!this.handleLostfocus
               && this.FontListbox.ClientRectangle.Contains(this.FontListbox.PointToClient(new Point(e.X, e.Y)))){
                this.FontListbox.LostFocus += this.ValueChanged;
                this.handleLostfocus = true;
            }
        }
        private void ValueChanged(object sender, EventArgs e)
        {
            if(this.edSvc != null){
                this.edSvc.CloseDropDown();
            }
        }
        public override void PaintValue(PaintValueEventArgs e)
        {
            string text = e.Value.ToString();
            var bp = new Bitmap(e.Bounds.Width, e.Bounds.Height);
            Graphics g = Graphics.FromImage(bp);
            g.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            IntPtr hdc = g.GetHdc();
            var gf = new GDIFont(text, 9);
            int a = 0;
            IntPtr res = NativeMethods.SelectObject(hdc, gf.hFont);
            NativeMethods.SetTextColor(hdc, ColorTranslator.ToWin32(SystemColors.Window));
            NativeMethods.SetBkMode(hdc, 0);
            NativeMethods.TabbedTextOut(hdc, 1, 1, "abc", 3, 0, ref a, 0);
            NativeMethods.SelectObject(hdc, res);
            gf.Dispose();
            g.ReleaseHdc(hdc);
            e.Graphics.DrawImage(bp, e.Bounds.Left, e.Bounds.Top);
            //	e.Graphics.DrawString ("abc",new Font (text,10f),SystemBrushes.Window,3,0);
        }
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    public class FontEnum
    {
        private Hashtable Fonts;
        public ICollection EnumFonts()
        {
            var bmp = new Bitmap(10, 10);
            Graphics g = Graphics.FromImage(bmp);
            IntPtr hDC = g.GetHdc();
            this.Fonts = new Hashtable();
            var lf = new LogFont{lfCharSet = 1};
            FONTENUMPROC callback = this.CallbackFunc;
            NativeMethods.EnumFontFamiliesEx(hDC, lf, callback, 0, 0);
            g.ReleaseHdc(hDC);
            g.Dispose();
            bmp.Dispose();
            return this.Fonts.Keys;
        }
        private int CallbackFunc(ENUMLOGFONTEX f, int a, int b, int LParam)
        {
            this.Fonts[f.elfLogFont.lfFaceName] = "abc";
            return 1;
        }
    }
}