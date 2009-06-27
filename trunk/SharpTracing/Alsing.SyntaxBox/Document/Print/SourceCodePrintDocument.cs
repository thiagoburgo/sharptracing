// *
// * Copyright (C) 2008 Roger Alsing : http://www.RogerAlsing.com
// *
// * This library is free software; you can redistribute it and/or modify it
// * under the terms of the GNU Lesser General Public License 2.1 or later, as
// * published by the Free Software Foundation. See the included license.txt
// * or http://www.gnu.org/copyleft/lesser.html for details.
// *
// *
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;

namespace Alsing.SourceCode
{
    /// <summary>
    /// Printer document class.
    /// </summary>
    /// <example >
    /// 
    /// 
    /// <b>Print the content of a SyntaxDocument:</b>
    /// <code>
    /// SourceCodePrintDocument PrintDoc=new SourceCodePrintDocument(MySyntaxDocument);
    ///
    ///	PrintDialog1.Document =PrintDoc;
    ///	if (PrintDialog1.ShowDialog ()==DialogResult.OK)
    ///		PrintDoc.Print ();
    /// </code>
    /// <hr/>
    /// <b>Print Preview the content of a SyntaxDocument</b>
    /// <code>
    /// SourceCodePrintDocument PrintDoc=new SourceCodePrintDocument(MySyntaxDocument);
    /// PrintPreviewDialog1.Document = PrintDoc
    /// PrintPreviewDialog1.ShowDialog ();
    /// </code>
    /// </example>
    [ToolboxItem(true)]
    public class SourceCodePrintDocument : PrintDocument
    {
        private Font fontBreak;
        private Font fontNormal;
        private RowList rc;
        private int RowIndex;
        public SourceCodePrintDocument() {}
        public SourceCodePrintDocument(SyntaxDocument document)
        {
            this.Document = document;
        }
        public SyntaxDocument Document { get; set; }
        //Override OnBeginPrint to set up the font we are going to use
        protected override void OnBeginPrint(PrintEventArgs ev)
        {
            base.OnBeginPrint(ev);
            this.fontNormal = new Font("Courier new", 8, FontStyle.Regular);
            this.fontBreak = new Font("Symbol", 8, FontStyle.Bold);
            //			fontBold						= new Font("Arial", 10,FontStyle.Bold);
            //			fontItalic						= new Font("Arial", 10,FontStyle.Italic);
            //			fontBoldItalic					= new Font("Arial", 10,FontStyle.Bold | FontStyle.Italic);
            //			fontUnderline					= new Font("Arial", 10,FontStyle.Underline);
            //			fontBoldUnderline				= new Font("Arial", 10,FontStyle.Bold | FontStyle.Underline);
            //			fontItalicUnderline				= new Font("Arial", 10,FontStyle.Italic | FontStyle.Underline);
            //			fontBoldItalicUnderline			= new Font("Arial", 10,FontStyle.Bold | FontStyle.Italic | FontStyle.Underline);
            this.RowIndex = 0;
        }
        //Override the OnPrintPage to provide the printing logic for the document
        protected override void OnPrintPage(PrintPageEventArgs ev)
        {
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float rightMargin = ev.MarginBounds.Right;
            float topMargin = ev.MarginBounds.Top;
            //ev.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            if(this.rc == null){
                this.Document.ParseAll();
                this.Document.ParseAll(true);
                this.rc = new RowList();
                foreach(Row r in this.Document){
                    bool hasbreak = false;
                    float x = leftMargin;
                    var newRow = new Row();
                    this.rc.Add(newRow);
                    foreach(Word w in r){
                        Font f = this.fontNormal;
                        if(w.Style != null){
                            FontStyle fs = 0;
                            if(w.Style.Bold){
                                fs |= FontStyle.Bold;
                            }
                            if(w.Style.Italic){
                                fs |= FontStyle.Italic;
                            }
                            if(w.Style.Underline){
                                fs |= FontStyle.Underline;
                            }
                            f = new Font("Courier new", 8, fs);
                        }
                        SizeF sf = ev.Graphics.MeasureString(w.Text, f);
                        if(x + sf.Width > rightMargin){
                            var chr = (char)0xbf;
                            var br = new Word{Text = (chr + ""), InfoTip = "break char"};
                            newRow.Add(br);
                            hasbreak = true;
                            newRow = new Row();
                            this.rc.Add(newRow);
                            x = leftMargin;
                        }
                        x += sf.Width;
                        newRow.Add(w);
                    }
                    if(hasbreak){
                        this.rc.Add(new Row());
                    }
                }
            }
            //------------------------------------------------------
            base.OnPrintPage(ev);
            float lpp = ev.MarginBounds.Height / this.fontNormal.GetHeight(ev.Graphics);
            while(count < lpp && (this.RowIndex < this.rc.Count)){
                float x = leftMargin;
                float yPos = topMargin + (count * this.fontNormal.GetHeight(ev.Graphics));
                Row r = this.rc[this.RowIndex];
                foreach(Word w in r){
                    if(w.InfoTip != null && w.InfoTip == "break char"){
                        ev.Graphics.DrawString(w.Text, this.fontBreak, Brushes.Black, x, yPos, new StringFormat());
                    } else{
                        SizeF sf = ev.Graphics.MeasureString(w.Text, this.fontNormal);
                        if(w.Text != null && (".,:;".IndexOf(w.Text) >= 0)){
                            sf.Width = 6;
                            x -= 4;
                        }
                        if(w.Text == "\t"){
                            sf.Width = ev.Graphics.MeasureString("...", this.fontNormal).Width;
                        }
                        Color c = Color.Black;
                        Font f = this.fontNormal;
                        if(w.Style != null){
                            c = w.Style.ForeColor;
                            FontStyle fs = 0;
                            if(w.Style.Bold){
                                fs |= FontStyle.Bold;
                            }
                            if(w.Style.Italic){
                                fs |= FontStyle.Italic;
                            }
                            if(w.Style.Underline){
                                fs |= FontStyle.Underline;
                            }
                            f = new Font("Courier new", 8, fs);
                            if(!w.Style.Transparent){
                                Color bg = w.Style.BackColor;
                                ev.Graphics.FillRectangle(new SolidBrush(bg), x, yPos, sf.Width,
                                                          this.fontNormal.GetHeight(ev.Graphics));
                            }
                        }
                        c = Color.FromArgb(c.R, c.G, c.B);
                        ev.Graphics.DrawString(w.Text, f, new SolidBrush(c), x, yPos, new StringFormat());
                        x += sf.Width;
                    }
                }
                count++;
                this.RowIndex++;
            }
            //If we have more lines then print another page
            ev.HasMorePages = this.RowIndex < this.rc.Count;
        }
    }
}