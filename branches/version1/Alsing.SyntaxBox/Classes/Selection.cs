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
using Alsing.SourceCode;

namespace Alsing.Windows.Forms.SyntaxBox
{
    /// <summary>
    /// Selection class used by the SyntaxBoxControl
    /// </summary>
    public class Selection
    {
        /// <summary>
        /// Event fired when the selection has changed.
        /// </summary>
        public event EventHandler Change = null;
        private void PositionChange(object s, EventArgs e)
        {
            this.OnChange();
        }
        private void OnChange()
        {
            if(this.Change != null){
                this.Change(this, null);
            }
        }

        #region Instance constructors
        /// <summary>
        /// Selection Constructor.
        /// </summary>
        /// <param name="control">Control that will use this selection</param>
        public Selection(EditViewControl control)
        {
            this.Control = control;
            this.Bounds = new TextRange();
        }
        #endregion Instance constructors

        #region Public instance properties
        /// <summary>
        /// Gets the text of the active selection
        /// </summary>
        public String Text
        {
            get
            {
                if(!this.IsValid){
                    return "";
                } else{
                    return this.Control.Document.GetRange(this.LogicalBounds);
                }
            }
            set
            {
                if(this.Text == value){
                    return;
                }
                //selection text bug fix 
                //
                //selection gets too short if \n is used instead of newline
                string tmp = value.Replace(Environment.NewLine, "\n");
                tmp = tmp.Replace("\n", Environment.NewLine);
                value = tmp;
                //---
                TextPoint oCaretPos = this.Control.Caret.Position;
                int nCaretX = oCaretPos.X;
                int nCaretY = oCaretPos.Y;
                this.Control.Document.StartUndoCapture();
                this.DeleteSelection();
                this.Control.Document.InsertText(value, oCaretPos.X, oCaretPos.Y);
                this.SelLength = value.Length;
                if(nCaretX != oCaretPos.X || nCaretY != oCaretPos.Y){
                    this.Control.Caret.Position = new TextPoint(this.Bounds.LastColumn, this.Bounds.LastRow);
                }
                this.Control.Document.EndUndoCapture();
                this.Control.Document.InvokeChange();
            }
        }
        /// <summary>
        /// Returns the normalized positions of the selection.
        /// Swapping start and end values if the selection is reversed.
        /// </summary>
        public TextRange LogicalBounds
        {
            get
            {
                var r = new TextRange();
                if(this.Bounds.FirstRow < this.Bounds.LastRow){
                    return this.Bounds;
                } else if(this.Bounds.FirstRow == this.Bounds.LastRow && this.Bounds.FirstColumn < this.Bounds.LastColumn){
                    return this.Bounds;
                } else{
                    r.FirstColumn = this.Bounds.LastColumn;
                    r.FirstRow = this.Bounds.LastRow;
                    r.LastColumn = this.Bounds.FirstColumn;
                    r.LastRow = this.Bounds.FirstRow;
                    return r;
                }
            }
        }
        /// <summary>
        /// Returns true if the selection contains One or more chars
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (this.LogicalBounds.FirstColumn != this.LogicalBounds.LastColumn
                        || this.LogicalBounds.FirstRow != this.LogicalBounds.LastRow);
            }
        }
        /// <summary>
        /// gets or sets the length of the selection in chars
        /// </summary>
        public int SelLength
        {
            get
            {
                var p1 = new TextPoint(this.Bounds.FirstColumn, this.Bounds.FirstRow);
                var p2 = new TextPoint(this.Bounds.LastColumn, this.Bounds.LastRow);
                int i1 = this.Control.Document.PointToIntPos(p1);
                int i2 = this.Control.Document.PointToIntPos(p2);
                return i2 - i1;
            }
            set { this.SelEnd = this.SelStart + value; }
        }
        /// <summary>
        /// Gets or Sets the Selection end as an index in the document text.
        /// </summary>
        public int SelEnd
        {
            get
            {
                var p = new TextPoint(this.Bounds.LastColumn, this.Bounds.LastRow);
                return this.Control.Document.PointToIntPos(p);
            }
            set
            {
                TextPoint p = this.Control.Document.IntPosToPoint(value);
                this.Bounds.LastColumn = p.X;
                this.Bounds.LastRow = p.Y;
            }
        }
        /// <summary>
        /// Gets or Sets the Selection start as an index in the document text.
        /// </summary>
        public int SelStart
        {
            get
            {
                var p = new TextPoint(this.Bounds.FirstColumn, this.Bounds.FirstRow);
                return this.Control.Document.PointToIntPos(p);
            }
            set
            {
                TextPoint p = this.Control.Document.IntPosToPoint(value);
                this.Bounds.FirstColumn = p.X;
                this.Bounds.FirstRow = p.Y;
            }
        }
        /// <summary>
        /// Gets or Sets the logical Selection start as an index in the document text.
        /// </summary>
        public int LogicalSelStart
        {
            get
            {
                var p = new TextPoint(this.LogicalBounds.FirstColumn, this.LogicalBounds.FirstRow);
                return this.Control.Document.PointToIntPos(p);
            }
            set
            {
                TextPoint p = this.Control.Document.IntPosToPoint(value);
                this.Bounds.FirstColumn = p.X;
                this.Bounds.FirstRow = p.Y;
            }
        }
        #endregion Public instance properties

        #region Public instance methods
        /// <summary>
        /// Indent the active selection one step.
        /// </summary>
        public void Indent()
        {
            if(!this.IsValid){
                return;
            }
            Row xtr = null;
            var ActionGroup = new UndoBlockCollection();
            for(int i = this.LogicalBounds.FirstRow; i <= this.LogicalBounds.LastRow; i++){
                xtr = this.Control.Document[i];
                xtr.Text = "\t" + xtr.Text;
                var b = new UndoBlock();
                b.Action = UndoAction.InsertRange;
                b.Text = "\t";
                b.Position.X = 0;
                b.Position.Y = i;
                ActionGroup.Add(b);
            }
            if(ActionGroup.Count > 0){
                this.Control.Document.AddToUndoList(ActionGroup);
            }
            this.Bounds = this.LogicalBounds;
            this.Bounds.FirstColumn = 0;
            this.Bounds.LastColumn = xtr.Text.Length;
            this.Control.Caret.Position.X = this.LogicalBounds.LastColumn;
            this.Control.Caret.Position.Y = this.LogicalBounds.LastRow;
        }
        /// <summary>
        /// Outdent the active selection one step
        /// </summary>
        public void Outdent()
        {
            if(!this.IsValid){
                return;
            }
            Row xtr = null;
            var ActionGroup = new UndoBlockCollection();
            for(int i = this.LogicalBounds.FirstRow; i <= this.LogicalBounds.LastRow; i++){
                xtr = this.Control.Document[i];
                var b = new UndoBlock();
                b.Action = UndoAction.DeleteRange;
                b.Position.X = 0;
                b.Position.Y = i;
                ActionGroup.Add(b);
                string s = xtr.Text;
                if(s.StartsWith("\t")){
                    b.Text = s.Substring(0, 1);
                    s = s.Substring(1);
                }
                if(s.StartsWith("    ")){
                    b.Text = s.Substring(0, 4);
                    s = s.Substring(4);
                }
                xtr.Text = s;
            }
            if(ActionGroup.Count > 0){
                this.Control.Document.AddToUndoList(ActionGroup);
            }
            this.Bounds = this.LogicalBounds;
            this.Bounds.FirstColumn = 0;
            this.Bounds.LastColumn = xtr.Text.Length;
            this.Control.Caret.Position.X = this.LogicalBounds.LastColumn;
            this.Control.Caret.Position.Y = this.LogicalBounds.LastRow;
        }
        public void Indent(string Pattern)
        {
            if(!this.IsValid){
                return;
            }
            Row xtr = null;
            var ActionGroup = new UndoBlockCollection();
            for(int i = this.LogicalBounds.FirstRow; i <= this.LogicalBounds.LastRow; i++){
                xtr = this.Control.Document[i];
                xtr.Text = Pattern + xtr.Text;
                var b = new UndoBlock();
                b.Action = UndoAction.InsertRange;
                b.Text = Pattern;
                b.Position.X = 0;
                b.Position.Y = i;
                ActionGroup.Add(b);
            }
            if(ActionGroup.Count > 0){
                this.Control.Document.AddToUndoList(ActionGroup);
            }
            this.Bounds = this.LogicalBounds;
            this.Bounds.FirstColumn = 0;
            this.Bounds.LastColumn = xtr.Text.Length;
            this.Control.Caret.Position.X = this.LogicalBounds.LastColumn;
            this.Control.Caret.Position.Y = this.LogicalBounds.LastRow;
        }
        /// <summary>
        /// Outdent the active selection one step
        /// </summary>
        public void Outdent(string Pattern)
        {
            if(!this.IsValid){
                return;
            }
            Row xtr = null;
            var ActionGroup = new UndoBlockCollection();
            for(int i = this.LogicalBounds.FirstRow; i <= this.LogicalBounds.LastRow; i++){
                xtr = this.Control.Document[i];
                var b = new UndoBlock();
                b.Action = UndoAction.DeleteRange;
                b.Position.X = 0;
                b.Position.Y = i;
                ActionGroup.Add(b);
                string s = xtr.Text;
                if(s.StartsWith(Pattern)){
                    b.Text = s.Substring(0, Pattern.Length);
                    s = s.Substring(Pattern.Length);
                }
                xtr.Text = s;
            }
            if(ActionGroup.Count > 0){
                this.Control.Document.AddToUndoList(ActionGroup);
            }
            this.Bounds = this.LogicalBounds;
            this.Bounds.FirstColumn = 0;
            this.Bounds.LastColumn = xtr.Text.Length;
            this.Control.Caret.Position.X = this.LogicalBounds.LastColumn;
            this.Control.Caret.Position.Y = this.LogicalBounds.LastRow;
        }
        /// <summary>
        /// Delete the active selection.
        /// <seealso cref="ClearSelection"/>
        /// </summary>
        public void DeleteSelection()
        {
            TextRange r = this.LogicalBounds;
            int x = r.FirstColumn;
            int y = r.FirstRow;
            this.Control.Document.DeleteRange(r);
            this.Control.Caret.Position.X = x;
            this.Control.Caret.Position.Y = y;
            this.ClearSelection();
            this.Control.ScrollIntoView();
        }
        /// <summary>
        /// Clear the active selection
        /// <seealso cref="DeleteSelection"/>
        /// </summary>
        public void ClearSelection()
        {
            this.Bounds.FirstColumn = this.Control.Caret.Position.X;
            this.Bounds.FirstRow = this.Control.Caret.Position.Y;
            this.Bounds.LastColumn = this.Control.Caret.Position.X;
            this.Bounds.LastRow = this.Control.Caret.Position.Y;
        }
        /// <summary>
        /// Make a selection from the current selection start to the position of the caret
        /// </summary>
        public void MakeSelection()
        {
            this.Bounds.LastColumn = this.Control.Caret.Position.X;
            this.Bounds.LastRow = this.Control.Caret.Position.Y;
        }
        /// <summary>
        /// Select all text.
        /// </summary>
        public void SelectAll()
        {
            this.Bounds.FirstColumn = 0;
            this.Bounds.FirstRow = 0;
            this.Bounds.LastColumn = this.Control.Document[this.Control.Document.Count - 1].Text.Length;
            this.Bounds.LastRow = this.Control.Document.Count - 1;
            this.Control.Caret.Position.X = this.Bounds.LastColumn;
            this.Control.Caret.Position.Y = this.Bounds.LastRow;
            this.Control.ScrollIntoView();
        }
        #endregion Public instance methods

        #region Public instance fields
        /// <summary>
        /// The bounds of the selection
        /// </summary>
        /// 
        private TextRange _Bounds;
        public TextRange Bounds
        {
            get { return this._Bounds; }
            set
            {
                if(this._Bounds != null){
                    this._Bounds.Change -= this.Bounds_Change;
                }
                this._Bounds = value;
                this._Bounds.Change += this.Bounds_Change;
                this.OnChange();
            }
        }
        private void Bounds_Change(object s, EventArgs e)
        {
            this.OnChange();
        }
        #endregion Public instance fields

        #region Protected instance fields
        private readonly EditViewControl Control;
        #endregion Protected instance fields
    }
}