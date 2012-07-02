// *
// * Copyright (C) 2008 Roger Alsing : http://www.RogerAlsing.com
// *
// * This library is free software; you can redistribute it and/or modify it
// * under the terms of the GNU Lesser General Public License 2.1 or later, as
// * published by the Free Software Foundation. See the included license.txt
// * or http://www.gnu.org/copyleft/lesser.html for details.
// *
// *

#region using ...
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using Alsing.Globalization;
using Alsing.SourceCode;
using Alsing.SyntaxBox.Properties;
using Alsing.Windows.Forms.CoreLib;
using Alsing.Windows.Forms.SyntaxBox.Painter;
using ScrollEventArgs=Alsing.Windows.Forms.IntelliMouse.ScrollEventArgs;

#endregion

namespace Alsing.Windows.Forms.SyntaxBox
{
    [ToolboxItem(false)]
    public class EditViewControl : SplitViewChildControl
    {
        #region General Declarations
        private readonly Caret _Caret;
        private readonly Selection _Selection;
        private bool _AutoListVisible;
        private bool _InfoTipVisible;
        private double _IntelliScrollPos;
        private bool _KeyDownHandled;
        private bool _OverWrite;
        /// <summary>
        /// The Point in the text where the Autolist was activated.
        /// </summary>
        public TextPoint AutoListStartPos;
        /// <summary>
        /// The Point in the text where the InfoTip was activated.
        /// </summary>		
        public TextPoint InfoTipStartPos;
        private int MouseX;
        private int MouseY;
        public IPainter Painter;
        public ViewPoint View;
        #endregion

        #region Internal controls
        private WeakReference _Control;
        private Timer CaretTimer;
        private IContainer components;
        private PictureBox Filler;
        private IntelliMouseControl IntelliMouse;
        private ToolTip tooltip;

        #region PUBLIC PROPERTY AUTOLIST
        private AutoListForm _AutoList;
        public AutoListForm AutoList
        {
            get
            {
                this.CreateAutoList();
                return this._AutoList;
            }
            set { this._AutoList = value; }
        }
        #endregion

        #region PUBLIC PROPERTY INFOTIP
        private InfoTipForm _InfoTip;
        public InfoTipForm InfoTip
        {
            get
            {
                this.CreateInfoTip();
                return this._InfoTip;
            }
            set { this._InfoTip = value; }
        }
        #endregion

        #region PUBLIC PROPERTY IMEWINDOW
        public IMEWindow IMEWindow { get; set; }
        #endregion

        #region PUBLIC PROPERTY FINDREPLACEDIALOG 
        private FindReplaceForm _FindReplaceDialog;
        public FindReplaceForm FindReplaceDialog
        {
            get
            {
                this.CreateFindForm();
                return this._FindReplaceDialog;
            }
            set { this._FindReplaceDialog = value; }
        }
        #endregion

        public bool HasAutoList
        {
            get { return this._AutoList != null; }
        }
        public bool HasInfoTip
        {
            get { return this._InfoTip != null; }
        }
        public SyntaxBoxControl _SyntaxBox
        {
            get
            {
                try{
                    if(this._Control != null && this._Control.IsAlive){
                        return (SyntaxBoxControl)this._Control.Target;
                    }
                    return null;
                } catch{
                    return null;
                }
            }
            set { this._Control = new WeakReference(value); }
        }
        #endregion

        #region Public events
        /// <summary>
        /// An event that is fired when the caret has moved.
        /// </summary>
        public event EventHandler CaretChange = null;
        /// <summary>
        /// An event that is fired when the selection has changed.
        /// </summary>
        public event EventHandler SelectionChange = null;
        /// <summary>
        /// An event that is fired when mouse down occurs on a row
        /// </summary>
        public event RowMouseHandler RowMouseDown = null;
        /// <summary>
        /// An event that is fired when mouse move occurs on a row
        /// </summary>
        public event RowMouseHandler RowMouseMove = null;
        /// <summary>
        /// An event that is fired when mouse up occurs on a row
        /// </summary>
        public event RowMouseHandler RowMouseUp = null;
        /// <summary>
        /// An event that is fired when a click occurs on a row
        /// </summary>
        public event RowMouseHandler RowClick = null;
        /// <summary>
        /// An event that is fired when a double click occurs on a row
        /// </summary>
        public event RowMouseHandler RowDoubleClick = null;
        /// <summary>
        /// An event that is fired when the control has updated the clipboard
        /// </summary>
        public event CopyHandler ClipboardUpdated = null;
        #endregion

