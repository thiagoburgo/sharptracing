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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Alsing.Drawing.GDI;
using Alsing.Globalization;
using Alsing.SourceCode;

namespace Alsing.Windows.Forms.SyntaxBox.Painter
{
    /// <summary>
    /// Painter class that uses GDI32 to render the content of a SyntaxBoxControl
    /// </summary>
    public class NativePainter : IPainter
    {
        private readonly EditViewControl Control;
        private Word BracketEnd;
        private Word BracketStart;
        private int FirstSpanRow;
        private RenderItems GFX;
        private int LastRow;
        private int LastSpanRow;
        private bool RenderCaretRowOnly;
        private int ResizeCount;
        private bool SpanFound;
        private int yOffset;
        /// <summary>
        /// NativePainter constructor.
        /// </summary>
        /// <param name="control">The control that will use the Painter</param>
        public NativePainter(EditViewControl control)
        {
            this.Control = control;
            this.InitGraphics();
        }

        #region IPainter Members
        /// <summary>
        /// Implementation of the IPainter Resize method
        /// </summary>
        public void Resize()
        {
            this.ResizeCount++;
            this.InitGraphics();
            //	Console.WriteLine ("painterresize {0} {1}",ResizeCount,Control.Name);
        }
        /// <summary>
        /// Implementation of the IPainter MeasureString method
        /// </summary>
        /// <param name="s">String to measure</param>
        /// <returns>Size of the string in pixels</returns>
        public Size MeasureString(string s)
        {
            try{
                this.GFX.StringBuffer.Font = this.GFX.FontNormal;
                return this.GFX.StringBuffer.MeasureTabbedString(s, this.Control.TabSize);
            } catch{
                return new Size(0, 0);
            }
        }
        /// <summary>
        /// Implementation of the IPainter InitGraphics method.
        /// Initializes GDI32 backbuffers and brushes.
        /// </summary>
        public void InitGraphics()
        {
            try{
                if(this.GFX.BackgroundBrush != null){
                    this.GFX.BackgroundBrush.Dispose();
                }
                if(this.GFX.GutterMarginBrush != null){
                    this.GFX.GutterMarginBrush.Dispose();
                }
                if(this.GFX.LineNumberMarginBrush != null){
                    this.GFX.LineNumberMarginBrush.Dispose();
                }
                if(this.GFX.HighLightLineBrush != null){
                    this.GFX.HighLightLineBrush.Dispose();
                }
                if(this.GFX.LineNumberMarginBorderBrush != null){
                    this.GFX.LineNumberMarginBorderBrush.Dispose();
                }
                if(this.GFX.GutterMarginBorderBrush != null){
                    this.GFX.GutterMarginBorderBrush.Dispose();
                }
                if(this.GFX.OutlineBrush != null){
                    this.GFX.OutlineBrush.Dispose();
                }
                this.GFX.BackgroundBrush = new GDIBrush(this.Control.BackColor);
                this.GFX.GutterMarginBrush = new GDIBrush(this.Control.GutterMarginColor);
                this.GFX.LineNumberMarginBrush = new GDIBrush(this.Control.LineNumberBackColor);
                this.GFX.HighLightLineBrush = new GDIBrush(this.Control.HighLightedLineColor);
                this.GFX.LineNumberMarginBorderBrush = new GDIBrush(this.Control.LineNumberBorderColor);
                this.GFX.GutterMarginBorderBrush = new GDIBrush(this.Control.GutterMarginBorderColor);
                this.GFX.OutlineBrush = new GDIBrush(this.Control.OutlineColor);
                if(this.GFX.FontNormal != null){
                    this.GFX.FontNormal.Dispose();
                }
                if(this.GFX.FontBold != null){
                    this.GFX.FontBold.Dispose();
                }
                if(this.GFX.FontItalic != null){
                    this.GFX.FontItalic.Dispose();
                }
                if(this.GFX.FontBoldItalic != null){
                    this.GFX.FontBoldItalic.Dispose();
                }
                if(this.GFX.FontUnderline != null){
                    this.GFX.FontUnderline.Dispose();
                }
                if(this.GFX.FontBoldUnderline != null){
                    this.GFX.FontBoldUnderline.Dispose();
                }
                if(this.GFX.FontItalicUnderline != null){
                    this.GFX.FontItalicUnderline.Dispose();
                }
                if(this.GFX.FontBoldItalicUnderline != null){
                    this.GFX.FontBoldItalicUnderline.Dispose();
                }
                //	string font="courier new";
                string font = this.Control.FontName;
                float fontsize = this.Control.FontSize;
                this.GFX.FontNormal = new GDIFont(font, fontsize, false, false, false, false);
                this.GFX.FontBold = new GDIFont(font, fontsize, true, false, false, false);
                this.GFX.FontItalic = new GDIFont(font, fontsize, false, true, false, false);
                this.GFX.FontBoldItalic = new GDIFont(font, fontsize, true, true, false, false);
                this.GFX.FontUnderline = new GDIFont(font, fontsize, false, false, true, false);
                this.GFX.FontBoldUnderline = new GDIFont(font, fontsize, true, false, true, false);
                this.GFX.FontItalicUnderline = new GDIFont(font, fontsize, false, true, true, false);
                this.GFX.FontBoldItalicUnderline = new GDIFont(font, fontsize, true, true, true, false);
                this.InitIMEWindow();
            } catch(Exception){}
            if(this.Control != null){
                if(this.Control.IsHandleCreated){
                    if(this.GFX.StringBuffer != null){
                        this.GFX.StringBuffer.Dispose();
                    }
                    if(this.GFX.SelectionBuffer != null){
                        this.GFX.SelectionBuffer.Dispose();
                    }
                    if(this.GFX.BackBuffer != null){
                        this.GFX.BackBuffer.Dispose();
                    }
                    this.GFX.StringBuffer = new GDISurface(1, 1, this.Control, true){Font = this.GFX.FontNormal};
                    int h = this.GFX.StringBuffer.MeasureTabbedString("ABC", 0).Height
                            + this.Control._SyntaxBox.RowPadding;
                    this.GFX.BackBuffer = new GDISurface(this.Control.ClientWidth, h, this.Control, true)
                                          {Font = this.GFX.FontNormal};
                    this.GFX.SelectionBuffer = new GDISurface(this.Control.ClientWidth, h, this.Control, true)
                                               {Font = this.GFX.FontNormal};
                    this.Control.View.RowHeight = this.GFX.BackBuffer.MeasureTabbedString("ABC", 0).Height
                                                  + this.Control._SyntaxBox.RowPadding;
                    this.Control.View.CharWidth = this.GFX.BackBuffer.MeasureTabbedString(" ", 0).Width;
                }
            }
        }
        /// <summary>
        /// Implementation of the IPainter RenderAll method.
        /// </summary>
        public void RenderAll()
        {
            //
            this.Control.View.RowHeight = this.GFX.BackBuffer.MeasureString("ABC").Height;
            this.Control.View.CharWidth = this.GFX.BackBuffer.MeasureString(" ").Width;
            this.Control.InitVars();
            Graphics g = this.Control.CreateGraphics();
            this.RenderAll(g);
            g.Dispose();
        }
        public void RenderCaret(Graphics g)
        {
            this.RenderCaretRowOnly = true;
            this.RenderAll(g);
            this.RenderCaretRowOnly = false;
        }
        /// <summary>
        /// Implementation of the IPainter RenderAll method
        /// </summary>
        /// <param name="g">Target Graphics object</param>
        public void RenderAll(Graphics g)
        {
            try{
                this.Control.InitVars();
                this.Control.InitScrollbars();
                this.SetBrackets();
                this.SetSpanIndicators();
                int j = this.Control.View.FirstVisibleRow;
                int diff = j - this.LastRow;
                this.LastRow = j;
                if(this.Control.SmoothScroll){
                    if(diff == 1){
                        for(int i = this.Control.View.RowHeight; i > 0; i -= this.Control.SmoothScrollSpeed){
                            this.yOffset = i + this.Control.View.YOffset;
                            this.RenderAll2();
                            g.Flush();
                            Thread.Sleep(0);
                        }
                    } else if(diff == -1){
                        for(int i = -this.Control.View.RowHeight; i < 0; i += this.Control.SmoothScrollSpeed){
                            this.yOffset = i + this.Control.View.YOffset;
                            this.RenderAll2();
                            g.Flush();
                            Thread.Sleep(0);
                        }
                    }
                }
                this.yOffset = this.Control.View.YOffset;
                this.RenderAll2();
                //g.Flush ();
                //System.Threading.Thread.Sleep (0);
            } catch{}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RowIndex"></param>
        public void RenderRow(int RowIndex)
        {
            this.RenderRow(RowIndex, 10);
        }
        /// <summary>
        /// Implementation of the iPainter CharFromPixel method
        /// </summary>
        /// <param name="X">Screen x position in pixels</param>
        /// <param name="Y">Screen y position in pixels</param>
        /// <returns>a Point where x is the column and y is the rowindex</returns>
        public TextPoint CharFromPixel(int X, int Y)
        {
            try{
                int RowIndex = Y / this.Control.View.RowHeight + this.Control.View.FirstVisibleRow;
                RowIndex = Math.Min(RowIndex, this.Control.Document.VisibleRows.Count);
                if(RowIndex == this.Control.Document.VisibleRows.Count){
                    RowIndex--;
                    Row r = this.Control.Document.VisibleRows[RowIndex];
                    if(r.IsCollapsed){
                        r = r.Expansion_EndRow;
                    }
                    return new TextPoint(r.Text.Length, r.Index);
                }
                RowIndex = Math.Max(RowIndex, 0);
                Row row;
                if(this.Control.Document.VisibleRows.Count != 0){
                    row = this.Control.Document.VisibleRows[RowIndex];
                    RowIndex = this.Control.Document.IndexOf(row);
                } else{
                    return new TextPoint(0, 0);
                }
                if(RowIndex == -1){
                    return new TextPoint(-1, -1);
                }
                //normal line
                if(!row.IsCollapsed){
                    return this.ColumnFromPixel(RowIndex, X);
                }
                //this.RenderRow (xtr.Index,-200);
                if(X < row.Expansion_PixelEnd - this.Control.View.FirstVisibleColumn * this.Control.View.CharWidth){
                    //start of collapsed line
                    return this.ColumnFromPixel(RowIndex, X);
                }
                if(X
                   >=
                   row.Expansion_EndRow.Expansion_PixelStart
                   - this.Control.View.FirstVisibleColumn * this.Control.View.CharWidth + this.Control.View.TextMargin){
                    //end of collapsed line
                    return this.ColumnFromPixel(row.Expansion_EndRow.Index,
                                                X - row.Expansion_EndRow.Expansion_PixelStart
                                                +
                                                this.MeasureRow(row.Expansion_EndRow,
                                                                row.Expansion_EndRow.Expansion_StartChar).Width);
                }
                //the collapsed text
                return new TextPoint(row.Expansion_EndChar, row.Index);
            } catch{
                this.Control._SyntaxBox.FontName = "Courier New";
                this.Control._SyntaxBox.FontSize = 10;
                return new TextPoint(0, 0);
            }
        }
        public int GetMaxCharWidth()
        {
            const string s = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int max = 0;
            foreach(char c in s){
                int tmp = this.MeasureString(c + "").Width;
                if(tmp > max){
                    max = tmp;
                }
            }
            return max;
        }
        public void Dispose()
        {
            if(this.GFX.FontNormal != null){
                this.GFX.FontNormal.Dispose();
            }
            if(this.GFX.FontBold != null){
                this.GFX.FontBold.Dispose();
            }
            if(this.GFX.FontItalic != null){
                this.GFX.FontItalic.Dispose();
            }
            if(this.GFX.FontBoldItalic != null){
                this.GFX.FontBoldItalic.Dispose();
            }
            if(this.GFX.FontUnderline != null){
                this.GFX.FontUnderline.Dispose();
            }
            if(this.GFX.FontBoldUnderline != null){
                this.GFX.FontBoldUnderline.Dispose();
            }
            if(this.GFX.FontItalicUnderline != null){
                this.GFX.FontItalicUnderline.Dispose();
            }
            if(this.GFX.FontBoldItalicUnderline != null){
                this.GFX.FontBoldItalicUnderline.Dispose();
            }
            if(this.GFX.BackgroundBrush != null){
                this.GFX.BackgroundBrush.Dispose();
            }
            if(this.GFX.GutterMarginBrush != null){
                this.GFX.GutterMarginBrush.Dispose();
            }
            if(this.GFX.LineNumberMarginBrush != null){
                this.GFX.LineNumberMarginBrush.Dispose();
            }
            if(this.GFX.HighLightLineBrush != null){
                this.GFX.HighLightLineBrush.Dispose();
            }
            if(this.GFX.LineNumberMarginBorderBrush != null){
                this.GFX.LineNumberMarginBorderBrush.Dispose();
            }
            if(this.GFX.GutterMarginBorderBrush != null){
                this.GFX.GutterMarginBorderBrush.Dispose();
            }
            if(this.GFX.OutlineBrush != null){
                this.GFX.OutlineBrush.Dispose();
            }
            if(this.GFX.StringBuffer != null){
                this.GFX.StringBuffer.Dispose();
            }
            if(this.GFX.SelectionBuffer != null){
                this.GFX.SelectionBuffer.Dispose();
            }
            if(this.GFX.BackBuffer != null){
                this.GFX.BackBuffer.Dispose();
            }
        }
        /// <summary>
        /// Implementation of the IPainter MeasureRow method.
        /// </summary>
        /// <param name="xtr">Row to measure</param>
        /// <param name="Count">Last char index</param>
        /// <returns>The size of the row in pixels</returns>
        public Size MeasureRow(Row xtr, int Count)
        {
            int width = 0;
            int taborig = -this.Control.View.FirstVisibleColumn * this.Control.View.CharWidth
                          + this.Control.View.TextMargin;
            int xpos = this.Control.View.TextMargin - this.Control.View.ClientAreaStart;
            if(xtr.InQueue){
                this.SetStringFont(false, false, false);
                int Padd = Math.Max(Count - xtr.Text.Length, 0);
                var PaddStr = new String(' ', Padd);
                string TotStr = xtr.Text + PaddStr;
                width =
                        this.GFX.StringBuffer.MeasureTabbedString(TotStr.Substring(0, Count), this.Control.PixelTabSize)
                                .Width;
            } else{
                int CharNo = 0;
                int TotWidth = 0;
                foreach(Word w in xtr.FormattedWords){
                    if(w.Type == WordType.Word && w.Style != null){
                        this.SetStringFont(w.Style.Bold, w.Style.Italic, w.Style.Underline);
                    } else{
                        this.SetStringFont(false, false, false);
                    }
                    if(w.Text.Length + CharNo >= Count || w == xtr.FormattedWords[xtr.FormattedWords.Count - 1]){
                        int CharPos = Count - CharNo;
                        int MaxChars = Math.Min(CharPos, w.Text.Length);
                        TotWidth +=
                                this.GFX.StringBuffer.DrawTabbedString(w.Text.Substring(0, MaxChars), xpos + TotWidth, 0,
                                                                       taborig, this.Control.PixelTabSize).Width;
                        width = TotWidth;
                        break;
                    }
                    TotWidth +=
                            this.GFX.StringBuffer.DrawTabbedString(w.Text, xpos + TotWidth, 0, taborig,
                                                                   this.Control.PixelTabSize).Width;
                    CharNo += w.Text.Length;
                }
                this.SetStringFont(false, false, false);
                int Padd = Math.Max(Count - xtr.Text.Length, 0);
                var PaddStr = new String(' ', Padd);
                width +=
                        this.GFX.StringBuffer.DrawTabbedString(PaddStr, xpos + TotWidth, 0, taborig,
                                                               this.Control.PixelTabSize).Width;
            }
            return new Size(width, 0);
            //	return GFX.BackBuffer.MeasureTabbedString (xtr.Text.Substring (0,Count),Control.PixelTabSize);
        }
        #endregion

        private void InitIMEWindow()
        {
            if(this.Control.IMEWindow != null){
                this.Control.IMEWindow.SetFont(this.Control.FontName, this.Control.FontSize);
            }
        }
        private void SetBrackets()
        {
            Span currentSpan;
            this.BracketEnd = null;
            this.BracketStart = null;
            Word CurrWord = this.Control.Caret.CurrentWord;
            if(CurrWord != null){
                currentSpan = CurrWord.Span;
                if(currentSpan != null){
                    if(CurrWord == currentSpan.StartWord || CurrWord == currentSpan.EndWord){
                        if(currentSpan.EndWord != null){
                            this.BracketEnd = currentSpan.EndWord;
                            this.BracketStart = currentSpan.StartWord;
                        }
                    }
                    try{
                        if(CurrWord.Pattern == null){
                            return;
                        }
                        if(CurrWord.Pattern.BracketType == BracketType.EndBracket){
                            Word w = this.Control.Document.GetStartBracketWord(CurrWord,
                                                                               CurrWord.Pattern.MatchingBracket,
                                                                               CurrWord.Span);
                            this.BracketEnd = CurrWord;
                            this.BracketStart = w;
                        }
                        if(CurrWord.Pattern.BracketType == BracketType.StartBracket){
                            Word w = this.Control.Document.GetEndBracketWord(CurrWord, CurrWord.Pattern.MatchingBracket,
                                                                             CurrWord.Span);
                            //	if(w!=null)
                            //	{
                            this.BracketEnd = w;
                            this.BracketStart = CurrWord;
                            //	}
                        }
                    } catch{}
                }
            }
        }
        private void SetSpanIndicators()
        {
            this.SpanFound = false;
            try{
                Span s = this.Control.Caret.CurrentSegment();
                if(s == null || s.StartWord == null || s.StartWord.Row == null || s.EndWord == null
                   || s.EndWord.Row == null){
                    return;
                }
                this.FirstSpanRow = s.StartWord.Row.Index;
                this.LastSpanRow = s.EndWord.Row.Index;
                this.SpanFound = true;
            } catch{}
        }
        private void RenderAll2()
        {
            try{
                int j = this.Control.View.FirstVisibleRow;
                if(this.Control.AutoListStartPos != null){
                    try{
                        if(this.Control.AutoListVisible){
                            Point alP = this.GetTextPointPixelPos(this.Control.AutoListStartPos);
                            if(alP == new Point(-1, -1)){
                                this.Control.AutoList.Visible = false;
                            } else{
                                alP.Y += this.Control.View.RowHeight + 2;
                                alP.X += -20;
                                alP = this.Control.PointToScreen(alP);
                                Screen screen = Screen.FromPoint(new Point(this.Control.Right, alP.Y));
                                if(alP.Y + this.Control.AutoList.Height > screen.WorkingArea.Height){
                                    alP.Y -= this.Control.View.RowHeight + 2 + this.Control.AutoList.Height;
                                }
                                if(alP.X + this.Control.AutoList.Width > screen.WorkingArea.Width){
                                    alP.X -= alP.X + this.Control.AutoList.Width - screen.WorkingArea.Width;
                                }
                                this.Control.AutoList.Location = alP;
                                //Control.Controls[0].Focus();
                                this.Control.Focus();
                            }
                        }
                    } catch{}
                }
                if(this.Control.InfoTipStartPos != null){
                    try{
                        if(this.Control.InfoTipVisible){
                            Point itP = this.GetTextPointPixelPos(this.Control.InfoTipStartPos);
                            if(itP == new Point(-1, -1)){
                                this.Control.InfoTip.Visible = false;
                            } else{
                                itP.Y += this.Control.View.RowHeight + 2;
                                itP.X += -20;
                                itP = this.Control.PointToScreen(itP);
                                Screen screen = Screen.FromPoint(new Point(this.Control.Right, itP.Y));
                                if(itP.Y + this.Control.InfoTip.Height > screen.WorkingArea.Height){
                                    itP.Y -= this.Control.View.RowHeight + 2 + this.Control.InfoTip.Height;
                                }
                                if(itP.X + this.Control.InfoTip.Width > screen.WorkingArea.Width){
                                    itP.X -= itP.X + this.Control.InfoTip.Width - screen.WorkingArea.Width;
                                }
                                this.Control.InfoTip.Location = itP;
                                this.Control.InfoTip.Visible = true;
                                Debug.WriteLine("Infotip Made Visible");
                            }
                        } else{
                            this.Control.InfoTip.Visible = false;
                            Debug.WriteLine("Infotip Made Invisible");
                        }
                    } catch{}
                }
                for(int i = 0; i < this.Control.View.VisibleRowCount; i++){
                    if(j >= 0 && j < this.Control.Document.VisibleRows.Count){
                        Row r = this.Control.Document.VisibleRows[j];
                        if(this.RenderCaretRowOnly){
                            if(r == this.Control.Caret.CurrentRow){
                                this.RenderRow(this.Control.Document.IndexOf(r), i);
                            }
                            //Control.Caret.CurrentRow.expansion_EndSpan.StartRow.Index
                            if(this.Control.Caret.CurrentRow.expansion_EndSpan != null
                               && this.Control.Caret.CurrentRow.expansion_EndSpan.StartRow != null
                               && this.Control.Caret.CurrentRow.expansion_EndSpan.StartRow == r){
                                this.RenderRow(this.Control.Document.IndexOf(r), i);
                            }
                        } else{
                            this.RenderRow(this.Control.Document.IndexOf(r), i);
                        }
                    } else{
                        if(this.RenderCaretRowOnly){} else{
                            this.RenderRow(this.Control.Document.Count, i);
                        }
                    }
                    j++;
                }
            } catch{}
        }
        private void RenderRow(int RowIndex, int RowPos)
        {
            if(RowIndex >= 0 && RowIndex < this.Control.Document.Count){
                //do keyword parse before we render the line...
                if(this.Control.Document[RowIndex].RowState == RowState.SpanParsed){
                    this.Control.Document.Parser.ParseRow(RowIndex, true);
                    this.Control.Document[RowIndex].RowState = RowState.AllParsed;
                }
            }
            try{
                GDISurface bbuff = this.GFX.BackBuffer;
                bool found = false;
                GDIBrush bg = this.GFX.BackgroundBrush;
                try{
                    if(RowIndex < this.Control.Document.Count && RowIndex >= 0){
                        Row r = this.Control.Document[RowIndex];
                        if(this.SpanFound && RowIndex >= this.FirstSpanRow && RowIndex <= this.LastSpanRow
                           && this.Control._SyntaxBox.ScopeBackColor != Color.Transparent){
                            bg = new GDIBrush(this.Control._SyntaxBox.ScopeBackColor);
                            found = true;
                        } else if(r.BackColor != Color.Transparent){
                            bg = new GDIBrush(r.BackColor);
                            found = true;
                        } else{
                            if(r.endSpan != null){
                                Span tmp = r.expansion_EndSpan;
                                while(tmp != null){
                                    if(tmp.spanDefinition.Transparent == false){
                                        bg = new GDIBrush(tmp.spanDefinition.BackColor);
                                        found = true;
                                        break;
                                    }
                                    tmp = tmp.Parent;
                                }
                                if(!found){
                                    tmp = r.endSpan;
                                    while(tmp != null){
                                        if(tmp.spanDefinition.Transparent == false){
                                            bg = new GDIBrush(tmp.spanDefinition.BackColor);
                                            found = true;
                                            break;
                                        }
                                        tmp = tmp.Parent;
                                    }
                                }
                                if(!found){
                                    tmp = r.expansion_EndSpan;
                                    while(tmp != null){
                                        if(tmp.spanDefinition.Transparent == false){
                                            bg = new GDIBrush(tmp.spanDefinition.BackColor);
                                            found = true;
                                            break;
                                        }
                                        tmp = tmp.Parent;
                                    }
                                }
                            }
                        }
                    }
                } catch{}
                if(RowIndex == this.Control.Caret.Position.Y && this.Control.HighLightActiveLine){
                    bbuff.Clear(this.GFX.HighLightLineBrush);
                } else if(RowIndex >= 0 && RowIndex < this.Control.Document.Count){
                    if(this.Control.Document[RowIndex].IsCollapsed){
                        if(this.Control.Document[RowIndex].Expansion_EndRow.Index == this.Control.Caret.Position.Y
                           && this.Control.HighLightActiveLine){
                            bbuff.Clear(this.GFX.HighLightLineBrush);
                        } else{
                            bbuff.Clear(bg);
                        }
                    } else{
                        bbuff.Clear(bg);
                    }
                } else{
                    bbuff.Clear(bg);
                }
                //only render normal text if any part of the row is visible
                if(RowIndex <= this.Control.Selection.LogicalBounds.FirstRow
                   || RowIndex >= this.Control.Selection.LogicalBounds.LastRow){
                    this.RenderText(RowIndex);
                }
                //only render selection text if the line is selected
                if(this.Control.Selection.IsValid){
                    if(RowIndex >= this.Control.Selection.LogicalBounds.FirstRow
                       && RowIndex <= this.Control.Selection.LogicalBounds.LastRow){
                        if(this.Control.ContainsFocus){
                            this.GFX.SelectionBuffer.Clear(this.Control.SelectionBackColor);
                        } else{
                            this.GFX.SelectionBuffer.Clear(this.Control.InactiveSelectionBackColor);
                        }
                        this.RenderSelectedText(RowIndex);
                    }
                }
                if(this.Control.ContainsFocus || this.Control.View.Action == EditAction.DragText){
                    this.RenderCaret(RowIndex, RowPos * this.Control.View.RowHeight + this.yOffset);
                }
                this.RenderSelection(RowIndex);
                this.RenderMargin(RowIndex);
                if(this.Control.Document.Folding){
                    this.RenderExpansion(RowIndex);
                }
                var e = new RowPaintEventArgs();
                var rec = new Rectangle(0, 0, this.Control.Width, this.Control.View.RowHeight);
                e.Graphics = Graphics.FromHdc(bbuff.hDC);
                e.Bounds = rec;
                e.Row = null;
                if(RowIndex >= 0 && RowIndex < this.Control.Document.Count){
                    e.Row = this.Control.Document[RowIndex];
                }
                this.Control._SyntaxBox.OnRenderRow(e);
                bbuff.Flush();
                bbuff.RenderToControl(0, RowPos * this.Control.View.RowHeight + this.yOffset);
                //GFX.SelectionBuffer.RenderToControl (0,RowPos*Control.View.RowHeight+this.yOffset);
                if(found){
                    bg.Dispose();
                }
            } catch{}
        }
        private void SetFont(bool bold, bool italic, bool underline, GDISurface surface)
        {
            if(bold && italic && underline){
                surface.Font = this.GFX.FontBoldItalicUnderline;
            } else if(bold && italic){
                surface.Font = this.GFX.FontBoldItalic;
            } else if(bold && underline){
                surface.Font = this.GFX.FontBoldUnderline;
            } else if(bold){
                surface.Font = this.GFX.FontBold;
            } else if(italic && underline){
                surface.Font = this.GFX.FontItalicUnderline;
            } else if(!italic && underline){
                surface.Font = this.GFX.FontUnderline;
            } else if(italic){
                surface.Font = this.GFX.FontItalic;
            } else if(true){
                surface.Font = this.GFX.FontNormal;
            }
        }
        private void SetStringFont(bool bold, bool italic, bool underline)
        {
            this.SetFont(bold, italic, underline, this.GFX.StringBuffer);
        }
        private void RenderCollapsedSelectedText(int RowIndex, int xPos)
        {
            GDISurface bbuff = this.GFX.SelectionBuffer;
            bbuff.Font = this.GFX.FontBold;
            bbuff.FontTransparent = true;
            bbuff.TextForeColor = this.Control.ContainsFocus
                                          ? this.Control.SelectionForeColor
                                          : this.Control.InactiveSelectionForeColor;
            //bbuff.TextForeColor =Color.DarkBlue;
            Row r = this.Control.Document[RowIndex];
            string str = r.CollapsedText;
            xPos++;
            int taborig = -this.Control.View.FirstVisibleColumn * this.Control.View.CharWidth
                          + this.Control.View.TextMargin;
            this.GFX.StringBuffer.Font = this.GFX.FontBold;
            int wdh = this.GFX.StringBuffer.DrawTabbedString(str, xPos + 1, 0, taborig, this.Control.PixelTabSize).Width;
            if(this.Control.ContainsFocus){
                bbuff.FillRect(this.Control.SelectionForeColor, xPos + 0, 0, wdh + 2, this.Control.View.RowHeight);
                bbuff.FillRect(this.Control.SelectionBackColor, xPos + 1, 1, wdh, this.Control.View.RowHeight - 2);
            } else{
                bbuff.FillRect(this.Control.InactiveSelectionForeColor, xPos + 0, 0, wdh + 2,
                               this.Control.View.RowHeight);
                bbuff.FillRect(this.Control.InactiveSelectionBackColor, xPos + 1, 1, wdh,
                               this.Control.View.RowHeight - 2);
            }
            wdh = bbuff.DrawTabbedString(str, xPos + 1, 0, taborig, this.Control.PixelTabSize).Width;
            //this can crash if document not fully parsed , on error resume next
            try{
                if(r.expansion_StartSpan.EndRow != null){
                    if(r.expansion_StartSpan.EndRow.RowState == RowState.SpanParsed){
                        this.Control.Document.Parser.ParseRow(r.expansion_StartSpan.EndRow.Index, true);
                    }
                    Word last = r.expansion_StartSpan.EndWord;
                    xPos += this.Control.View.FirstVisibleColumn * this.Control.View.CharWidth;
                    r.expansion_StartSpan.EndRow.Expansion_PixelStart = xPos + wdh - this.Control.View.TextMargin + 2;
                    r.Expansion_PixelEnd = xPos - 1;
                    this.RenderSelectedText(this.Control.Document.IndexOf(r.expansion_StartSpan.EndRow),
                                            r.expansion_StartSpan.EndRow.Expansion_PixelStart, last);
                }
            } catch{}
        }
        private void RenderCollapsedText(int RowIndex, int xPos)
        {
            GDISurface bbuff = this.GFX.BackBuffer;
            bbuff.Font = this.GFX.FontBold;
            bbuff.FontTransparent = true;
            bbuff.TextForeColor = this.Control.OutlineColor;
            Row r = this.Control.Document[RowIndex];
            string str = r.CollapsedText;
            xPos++;
            int taborig = -this.Control.View.FirstVisibleColumn * this.Control.View.CharWidth
                          + this.Control.View.TextMargin;
            this.GFX.StringBuffer.Font = this.GFX.FontBold;
            int wdh = this.GFX.StringBuffer.DrawTabbedString(str, xPos + 1, 0, taborig, this.Control.PixelTabSize).Width;
            bbuff.FillRect(this.GFX.OutlineBrush, xPos + 0, 0, wdh + 2, this.Control.View.RowHeight);
            bbuff.FillRect(this.GFX.BackgroundBrush, xPos + 1, 1, wdh, this.Control.View.RowHeight - 2);
            wdh = bbuff.DrawTabbedString(str, xPos + 1, 0, taborig, this.Control.PixelTabSize).Width;
            //this can crash if document not fully parsed , on error resume next
            try{
                if(r.expansion_StartSpan.EndRow != null){
                    if(r.expansion_StartSpan.EndRow.RowState == RowState.SpanParsed){
                        this.Control.Document.Parser.ParseRow(r.expansion_StartSpan.EndRow.Index, true);
                    }
                    Word last = r.expansion_StartSpan.EndWord;
                    xPos += this.Control.View.FirstVisibleColumn * this.Control.View.CharWidth;
                    r.expansion_StartSpan.EndRow.Expansion_PixelStart = xPos + wdh - this.Control.View.TextMargin + 2;
                    r.Expansion_PixelEnd = xPos - 1;
                    this.RenderText(this.Control.Document.IndexOf(r.expansion_StartSpan.EndRow),
                                    r.expansion_StartSpan.EndRow.Expansion_PixelStart, last);
                }
            } catch{}
        }
        private void RenderText(int RowIndex)
        {
            this.RenderText(RowIndex, 0, null);
        }
        private void RenderText(int RowIndex, int XOffset, Word StartWord)
        {
            GDISurface bbuff = this.GFX.BackBuffer;
            bbuff.Font = this.GFX.FontNormal;
            bbuff.FontTransparent = true;
            bool DrawBreakpoint = false;
            if(RowIndex <= this.Control.Document.Count - 1){
                bbuff.TextForeColor = Color.Black;
                Row xtr = this.Control.Document[RowIndex];
                //if (xtr.startSpan != null)
                //	bbuff.DrawTabbedString (xtr.startSpan.GetHashCode ().ToString (System.Globalization.CultureInfo.InvariantCulture),100,0,0,0);
                //bbuff.TextForeColor = Color.Black;
                //bbuff.DrawTabbedString (xtr.Text,(int)(Control.View.TextMargin -Control.View.ClientAreaStart),1,-Control.View.FirstVisibleColumn*Control.View.CharWidth+Control.View.TextMargin,Control.PixelTabSize);					
                int xpos = this.Control.View.TextMargin - this.Control.View.ClientAreaStart + XOffset;
                int wdh = 0;
                int taborig = -this.Control.View.FirstVisibleColumn * this.Control.View.CharWidth
                              + this.Control.View.TextMargin;
                bool ws = this.Control.ShowWhitespace;
                bool StartDraw = false;
                if(StartWord == null){
                    StartDraw = true;
                }
                xtr.Expansion_StartChar = 0;
                xtr.Expansion_EndChar = 0;
                bool HasExpansion = false;
                foreach(Word w in xtr.FormattedWords){
                    if(StartDraw){
                        if(w.Span == xtr.expansion_StartSpan && xtr.expansion_StartSpan != null){
                            if(xtr.expansion_StartSpan.Expanded == false){
                                this.RenderCollapsedText(RowIndex, xpos);
                                HasExpansion = true;
                                break;
                            }
                        }
                        if((w.Type == WordType.Space || w.Type == WordType.Tab) && !DrawBreakpoint
                           && this.Control.ShowTabGuides){
                            int xtab = xpos
                                       - (this.Control.View.TextMargin - this.Control.View.ClientAreaStart + XOffset);
                            if((xtab / (double)this.Control.PixelTabSize) == (xtab / this.Control.PixelTabSize)){
                                bbuff.FillRect(this.Control.TabGuideColor, xpos, 0, 1, this.Control.View.RowHeight);
                            }
                        }
                        if(w.Type == WordType.Word || ws == false){
                            if(w.Style != null){
                                this.SetFont(w.Style.Bold, w.Style.Italic, w.Style.Underline, bbuff);
                                bbuff.TextBackColor = w.Style.BackColor;
                                bbuff.TextForeColor = w.Style.ForeColor;
                                bbuff.FontTransparent = w.Style.Transparent;
                            } else{
                                bbuff.Font = this.GFX.FontNormal;
                                bbuff.TextForeColor = Color.Black;
                                bbuff.FontTransparent = true;
                            }
                            if(w.Type == WordType.Word){
                                DrawBreakpoint = true;
                            }
                            if(xtr.Breakpoint && DrawBreakpoint){
                                bbuff.TextForeColor = this.Control.BreakPointForeColor;
                                bbuff.TextBackColor = this.Control.BreakPointBackColor;
                                bbuff.FontTransparent = false;
                            }
                            if(this.Control.BracketMatching && (w == this.BracketEnd || w == this.BracketStart)){
                                bbuff.TextForeColor = this.Control.BracketForeColor;
                                if(this.Control.BracketBackColor != Color.Transparent){
                                    bbuff.TextBackColor = this.Control.BracketBackColor;
                                    bbuff.FontTransparent = false;
                                }
                                wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig, this.Control.PixelTabSize).Width;
                                if(this.Control.BracketBorderColor != Color.Transparent){
                                    bbuff.DrawRect(this.Control.BracketBorderColor, xpos - 1, 0, wdh,
                                                   this.Control.View.RowHeight - 1);
                                }
                            } else{
                                wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig, this.Control.PixelTabSize).Width;
                            }
                            //render errors
                            if(w.HasError){
                                //bbuff.FillRect (Color.Red,xpos,Control.View.RowHeight-2,wdh,2);
                                int ey = this.Control.View.RowHeight - 1;
                                Color c = w.ErrorColor;
                                for(int x = 0; x < wdh + 3; x += 4){
                                    bbuff.DrawLine(c, new Point(xpos + x, ey), new Point(xpos + x + 2, ey - 2));
                                    bbuff.DrawLine(c, new Point(xpos + x + 2, ey - 2), new Point(xpos + x + 4, ey));
                                }
                            }
                        } else if(w.Type == WordType.Space){
                            bbuff.Font = this.GFX.FontNormal;
                            bbuff.TextForeColor = this.Control.WhitespaceColor;
                            bbuff.FontTransparent = true;
                            if(xtr.Breakpoint && DrawBreakpoint){
                                bbuff.TextForeColor = this.Control.BreakPointForeColor;
                                bbuff.TextBackColor = this.Control.BreakPointBackColor;
                                bbuff.FontTransparent = false;
                            }
                            bbuff.DrawTabbedString("·", xpos, 0, taborig, this.Control.PixelTabSize);
                            wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig, this.Control.PixelTabSize).Width;
                        } else if(w.Type == WordType.Tab){
                            bbuff.Font = this.GFX.FontNormal;
                            bbuff.TextForeColor = this.Control.WhitespaceColor;
                            bbuff.FontTransparent = true;
                            if(xtr.Breakpoint && DrawBreakpoint){
                                bbuff.TextForeColor = this.Control.BreakPointForeColor;
                                bbuff.TextBackColor = this.Control.BreakPointBackColor;
                                bbuff.FontTransparent = false;
                            }
                            bbuff.DrawTabbedString("»", xpos, 0, taborig, this.Control.PixelTabSize);
                            wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig, this.Control.PixelTabSize).Width;
                        }
                        if(w.Pattern != null){
                            if(w.Pattern.IsSeparator){
                                bbuff.FillRect(this.Control.SeparatorColor, this.Control.View.TextMargin - 4,
                                               this.Control.View.RowHeight - 1, this.Control.View.ClientAreaWidth, 1);
                            }
                        }
                        xpos += wdh;
                    }
                    if(!StartDraw){
                        xtr.Expansion_StartChar += w.Text.Length;
                    }
                    if(w == StartWord){
                        StartDraw = true;
                    }
                    xtr.Expansion_EndChar += w.Text.Length;
                }
                if(xtr.IsCollapsed){} else if(xtr.endSpan != null && xtr.endSpan.spanDefinition != null
                                              && xtr.endSpan.spanDefinition.Style != null){
                    bbuff.FillRect(xtr.endSpan.spanDefinition.Style.BackColor, xpos, 0, this.Control.Width - xpos,
                                   this.Control.View.RowHeight);
                }
                if(this.Control._SyntaxBox.ShowEOLMarker && !HasExpansion){
                    bbuff.Font = this.GFX.FontNormal;
                    bbuff.TextForeColor = this.Control._SyntaxBox.EOLMarkerColor;
                    bbuff.FontTransparent = true;
                    bbuff.DrawTabbedString("¶", xpos, 0, taborig, this.Control.PixelTabSize);
                }
            }
        }
        private void RenderSelectedText(int RowIndex)
        {
            this.RenderSelectedText(RowIndex, 0, null);
        }
        private void RenderSelectedText(int RowIndex, int XOffset, Word StartWord)
        {
            GDISurface bbuff = this.GFX.SelectionBuffer;
            bbuff.Font = this.GFX.FontNormal;
            bbuff.FontTransparent = true;
            if(RowIndex <= this.Control.Document.Count - 1){
                bbuff.TextForeColor = this.Control.ContainsFocus
                                              ? this.Control.SelectionForeColor
                                              : this.Control.InactiveSelectionForeColor;
                Row xtr = this.Control.Document[RowIndex];
                //if (xtr.startSpan != null)
                //	bbuff.DrawTabbedString (xtr.startSpan.GetHashCode ().ToString (System.Globalization.CultureInfo.InvariantCulture),100,0,0,0);
                //bbuff.TextForeColor = Color.Black;
                //bbuff.DrawTabbedString (xtr.Text,(int)(Control.View.TextMargin -Control.View.ClientAreaStart),1,-Control.View.FirstVisibleColumn*Control.View.CharWidth+Control.View.TextMargin,Control.PixelTabSize);					
                int xpos = this.Control.View.TextMargin - this.Control.View.ClientAreaStart + XOffset;
                int wdh = 0;
                int taborig = -this.Control.View.FirstVisibleColumn * this.Control.View.CharWidth
                              + this.Control.View.TextMargin;
                bool ws = this.Control.ShowWhitespace;
                bool StartDraw = false;
                if(StartWord == null){
                    StartDraw = true;
                }
                xtr.Expansion_StartChar = 0;
                xtr.Expansion_EndChar = 0;
                bool HasExpansion = false;
                foreach(Word w in xtr.FormattedWords){
                    if(StartDraw){
                        if(w.Span == xtr.expansion_StartSpan && xtr.expansion_StartSpan != null){
                            if(xtr.expansion_StartSpan.Expanded == false){
                                this.RenderCollapsedSelectedText(RowIndex, xpos);
                                HasExpansion = true;
                                break;
                            }
                        }
                        if(w.Type == WordType.Word || ws == false){
                            if(w.Style != null){
                                this.SetFont(w.Style.Bold, w.Style.Italic, w.Style.Underline, bbuff);
                            } else{
                                bbuff.Font = this.GFX.FontNormal;
                            }
                            wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig, this.Control.PixelTabSize).Width;
                            //render errors
                            if(w.HasError){
                                //bbuff.FillRect (Color.Red,xpos,Control.View.RowHeight-2,wdh,2);
                                int ey = this.Control.View.RowHeight - 1;
                                Color c = w.ErrorColor;
                                for(int x = 0; x < wdh + 3; x += 4){
                                    bbuff.DrawLine(c, new Point(xpos + x, ey), new Point(xpos + x + 2, ey - 2));
                                    bbuff.DrawLine(c, new Point(xpos + x + 2, ey - 2), new Point(xpos + x + 4, ey));
                                }
                            }
                        } else if(w.Type == WordType.Space){
                            bbuff.Font = this.GFX.FontNormal;
                            bbuff.DrawTabbedString("·", xpos, 0, taborig, this.Control.PixelTabSize);
                            wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig, this.Control.PixelTabSize).Width;
                        } else if(w.Type == WordType.Tab){
                            bbuff.Font = this.GFX.FontNormal;
                            bbuff.DrawTabbedString("»", xpos, 0, taborig, this.Control.PixelTabSize);
                            wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig, this.Control.PixelTabSize).Width;
                        }
                        if(w.Pattern != null){
                            if(w.Pattern.IsSeparator){
                                bbuff.FillRect(this.Control.SeparatorColor, this.Control.View.TextMargin - 4,
                                               this.Control.View.RowHeight - 1, this.Control.View.ClientAreaWidth, 1);
                            }
                        }
                        xpos += wdh;
                    }
                    if(!StartDraw){
                        xtr.Expansion_StartChar += w.Text.Length;
                    }
                    if(w == StartWord){
                        StartDraw = true;
                    }
                    xtr.Expansion_EndChar += w.Text.Length;
                }
                if(xtr.IsCollapsed){} else if(xtr.endSpan != null && xtr.endSpan.spanDefinition != null
                                              && xtr.endSpan.spanDefinition.Style != null){
                    this.GFX.BackBuffer.FillRect(xtr.endSpan.spanDefinition.Style.BackColor, xpos, 0,
                                                 this.Control.Width - xpos, this.Control.View.RowHeight);
                }
                if(this.Control._SyntaxBox.ShowEOLMarker && !HasExpansion){
                    bbuff.Font = this.GFX.FontNormal;
                    bbuff.TextForeColor = this.Control.SelectionForeColor;
                    bbuff.FontTransparent = true;
                    bbuff.DrawTabbedString("¶", xpos, 0, taborig, this.Control.PixelTabSize);
                }
            }
        }
        private void RenderCaret(int RowIndex, int ypos)
        {
            int StartRow = -1;
            int cr = this.Control.Caret.Position.Y;
            if(cr >= 0 && cr <= this.Control.Document.Count - 1){
                Row r = this.Control.Document[cr];
                if(r.expansion_EndSpan != null){
                    if(r.expansion_EndSpan.Expanded == false){
                        r = r.expansion_EndSpan.StartRow;
                        StartRow = r.Index;
                    }
                }
            }
            bool Collapsed = (RowIndex == StartRow);
            if(RowIndex != cr && RowIndex != StartRow){
                return;
            }
            if(this.Control.View.Action == EditAction.DragText){
                //drop Control.Caret
                Row xtr = this.Control.Document[cr];
                int pos = this.MeasureRow(xtr, this.Control.Caret.Position.X).Width + 1;
                if(Collapsed){
                    pos += xtr.Expansion_PixelStart;
                    pos -= this.MeasureRow(xtr, xtr.Expansion_StartChar).Width;
                }
                this.GFX.BackBuffer.InvertRect(
                        pos + this.Control.View.TextMargin - this.Control.View.ClientAreaStart - 1, 0, 3,
                        this.Control.View.RowHeight);
                this.GFX.BackBuffer.InvertRect(pos + this.Control.View.TextMargin - this.Control.View.ClientAreaStart, 1,
                                               1, this.Control.View.RowHeight - 2);
            } else{
                //normal Control.Caret
                Row xtr = this.Control.Document[cr];
                if(!this.Control.OverWrite){
                    int pos = this.Control.View.TextMargin - this.Control.View.ClientAreaStart;
                    pos += this.MeasureRow(xtr, this.Control.Caret.Position.X).Width + 1;
                    if(Collapsed){
                        pos += xtr.Expansion_PixelStart;
                        pos -= this.MeasureRow(xtr, xtr.Expansion_StartChar).Width;
                    }
                    int wdh = this.Control.View.CharWidth / 12 + 1;
                    if(wdh < 2){
                        wdh = 2;
                    }
                    if(this.Control.Caret.Blink){
                        this.GFX.BackBuffer.InvertRect(pos, 0, wdh, this.Control.View.RowHeight);
                    }
                    if(this.Control.IMEWindow == null){
                        this.Control.IMEWindow = new IMEWindow(this.Control.Handle, this.Control.FontName,
                                                               this.Control.FontSize);
                        this.InitIMEWindow();
                    }
                    this.Control.IMEWindow.Loation = new Point(pos, ypos);
                } else{
                    int pos1 = this.MeasureRow(xtr, this.Control.Caret.Position.X).Width;
                    int pos2 = this.MeasureRow(xtr, this.Control.Caret.Position.X + 1).Width;
                    int wdh = pos2 - pos1;
                    if(Collapsed){
                        pos1 += xtr.Expansion_PixelStart;
                        pos1 -= this.MeasureRow(xtr, xtr.Expansion_StartChar).Width;
                    }
                    int pos = pos1 + this.Control.View.TextMargin - this.Control.View.ClientAreaStart;
                    if(this.Control.Caret.Blink){
                        this.GFX.BackBuffer.InvertRect(pos, 0, wdh, this.Control.View.RowHeight);
                    }
                    this.Control.IMEWindow.Loation = new Point(pos, ypos);
                }
            }
        }
        private void RenderMargin(int RowIndex)
        {
            GDISurface bbuff = this.GFX.BackBuffer;
            if(this.Control.ShowGutterMargin){
                bbuff.FillRect(this.GFX.GutterMarginBrush, 0, 0, this.Control.View.GutterMarginWidth,
                               this.Control.View.RowHeight);
                bbuff.FillRect(this.GFX.GutterMarginBorderBrush, this.Control.View.GutterMarginWidth - 1, 0, 1,
                               this.Control.View.RowHeight);
                if(RowIndex <= this.Control.Document.Count - 1){
                    Row r = this.Control.Document[RowIndex];
                    if(this.Control.View.RowHeight >= this.Control._SyntaxBox.GutterIcons.ImageSize.Height){
                        if(r.Bookmarked){
                            this.Control._SyntaxBox.GutterIcons.Draw(Graphics.FromHdc(bbuff.hDC), 0, 0, 1);
                        }
                        if(r.Breakpoint){
                            this.Control._SyntaxBox.GutterIcons.Draw(Graphics.FromHdc(bbuff.hDC), 0, 0, 0);
                        }
                    } else{
                        int w = this.Control.View.RowHeight;
                        if(r.Bookmarked){
                            this.Control._SyntaxBox.GutterIcons.Draw(Graphics.FromHdc(bbuff.hDC), 0, 0, w, w, 1);
                        }
                        if(r.Breakpoint){
                            this.Control._SyntaxBox.GutterIcons.Draw(Graphics.FromHdc(bbuff.hDC), 0, 0, w, w, 0);
                        }
                    }
                    if(r.Images != null){
                        foreach(int i in r.Images){
                            if(this.Control.View.RowHeight >= this.Control._SyntaxBox.GutterIcons.ImageSize.Height){
                                this.Control._SyntaxBox.GutterIcons.Draw(Graphics.FromHdc(bbuff.hDC), 0, 0, i);
                            } else{
                                int w = this.Control.View.RowHeight;
                                this.Control._SyntaxBox.GutterIcons.Draw(Graphics.FromHdc(bbuff.hDC), 0, 0, w, w, i);
                            }
                        }
                    }
                }
            }
            if(this.Control.ShowLineNumbers){
                bbuff.FillRect(this.GFX.LineNumberMarginBrush, this.Control.View.GutterMarginWidth, 0,
                               this.Control.View.LineNumberMarginWidth + 1, this.Control.View.RowHeight);
                //bbuff.FillRect (GFX.LineNumberMarginBrush  ,Control.View.GutterMarginWidth+Control.View.LineNumberMarginWidth,0,1,Control.View.RowHeight);
                for(int j = 0; j < this.Control.View.RowHeight; j += 2){
                    bbuff.FillRect(this.GFX.LineNumberMarginBorderBrush,
                                   this.Control.View.GutterMarginWidth + this.Control.View.LineNumberMarginWidth, j, 1,
                                   1);
                }
            }
            if(!this.Control.ShowLineNumbers || !this.Control.ShowGutterMargin){
                bbuff.FillRect(this.GFX.BackgroundBrush, this.Control.View.TotalMarginWidth, 0,
                               this.Control.View.TextMargin - this.Control.View.TotalMarginWidth - 3,
                               this.Control.View.RowHeight);
            } else{
                bbuff.FillRect(this.GFX.BackgroundBrush, this.Control.View.TotalMarginWidth + 1, 0,
                               this.Control.View.TextMargin - this.Control.View.TotalMarginWidth - 4,
                               this.Control.View.RowHeight);
            }
            if(this.Control.ShowLineNumbers){
                bbuff.Font = this.GFX.FontNormal;
                bbuff.FontTransparent = true;
                bbuff.TextForeColor = this.Control.LineNumberForeColor;
                if(RowIndex <= this.Control.Document.Count - 1){
                    int nw = this.MeasureString((RowIndex + 1).ToString(CultureInfo.InvariantCulture)).Width;
                    bbuff.DrawTabbedString((RowIndex + 1).ToString(CultureInfo.InvariantCulture),
                                           this.Control.View.GutterMarginWidth + this.Control.View.LineNumberMarginWidth
                                           - nw - 1, 1, 0, this.Control.PixelTabSize);
                }
            }
        }
        private void RenderExpansion(int RowIndex)
        {
            if(this.Control == null){
                throw new NullReferenceException("Control may not be null");
            }
            const int expansionOffset = 10;
            if(RowIndex <= this.Control.Document.Count - 1){
                const int yo = 4;
                Row xtr = this.Control.Document[RowIndex];
                GDISurface bbuff = this.GFX.BackBuffer;
                if(xtr.endSpan != null){
                    if(xtr.expansion_StartSpan != null && xtr.startSpan.Parent == null){
                        if(!xtr.IsCollapsed){
                            bbuff.FillRect(this.GFX.OutlineBrush, this.Control.View.TotalMarginWidth + expansionOffset,
                                           yo, 1, this.Control.View.RowHeight - yo);
                        }
                    } else if((xtr.endSpan.Parent != null || xtr.expansion_EndSpan != null)){
                        bbuff.FillRect(this.GFX.OutlineBrush, this.Control.View.TotalMarginWidth + expansionOffset, 0, 1,
                                       this.Control.View.RowHeight);
                    }
                    if(xtr.expansion_StartSpan != null){
                        bbuff.FillRect(this.GFX.OutlineBrush, this.Control.View.TotalMarginWidth + expansionOffset - 4,
                                       yo, 9, 9);
                        bbuff.FillRect(this.GFX.BackgroundBrush,
                                       this.Control.View.TotalMarginWidth + expansionOffset - 3, yo + 1, 7, 7);
                        //render plus / minus
                        bbuff.FillRect(this.GFX.OutlineBrush, this.Control.View.TotalMarginWidth + expansionOffset - 2,
                                       yo + 4, 5, 1);
                        if(!xtr.expansion_StartSpan.Expanded){
                            bbuff.FillRect(this.GFX.OutlineBrush, this.Control.View.TotalMarginWidth + expansionOffset,
                                           yo + 2, 1, 5);
                        }
                    }
                    if(xtr.expansion_EndSpan != null){
                        bbuff.FillRect(this.GFX.OutlineBrush, this.Control.View.TotalMarginWidth + expansionOffset + 1,
                                       this.Control.View.RowHeight - 1, 5, 1);
                    }
                }
                //				//RENDER SPAN LINES
                //				if (SpanFound)
                //				{
                //					if (RowIndex==FirstSpanRow)
                //						bbuff.FillRect (GFX.OutlineBrush,this.Control.View.TotalMarginWidth +14,0,Control.ClientWidth ,1);
                //
                //					if (RowIndex==LastSpanRow)
                //						bbuff.FillRect (GFX.OutlineBrush,this.Control.View.TotalMarginWidth +14,Control.View.RowHeight-1,Control.ClientWidth,1);				
                //				}
                //RENDER SPAN MARGIN
                if(this.SpanFound && this.Control._SyntaxBox.ScopeIndicatorColor != Color.Transparent
                   && this.Control._SyntaxBox.ShowScopeIndicator){
                    if(RowIndex >= this.FirstSpanRow && RowIndex <= this.LastSpanRow){
                        bbuff.FillRect(this.Control._SyntaxBox.ScopeIndicatorColor,
                                       this.Control.View.TotalMarginWidth + 14, 0, 2, this.Control.View.RowHeight);
                    }
                    if(RowIndex == this.FirstSpanRow){
                        bbuff.FillRect(this.Control._SyntaxBox.ScopeIndicatorColor,
                                       this.Control.View.TotalMarginWidth + 14, 0, 4, 2);
                    }
                    if(RowIndex == this.LastSpanRow){
                        bbuff.FillRect(this.Control._SyntaxBox.ScopeIndicatorColor,
                                       this.Control.View.TotalMarginWidth + 14, this.Control.View.RowHeight - 2, 4, 2);
                    }
                }
                if(this.Control._SyntaxBox.ShowRevisionMarks){
                    if(xtr.RevisionMark == RowRevisionMark.BeforeSave){
                        bbuff.FillRect(this.Control._SyntaxBox.RevisionMarkBeforeSave,
                                       this.Control.View.TotalMarginWidth + 1, 0, 3, this.Control.View.RowHeight);
                    } else if(xtr.RevisionMark == RowRevisionMark.AfterSave){
                        bbuff.FillRect(this.Control._SyntaxBox.RevisionMarkAfterSave,
                                       this.Control.View.TotalMarginWidth + 1, 0, 3, this.Control.View.RowHeight);
                    }
                }
            }
        }
        //draws aControl.Selection.LogicalBounds row in the backbuffer
        private void RenderSelection(int RowIndex)
        {
            if(RowIndex <= this.Control.Document.Count - 1 && this.Control.Selection.IsValid){
                Row xtr = this.Control.Document[RowIndex];
                if(!xtr.IsCollapsed){
                    if((RowIndex > this.Control.Selection.LogicalBounds.FirstRow)
                       && (RowIndex < this.Control.Selection.LogicalBounds.LastRow)){
                        int width = this.MeasureRow(xtr, xtr.Text.Length).Width + this.MeasureString("¶").Width + 3;
                        this.RenderBox(this.Control.View.TextMargin, 0,
                                       Math.Max(width - this.Control.View.ClientAreaStart, 0),
                                       this.Control.View.RowHeight);
                    } else if((RowIndex == this.Control.Selection.LogicalBounds.FirstRow)
                              && (RowIndex == this.Control.Selection.LogicalBounds.LastRow)){
                        int start =
                                this.MeasureRow(xtr,
                                                Math.Min(xtr.Text.Length,
                                                         this.Control.Selection.LogicalBounds.FirstColumn)).Width;
                        int width =
                                this.MeasureRow(xtr,
                                                Math.Min(xtr.Text.Length,
                                                         this.Control.Selection.LogicalBounds.LastColumn)).Width
                                - start;
                        this.RenderBox(this.Control.View.TextMargin + start - this.Control.View.ClientAreaStart, 0,
                                       width, this.Control.View.RowHeight);
                    } else if(RowIndex == this.Control.Selection.LogicalBounds.LastRow){
                        int width =
                                this.MeasureRow(xtr,
                                                Math.Min(xtr.Text.Length,
                                                         this.Control.Selection.LogicalBounds.LastColumn)).Width;
                        this.RenderBox(this.Control.View.TextMargin, 0,
                                       Math.Max(width - this.Control.View.ClientAreaStart, 0),
                                       this.Control.View.RowHeight);
                    } else if(RowIndex == this.Control.Selection.LogicalBounds.FirstRow){
                        int start =
                                this.MeasureRow(xtr,
                                                Math.Min(xtr.Text.Length,
                                                         this.Control.Selection.LogicalBounds.FirstColumn)).Width;
                        int width = this.MeasureRow(xtr, xtr.Text.Length).Width + this.MeasureString("¶").Width + 3
                                    - start;
                        this.RenderBox(this.Control.View.TextMargin + start - this.Control.View.ClientAreaStart, 0,
                                       width, this.Control.View.RowHeight);
                    }
                } else{
                    this.RenderCollapsedSelection(RowIndex);
                }
            }
        }
        private void RenderCollapsedSelection(int RowIndex)
        {
            Row xtr = this.Control.Document[RowIndex];
            if((RowIndex > this.Control.Selection.LogicalBounds.FirstRow)
               && (RowIndex < this.Control.Selection.LogicalBounds.LastRow)){
                int width = this.MeasureRow(xtr, xtr.Expansion_EndChar).Width;
                this.RenderBox(this.Control.View.TextMargin, 0, Math.Max(width - this.Control.View.ClientAreaStart, 0),
                               this.Control.View.RowHeight);
            } else if((RowIndex == this.Control.Selection.LogicalBounds.FirstRow)
                      && (RowIndex == this.Control.Selection.LogicalBounds.LastRow)){
                int start =
                        this.MeasureRow(xtr,
                                        Math.Min(xtr.Text.Length, this.Control.Selection.LogicalBounds.FirstColumn))
                                .Width;
                int min = Math.Min(xtr.Text.Length, this.Control.Selection.LogicalBounds.LastColumn);
                min = Math.Min(min, xtr.Expansion_EndChar);
                int width = this.MeasureRow(xtr, min).Width - start;
                this.RenderBox(this.Control.View.TextMargin + start - this.Control.View.ClientAreaStart, 0, width,
                               this.Control.View.RowHeight);
            } else if(RowIndex == this.Control.Selection.LogicalBounds.LastRow){
                int width =
                        this.MeasureRow(xtr,
                                        Math.Min(xtr.Text.Length, this.Control.Selection.LogicalBounds.LastColumn)).
                                Width;
                this.RenderBox(this.Control.View.TextMargin, 0,
                               Math.Max(width - this.Control.View.ClientAreaStart, 0), this.Control.View.RowHeight);
            } else if(RowIndex == this.Control.Selection.LogicalBounds.FirstRow){
                int start =
                        this.MeasureRow(xtr,
                                        Math.Min(xtr.Text.Length, this.Control.Selection.LogicalBounds.FirstColumn))
                                .Width;
                int width = this.MeasureRow(xtr, Math.Min(xtr.Text.Length, xtr.Expansion_EndChar)).Width - start;
                this.RenderBox(this.Control.View.TextMargin + start - this.Control.View.ClientAreaStart, 0, width,
                               this.Control.View.RowHeight);
            }
            if(this.Control.Selection.LogicalBounds.LastRow > RowIndex
               && this.Control.Selection.LogicalBounds.FirstRow <= RowIndex){
                int start = xtr.Expansion_PixelEnd;
                int end = xtr.Expansion_EndRow.Expansion_PixelStart - start + this.Control.View.TextMargin;
                //start+=100;
                //end=200;
                this.RenderBox(start - this.Control.View.ClientAreaStart, 0, end, this.Control.View.RowHeight);
            }
            RowIndex = xtr.Expansion_EndRow.Index;
            xtr = xtr.Expansion_EndRow;
            if(this.Control.Selection.LogicalBounds.FirstRow <= RowIndex
               && this.Control.Selection.LogicalBounds.LastRow >= RowIndex){
                int endchar = this.Control.Selection.LogicalBounds.LastRow != RowIndex
                                      ? xtr.Text.Length
                                      : Math.Min(this.Control.Selection.LogicalBounds.LastColumn, xtr.Text.Length);
                int end = this.MeasureRow(xtr, endchar).Width;
                end += xtr.Expansion_PixelStart;
                end -= this.MeasureRow(xtr, xtr.Expansion_StartChar).Width;
                int start;
                if(this.Control.Selection.LogicalBounds.FirstRow == RowIndex){
                    int startchar = Math.Max(this.Control.Selection.LogicalBounds.FirstColumn, xtr.Expansion_StartChar);
                    start = this.MeasureRow(xtr, startchar).Width;
                    start += xtr.Expansion_PixelStart;
                    start -= this.MeasureRow(xtr, xtr.Expansion_StartChar).Width;
                } else{
                    start = this.MeasureRow(xtr, xtr.Expansion_StartChar).Width;
                    start += xtr.Expansion_PixelStart;
                    start -= this.MeasureRow(xtr, xtr.Expansion_StartChar).Width;
                }
                end -= start;
                if(this.Control.Selection.LogicalBounds.LastRow != RowIndex){
                    end += 6;
                }
                this.RenderBox(this.Control.View.TextMargin + start - this.Control.View.ClientAreaStart, 0, end,
                               this.Control.View.RowHeight);
            }
        }
        private void RenderBox(int x, int y, int width, int height)
        {
            this.GFX.SelectionBuffer.RenderTo(this.GFX.BackBuffer, x, y, width, height, x, y);
        }
        private TextPoint ColumnFromPixel(int RowIndex, int X)
        {
            Row xtr = this.Control.Document[RowIndex];
            X -= this.Control.View.TextMargin - 2 - this.Control.View.FirstVisibleColumn * this.Control.View.CharWidth;
            if(xtr.Count == 0){
                if(this.Control.VirtualWhitespace && this.Control.View.CharWidth > 0){
                    return new TextPoint(X / this.Control.View.CharWidth, RowIndex);
                }
                return new TextPoint(0, RowIndex);
            }
            int taborig = -this.Control.View.FirstVisibleColumn * this.Control.View.CharWidth
                          + this.Control.View.TextMargin;
            int xpos = this.Control.View.TextMargin - this.Control.View.ClientAreaStart;
            int CharNo = 0;
            int TotWidth = 0;
            Word Word = null;
            int WordStart = 0;
            foreach(Word w in xtr.FormattedWords){
                Word = w;
                WordStart = TotWidth;
                if(w.Type == WordType.Word && w.Style != null){
                    this.SetStringFont(w.Style.Bold, w.Style.Italic, w.Style.Underline);
                } else{
                    this.SetStringFont(false, false, false);
                }
                int tmpWidth =
                        this.GFX.StringBuffer.DrawTabbedString(w.Text, xpos + TotWidth, 0, taborig,
                                                               this.Control.PixelTabSize).Width;
                if(TotWidth + tmpWidth >= X){
                    break;
                }
                //dont do this for the last word
                if(w != xtr.FormattedWords[xtr.FormattedWords.Count - 1]){
                    TotWidth += tmpWidth;
                    CharNo += w.Text.Length;
                }
            }
            //CharNo is the index in the text where 'word' starts
            //'Word' is the word object that contains the 'X'
            //'WordStart' contains the pixel start position for 'Word'
            if(Word != null){
                if(Word.Type == WordType.Word && Word.Style != null){
                    this.SetStringFont(Word.Style.Bold, Word.Style.Italic, Word.Style.Underline);
                } else{
                    this.SetStringFont(false, false, false);
                }
            }
            //now , lets measure each char and get a correct pos
            bool found = false;
            if(Word != null){
                foreach(char c in Word.Text){
                    int tmpWidth =
                            this.GFX.StringBuffer.DrawTabbedString(c + "", xpos + WordStart, 0, taborig,
                                                                   this.Control.PixelTabSize).Width;
                    if(WordStart + tmpWidth >= X){
                        found = true;
                        break;
                    }
                    CharNo++;
                    WordStart += tmpWidth;
                }
            }
            if(!found && this.Control.View.CharWidth > 0 && this.Control.VirtualWhitespace){
                int xx = X - WordStart;
                int cn = xx / this.Control.View.CharWidth;
                CharNo += cn;
            }
            if(CharNo < 0){
                CharNo = 0;
            }
            return new TextPoint(CharNo, RowIndex);
        }
        private Point GetTextPointPixelPos(TextPoint tp)
        {
            Row xtr = this.Control.Document[tp.Y];
            if(xtr.RowState == RowState.SpanParsed){
                this.Control.Document.Parser.ParseRow(xtr.Index, true);
            }
            Row r = xtr.IsCollapsedEndPart ? xtr.Expansion_StartRow : xtr;
            int index = r.VisibleIndex;
            int yPos = (index - this.Control.View.FirstVisibleRow);
            if(yPos < 0 || yPos > this.Control.View.VisibleRowCount){
                return new Point(-1, -1);
            }
            yPos *= this.Control.View.RowHeight;
            bool Collapsed = (xtr.IsCollapsedEndPart);
            int pos = this.MeasureRow(xtr, tp.X).Width + 1;
            if(Collapsed){
                pos += xtr.Expansion_PixelStart;
                pos -= this.MeasureRow(xtr, xtr.Expansion_StartChar).Width;
            }
            int xPos = pos + this.Control.View.TextMargin - this.Control.View.ClientAreaStart;
            if(xPos < this.Control.View.TextMargin
               || xPos > this.Control.View.ClientAreaWidth + this.Control.View.TextMargin){
                return new Point(-1, -1);
            }
            return new Point(xPos, yPos);
        }
    }
}