        private void CreateAutoList()
        {
            if(this._SyntaxBox != null && !this._SyntaxBox.DisableAutoList && this._AutoList == null){
                Debug.WriteLine("Creating Autolist");
                this.AutoList = new AutoListForm();
                NativeMethods.SetWindowLong(this.AutoList.Handle, NativeMethods.GWL_STYLE, NativeMethods.WS_CHILD);
                this.AutoList.SendToBack();
                this.AutoList.Visible = false;
                //this.Controls.Add (this.AutoList);
                this.AutoList.DoubleClick += this.AutoListDoubleClick;
                this.AutoList.Images = this._SyntaxBox.AutoListIcons;
            }
        }
        private void CreateFindForm()
        {
            if(!this._SyntaxBox.DisableFindForm && this._FindReplaceDialog == null){
                Debug.WriteLine("Creating Findform");
                this.FindReplaceDialog = new FindReplaceForm(this);
            }
        }
        private void CreateInfoTip()
        {
            if(this._SyntaxBox != null && !this._SyntaxBox.DisableInfoTip && this._InfoTip == null){
                Debug.WriteLine("Creating Infotip");
                this.InfoTip = new InfoTipForm(this);
                NativeMethods.SetWindowLong(this.InfoTip.Handle, NativeMethods.GWL_STYLE, NativeMethods.WS_CHILD);
                this.InfoTip.SendToBack();
                this.InfoTip.Visible = false;
            }
        }
        private void IntelliMouse_BeginScroll(object sender, EventArgs e)
        {
            this._IntelliScrollPos = 0;
            this.View.YOffset = 0;
        }
        private void IntelliMouse_EndScroll(object sender, EventArgs e)
        {
            this.View.YOffset = 0;
            this.Redraw();
        }
        private void IntelliMouse_Scroll(object sender, ScrollEventArgs e)
        {
            if(e.DeltaY < 0 && this.vScroll.Value == 0){
                this.View.YOffset = 0;
                this.Redraw();
                return;
            }
            if(e.DeltaY > 0 && this.vScroll.Value >= this.vScroll.Maximum - this.View.VisibleRowCount + 1){
                this.View.YOffset = 0;
                this.Redraw();
                return;
            }
            this._IntelliScrollPos += e.DeltaY / (double)8;
            int scrollrows = (int)(this._IntelliScrollPos) / this.View.RowHeight;
            if(scrollrows != 0){
                this._IntelliScrollPos -= scrollrows * this.View.RowHeight;
            }
            this.View.YOffset = - (int)this._IntelliScrollPos;
            this.ScrollScreen(scrollrows);
        }
        protected override void WndProc(ref Message m)
        {
            if(m.Msg == (int)WindowMessage.WM_DESTROY){
                try{
                    if(this.FindReplaceDialog != null){
                        this.FindReplaceDialog.Close();
                    }
                    if(this.AutoList != null){
                        this.AutoList.Close();
                    }
                    if(this.InfoTip != null){
                        this.InfoTip.Close();
                    }
                } catch{}
            }
            base.WndProc(ref m);
        }
        protected void CopyAsRTF()
        {
            TextStyle[] styles = this.Document.Parser.SyntaxDefinition.Styles;
            this.Document.ParseAll(true);
            int r1 = this.Selection.LogicalBounds.FirstRow;
            int r2 = this.Selection.LogicalBounds.LastRow;
            int c1 = this.Selection.LogicalBounds.FirstColumn;
            int c2 = this.Selection.LogicalBounds.LastColumn;
            var sb = new StringBuilder();
            sb.Append(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1053{\fonttbl{\f0\fmodern\fprq1\fcharset0 " + this.FontName
                      + @";}}");
            sb.Append(@"{\colortbl ;");
            foreach(TextStyle ts in styles){
                sb.AppendFormat("\\red{0}\\green{1}\\blue{2};", ts.ForeColor.R, ts.ForeColor.G, ts.ForeColor.B);
                sb.AppendFormat("\\red{0}\\green{1}\\blue{2};", ts.BackColor.R, ts.BackColor.G, ts.BackColor.B);
            }
            sb.Append(@";}");
            sb.Append(@"\viewkind4\uc1\pard\f0\fs20");
            bool Done = false;
            for(int i = r1; i <= r2; i++){
                Row row = this.Document[i];
                foreach(Word w in row){
                    if(i == r1 && w.Column + w.Text.Length < c1){
                        continue;
                    }
                    bool IsFirst = (i == r1 && w.Column <= c1 && w.Column + w.Text.Length > c1);
                    bool IsLast = (i == r2 && w.Column < c2 && w.Column + w.Text.Length > c2);
                    if(w.Type == WordType.Word && w.Style != null){
                        int clrindex = Array.IndexOf(styles, w.Style);
                        clrindex *= 2;
                        clrindex++;
                        sb.Append("{\\cf" + clrindex.ToString(CultureInfo.InvariantCulture));
                        if(!w.Style.Transparent){
                            sb.Append("\\highlight" + (clrindex + 1).ToString(CultureInfo.InvariantCulture));
                        }
                        sb.Append(" ");
                    }
                    if(w.Style != null){
                        if(w.Style.Bold){
                            sb.Append(@"\b ");
                        }
                        if(w.Style.Underline){
                            sb.Append(@"\ul ");
                        }
                        if(w.Style.Italic){
                            sb.Append(@"\i ");
                        }
                    }
                    string wordtext = w.Text;
                    if(IsLast){
                        wordtext = wordtext.Substring(0, c2 - w.Column);
                    }
                    if(IsFirst){
                        wordtext = wordtext.Substring(c1 - w.Column);
                    }
                    wordtext =
                            wordtext.Replace(@"\", @" \ \ ").Replace(@"
        }
        ", @" \
      }
      ").
                                    Replace(@"
      {
        ", @" \
        {
          ");
                    sb.Append(wordtext);
                    if(w.Style != null){
                        if(w.Style.Bold){
                            sb.Append(@"\b0 ");
                        }
                        if(w.Style.Underline){
                            sb.Append(@"\ul0 ");
                        }
                        if(w.Style.Italic){
                            sb.Append(@"\i0 ");
                        }
                    }
                    if(w.Type == WordType.Word && w.Style != null){
                        sb.Append("}");
                    }
                    if(IsLast){
                        Done = true;
                        break;
                    }
                }
                if(Done){
                    break;
                }
                sb.Append(@"\par");
            }
            var da = new DataObject();
            da.SetData(DataFormats.Rtf, sb.ToString());
            string s = this.Selection.Text;
            da.SetData(DataFormats.Text, s);
            Clipboard.SetDataObject(da);
            var ea = new CopyEventArgs{Text = s};
            this.OnClipboardUpdated(ea);
        }

        #region Constructor
        /// <summary>
        /// Default constructor for the SyntaxBoxControl
        /// </summary>
        public EditViewControl(SyntaxBoxControl Parent)
        {
            this._SyntaxBox = Parent;
            this.Painter = new NativePainter(this);
            this._Selection = new Selection(this);
            this._Caret = new Caret(this);
            this._Caret.Change += this.CaretChanged;
            this._Selection.Change += this.SelectionChanged;
            this.InitializeComponent();
            this.CreateAutoList();
            //CreateFindForm ();
            this.CreateInfoTip();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, false);
            this.SetStyle(ControlStyles.DoubleBuffer, false);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Opaque, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.UpdateStyles();
            //			this.IMEWindow = new Alsing.Globalization.IMEWindow (this.Handle,_SyntaxBox.FontName,_SyntaxBox.FontSize);
        }
        #endregion

        #region DISPOSE()
        /// <summary>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.RemoveFocus();
#if DEBUG
            try
            {
                Console.WriteLine("disposing editview");
            }
            catch
            {
            }
#endif
            if(disposing){
                if(this.components != null){
                    this.components.Dispose();
                }
                try{
                    if(this.Painter != null){
                        this.Painter.Dispose();
                    }
                } catch{}
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Private/Protected/public Properties
        public int PixelTabSize
        {
            get { return this._SyntaxBox.TabSize * this.View.CharWidth; }
        }
        #endregion

        #region Private/Protected/Internal Methods
        private int MaxCharWidth = 8;
        private void DoResize()
        {
            if(this.Visible && this.Width > 0 && this.Height > 0 && this.IsHandleCreated){
                try{
                    if(this.Filler == null){
                        return;
                    }
                    this.TopThumb.Width = SystemInformation.VerticalScrollBarWidth;
                    this.LeftThumb.Height = SystemInformation.HorizontalScrollBarHeight;
                    this.vScroll.Width = SystemInformation.VerticalScrollBarWidth;
                    this.hScroll.Height = SystemInformation.HorizontalScrollBarHeight;
                    if(this.TopThumbVisible){
                        this.vScroll.Top = this.TopThumb.Height;
                        if(this.hScroll.Visible){
                            this.vScroll.Height = this.ClientHeight - this.hScroll.Height - this.TopThumb.Height;
                        } else{
                            this.vScroll.Height = this.ClientHeight - this.TopThumb.Height;
                        }
                    } else{
                        if(this.hScroll.Visible){
                            this.vScroll.Height = this.ClientHeight - this.hScroll.Height;
                        } else{
                            this.vScroll.Height = this.ClientHeight;
                        }
                        this.vScroll.Top = 0;
                    }
                    if(this.LeftThumbVisible){
                        this.hScroll.Left = this.LeftThumb.Width;
                        if(this.vScroll.Visible){
                            this.hScroll.Width = this.ClientWidth - this.vScroll.Width - this.LeftThumb.Width;
                        } else{
                            this.hScroll.Width = this.ClientWidth - this.LeftThumb.Width;
                        }
                    } else{
                        if(this.vScroll.Visible){
                            this.hScroll.Width = this.ClientWidth - this.vScroll.Width;
                        } else{
                            this.hScroll.Width = this.ClientWidth;
                        }
                        this.hScroll.Left = 0;
                    }
                    if(this.Width != this.OldWidth && this.Width > 0){
                        this.OldWidth = this.Width;
                        if(this.Painter != null){
                            this.Painter.Resize();
                        }
                    }
                    this.vScroll.Left = this.ClientWidth - this.vScroll.Width;
                    this.hScroll.Top = this.ClientHeight - this.hScroll.Height;
                    this.LeftThumb.Left = 0;
                    this.LeftThumb.Top = this.hScroll.Top;
                    this.TopThumb.Left = this.vScroll.Left;
                    this.TopThumb.Top = 0;
                    this.Filler.Left = this.vScroll.Left;
                    this.Filler.Top = this.hScroll.Top;
                    this.Filler.Width = this.vScroll.Width;
                    this.Filler.Height = this.hScroll.Height;
                } catch{}
            }
        }
        private void InsertText(string text)
        {
            this.Caret.CropPosition();
            if(this.Selection.IsValid){
                this.Selection.DeleteSelection();
                this.InsertText(text);
            } else{
                if(!this._OverWrite || text.Length > 1){
                    TextPoint p = this.Document.InsertText(text, this.Caret.Position.X, this.Caret.Position.Y);
                    this.Caret.CurrentRow.Parse(true);
                    if(text.Length == 1){
                        this.Caret.SetPos(p);
                        this.Caret.CaretMoved(false);
                    } else{
                        //Document.i = true;
                        this.Document.ResetVisibleRows();
                        this.Caret.SetPos(p);
                        this.Caret.CaretMoved(false);
                    }
                } else{
                    var r = new TextRange{
                                                 FirstColumn = this.Caret.Position.X,
                                                 FirstRow = this.Caret.Position.Y,
                                                 LastColumn = (this.Caret.Position.X + 1),
                                                 LastRow = this.Caret.Position.Y
                                         };
                    var ag = new UndoBlockCollection();
                    var b = new UndoBlock{
                                                 Action = UndoAction.DeleteRange,
                                                 Text = this.Document.GetRange(r),
                                                 Position = this.Caret.Position
                                         };
                    ag.Add(b);
                    this.Document.DeleteRange(r, false);
                    b = new UndoBlock{Action = UndoAction.InsertRange};
                    string NewChar = text;
                    b.Text = NewChar;
                    b.Position = this.Caret.Position;
                    ag.Add(b);
                    this.Document.AddToUndoList(ag);
                    this.Document.InsertText(NewChar, this.Caret.Position.X, this.Caret.Position.Y, false);
                    this.Caret.CurrentRow.Parse(true);
                    this.Caret.MoveRight(false);
                }
            }
            //	this.ScrollIntoView ();
        }
        private void InsertEnter()
        {
            this.Caret.CropPosition();
            if(this.Selection.IsValid){
                this.Selection.DeleteSelection();
                this.InsertEnter();
            } else{
                switch(this.Indent){
                    case IndentStyle.None:
                    {
                        this.Document.InsertText("\n", this.Caret.Position.X, this.Caret.Position.Y);
                        //depends on what sort of indention we are using....
                        this.Caret.CurrentRow.Parse();
                        this.Caret.MoveDown(false);
                        this.Caret.CurrentRow.Parse(false);
                        this.Caret.CurrentRow.Parse(true);
                        this.Caret.Position.X = 0;
                        this.Caret.SetPos(this.Caret.Position);
                        break;
                    }
                    case IndentStyle.LastRow:
                    {
                        Row xtr = this.Caret.CurrentRow;
                        string indent = xtr.GetLeadingWhitespace();
                        int Max = Math.Min(indent.Length, this.Caret.Position.X);
                        string split = "\n" + indent.Substring(0, Max);
                        this.Document.InsertText(split, this.Caret.Position.X, this.Caret.Position.Y);
                        this.Document.ResetVisibleRows();
                        this.Caret.CurrentRow.Parse(false);
                        this.Caret.CurrentRow.Parse(true);
                        this.Caret.MoveDown(false);
                        this.Caret.CurrentRow.Parse(false);
                        this.Caret.CurrentRow.Parse(true);
                        this.Caret.Position.X = indent.Length;
                        this.Caret.SetPos(this.Caret.Position);
                        xtr.Parse(false);
                        xtr.Parse(true);
                        xtr.NextRow.Parse(false);
                        xtr.NextRow.Parse(true);
                        break;
                    }
                    case IndentStyle.Scope:
                    {
                        Row xtr = this.Caret.CurrentRow;
                        xtr.Parse(true);
                        if(xtr.ShouldOutdent){
                            this.OutdentEndRow();
                        }
                        this.Document.InsertText("\n", this.Caret.Position.X, this.Caret.Position.Y);
                        //depends on what sort of indention we are using....
                        this.Caret.CurrentRow.Parse();
                        this.Caret.MoveDown(false);
                        this.Caret.CurrentRow.Parse(false);
                        var indent = new String('\t', this.Caret.CurrentRow.Depth);
                        this.Document.InsertText(indent, 0, this.Caret.Position.Y);
                        this.Caret.CurrentRow.Parse(false);
                        this.Caret.CurrentRow.Parse(true);
                        this.Caret.Position.X = indent.Length;
                        this.Caret.SetPos(this.Caret.Position);
                        this.Caret.CropPosition();
                        this.Selection.ClearSelection();
                        xtr.Parse(false);
                        xtr.Parse(true);
                        xtr.NextRow.Parse(false);
                        xtr.NextRow.Parse(true);
                        break;
                    }
                    case IndentStyle.Smart:
                    {
                        Row xtr = this.Caret.CurrentRow;
                        if(xtr.ShouldOutdent){
                            this.OutdentEndRow();
                        }
                        this.Document.InsertText("\n", this.Caret.Position.X, this.Caret.Position.Y);
                        this.Caret.MoveDown(false);
                        this.Caret.CurrentRow.Parse(false);
                        this.Caret.CurrentRow.Parse(true);
                        this.Caret.CurrentRow.startSpan.StartRow.Parse(false);
                        this.Caret.CurrentRow.startSpan.StartRow.Parse(true);
                        string prev = "\t" + this.Caret.CurrentRow.startSpan.StartRow.GetVirtualLeadingWhitespace();
                        string indent = this.Caret.CurrentRow.PrevRow.GetLeadingWhitespace();
                        if(indent.Length < prev.Length){
                            indent = prev;
                        }
                        string ts = "\t" + new String(' ', this.TabSize);
                        while(indent.IndexOf(ts) >= 0){
                            indent = indent.Replace(ts, "\t\t");
                        }
                        this.Document.InsertText(indent, 0, this.Caret.Position.Y);
                        this.Caret.CurrentRow.Parse(false);
                        this.Caret.CurrentRow.Parse(true);
                        this.Caret.Position.X = indent.Length;
                        this.Caret.SetPos(this.Caret.Position);
                        this.Caret.CropPosition();
                        this.Selection.ClearSelection();
                        xtr.Parse(false);
                        xtr.Parse(true);
                        xtr.NextRow.Parse(false);
                        xtr.NextRow.Parse(true);
                        break;
                    }
                }
                this.ScrollIntoView();
            }
        }
        private void OutdentEndRow()
        {
            try{
                if(this.Indent == IndentStyle.Scope){
                    Row xtr = this.Caret.CurrentRow;
                    var indent1 = new String('\t', this.Caret.CurrentRow.Depth);
                    var tr = new TextRange{
                                                  FirstColumn = 0,
                                                  LastColumn = xtr.GetLeadingWhitespace().Length,
                                                  FirstRow = xtr.Index,
                                                  LastRow = xtr.Index
                                          };
                    this.Document.DeleteRange(tr);
                    this.Document.InsertText(indent1, 0, xtr.Index, true);
                    int diff = indent1.Length - tr.LastColumn;
                    this.Caret.Position.X += diff;
                    this.Caret.SetPos(this.Caret.Position);
                    this.Caret.CropPosition();
                    this.Selection.ClearSelection();
                    this.Caret.CurrentRow.Parse(false);
                    this.Caret.CurrentRow.Parse(true);
                } else if(this.Indent == IndentStyle.Smart){
                    Row xtr = this.Caret.CurrentRow;
                    if(xtr.FirstNonWsWord == xtr.expansion_EndSpan.EndWord){
                        //int j=xtr.Expansion_StartRow.StartWordIndex;
                        string indent1 = xtr.startSpan.StartWord.Row.GetVirtualLeadingWhitespace();
                        var tr = new TextRange{
                                                      FirstColumn = 0,
                                                      LastColumn = xtr.GetLeadingWhitespace().Length,
                                                      FirstRow = xtr.Index,
                                                      LastRow = xtr.Index
                                              };
                        this.Document.DeleteRange(tr);
                        string ts = "\t" + new String(' ', this.TabSize);
                        while(indent1.IndexOf(ts) >= 0){
                            indent1 = indent1.Replace(ts, "\t\t");
                        }
                        this.Document.InsertText(indent1, 0, xtr.Index, true);
                        int diff = indent1.Length - tr.LastColumn;
                        this.Caret.Position.X += diff;
                        this.Caret.SetPos(this.Caret.Position);
                        this.Caret.CropPosition();
                        this.Selection.ClearSelection();
                        this.Caret.CurrentRow.Parse(false);
                        this.Caret.CurrentRow.Parse(true);
                    }
                }
            } catch{}
        }
        private void DeleteForward()
        {
            this.Caret.CropPosition();
            if(this.Selection.IsValid){
                this.Selection.DeleteSelection();
            } else{
                Row xtr = this.Caret.CurrentRow;
                if(this.Caret.Position.X == xtr.Text.Length){
                    if(this.Caret.Position.Y <= this.Document.Count - 2){
                        var r = new TextRange{FirstColumn = this.Caret.Position.X, FirstRow = this.Caret.Position.Y};
                        r.LastRow = r.FirstRow + 1;
                        r.LastColumn = 0;
                        this.Document.DeleteRange(r);
                        this.Document.ResetVisibleRows();
                    }
                } else{
                    var r = new TextRange{FirstColumn = this.Caret.Position.X, FirstRow = this.Caret.Position.Y};
                    r.LastRow = r.FirstRow;
                    r.LastColumn = r.FirstColumn + 1;
                    this.Document.DeleteRange(r);
                    this.Document.ResetVisibleRows();
                    this.Caret.CurrentRow.Parse(false);
                    this.Caret.CurrentRow.Parse(true);
                }
            }
        }
        private void DeleteBackwards()
        {
            this.Caret.CropPosition();
            if(this.Selection.IsValid){
                this.Selection.DeleteSelection();
            } else{
                Row xtr = this.Caret.CurrentRow;
                if(this.Caret.Position.X == 0){
                    if(this.Caret.Position.Y > 0){
                        this.Caret.Position.Y--;
                        this.Caret.MoveEnd(false);
                        this.DeleteForward();
                        //Caret.CurrentRow.Parse ();
                        this.Document.ResetVisibleRows();
                    }
                } else{
                    if(this.Caret.Position.X >= xtr.Text.Length){
                        var r = new TextRange
                                {FirstColumn = (this.Caret.Position.X - 1), FirstRow = this.Caret.Position.Y};
                        r.LastRow = r.FirstRow;
                        r.LastColumn = r.FirstColumn + 1;
                        this.Document.DeleteRange(r);
                        this.Document.ResetVisibleRows();
                        this.Caret.MoveEnd(false);
                        this.Caret.CurrentRow.Parse();
                    } else{
                        var r = new TextRange
                                {FirstColumn = (this.Caret.Position.X - 1), FirstRow = this.Caret.Position.Y};
                        r.LastRow = r.FirstRow;
                        r.LastColumn = r.FirstColumn + 1;
                        this.Document.DeleteRange(r);
                        this.Document.ResetVisibleRows();
                        this.Caret.MoveLeft(false);
                        this.Caret.CurrentRow.Parse();
                    }
                }
            }
        }
        private void ScrollScreen(int Amount)
        {
            try{
                this.tooltip.RemoveAll();
                int newval = this.vScroll.Value + Amount;
                newval = Math.Max(newval, this.vScroll.Minimum);
                newval = Math.Min(newval, this.vScroll.Maximum);
                if(newval >= this.vScroll.Maximum - this.View.VisibleRowCount + 1){
                    newval = this.vScroll.Maximum - this.View.VisibleRowCount + 1;
                }
                newval = Math.Max(newval, this.vScroll.Minimum);
                this.vScroll.Value = newval;
                this.Redraw();
            } catch{}
        }
        private void PasteText()
        {
            try{
                IDataObject iData = Clipboard.GetDataObject();
                if(iData != null){
                    if(iData.GetDataPresent(DataFormats.UnicodeText)){
                        // Yes it is, so display it in a text box.
                        var s = (string)iData.GetData(DataFormats.UnicodeText);
                        this.InsertText(s);
                        if(this.ParseOnPaste){
                            this.Document.ParseAll(true);
                        }
                    } else if(iData.GetDataPresent(DataFormats.Text)){
                        // Yes it is, so display it in a text box.
                        var s = (string)iData.GetData(DataFormats.Text);
                        this.InsertText(s);
                        if(this.ParseOnPaste){
                            this.Document.ParseAll(true);
                        }
                    }
                }
            } catch{
                //ignore
            }
        }
        private void BeginDragDrop()
        {
            this.DoDragDrop(this.Selection.Text, DragDropEffects.All);
        }
        private void Redraw()
        {
            this.Invalidate();
        }
        private void RedrawCaret()
        {
            Graphics g = this.CreateGraphics();
            this.Painter.RenderCaret(g);
            g.Dispose();
        }
        private void SetMouseCursor(int x, int y)
        {
            if(this._SyntaxBox.LockCursorUpdate){
                this.Cursor = this._SyntaxBox.Cursor;
                return;
            }
            if(this.View.Action == EditAction.DragText){
                this.Cursor = Cursors.Hand;
                //Cursor.Current = Cursors.Hand;
            } else{
                if(x < this.View.TotalMarginWidth){
                    if(x < this.View.GutterMarginWidth){
                        this.Cursor = Cursors.Arrow;
                    } else{
                        var ms = new MemoryStream(Resources.FlippedCursor);
                        this.Cursor = new Cursor(ms);
                    }
                } else{
                    if(x > this.View.TextMargin - 8){
                        if(this.IsOverSelection(x, y)){
                            this.Cursor = Cursors.Arrow;
                        } else{
                            TextPoint tp = this.Painter.CharFromPixel(x, y);
                            Word w = this.Document.GetWordFromPos(tp);
                            if(w != null && w.Pattern != null && w.Pattern.Category != null){
                                var e = new WordMouseEventArgs{
                                                                      Pattern = w.Pattern,
                                                                      Button = MouseButtons.None,
                                                                      Cursor = Cursors.Hand,
                                                                      Word = w
                                                              };
                                this._SyntaxBox.OnWordMouseHover(ref e);
                                this.Cursor = e.Cursor;
                            } else{
                                this.Cursor = Cursors.IBeam;
                            }
                        }
                    } else{
                        this.Cursor = Cursors.Arrow;
                    }
                }
            }
        }
        private void CopyText()
        {
            //no freaking vs.net copy empty selection 
            if(!this.Selection.IsValid){
                return;
            }
            if(this._SyntaxBox.CopyAsRTF){
                this.CopyAsRTF();
            } else{
                try{
                    string t = this.Selection.Text;
                    Clipboard.SetDataObject(t, true);
                    var ea = new CopyEventArgs{Text = t};
                    this.OnClipboardUpdated(ea);
                } catch{
                    try{
                        string t = this.Selection.Text;
                        Clipboard.SetDataObject(t, true);
                        var ea = new CopyEventArgs{Text = t};
                        this.OnClipboardUpdated(ea);
                    } catch{}
                }
            }
        }
        /// <summary>
        /// For public use only
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override bool IsInputKey(Keys key)
        {
            switch(key){
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Left:
                case Keys.Tab:
                case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Enter:
                    return true;
            }
            return true; //base.IsInputKey(key);			
        }
        protected override bool IsInputChar(char c)
        {
            return true;
        }
        public void RemoveFocus()
        {
            if(this.InfoTip == null || this.AutoList == null){
                return;
            }
            if(!this.ContainsFocus && !this.InfoTip.ContainsFocus && !this.AutoList.ContainsFocus){
                this.CaretTimer.Enabled = false;
                this.Caret.Blink = false;
                this._AutoListVisible = false;
                this._InfoTipVisible = false;
            }
            this.Redraw();
        }
        private void SelectCurrentWord()
        {
            Row xtr = this.Caret.CurrentRow;
            if(xtr.Text == ""){
                return;
            }
            if(this.Caret.Position.X >= xtr.Text.Length){
                return;
            }
            string Char = xtr.Text.Substring(this.Caret.Position.X, 1);
            int Type = CharType(Char);
            int left = this.Caret.Position.X;
            int right = this.Caret.Position.X;
            while(left >= 0 && CharType(xtr.Text.Substring(left, 1)) == Type){
                left--;
            }
            while(right <= xtr.Text.Length - 1 && CharType(xtr.Text.Substring(right, 1)) == Type){
                right++;
            }
            this.Selection.Bounds.FirstRow = this.Selection.Bounds.LastRow = xtr.Index;
            this.Selection.Bounds.FirstColumn = left + 1;
            this.Selection.Bounds.LastColumn = right;
            this.Caret.Position.X = right;
            this.Caret.SetPos(this.Caret.Position);
            this.Redraw();
        }
        private static int CharType(string s)
        {
            const string g1 = " \t";
            const string g2 = ".,-+'?´=)(/&%¤#!\"\\<>[]$£@*:;{}";
            if(g1.IndexOf(s) >= 0){
                return 1;
            }
            if(g2.IndexOf(s) >= 0){
                return 2;
            }
            return 3;
        }
        private void SelectPattern(int RowIndex, int Column, int Length)
        {
            this.Selection.Bounds.FirstColumn = Column;
            this.Selection.Bounds.FirstRow = RowIndex;
            this.Selection.Bounds.LastColumn = Column + Length;
            this.Selection.Bounds.LastRow = RowIndex;
            this.Caret.Position.X = Column + Length;
            this.Caret.Position.Y = RowIndex;
            this.Caret.CurrentRow.EnsureVisible();
            this.ScrollIntoView();
            this.Redraw();
        }
        public void InitVars()
        {
            //setup viewpoint data
            if(this.View.RowHeight == 0){
                this.View.RowHeight = 48;
            }
            if(this.View.CharWidth == 0){
                this.View.CharWidth = 16;
            }
            //View.RowHeight=16;
            //View.CharWidth=8;
            this.View.FirstVisibleColumn = this.hScroll.Value;
            this.View.FirstVisibleRow = this.vScroll.Value;
            //	View.yOffset =_yOffset;
            this.View.VisibleRowCount = 0;
            if(this.hScroll.Visible){
                this.View.VisibleRowCount = (this.Height - this.hScroll.Height) / this.View.RowHeight + 1;
            } else{
                this.View.VisibleRowCount = (this.Height - this.hScroll.Height) / this.View.RowHeight + 2;
            }
            this.View.GutterMarginWidth = this.ShowGutterMargin ? this.GutterMarginWidth : 0;
            if(this.ShowLineNumbers){
                int chars = (this.Document.Count).ToString(CultureInfo.InvariantCulture).Length;
                var s = new String('9', chars);
                this.View.LineNumberMarginWidth = 10 + this.Painter.MeasureString(s).Width;
            } else{
                this.View.LineNumberMarginWidth = 0;
            }
            this.View.TotalMarginWidth = this.View.GutterMarginWidth + this.View.LineNumberMarginWidth;
            if(this.Document.Folding){
                this.View.TextMargin = this.View.TotalMarginWidth + 20;
            } else{
                this.View.TextMargin = this.View.TotalMarginWidth + 7;
            }
            this.View.ClientAreaWidth = this.Width - this.vScroll.Width - this.View.TextMargin;
            this.View.ClientAreaStart = this.View.FirstVisibleColumn * this.View.CharWidth;
        }
        public void CalcMaxCharWidth()
        {
            this.MaxCharWidth = this.Painter.GetMaxCharWidth();
        }
        public void SetMaxHorizontalScroll()
        {
            this.CalcMaxCharWidth();
            int CharWidth = this.View.CharWidth;
            if(CharWidth == 0){
                CharWidth = 1;
            }
            if(this.View.ClientAreaWidth / CharWidth < 0){
                this.hScroll.Maximum = 1000;
                return;
            }
            this.hScroll.LargeChange = this.View.ClientAreaWidth / CharWidth;
            try{
                int max = 0;
                for(int i = this.View.FirstVisibleRow; i < this.Document.VisibleRows.Count; i++){
                    if(i >= this.View.VisibleRowCount + this.View.FirstVisibleRow){
                        break;
                    }
                    string l = this.Document.VisibleRows[i].IsCollapsed
                                       ? this.Document.VisibleRows[i].VirtualCollapsedRow.Text
                                       : this.Document.VisibleRows[i].Text;
                    l = l.Replace("\t", new string(' ', this.TabSize));
                    if(l.Length > max){
                        max = l.Length;
                    }
                }
                int pixels = max * this.MaxCharWidth;
                int chars = pixels / CharWidth;
                if(this.hScroll.Value <= chars){
                    this.hScroll.Maximum = chars;
                }
            } catch{
                this.hScroll.Maximum = 1000;
            }
        }
        public void InitScrollbars()
        {
            if(this.Document.VisibleRows.Count > 0){
                this.vScroll.Maximum = this.Document.VisibleRows.Count + 1;
                //+this.View.VisibleRowCount-2;// - View.VisibleRowCount  ;
                this.vScroll.LargeChange = this.View.VisibleRowCount;
                this.SetMaxHorizontalScroll();
            } else{
                this.vScroll.Maximum = 1;
            }
        }
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new Container();
            var resources = new ResourceManager(typeof(EditViewControl));
            this.Filler = new PictureBox();
            this.CaretTimer = new Timer(this.components);
            this.tooltip = new ToolTip(this.components);
            this.SuspendLayout();
            if(!this._SyntaxBox.DisableIntelliMouse){
                this.IntelliMouse = new IntelliMouseControl{
                                                                   BackgroundImage = ((Bitmap)(resources.GetObject("IntelliMouse.BackgroundImage"))),
                                                                   Image = ((Bitmap)(resources.GetObject("IntelliMouse.Image"))),
                                                                   Location = new Point(197, 157),
                                                                   Name = "IntelliMouse",
                                                                   Size = new Size(28, 28),
                                                                   TabIndex = 4,
                                                                   TransparencyKey = Color.FromArgb(((255)), ((0)), ((255))),
                                                                   Visible = false
                                                           };
                // 
                // IntelliMouse
                // 
                this.IntelliMouse.EndScroll += this.IntelliMouse_EndScroll;
                this.IntelliMouse.BeginScroll += this.IntelliMouse_BeginScroll;
                this.IntelliMouse.Scroll += this.IntelliMouse_Scroll;
            }
            // 
            // hScroll
            // 
            this.hScroll.Cursor = Cursors.Default;
            this.hScroll.Scroll += this.hScroll_Scroll;
            // 
            // vScroll
            // 
            this.vScroll.Cursor = Cursors.Default;
            this.vScroll.Scroll += this.vScroll_Scroll;
            // 
            // CaretTimer
            // 
            this.CaretTimer.Enabled = true;
            this.CaretTimer.Interval = 500;
            this.CaretTimer.Tick += this.CaretTimer_Tick;
            // 
            // tooltip
            // 
            this.tooltip.AutoPopDelay = 50000;
            this.tooltip.InitialDelay = 0;
            this.tooltip.ReshowDelay = 1000;
            this.tooltip.ShowAlways = true;
            // 
            // TopThumb
            // 
            this.TopThumb.BackColor = SystemColors.Control;
            this.TopThumb.Cursor = Cursors.HSplit;
            this.TopThumb.Location = new Point(101, 17);
            this.TopThumb.Name = "TopThumb";
            this.TopThumb.Size = new Size(16, 8);
            this.TopThumb.TabIndex = 3;
            this.TopThumb.Visible = false;
            // 
            // LeftThumb
            // 
            this.LeftThumb.BackColor = SystemColors.Control;
            this.LeftThumb.Cursor = Cursors.VSplit;
            this.LeftThumb.Location = new Point(423, 17);
            this.LeftThumb.Name = "LeftThumb";
            this.LeftThumb.Size = new Size(8, 16);
            this.LeftThumb.TabIndex = 3;
            this.LeftThumb.Visible = false;
            // 
            // EditViewControl
            // 
            try{
                this.AllowDrop = true;
            } catch{
                //	Console.WriteLine ("error in editview allowdrop {0}",x.Message);
            }
            this.Controls.AddRange(new Control[]{this.IntelliMouse});
            this.Size = new Size(0, 0);
            this.LostFocus += this.EditViewControl_Leave;
            this.GotFocus += this.EditViewControl_Enter;
            this.ResumeLayout(false);
        }
        public void InsertAutolistText()
        {
            var tr = new TextRange{
                                          FirstRow = this.Caret.Position.Y,
                                          LastRow = this.Caret.Position.Y,
                                          FirstColumn = this.AutoListStartPos.X,
                                          LastColumn = this.Caret.Position.X
                                  };
            this.Document.DeleteRange(tr, true);
            this.Caret.Position.X = this.AutoListStartPos.X;
            this.InsertText(this.AutoList.SelectedText);
            this.SetFocus();
        }
        private void MoveCaretToNextWord(bool Select)
        {
            int x = this.Caret.Position.X;
            int y = this.Caret.Position.Y;
            int StartType;
            bool found = false;
            if(x == this.Caret.CurrentRow.Text.Length){
                StartType = 1;
            } else{
                string StartChar = this.Document[y].Text.Substring(this.Caret.Position.X, 1);
                StartType = CharType(StartChar);
            }
            while(y < this.Document.Count){
                while(x < this.Document[y].Text.Length){
                    string Char = this.Document[y].Text.Substring(x, 1);
                    int Type = CharType(Char);
                    if(Type != StartType){
                        if(Type == 1){
                            StartType = 1;
                        } else{
                            found = true;
                            break;
                        }
                    }
                    x++;
                }
                if(found){
                    break;
                }
                x = 0;
                y++;
            }
            if(y >= this.Document.Count - 1){
                y = this.Document.Count - 1;
                if(x >= this.Document[y].Text.Length){
                    x = this.Document[y].Text.Length - 1;
                }
                if(x == - 1){
                    x = 0;
                }
            }
            this.Caret.SetPos(new TextPoint(x, y));
            if(!Select){
                this.Selection.ClearSelection();
            }
            if(Select){
                this.Selection.MakeSelection();
            }
            this.ScrollIntoView();
        }
        public void InitGraphics()
        {
            this.Painter.InitGraphics();
        }
        private void MoveCaretToPrevWord(bool Select)
        {
            int x = this.Caret.Position.X;
            int y = this.Caret.Position.Y;
            int StartType;
            bool found = false;
            if(x == this.Caret.CurrentRow.Text.Length){
                StartType = 1;
                x = this.Caret.CurrentRow.Text.Length - 1;
            } else{
                string StartChar = this.Document[y].Text.Substring(this.Caret.Position.X, 1);
                StartType = CharType(StartChar);
            }
            while(y >= 0){
                while(x >= 0 && x < this.Document[y].Text.Length){
                    string Char = this.Document[y].Text.Substring(x, 1);
                    int Type = CharType(Char);
                    if(Type != StartType){
                        found = true;
                        while(x > 0){
                            string Char2 = this.Document[y].Text.Substring(x, 1);
                            int Type2 = CharType(Char2);
                            if(Type2 != Type){
                                x++;
                                break;
                            }
                            x--;
                        }
                        break;
                    }
                    x--;
                }
                if(found){
                    break;
                }
                if(y == 0){
                    x = 0;
                    break;
                }
                y--;
                x = this.Document[y].Text.Length - 1;
            }
            this.Caret.SetPos(new TextPoint(x, y));
            if(!Select){
                this.Selection.ClearSelection();
            }
            if(Select){
                this.Selection.MakeSelection();
            }
            this.ScrollIntoView();
        }
        private void SetFocus()
        {
            this.Focus();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Displays the GotoLine dialog.
        /// </summary>
        public void ShowGotoLine()
        {
            var go = new GotoLineForm(this, this.Document.Count);
            //			if (this.TopLevelControl is Form)
            //				go.Owner=(Form)this.TopLevelControl;
            go.ShowDialog(this.TopLevelControl);
        }
        /// <summary>
        /// -
        /// </summary>
        public void ShowSettings()
        {
            //	SettingsForm se=new SettingsForm (this);
            //	se.ShowDialog();
        }
        /// <summary>
        /// Places the caret on a specified line and scrolls the caret into view.
        /// </summary>
        /// <param name="RowIndex">the zero based index of the line to jump to</param>
        public void GotoLine(int RowIndex)
        {
            if(RowIndex >= this.Document.Count){
                RowIndex = this.Document.Count - 1;
            }
            if(RowIndex < 0){
                RowIndex = 0;
            }
            this.Caret.Position.Y = RowIndex;
            this.Caret.Position.X = 0;
            this.Caret.CurrentRow.EnsureVisible();
            this.ClearSelection();
            this.ScrollIntoView();
            this.Redraw();
        }
        /// <summary>
        /// Clears the active selection.
        /// </summary>
        public void ClearSelection()
        {
            this.Selection.ClearSelection();
            this.Redraw();
        }
        /// <summary>
        /// Returns if a specified pixel position is over the current selection.
        /// </summary>
        /// <param name="x">X Position in pixels</param>
        /// <param name="y">Y Position in pixels</param>
        /// <returns>true if over selection othewise false</returns>
        public bool IsOverSelection(int x, int y)
        {
            TextPoint p = this.Painter.CharFromPixel(x, y);
            if(p.Y >= this.Selection.LogicalBounds.FirstRow && p.Y <= this.Selection.LogicalBounds.LastRow
               && this.Selection.IsValid){
                if(p.Y > this.Selection.LogicalBounds.FirstRow && p.Y < this.Selection.LogicalBounds.LastRow
                   && this.Selection.IsValid){
                    return true;
                }
                if(p.Y == this.Selection.LogicalBounds.FirstRow
                   && this.Selection.LogicalBounds.FirstRow == this.Selection.LogicalBounds.LastRow){
                    if(p.X >= this.Selection.LogicalBounds.FirstColumn && p.X <= this.Selection.LogicalBounds.LastColumn){
                        return true;
                    }
                    return false;
                }
                if(p.X >= this.Selection.LogicalBounds.FirstColumn && p.Y == this.Selection.LogicalBounds.FirstRow){
                    return true;
                }
                if(p.X <= this.Selection.LogicalBounds.LastColumn && p.Y == this.Selection.LogicalBounds.LastRow){
                    return true;
                }
                return false;
            }
            return false;
            //no chance we are over Selection.LogicalBounds
        }
        /// <summary>
        /// Scrolls a given position in the text into view.
        /// </summary>
        /// <param name="Pos">Position in text</param>
        public void ScrollIntoView(TextPoint Pos)
        {
            TextPoint tmp = this.Caret.Position;
            this.Caret.Position = Pos;
            this.Caret.CurrentRow.EnsureVisible();
            this.ScrollIntoView();
            this.Caret.Position = tmp;
            this.Invalidate();
        }
        public void ScrollIntoView(int RowIndex)
        {
            Row r = this.Document[RowIndex];
            r.EnsureVisible();
            this.vScroll.Value = r.VisibleIndex;
            this.Invalidate();
        }
        /// <summary>
        /// Scrolls the caret into view.
        /// </summary>
        public void ScrollIntoView()
        {
            this.InitScrollbars();
            this.Caret.CropPosition();
            try{
                Row xtr2 = this.Caret.CurrentRow;
                if(xtr2.VisibleIndex >= this.View.FirstVisibleRow + this.View.VisibleRowCount - 2){
                    int Diff = this.Caret.CurrentRow.VisibleIndex
                               - (this.View.FirstVisibleRow + this.View.VisibleRowCount - 2) + this.View.FirstVisibleRow;
                    if(Diff > this.Document.VisibleRows.Count - 1){
                        Diff = this.Document.VisibleRows.Count - 1;
                    }
                    Row r = this.Document.VisibleRows[Diff];
                    int index = r.VisibleIndex;
                    if(index != - 1){
                        this.vScroll.Value = index;
                    }
                }
            } catch{}
            if(this.Caret.CurrentRow.VisibleIndex < this.View.FirstVisibleRow){
                Row r = this.Caret.CurrentRow;
                int index = r.VisibleIndex;
                if(index != - 1){
                    this.vScroll.Value = index;
                }
            }
            Row xtr = this.Caret.CurrentRow;
            int x;
            if(this.Caret.CurrentRow.IsCollapsedEndPart){
                x = this.Painter.MeasureRow(xtr, this.Caret.Position.X).Width
                    + this.Caret.CurrentRow.Expansion_PixelStart;
                x -= this.Painter.MeasureRow(xtr, xtr.Expansion_StartChar).Width;
                if(x >= this.View.ClientAreaWidth + this.View.ClientAreaStart){
                    this.hScroll.Value = Math.Min(this.hScroll.Maximum,
                                                  ((x - this.View.ClientAreaWidth) / this.View.CharWidth) + 15);
                }
                if(x < this.View.ClientAreaStart + 10){
                    this.hScroll.Value = Math.Max(this.hScroll.Minimum, ((x) / this.View.CharWidth) - 15);
                }
            } else{
                x = this.Painter.MeasureRow(xtr, this.Caret.Position.X).Width;
                if(x >= this.View.ClientAreaWidth + this.View.ClientAreaStart){
                    this.hScroll.Value = Math.Min(this.hScroll.Maximum,
                                                  ((x - this.View.ClientAreaWidth) / this.View.CharWidth) + 15);
                }
                if(x < this.View.ClientAreaStart){
                    this.hScroll.Value = Math.Max(this.hScroll.Minimum, ((x) / this.View.CharWidth) - 15);
                }
            }
        }
        /// <summary>
        /// Moves the caret to the next line that has a bookmark.
        /// </summary>
        public void GotoNextBookmark()
        {
            int index = this.Document.GetNextBookmark(this.Caret.Position.Y);
            this.Caret.SetPos(new TextPoint(0, index));
            this.ScrollIntoView();
            this.Redraw();
        }
        /// <summary>
        /// Moves the caret to the previous line that has a bookmark.
        /// </summary>
        public void GotoPreviousBookmark()
        {
            int index = this.Document.GetPreviousBookmark(this.Caret.Position.Y);
            this.Caret.SetPos(new TextPoint(0, index));
            this.ScrollIntoView();
            this.Redraw();
        }
        /// <summary>
        /// Selects next occurance of the given pattern.
        /// </summary>
        /// <param name="Pattern">Pattern to find</param>
        /// <param name="MatchCase">Case sensitive</param>
        /// <param name="WholeWords">Match whole words only</param>
        /// <param name="UseRegEx"></param>
        public bool SelectNext(string Pattern, bool MatchCase, bool WholeWords, bool UseRegEx)
        {
            string pattern = Pattern;
            for(int i = this.Caret.Position.Y; i < this.Document.Count; i++){
                Row r = this.Document[i];
                string t = r.Text;
                if(WholeWords){
                    string s = " " + r.Text + " ";
                    t = "";
                    pattern = " " + Pattern + " ";
                    foreach(char c in s){
                        if(".,+-*^\\/()[]{}@:;'?£$#%& \t=<>".IndexOf(c) >= 0){
                            t += " ";
                        } else{
                            t += c;
                        }
                    }
                }
                if(!MatchCase){
                    t = t.ToLowerInvariant();
                    pattern = pattern.ToLowerInvariant();
                }
                int Col = t.IndexOf(pattern);
                int StartCol = this.Caret.Position.X;
                int StartRow = this.Caret.Position.Y;
                if((Col >= StartCol) || (i > StartRow && Col >= 0)){
                    this.SelectPattern(i, Col, Pattern.Length);
                    return true;
                }
            }
            return false;
        }
        public bool ReplaceSelection(string text)
        {
            if(!this.Selection.IsValid){
                return false;
            }
            int x = this.Selection.LogicalBounds.FirstColumn;
            int y = this.Selection.LogicalBounds.FirstRow;
            this.Selection.DeleteSelection();
            this.Caret.Position.X = x;
            this.Caret.Position.Y = y;
            this.InsertText(text);
            this.Selection.Bounds.FirstRow = y;
            this.Selection.Bounds.FirstColumn = x + text.Length;
            this.Selection.Bounds.LastRow = y;
            this.Selection.Bounds.LastColumn = x + text.Length;
            this.Caret.Position.X = x + text.Length;
            this.Caret.Position.Y = y;
            return true;
        }
        /// <summary>
        /// Toggles a bookmark on/off on the active row.
        /// </summary>
        public void ToggleBookmark()
        {
            this.Document[this.Caret.Position.Y].Bookmarked = !this.Document[this.Caret.Position.Y].Bookmarked;
            this.Redraw();
        }
        /// <summary>
        /// Deletes selected text if possible otherwise deletes forward. (delete key)
        /// </summary>
        public void Delete()
        {
            this.DeleteForward();
            this.Refresh();
        }
        /// <summary>
        /// Selects all text in the active document. (control + a)
        /// </summary>
        public void SelectAll()
        {
            this.Selection.SelectAll();
            this.Redraw();
        }
        /// <summary>
        /// Paste text from clipboard to current caret position. (control + v)
        /// </summary>
        public void Paste()
        {
            this.PasteText();
            this.Refresh();
        }
        /// <summary>
        /// Copies selected text to clipboard. (control + c)
        /// </summary>
        public void Copy()
        {
            this.CopyText();
        }
        /// <summary>
        /// Cuts selected text to clipboard. (control + x)
        /// </summary>
        public void Cut()
        {
            this.CopyText();
            this.Selection.DeleteSelection();
        }
        /// <summary>
        /// Removes the current row
        /// </summary>
        public void RemoveCurrentRow()
        {
            if(this.Caret.CurrentRow != null && this.Document.Count > 1){
                this.Document.Remove(this.Caret.CurrentRow.Index, true);
                this.Document.ResetVisibleRows();
                this.Caret.CropPosition();
                this.Caret.CurrentRow.Text = this.Caret.CurrentRow.Text;
                this.Caret.CurrentRow.Parse(true);
                this.Document.ResetVisibleRows();
                this.ScrollIntoView();
                //this.Refresh ();
            }
        }
        public void CutClear()
        {
            if(this.Selection.IsValid){
                this.Cut();
            } else{
                this.RemoveCurrentRow();
            }
        }
        /// <summary>
        /// Redo last undo action. (control + y)
        /// </summary>
        public void Redo()
        {
            TextPoint p = this.Document.Redo();
            if(p.X != - 1 && p.Y != - 1){
                this.Caret.Position = p;
                this.Selection.ClearSelection();
                this.ScrollIntoView();
            }
        }
        /// <summary>
        /// Undo last edit action. (control + z)
        /// </summary>
        public void Undo()
        {
            TextPoint p = this.Document.Undo();
            if(p.X != - 1 && p.Y != - 1){
                this.Caret.Position = p;
                this.Selection.ClearSelection();
                this.ScrollIntoView();
            }
        }
        /// <summary>
        /// Returns a point where x is the column and y is the row from a given pixel position.
        /// </summary>
        /// <param name="x">X Position in pixels</param>
        /// <param name="y">Y Position in pixels</param>
        /// <returns>Column and Rowindex</returns>
        public TextPoint CharFromPixel(int x, int y)
        {
            return this.Painter.CharFromPixel(x, y);
        }
        public void ShowFind()
        {
            if(this.FindReplaceDialog != null){
                this.FindReplaceDialog.TopLevel = true;
                if(this.TopLevelControl is Form){
                    this.FindReplaceDialog.Owner = (Form)this.TopLevelControl;
                }
                this.FindReplaceDialog.ShowFind();
            }
        }
        public void ShowReplace()
        {
            if(this.FindReplaceDialog != null){
                this.FindReplaceDialog.TopLevel = true;
                if(this.TopLevelControl is Form){
                    this.FindReplaceDialog.Owner = (Form)this.TopLevelControl;
                }
                this.FindReplaceDialog.ShowReplace();
            }
        }
        public void AutoListBeginLoad()
        {
            this.AutoList.BeginLoad();
        }
        public void AutoListEndLoad()
        {
            this.AutoList.EndLoad();
        }
        public void FindNext()
        {
            this.FindReplaceDialog.FindNext();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns true if the control is in overwrite mode.
        /// </summary>
        [Browsable(false)]
        public bool OverWrite
        {
            get { return this._OverWrite; }
        }
        /// <summary>
        /// Returns True if the control contains a selected text.
        /// </summary>
        [Browsable(false)]
        public bool CanCopy
        {
            get { return this.Selection.IsValid; }
        }
        /// <summary>
        /// Returns true if there is any valid text data inside the Clipboard.
        /// </summary>
        [Browsable(false)]
        public bool CanPaste
        {
            get
            {
                string s = "";
                try{
                    IDataObject iData = Clipboard.GetDataObject();
                    if(iData != null){
                        if(iData.GetDataPresent(DataFormats.Text)){
                            // Yes it is, so display it in a text box.
                            s = (String)iData.GetData(DataFormats.Text);
                        }
                    }
                    if(s != null){
                        return true;
                    }
                } catch{}
                return false;
            }
        }
        /// <summary>
        /// Returns true if the undobuffer contains one or more undo actions.
        /// </summary>
        [Browsable(false)]
        public bool CanUndo
        {
            get { return (this.Document.UndoStep > 0); }
        }
        /// <summary>
        /// Returns true if the control can redo the last undo action/s
        /// </summary>
        [Browsable(false)]
        public bool CanRedo
        {
            get { return (this.Document.UndoStep < this.Document.UndoBuffer.Count - 1); }
        }
        /// <summary>
        /// Gets the size (in pixels) of the font to use when rendering the the content.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public float FontSize
        {
            get { return this._SyntaxBox.FontSize; }
        }
        /// <summary>
        /// Gets the indention style to use when inserting new lines.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public IndentStyle Indent
        {
            get { return this._SyntaxBox.Indent; }
        }
        /// <summary>
        /// Gets the SyntaxDocument the control is currently attatched to.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        [Category("Content"), Description("The SyntaxDocument that is attatched to the contro")]
        public SyntaxDocument Document
        {
            get { return this._SyntaxBox.Document; }
        }
        /// <summary>
        /// Gets the delay in MS before the tooltip is displayed when hovering a collapsed section.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public int TooltipDelay
        {
            get { return this._SyntaxBox.TooltipDelay; }
        }
        // ROB: Required to support CollapsedBlockTooltipsEnabled
        public bool CollapsedBlockTooltipsEnabled
        {
            get { return this._SyntaxBox.CollapsedBlockTooltipsEnabled; }
        }
        // END-ROB ----------------------------------------------------------
        /// <summary>
        /// Gets if the control is readonly.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool ReadOnly
        {
            get { return this._SyntaxBox.ReadOnly; }
        }
        /// <summary>
        /// Gets the name of the font to use when rendering the control.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public string FontName
        {
            get { return this._SyntaxBox.FontName; }
        }
        /// <summary>
        /// Gets if the control should render bracket matching.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool BracketMatching
        {
            get { return this._SyntaxBox.BracketMatching; }
        }
        /// <summary>
        /// Gets if the control should render whitespace chars.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool VirtualWhitespace
        {
            get { return this._SyntaxBox.VirtualWhitespace; }
        }
        /// <summary>
        /// Gets the Color of the horizontal separators (a'la visual basic 6).
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color SeparatorColor
        {
            get { return this._SyntaxBox.SeparatorColor; }
        }
        /// <summary>
        /// Gets the text color to use when rendering bracket matching.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color BracketForeColor
        {
            get { return this._SyntaxBox.BracketForeColor; }
        }
        /// <summary>
        /// Gets the back color to use when rendering bracket matching.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color BracketBackColor
        {
            get { return this._SyntaxBox.BracketBackColor; }
        }
        /// <summary>
        /// Gets the back color to use when rendering the selected text.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color SelectionBackColor
        {
            get { return this._SyntaxBox.SelectionBackColor; }
        }
        /// <summary>
        /// Gets the text color to use when rendering the selected text.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color SelectionForeColor
        {
            get { return this._SyntaxBox.SelectionForeColor; }
        }
        /// <summary>
        /// Gets the back color to use when rendering the inactive selected text.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color InactiveSelectionBackColor
        {
            get { return this._SyntaxBox.InactiveSelectionBackColor; }
        }
        /// <summary>
        /// Gets the text color to use when rendering the inactive selected text.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color InactiveSelectionForeColor
        {
            get { return this._SyntaxBox.InactiveSelectionForeColor; }
        }
        /// <summary>
        /// Gets the color of the border between the gutter area and the line number area.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color GutterMarginBorderColor
        {
            get { return this._SyntaxBox.GutterMarginBorderColor; }
        }
        /// <summary>
        /// Gets the color of the border between the line number area and the folding area.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color LineNumberBorderColor
        {
            get { return this._SyntaxBox.LineNumberBorderColor; }
        }
        /// <summary>
        /// Gets the text color to use when rendering breakpoints.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color BreakPointForeColor
        {
            get { return this._SyntaxBox.BreakPointForeColor; }
        }
        /// <summary>
        /// Gets the back color to use when rendering breakpoints.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color BreakPointBackColor
        {
            get { return this._SyntaxBox.BreakPointBackColor; }
        }
        /// <summary>
        /// Gets the text color to use when rendering line numbers.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color LineNumberForeColor
        {
            get { return this._SyntaxBox.LineNumberForeColor; }
        }
        /// <summary>
        /// Gets the back color to use when rendering line number area.
        /// </summary>
        public Color LineNumberBackColor
        {
            get { return this._SyntaxBox.LineNumberBackColor; }
        }
        /// <summary>
        /// Gets the color of the gutter margin.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color GutterMarginColor
        {
            get { return this._SyntaxBox.GutterMarginColor; }
        }
        /// <summary>
        /// Gets or Sets the background Color of the client area.
        /// </summary>
        [Category("Appearance"), Description("The background color of the client area")]
        public new Color BackColor
        {
            get { return this._SyntaxBox.BackColor; }
            set { this._SyntaxBox.BackColor = value; }
        }
        /// <summary>
        /// Gets the back color to use when rendering the active line.
        /// </summary>
        public Color HighLightedLineColor
        {
            get { return this._SyntaxBox.HighLightedLineColor; }
        }
        /// <summary>
        /// Get if the control should highlight the active line.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool HighLightActiveLine
        {
            get { return this._SyntaxBox.HighLightActiveLine; }
        }
        /// <summary>
        /// Get if the control should render whitespace chars.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool ShowWhitespace
        {
            get { return this._SyntaxBox.ShowWhitespace; }
        }
        /// <summary>
        /// Get if the line number margin is visible or not.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool ShowLineNumbers
        {
            get { return this._SyntaxBox.ShowLineNumbers; }
        }
        /// <summary>
        /// Get if the gutter margin is visible or not.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool ShowGutterMargin
        {
            get { return this._SyntaxBox.ShowGutterMargin; }
        }
        /// <summary>
        /// Get the Width of the gutter margin (in pixels)
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public int GutterMarginWidth
        {
            get { return this._SyntaxBox.GutterMarginWidth; }
        }
        /// <summary>
        /// Get the numbers of space chars in a tab.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public int TabSize
        {
            get { return this._SyntaxBox.TabSize; }
        }
        /// <summary>
        /// Get whether or not TabsToSpaces is turned on.
        /// </summary>
        public bool TabsToSpaces
        {
            get { return this._SyntaxBox.TabsToSpaces; }
        }
        /// <summary>
        /// Get if the control should render 'Tab guides'
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool ShowTabGuides
        {
            get { return this._SyntaxBox.ShowTabGuides; }
        }
        /// <summary>
        /// Gets the color to use when rendering whitespace chars.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color WhitespaceColor
        {
            get { return this._SyntaxBox.WhitespaceColor; }
        }
        /// <summary>
        /// Gets the color to use when rendering tab guides.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color TabGuideColor
        {
            get { return this._SyntaxBox.TabGuideColor; }
        }
        /// <summary>
        /// Get the color to use when rendering bracket matching borders.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        /// <remarks>
        /// NOTE: Use the Color.Transparent turn off the bracket match borders.
        /// </remarks>
        public Color BracketBorderColor
        {
            get { return this._SyntaxBox.BracketBorderColor; }
        }
        /// <summary>
        /// Get the color to use when rendering Outline symbols.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color OutlineColor
        {
            get { return this._SyntaxBox.OutlineColor; }
        }
        /// <summary>
        /// Positions the AutoList
        /// </summary>
        [Category("Behavior")]
        public TextPoint AutoListPosition
        {
            get { return this.AutoListStartPos; }
            set { this.AutoListStartPos = value; }
        }
        /// <summary>
        /// Positions the InfoTip
        /// </summary>
        [Category("Behavior")]
        public TextPoint InfoTipPosition
        {
            get { return this.InfoTipStartPos; }
            set { this.InfoTipStartPos = value; }
        }
        /// <summary>
        /// Gets or Sets if the intellisense list is visible.
        /// </summary>
        [Category("Behavior")]
        public bool AutoListVisible
        {
            set
            {
                this.CreateAutoList();
                if(this.AutoList == null){
                    return;
                }
                if(value){
                    this.AutoList.TopLevel = true;
                    this.AutoList.BringToFront();
                    // ROB: Fuck knows what I did to cause having to do this..
                    // Show it off the screen, let the painter position it.
                    this.AutoList.Location = new Point(-16000, -16000);
                    this.AutoList.Show();
                    this.InfoTip.BringToFront();
                    if(this.TopLevelControl is Form){
                        this.AutoList.Owner = (Form)this.TopLevelControl;
                    }
                } else{
                    // ROB: Another hack.
                    this.AutoList.Hide();
                }
                this._AutoListVisible = value;
                this.InfoTip.BringToFront();
                this.Redraw();
            }
            get { return this._AutoListVisible; }
        }
        /// <summary>
        /// Gets or Sets if the infotip is visible
        /// </summary>
        [Category("Behavior")]
        public bool InfoTipVisible
        {
            set
            {
                this.CreateInfoTip();
                if(this.InfoTip == null){
                    return;
                }
                if(value){
                    this.InfoTip.TopLevel = true;
                    this.AutoList.BringToFront();
                    if(this.TopLevelControl is Form){
                        this.InfoTip.Owner = (Form)this.TopLevelControl;
                    }
                }
                this.InfoTip.BringToFront();
                this._InfoTipVisible = value;
                if(this.InfoTip != null && value){
                    this.InfoTip.Init();
                }
                // ROB: Cludge for infotip bug, whereby infotip does not close when made invisible..
                if(this._InfoTip != null && !value){
                    this._InfoTip.Visible = false;
                }
            }
            get { return this._InfoTipVisible; }
        }
        /// <summary>
        /// Get if the control should use smooth scrolling.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool SmoothScroll
        {
            get { return this._SyntaxBox.SmoothScroll; }
        }
        /// <summary>
        /// Get the number of pixels the screen should be scrolled per frame when using smooth scrolling.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public int SmoothScrollSpeed
        {
            get { return this._SyntaxBox.SmoothScrollSpeed; }
        }
        /// <summary>
        /// Get if the control should parse all text when text is pasted from the clipboard.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool ParseOnPaste
        {
            get { return this._SyntaxBox.ParseOnPaste; }
        }
        /// <summary>
        /// Gets the Caret object.
        /// </summary>
        public Caret Caret
        {
            get { return this._Caret; }
        }
        /// <summary>
        /// Gets the Selection object.
        /// </summary>
        public Selection Selection
        {
            get { return this._Selection; }
        }
        #endregion

        #region eventhandlers
        private int OldWidth;
        private void OnClipboardUpdated(CopyEventArgs e)
        {
            if(this.ClipboardUpdated != null){
                this.ClipboardUpdated(this, e);
            }
        }
        private void OnRowMouseDown(RowMouseEventArgs e)
        {
            if(this.RowMouseDown != null){
                this.RowMouseDown(this, e);
            }
        }
        private void OnRowMouseMove(RowMouseEventArgs e)
        {
            if(this.RowMouseMove != null){
                this.RowMouseMove(this, e);
            }
        }
        private void OnRowMouseUp(RowMouseEventArgs e)
        {
            if(this.RowMouseUp != null){
                this.RowMouseUp(this, e);
            }
        }
        private void OnRowClick(RowMouseEventArgs e)
        {
            if(this.RowClick != null){
                this.RowClick(this, e);
            }
        }
        private void OnRowDoubleClick(RowMouseEventArgs e)
        {
            if(this.RowDoubleClick != null){
                this.RowDoubleClick(this, e);
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            this.DoResize();
            this.Refresh();
        }
        public void OnParse()
        {
            this.Redraw();
        }
        public void OnChange()
        {
            if(this.Caret.Position.Y > this.Document.Count - 1){
                this.Caret.Position.Y = this.Document.Count - 1;
                //this.Caret.MoveAbsoluteHome (false);
                this.ScrollIntoView();
            }
            try{
                if(this.VirtualWhitespace == false && this.Caret.CurrentRow != null
                   && this.Caret.Position.X > this.Caret.CurrentRow.Text.Length){
                    this.Caret.Position.X = this.Caret.CurrentRow.Text.Length;
                    this.Redraw();
                }
            } catch{}
            if(!this.ContainsFocus){
                this.Selection.ClearSelection();
            }
            if(this.Selection.LogicalBounds.FirstRow > this.Document.Count){
                this.Selection.Bounds.FirstColumn = this.Caret.Position.X;
                this.Selection.Bounds.LastColumn = this.Caret.Position.X;
                this.Selection.Bounds.FirstRow = this.Caret.Position.Y;
                this.Selection.Bounds.LastRow = this.Caret.Position.Y;
            }
            if(this.Selection.LogicalBounds.LastRow > this.Document.Count){
                this.Selection.Bounds.FirstColumn = this.Caret.Position.X;
                this.Selection.Bounds.LastColumn = this.Caret.Position.X;
                this.Selection.Bounds.FirstRow = this.Caret.Position.Y;
                this.Selection.Bounds.LastRow = this.Caret.Position.Y;
            }
            this.Redraw();
        }
        /// <summary>
        /// Overrides the default OnKeyDown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            this._KeyDownHandled = e.Handled;
            if(e.KeyCode == Keys.Escape && (this.InfoTipVisible || this.AutoListVisible)){
                this.InfoTipVisible = false;
                this.AutoListVisible = false;
                e.Handled = true;
                this.Redraw();
                return;
            }
            if(!e.Handled && this.InfoTipVisible && this.InfoTip.Count > 1){
                //move infotip selection
                if(e.KeyCode == Keys.Up){
                    this._SyntaxBox.InfoTipSelectedIndex++;
                    e.Handled = true;
                    return;
                }
                if(e.KeyCode == Keys.Down){
                    this._SyntaxBox.InfoTipSelectedIndex--;
                    e.Handled = true;
                    return;
                }
            }
            if(!e.Handled && this.AutoListVisible){
                //move autolist selection
                if((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.PageUp
                    || e.KeyCode == Keys.PageDown)){
                    this.AutoList.SendKey((int)e.KeyCode);
                    e.Handled = true;
                    return;
                }
                //inject inser text from autolist
                if(e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab){
                    string s = this.AutoList.SelectedText;
                    if(s != ""){
                        this.InsertAutolistText();
                    }
                    this.AutoListVisible = false;
                    e.Handled = true;
                    this.Redraw();
                    return;
                }
            }
            if(!e.Handled){
                //do keyboard actions
                foreach(KeyboardAction ka in this._SyntaxBox.KeyboardActions){
                    if(!this.ReadOnly || ka.AllowReadOnly){
                        if((ka.Key == (Keys)(int)e.KeyCode) && ka.Alt == e.Alt && ka.Shift == e.Shift
                           && ka.Control == e.Control){
                            ka.Action();
                        }
                        //if keys match , call action delegate
                    }
                }
                //------------------------------------------------------------------------------------------------------------
                switch((Keys)(int)e.KeyCode){
                    case Keys.ShiftKey:
                    case Keys.ControlKey:
                    case Keys.Alt:
                        return;
                    case Keys.Down:
                        if(e.Control){
                            this.ScrollScreen(1);
                        } else{
                            this.Caret.MoveDown(e.Shift);
                            this.Redraw();
                        }
                        break;
                    case Keys.Up:
                        if(e.Control){
                            this.ScrollScreen(- 1);
                        } else{
                            this.Caret.MoveUp(e.Shift);
                        }
                        this.Redraw();
                        break;
                    case Keys.Left:
                    {
                        if(e.Control){
                            this.MoveCaretToPrevWord(e.Shift);
                        } else{
                            this.Caret.MoveLeft(e.Shift);
                        }
                    }
                        this.Redraw();
                        break;
                    case Keys.Right:
                    {
                        if(e.Control){
                            this.MoveCaretToNextWord(e.Shift);
                        } else{
                            this.Caret.MoveRight(e.Shift);
                        }
                    }
                        this.Redraw();
                        break;
                    case Keys.End:
                        if(e.Control){
                            this.Caret.MoveAbsoluteEnd(e.Shift);
                        } else{
                            this.Caret.MoveEnd(e.Shift);
                        }
                        this.Redraw();
                        break;
                    case Keys.Home:
                        if(e.Control){
                            this.Caret.MoveAbsoluteHome(e.Shift);
                        } else{
                            this.Caret.MoveHome(e.Shift);
                        }
                        this.Redraw();
                        break;
                    case Keys.PageDown:
                        this.Caret.MoveDown(this.View.VisibleRowCount - 2, e.Shift);
                        this.Redraw();
                        break;
                    case Keys.PageUp:
                        this.Caret.MoveUp(this.View.VisibleRowCount - 2, e.Shift);
                        this.Redraw();
                        break;
                    default:
                        break;
                }
                //dont do if readonly
                if(!this.ReadOnly){
                    switch((Keys)(int)e.KeyCode){
                        case Keys.Enter:
                        {
                            if(e.Control){
                                if(this.Caret.CurrentRow.CanFold){
                                    this.Caret.MoveHome(false);
                                    this.Document.ToggleRow(this.Caret.CurrentRow);
                                    this.Redraw();
                                }
                            } else{
                                this.InsertEnter();
                            }
                            break;
                        }
                        case Keys.Back:
                            if(!e.Control){
                                this.DeleteBackwards();
                            } else{
                                if(this.Selection.IsValid){
                                    this.Selection.DeleteSelection();
                                } else{
                                    this.Selection.ClearSelection();
                                    this.MoveCaretToPrevWord(true);
                                    this.Selection.DeleteSelection();
                                }
                                this.Caret.CurrentRow.Parse(true);
                            }
                            break;
                        case Keys.Delete:
                        {
                            if(!e.Control && !e.Alt && !e.Shift){
                                this.Delete();
                            } else if(e.Control && !e.Alt && !e.Shift){
                                if(this.Selection.IsValid){
                                    this.Cut();
                                } else{
                                    this.Selection.ClearSelection();
                                    this.MoveCaretToNextWord(true);
                                    this.Selection.DeleteSelection();
                                }
                                this.Caret.CurrentRow.Parse(true);
                            }
                            break;
                        }
                        case Keys.Insert:
                        {
                            if(!e.Control && !e.Alt && !e.Shift){
                                this._OverWrite = !this._OverWrite;
                            }
                            break;
                        }
                        case Keys.Tab:
                        {
                            if(!this.Selection.IsValid){
                                // ROB: Implementation of .TabsToSpaces
                                if(!this.TabsToSpaces){
                                    this.InsertText("\t");
                                } else{
                                    this.InsertText(new string(' ', this.TabSize));
                                }
                                // ROB-END
                            }
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                }
                this.Caret.Blink = true;
                //this.Redraw ();
            }
        }
        /// <summary>
        /// Overrides the default OnKeyPress
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if(!e.Handled && !this._KeyDownHandled && e.KeyChar != (char)127){
                if((e.KeyChar) < 32){
                    return;
                }
                if(!this.ReadOnly){
                    switch((Keys)(int)e.KeyChar){
                        default:
                        {
                            this.InsertText(e.KeyChar.ToString(CultureInfo.InvariantCulture));
                            if(this.Indent == IndentStyle.Scope || this.Indent == IndentStyle.Smart){
                                if(this.Caret.CurrentRow.ShouldOutdent){
                                    this.OutdentEndRow();
                                }
                            }
                            break;
                        }
                    }
                }
            }
            if(this.AutoListVisible && !e.Handled && this._SyntaxBox.AutoListAutoSelect){
                string s = this.Caret.CurrentRow.Text;
                try{
                    if(this.Caret.Position.X - this.AutoListStartPos.X >= 0){
                        s = s.Substring(this.AutoListStartPos.X, this.Caret.Position.X - this.AutoListStartPos.X);
                        this.AutoList.SelectItem(s);
                    }
                } catch{}
            }
        }
        /// <summary>
        /// Overrides the default OnMouseDown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.MouseX = e.X;
            this.MouseY = e.Y;
            this.SetFocus();
            base.OnMouseDown(e);
            TextPoint pos = this.Painter.CharFromPixel(e.X, e.Y);
            Row row = null;
            if(pos.Y >= 0 && pos.Y < this.Document.Count){
                row = this.Document[pos.Y];
            }

            #region RowEvent
            var rea = new RowMouseEventArgs{Row = row, Button = e.Button, MouseX = this.MouseX, MouseY = this.MouseY};
            if(e.X >= this.View.TextMargin - 7){
                rea.Area = RowArea.TextArea;
            } else if(e.X < this.View.GutterMarginWidth){
                rea.Area = RowArea.GutterArea;
            } else if(e.X < this.View.LineNumberMarginWidth + this.View.GutterMarginWidth){
                rea.Area = RowArea.LineNumberArea;
            } else if(e.X < this.View.TextMargin - 7){
                rea.Area = RowArea.FoldingArea;
            }
            this.OnRowMouseDown(rea);
            #endregion

            try{
                Row r2 = this.Document[pos.Y];
                if(r2 != null){
                    if(e.X >= r2.Expansion_PixelEnd && r2.IsCollapsed){
                        if(r2.expansion_StartSpan != null){
                            if(r2.expansion_StartSpan.StartRow != null && r2.expansion_StartSpan.EndRow != null
                               && r2.expansion_StartSpan.Expanded == false){
                                if(!this.IsOverSelection(e.X, e.Y)){
                                    this.Caret.Position.X = pos.X;
                                    this.Caret.Position.Y = pos.Y;
                                    this.Selection.ClearSelection();
                                    Row r3 = r2.Expansion_EndRow;
                                    int x3 = r3.Expansion_StartChar;
                                    this.Caret.Position.X = x3;
                                    this.Caret.Position.Y = r3.Index;
                                    this.Selection.MakeSelection();
                                    this.Redraw();
                                    this.View.Action = EditAction.SelectText;
                                    return;
                                }
                            }
                        }
                    }
                }
            } catch{
                //this is untested code...
            }
            bool shift = NativeMethods.IsKeyPressed(Keys.ShiftKey);
            if(e.X > this.View.TotalMarginWidth){
                if(e.X > this.View.TextMargin - 8){
                    if(!this.IsOverSelection(e.X, e.Y)){
                        //selecting
                        if(e.Button == MouseButtons.Left){
                            if(!shift){
                                TextPoint tp = pos;
                                Word w = this.Document.GetWordFromPos(tp);
                                if(w != null && w.Pattern != null && w.Pattern.Category != null){
                                    var pe = new WordMouseEventArgs
                                             {Pattern = w.Pattern, Button = e.Button, Cursor = Cursors.Hand, Word = w};
                                    this._SyntaxBox.OnWordMouseDown(ref pe);
                                    this.Cursor = pe.Cursor;
                                    return;
                                }
                                this.View.Action = EditAction.SelectText;
                                this.Caret.SetPos(pos);
                                this.Selection.ClearSelection();
                                this.Caret.Blink = true;
                                this.Redraw();
                            } else{
                                this.View.Action = EditAction.SelectText;
                                this.Caret.SetPos(pos);
                                this.Selection.MakeSelection();
                                this.Caret.Blink = true;
                                this.Redraw();
                            }
                        }
                    }
                } else{
                    if(row != null){
                        if(row.expansion_StartSpan != null){
                            this.Caret.SetPos(new TextPoint(0, pos.Y));
                            this.Selection.ClearSelection();
                            this.Document.ToggleRow(row);
                            this.Redraw();
                        }
                    }
                }
            } else{
                if(e.X < this.View.GutterMarginWidth){
                    if(this._SyntaxBox.AllowBreakPoints){
                        Row r = this.Document[this.Painter.CharFromPixel(e.X, e.Y).Y];
                        r.Breakpoint = !r.Breakpoint;
                        this.Redraw();
                    } else{
                        Row r = this.Document[this.Painter.CharFromPixel(e.X, e.Y).Y];
                        r.Breakpoint = false;
                        this.Redraw();
                    }
                } else{
                    this.View.Action = EditAction.SelectText;
                    this.Caret.SetPos(this.Painter.CharFromPixel(0, e.Y));
                    this.Selection.ClearSelection();
                    this.Caret.MoveDown(true);
                    this.Redraw();
                }
            }
            this.SetMouseCursor(e.X, e.Y);
        }
        /// <summary>
        /// Overrides the default OnMouseMove
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.MouseX = e.X;
            this.MouseY = e.Y;
            TextPoint pos = this.Painter.CharFromPixel(e.X, e.Y);
            Row row = null;
            if(pos.Y >= 0 && pos.Y < this.Document.Count){
                row = this.Document[pos.Y];
            }

            #region RowEvent
            var rea = new RowMouseEventArgs{Row = row, Button = e.Button, MouseX = this.MouseX, MouseY = this.MouseY};
            if(e.X >= this.View.TextMargin - 7){
                rea.Area = RowArea.TextArea;
            } else if(e.X < this.View.GutterMarginWidth){
                rea.Area = RowArea.GutterArea;
            } else if(e.X < this.View.LineNumberMarginWidth + this.View.GutterMarginWidth){
                rea.Area = RowArea.LineNumberArea;
            } else if(e.X < this.View.TextMargin - 7){
                rea.Area = RowArea.FoldingArea;
            }
            this.OnRowMouseMove(rea);
            #endregion

            try{
                if(this.Document != null){
                    if(e.Button == MouseButtons.Left){
                        if(this.View.Action == EditAction.SelectText){
                            //Selection ACTIONS!!!!!!!!!!!!!!
                            this.Caret.Blink = true;
                            this.Caret.SetPos(pos);
                            if(e.X <= this.View.TotalMarginWidth){
                                this.Caret.MoveDown(true);
                            }
                            this.Caret.CropPosition();
                            this.Selection.MakeSelection();
                            this.ScrollIntoView();
                            this.Redraw();
                        } else if(this.View.Action == EditAction.None){
                            if(this.IsOverSelection(e.X, e.Y)){
                                this.BeginDragDrop();
                            }
                        } else if(this.View.Action == EditAction.DragText){}
                    } else{
                        TextPoint p = pos;
                        Row r = this.Document[p.Y];
                        bool DidShow = false;
                        if(r != null){
                            if(e.X >= r.Expansion_PixelEnd && r.IsCollapsed){
                                // ROB: Added check for Collapsed tooltips.
                                if(this.CollapsedBlockTooltipsEnabled){
                                    if(r.expansion_StartSpan != null){
                                        if(r.expansion_StartSpan.StartRow != null
                                           && r.expansion_StartSpan.EndRow != null
                                           && r.expansion_StartSpan.Expanded == false){
                                            string t = "";
                                            int j = 0;
                                            for(int i = r.expansion_StartSpan.StartRow.Index;
                                                    i <= r.expansion_StartSpan.EndRow.Index;
                                                    i++){
                                                if(j > 0){
                                                    t += "\n";
                                                }
                                                Row tmp = this.Document[i];
                                                string tmpstr = tmp.Text.Replace("\t", "    ");
                                                t += tmpstr;
                                                if(j > 20){
                                                    t += "...";
                                                    break;
                                                }
                                                j++;
                                            }
                                            //tooltip.res
                                            this.tooltip.InitialDelay = this.TooltipDelay;
                                            if(this.tooltip.GetToolTip(this) != t){
                                                this.tooltip.SetToolTip(this, t);
                                            }
                                            this.tooltip.Active = true;
                                            DidShow = true;
                                        }
                                    }
                                }
                            } else{
                                Word w = this.Document.GetFormatWordFromPos(p);
                                if(w != null){
                                    if(w.InfoTip != null){
                                        this.tooltip.InitialDelay = this.TooltipDelay;
                                        if(this.tooltip.GetToolTip(this) != w.InfoTip){
                                            this.tooltip.SetToolTip(this, w.InfoTip);
                                        }
                                        this.tooltip.Active = true;
                                        DidShow = true;
                                    }
                                }
                            }
                        }
                        if(this.tooltip != null){
                            if(!DidShow){
                                this.tooltip.SetToolTip(this, "");
                            }
                        }
                    }
                    this.SetMouseCursor(e.X, e.Y);
                    base.OnMouseMove(e);
                }
            } catch{}
        }
        /// <summary>
        /// Overrides the default OnMouseUp
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.MouseX = e.X;
            this.MouseY = e.Y;
            TextPoint pos = this.Painter.CharFromPixel(e.X, e.Y);
            Row row = null;
            if(pos.Y >= 0 && pos.Y < this.Document.Count){
                row = this.Document[pos.Y];
            }

            #region RowEvent
            var rea = new RowMouseEventArgs{Row = row, Button = e.Button, MouseX = this.MouseX, MouseY = this.MouseY};
            if(e.X >= this.View.TextMargin - 7){
                rea.Area = RowArea.TextArea;
            } else if(e.X < this.View.GutterMarginWidth){
                rea.Area = RowArea.GutterArea;
            } else if(e.X < this.View.LineNumberMarginWidth + this.View.GutterMarginWidth){
                rea.Area = RowArea.LineNumberArea;
            } else if(e.X < this.View.TextMargin - 7){
                rea.Area = RowArea.FoldingArea;
            }
            this.OnRowMouseUp(rea);
            #endregion

            if(this.View.Action == EditAction.None){
                if(e.X > this.View.TotalMarginWidth){
                    if(this.IsOverSelection(e.X, e.Y) && e.Button == MouseButtons.Left){
                        this.View.Action = EditAction.SelectText;
                        this.Caret.SetPos(this.Painter.CharFromPixel(e.X, e.Y));
                        this.Selection.ClearSelection();
                        this.Redraw();
                    }
                }
            }
            this.View.Action = EditAction.None;
            base.OnMouseUp(e);
        }
        /// <summary>
        /// Overrides the default OnMouseWheel
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int l = SystemInformation.MouseWheelScrollLines;
            this.ScrollScreen(- (e.Delta / 120) * l);
            base.OnMouseWheel(e);
        }
        /// <summary>
        /// Overrides the default OnPaint
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if(this.Document != null && this.Width > 0 && this.Height > 0){
                this.Painter.RenderAll(e.Graphics);
            }
        }
        /// <summary>
        /// Overrides the default OnResize
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.DoResize();
        }
        /// <summary>
        /// Overrides the default OnDragOver
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            if(!this.ReadOnly){
                if(this.Document != null){
                    this.View.Action = EditAction.DragText;
                    Point pt = this.PointToClient(new Point(drgevent.X, drgevent.Y));
                    int x = pt.X;
                    int y = pt.Y;
                    //	drgevent.Effect = DragDropEffects.All  ;
                    //Caret.Position = Painter.CharFromPixel(x,y);
                    drgevent.Effect = (drgevent.KeyState & 8) == 8 ? DragDropEffects.Copy : DragDropEffects.Move;
                    this.Caret.SetPos(this.Painter.CharFromPixel(x, y));
                    this.Redraw();
                }
            } else{
                drgevent.Effect = DragDropEffects.None;
            }
        }
        /// <summary>
        /// Overrides the default OnDragDrop
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if(!this.ReadOnly){
                if(this.Document != null){
                    this.View.Action = EditAction.None;
                    int SelStart = this.Selection.LogicalSelStart;
                    int DropStart = this.Document.PointToIntPos(this.Caret.Position);
                    string s = drgevent.Data.GetData(typeof(string)).ToString();
                    //int SelLen=s.Replace ("\r\n","\n").Length ;
                    int SelLen = s.Length;
                    if(DropStart >= SelStart && DropStart <= SelStart + Math.Abs(this.Selection.SelLength)){
                        DropStart = SelStart;
                    } else if(DropStart >= SelStart + SelLen){
                        DropStart -= SelLen;
                    }
                    this.Document.StartUndoCapture();
                    if((drgevent.KeyState & 8) == 0){
                        this._SyntaxBox.Selection.DeleteSelection();
                        this.Caret.Position = this.Document.IntPosToPoint(DropStart);
                    }
                    this.Document.InsertText(s, this.Caret.Position.X, this.Caret.Position.Y);
                    this.Document.EndUndoCapture();
                    this.Selection.SelStart = this.Document.PointToIntPos(this.Caret.Position);
                    this.Selection.SelLength = SelLen;
                    this.Document.ResetVisibleRows();
                    this.ScrollIntoView();
                    this.Redraw();
                    drgevent.Effect = DragDropEffects.All;
                    if(this.ParseOnPaste){
                        this.Document.ParseAll(true);
                    }
                    this.View.Action = EditAction.None;
                }
            }
        }
        /// <summary>
        ///  Overrides the default OnDragEnter
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragEnter(DragEventArgs drgevent) {}
        /// <summary>
        ///  Overrides the default OnDragLeave
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragLeave(EventArgs e)
        {
            this.View.Action = EditAction.None;
        }
        /// <summary>
        ///  Overrides the default OnDoubleClick
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            TextPoint pos = this.Painter.CharFromPixel(this.MouseX, this.MouseY);
            Row row = null;
            if(pos.Y >= 0 && pos.Y < this.Document.Count){
                row = this.Document[pos.Y];
            }

            #region RowEvent
            var rea = new RowMouseEventArgs
                      {Row = row, Button = MouseButtons.None, MouseX = this.MouseX, MouseY = this.MouseY};
            if(this.MouseX >= this.View.TextMargin - 7){
                rea.Area = RowArea.TextArea;
            } else if(this.MouseX < this.View.GutterMarginWidth){
                rea.Area = RowArea.GutterArea;
            } else if(this.MouseX < this.View.LineNumberMarginWidth + this.View.GutterMarginWidth){
                rea.Area = RowArea.LineNumberArea;
            } else if(this.MouseX < this.View.TextMargin - 7){
                rea.Area = RowArea.FoldingArea;
            }
            this.OnRowDoubleClick(rea);
            #endregion

            try{
                Row r2 = this.Document[pos.Y];
                if(r2 != null){
                    if(this.MouseX >= r2.Expansion_PixelEnd && r2.IsCollapsed){
                        if(r2.expansion_StartSpan != null){
                            if(r2.expansion_StartSpan.StartRow != null && r2.expansion_StartSpan.EndRow != null
                               && r2.expansion_StartSpan.Expanded == false){
                                r2.Expanded = true;
                                this.Document.ResetVisibleRows();
                                this.Redraw();
                                return;
                            }
                        }
                    }
                }
            } catch{
                //this is untested code...
            }
            if(this.MouseX > this.View.TotalMarginWidth){
                this.SelectCurrentWord();
            }
        }
        protected override void OnClick(EventArgs e)
        {
            TextPoint pos = this.Painter.CharFromPixel(this.MouseX, this.MouseY);
            Row row = null;
            if(pos.Y >= 0 && pos.Y < this.Document.Count){
                row = this.Document[pos.Y];
            }

            #region RowEvent
            var rea = new RowMouseEventArgs
                      {Row = row, Button = MouseButtons.None, MouseX = this.MouseX, MouseY = this.MouseY};
            if(this.MouseX >= this.View.TextMargin - 7){
                rea.Area = RowArea.TextArea;
            } else if(this.MouseX < this.View.GutterMarginWidth){
                rea.Area = RowArea.GutterArea;
            } else if(this.MouseX < this.View.LineNumberMarginWidth + this.View.GutterMarginWidth){
                rea.Area = RowArea.LineNumberArea;
            } else if(this.MouseX < this.View.TextMargin - 7){
                rea.Area = RowArea.FoldingArea;
            }
            this.OnRowClick(rea);
            #endregion
        }
        private void vScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            this.SetMaxHorizontalScroll();
            this._InfoTipVisible = false;
            this._AutoListVisible = false;
            this.SetFocus();
            int diff = e.NewValue - this.vScroll.Value;
            if((diff == - 1 || diff == 1)
               && (e.Type == ScrollEventType.SmallDecrement || e.Type == ScrollEventType.SmallIncrement)){
                this.ScrollScreen(diff);
            } else{
                this.Invalidate();
            }
        }
        private void hScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            this._InfoTipVisible = false;
            this._AutoListVisible = false;
            this.SetFocus();
            this.Invalidate();
        }
        private void CaretTimer_Tick(object sender, EventArgs e)
        {
            this.Caret.Blink = !this.Caret.Blink;
            this.RedrawCaret();
        }
        private void AutoListDoubleClick(object sender, EventArgs e)
        {
            string s = this.AutoList.SelectedText;
            if(s != ""){
                this.InsertAutolistText();
            }
            this.AutoListVisible = false;
            this.Redraw();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if(this.tooltip != null){
                this.tooltip.RemoveAll();
            }
        }
        private void CaretChanged(object s, EventArgs e)
        {
            this.OnCaretChange();
        }
        private void EditViewControl_Leave(object sender, EventArgs e)
        {
            this.RemoveFocus();
        }
        private void EditViewControl_Enter(object sender, EventArgs e)
        {
            this.CaretTimer.Enabled = true;
        }
        private void SelectionChanged(object s, EventArgs e)
        {
            this.OnSelectionChange();
        }
        private void OnCaretChange()
        {
            if(this.CaretChange != null){
                this.CaretChange(this, null);
            }
        }
        private void OnSelectionChange()
        {
            if(this.SelectionChange != null){
                this.SelectionChange(this, null);
            }
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            if(this.Visible == false){
                this.RemoveFocus();
            }
            base.OnVisibleChanged(e);
            this.DoResize();
        }
        #endregion
    }
}