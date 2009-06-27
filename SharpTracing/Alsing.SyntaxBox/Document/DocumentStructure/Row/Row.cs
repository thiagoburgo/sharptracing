// *
// * Copyright (C) 2008 Roger Alsing : http://www.RogerAlsing.com
// *
// * This library is free software; you can redistribute it and/or modify it
// * under the terms of the GNU Lesser General Public License 2.1 or later, as
// * published by the Free Software Foundation. See the included license.txt
// * or http://www.gnu.org/copyleft/lesser.html for details.
// *
// *
// * contributions by Sebastian Faltoni
using System.Collections;
using System.Drawing;

namespace Alsing.SourceCode
{
    /// <summary>
    /// Parser state of a row
    /// </summary>
    public enum RowState
    {
        /// <summary>
        /// the row is not parsed
        /// </summary>
        NotParsed = 0,
        /// <summary>
        /// the row is span parsed
        /// </summary>
        SpanParsed = 1,
        /// <summary>
        /// the row is both span and keyword parsed
        /// </summary>
        AllParsed = 2
    }

    public enum RowRevisionMark
    {
        Unchanged,
        BeforeSave,
        AfterSave
    }

    /// <summary>
    /// The row class represents a row in a SyntaxDocument
    /// </summary>
    public sealed class Row : IEnumerable
    {
        #region General Declarations
        private RowRevisionMark _RevisionMark = RowRevisionMark.BeforeSave;
        private RowState _RowState = RowState.NotParsed;
        /// <summary>
        /// The owner document
        /// </summary>
        public SyntaxDocument Document;
        /// <summary>
        /// The first span that terminates on this row.
        /// </summary>
        public Span endSpan;
        /// <summary>
        /// Segments that ends in this row
        /// </summary>
        public SpanList endSpans = new SpanList();
        /// <summary>
        /// For public use only
        /// </summary>
        public int Expansion_EndChar;
        /// <summary>
        /// 
        /// </summary>
        public Span expansion_EndSpan;
        /// <summary>
        /// For public use only
        /// </summary>
        public int Expansion_PixelEnd;
        /// <summary>
        /// For public use only
        /// </summary>
        public int Expansion_PixelStart;
        /// <summary>
        /// For public use only
        /// </summary>
        public int Expansion_StartChar;
        /// <summary>
        /// 
        /// </summary>
        public Span expansion_StartSpan;
        public WordList FormattedWords = new WordList();
        /// <summary>
        /// Collection of Image indices assigned to a row.
        /// </summary>
        /// <example>
        /// <b>Add an image to the current row.</b>
        /// <code>
        /// MySyntaxBox.Caret.CurrentRow.Images.Add(3);
        /// </code>
        /// </example>
        public ImageIndexList Images = new ImageIndexList();
        /// <summary>
        /// For public use only
        /// </summary>
        public int Indent; //value indicating how much this line should be indented (c style)
        /// <summary>
        /// Returns true if the row is in the owner documents keyword parse queue
        /// </summary>
        public bool InKeywordQueue; //is this line in the parseQueue?
        /// <summary>
        /// Returns true if the row is in the owner documents parse queue
        /// </summary>
        public bool InQueue; //is this line in the parseQueue?
        private bool mBookmarked; //is this line bookmarked?
        private bool mBreakpoint; //Does this line have a breakpoint?
        private string mText = "";
        /// <summary>
        /// The first collapsable span on this row.
        /// </summary>
        public Span startSpan;
        /// <summary>
        /// Segments that start on this row
        /// </summary>
        public SpanList startSpans = new SpanList();
        /// <summary>
        /// Object tag for storage of custom user data..
        /// </summary>
        /// <example>
        /// <b>Assign custom data to a row</b>
        /// <code>
        /// //custom data class
        /// class CustomData{
        ///		public int		abc=123;
        ///		publci string	def="abc";
        /// }
        /// 
        /// ...
        /// 
        /// //assign custom data to a row
        /// Row MyRow=MySyntaxBox.Caret.CurrentRow;
        /// CustomData MyData=new CustomData();
        /// MyData.abc=1337;
        /// MyRow.Tag=MyData;
        /// 
        /// ...
        /// 
        /// //read custom data from a row
        /// Row MyRow=MySyntaxBox.Caret.CurrentRow;
        /// if (MyRow.Tag != null){
        ///		CustomData MyData=(CustomData)MyRow.Tag;
        ///		if (MyData.abc==1337){
        ///			//Do something...
        ///		}
        /// }
        /// 
        /// 
        /// </code>
        /// </example>
        public object Tag;
        internal WordList words = new WordList();

        #region PUBLIC PROPERTY BACKCOLOR
        private Color _BackColor = Color.Transparent;
        public Color BackColor
        {
            get { return this._BackColor; }
            set { this._BackColor = value; }
        }
        #endregion

        public int Depth
        {
            get
            {
                int i = 0;
                Span s = this.startSpan;
                while(s != null){
                    if(s.Scope != null && s.Scope.CauseIndent){
                        i++;
                    }
                    s = s.Parent;
                }
                //				if (i>0)
                //					i--;
                if(this.ShouldOutdent){
                    i--;
                }
                return i;
            }
        }
        public bool ShouldOutdent
        {
            get
            {
                if(this.startSpan.EndRow == this){
                    if(this.startSpan.Scope.CauseIndent){
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// The parse state of this row
        /// </summary>
        /// <example>
        /// <b>Test if the current row is fully parsed.</b>
        /// <code>
        /// if (MySyntaxBox.Caret.CurrentRow.RowState==RowState.AllParsed)
        /// {
        ///		//do something
        /// }
        /// </code>
        /// </example>
        public RowState RowState
        {
            get { return this._RowState; }
            set
            {
                if(value == this._RowState){
                    return;
                }
                if(value == RowState.SpanParsed && !this.InKeywordQueue){
                    this.Document.KeywordQueue.Add(this);
                    this.InKeywordQueue = true;
                }
                if((value == RowState.AllParsed || value == RowState.NotParsed) && this.InKeywordQueue){
                    this.Document.KeywordQueue.Remove(this);
                    this.InKeywordQueue = false;
                }
                this._RowState = value;
            }
        }
        public RowRevisionMark RevisionMark
        {
            get { return this._RevisionMark; }
            set { this._RevisionMark = value; }
        }
        #endregion

        /// <summary>
        /// Gets or Sets if this row has a bookmark or not.
        /// </summary>
        public bool Bookmarked
        {
            get { return this.mBookmarked; }
            set
            {
                this.mBookmarked = value;
                if(value){
                    this.Document.InvokeBookmarkAdded(this);
                } else{
                    this.Document.InvokeBookmarkRemoved(this);
                }
                this.Document.InvokeChange();
            }
        }
        /// <summary>
        /// Gets or Sets if this row has a breakpoint or not.
        /// </summary>
        public bool Breakpoint
        {
            get { return this.mBreakpoint; }
            set
            {
                this.mBreakpoint = value;
                if(value){
                    this.Document.InvokeBreakPointAdded(this);
                } else{
                    this.Document.InvokeBreakPointRemoved(this);
                }
                this.Document.InvokeChange();
            }
        }
        /// <summary>
        /// Returns the number of words in the row.
        /// (this only applied if the row is fully parsed)
        /// </summary>
        public int Count
        {
            get { return this.words.Count; }
        }
        /// <summary>
        /// Gets or Sets the text of the row.
        /// </summary>
        public string Text
        {
            get { return this.mText; }
            set
            {
                bool ParsePreview = false;
                if(this.mText != value){
                    ParsePreview = true;
                    this.Document.Modified = true;
                    this.RevisionMark = RowRevisionMark.BeforeSave;
                }
                this.mText = value;
                if(this.Document != null){
                    if(ParsePreview){
                        this.Document.Parser.ParsePreviewLine(this.Document.IndexOf(this));
                        this.Document.OnApplyFormatRanges(this);
                    }
                    this.AddToParseQueue();
                }
            }
        }
        /// <summary>
        /// Return the Word object at the specified index.
        /// </summary>
        public Word this[int index]
        {
            get
            {
                if(index >= 0){
                    return this.words[index];
                }
                return new Word();
            }
        }
        public int StartWordIndex
        {
            get
            {
                if(this.expansion_StartSpan == null){
                    return 0;
                }
                //				if (this.expansion_StartSpan.StartRow != this)
                //					return 0;
                Word w = this.expansion_StartSpan.StartWord;
                int i = 0;
                foreach(Word wo in this){
                    if(wo == w){
                        break;
                    }
                    i += wo.Text.Length;
                }
                return i;
            }
        }
        public Word FirstNonWsWord
        {
            get
            {
                foreach(Word w in this){
                    if(w.Type == WordType.Word){
                        return w;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Returns the index of this row in the owner SyntaxDocument.
        /// </summary>
        public int Index
        {
            get { return this.Document.IndexOf(this); }
        }
        /// <summary>
        /// Returns the visible index of this row in the owner SyntaxDocument
        /// </summary>
        public int VisibleIndex
        {
            get
            {
                int i = this.Document.VisibleRows.IndexOf(this);
                if(i == -1){
                    if(this.startSpan != null && this.startSpan.StartRow != null && this.startSpan.StartRow != this){
                        return this.startSpan.StartRow.VisibleIndex;
                    }
                    return this.Index;
                }
                return this.Document.VisibleRows.IndexOf(this);
            }
        }
        /// <summary>
        /// Returns the next visible row.
        /// </summary>
        public Row NextVisibleRow
        {
            get
            {
                int i = this.VisibleIndex;
                if(i > this.Document.VisibleRows.Count){
                    return null;
                }
                if(i + 1 < this.Document.VisibleRows.Count){
                    return this.Document.VisibleRows[i + 1];
                }
                return null;
            }
        }
        /// <summary>
        /// Returns the next row
        /// </summary>
        public Row NextRow
        {
            get
            {
                int i = this.Index;
                if(i + 1 <= this.Document.Lines.Length - 1){
                    return this.Document[i + 1];
                }
                return null;
            }
        }
        /// <summary>
        /// Returns the first visible row before this row.
        /// </summary>
        public Row PrevVisibleRow
        {
            get
            {
                int i = this.VisibleIndex;
                if(i < 0){
                    return null;
                }
                if(i - 1 >= 0){
                    return this.Document.VisibleRows[i - 1];
                }
                return null;
            }
        }
        /// <summary>
        /// Returns true if the row is collapsed
        /// </summary>
        public bool IsCollapsed
        {
            get
            {
                if(this.expansion_StartSpan != null){
                    if(this.expansion_StartSpan.Expanded == false){
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// Returns true if this row is the last part of a collepsed span
        /// </summary>
        public bool IsCollapsedEndPart
        {
            get
            {
                if(this.expansion_EndSpan != null){
                    if(this.expansion_EndSpan.Expanded == false){
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// Returns true if this row can fold
        /// </summary>
        public bool CanFold
        {
            get
            {
                return (this.expansion_StartSpan != null && this.expansion_StartSpan.EndRow != null
                        && this.Document.IndexOf(this.expansion_StartSpan.EndRow) != 0);
            }
        }
        /// <summary>
        /// Gets or Sets if this row is expanded.
        /// </summary>
        public bool Expanded
        {
            get
            {
                if(this.CanFold){
                    return (this.expansion_StartSpan.Expanded);
                }
                return false;
            }
            set
            {
                if(this.CanFold){
                    this.expansion_StartSpan.Expanded = value;
                }
            }
        }
        public string ExpansionText
        {
            get { return this.expansion_StartSpan.Scope.ExpansionText; }
            set
            {
                Scope oScope = this.expansion_StartSpan.Scope;
                var oNewScope = new Scope{
                                                 CaseSensitive = oScope.CaseSensitive,
                                                 CauseIndent = oScope.CauseIndent,
                                                 DefaultExpanded = oScope.DefaultExpanded,
                                                 EndPatterns = oScope.EndPatterns,
                                                 NormalizeCase = oScope.NormalizeCase,
                                                 Parent = oScope.Parent,
                                                 spawnSpanOnEnd = oScope.spawnSpanOnEnd,
                                                 spawnSpanOnStart = oScope.spawnSpanOnStart,
                                                 Start = oScope.Start,
                                                 Style = oScope.Style,
                                                 ExpansionText = value
                                         };
                this.expansion_StartSpan.Scope = oNewScope;
                this.Document.InvokeChange();
            }
        }
        /// <summary>
        /// Returns true if this row is the end part of a collapsable span
        /// </summary>
        public bool CanFoldEndPart
        {
            get { return (this.expansion_EndSpan != null); }
        }
        /// <summary>
        /// For public use only
        /// </summary>
        public bool HasExpansionLine
        {
            get { return (this.endSpan.Parent != null); }
        }
        /// <summary>
        /// Returns the last row of a collapsable span
        /// (this only applies if this row is the start row of the span)
        /// </summary>
        public Row Expansion_EndRow
        {
            get
            {
                if(this.CanFold){
                    return this.expansion_StartSpan.EndRow;
                }
                return this;
            }
        }
        /// <summary>
        /// Returns the first row of a collapsable span
        /// (this only applies if this row is the last row of the span)
        /// </summary>
        public Row Expansion_StartRow
        {
            get
            {
                if(this.CanFoldEndPart){
                    return this.expansion_EndSpan.StartRow;
                }
                return this;
            }
        }
        /// <summary>
        /// For public use only
        /// </summary>
        public Row VirtualCollapsedRow
        {
            get
            {
                var r = new Row();
                foreach(Word w in this){
                    if(this.expansion_StartSpan == w.Span){
                        break;
                    }
                    r.Add(w);
                }
                Word wo = r.Add(this.CollapsedText);
                wo.Style = new TextStyle{BackColor = Color.Silver, ForeColor = Color.DarkBlue, Bold = true};
                bool found = false;
                if(this.Expansion_EndRow != null){
                    foreach(Word w in this.Expansion_EndRow){
                        if(found){
                            r.Add(w);
                        }
                        if(w == this.Expansion_EndRow.expansion_EndSpan.EndWord){
                            found = true;
                        }
                    }
                }
                return r;
            }
        }
        /// <summary>
        /// Returns the text that should be displayed if the row is collapsed.
        /// </summary>
        public string CollapsedText
        {
            get
            {
                string str = "";
                int pos = 0;
                foreach(Word w in this){
                    pos += w.Text.Length;
                    if(w.Span == this.expansion_StartSpan){
                        str = this.Text.Substring(pos).Trim();
                        break;
                    }
                }
                if(this.expansion_StartSpan.Scope.ExpansionText != ""){
                    str = this.expansion_StartSpan.Scope.ExpansionText.Replace("***", str);
                }
                return str;
            }
        }
        /// <summary>
        /// Returns the row before this row.
        /// </summary>
        public Row PrevRow
        {
            get
            {
                int i = this.Index;
                if(i - 1 >= 0){
                    return this.Document[i - 1];
                }
                return null;
            }
        }

        #region IEnumerable Members
        /// <summary>
        /// Get the Word enumerator for this row
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return this.words.GetEnumerator();
        }
        #endregion

        public void Clear()
        {
            this.words.Clear();
        }
        /// <summary>
        /// If the row is hidden inside a collapsed span , call this method to make the collapsed segments expanded.
        /// </summary>
        public void EnsureVisible()
        {
            if(this.RowState == RowState.NotParsed){
                return;
            }
            Span seg = this.startSpan;
            while(seg != null){
                seg.Expanded = true;
                seg = seg.Parent;
            }
            this.Document.ResetVisibleRows();
        }
        public Word Add(string text)
        {
            var xw = new Word{Row = this, Text = text};
            this.words.Add(xw);
            return xw;
        }
        /// <summary>
        /// Adds this row to the parse queue
        /// </summary>
        public void AddToParseQueue()
        {
            if(!this.InQueue){
                this.Document.ParseQueue.Add(this);
            }
            this.InQueue = true;
            this.RowState = RowState.NotParsed;
        }
        /// <summary>
        /// Assigns a new text to the row.
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string Text)
        {
            this.Document.StartUndoCapture();
            TextPoint tp = new TextPoint(0, this.Index);
            TextRange tr = new TextRange();
            tr.FirstColumn = 0;
            tr.FirstRow = tp.Y;
            tr.LastColumn = this.Text.Length;
            tr.LastRow = tp.Y;
            this.Document.StartUndoCapture();
            //delete the current line
            this.Document.PushUndoBlock(UndoAction.DeleteRange, this.Document.GetRange(tr), tr.FirstColumn, tr.FirstRow,
                                        this.RevisionMark);
            //alter the text
            this.Document.PushUndoBlock(UndoAction.InsertRange, Text, tp.X, tp.Y, this.RevisionMark);
            this.Text = Text;
            this.Document.EndUndoCapture();
            this.Document.InvokeChange();
        }
        /// <summary>
        /// Call this method to make all words match the case of their patterns.
        /// (this only applies if the row is fully parsed)
        /// </summary>
        public void MatchCase()
        {
            string s = "";
            foreach(Word w in this.words){
                s = s + w.Text;
            }
            this.mText = s;
        }
        /// <summary>
        /// Force a span parse on the row.
        /// </summary>
        public void Parse()
        {
            this.Document.ParseRow(this);
        }
        /// <summary>
        /// Forces the parser to parse this row directly
        /// </summary>
        /// <param name="ParseKeywords">true if keywords and operators should be parsed</param>
        public void Parse(bool ParseKeywords)
        {
            this.Document.ParseRow(this, ParseKeywords);
        }
        public void SetExpansionSegment()
        {
            this.expansion_StartSpan = null;
            this.expansion_EndSpan = null;
            foreach(Span s in this.startSpans){
                if(!this.endSpans.Contains(s)){
                    this.expansion_StartSpan = s;
                    break;
                }
            }
            foreach(Span s in this.endSpans){
                if(!this.startSpans.Contains(s)){
                    this.expansion_EndSpan = s;
                    break;
                }
            }
            if(this.expansion_EndSpan != null){
                this.expansion_StartSpan = null;
            }
        }
        /// <summary>
        /// Returns the whitespace string at the begining of this row.
        /// </summary>
        /// <returns>a string containing the whitespace at the begining of this row</returns>
        public string GetLeadingWhitespace()
        {
            string s = this.mText;
            int i;
            s = s.Replace("	", " ");
            for(i = 0; i < s.Length; i++){
                if(s.Substring(i, 1) == " "){} else{
                    break;
                }
            }
            return this.mText.Substring(0, i);
        }
        public string GetVirtualLeadingWhitespace()
        {
            int i = this.StartWordIndex;
            string ws = "";
            foreach(char c in this.Text){
                if(c == '\t'){
                    ws += c;
                } else{
                    ws += ' ';
                }
                i--;
                if(i <= 0){
                    break;
                }
            }
            return ws;
        }
        /// <summary>
        /// Adds a word object to this row
        /// </summary>
        /// <param name="word">Word object</param>
        public void Add(Word word)
        {
            word.Row = this;
            this.words.Add(word);
        }
        /// <summary>
        /// Returns the index of a specific Word object
        /// </summary>
        /// <param name="word">Word object to find</param>
        /// <returns>index of the word in the row</returns>
        public int IndexOf(Word word)
        {
            return this.words.IndexOf(word);
        }
        /// <summary>
        /// For public use only
        /// </summary>
        /// <param name="PatternList"></param>
        /// <param name="StartWord"></param>
        /// <param name="IgnoreStartWord"></param>
        /// <returns></returns>
        public Word FindRightWordByPatternList(PatternList PatternList, Word StartWord, bool IgnoreStartWord)
        {
            int i = StartWord.Index;
            if(IgnoreStartWord){
                i++;
            }
            while(i < this.words.Count){
                Word w = this[i];
                if(w.Pattern != null){
                    if(w.Pattern.Parent != null){
                        if(w.Pattern.Parent == PatternList && w.Type != WordType.Space && w.Type != WordType.Tab){
                            return w;
                        }
                    }
                }
                i++;
            }
            return null;
        }
        /// <summary>
        /// For public use only
        /// </summary>
        /// <param name="PatternListName"></param>
        /// <param name="StartWord"></param>
        /// <param name="IgnoreStartWord"></param>
        /// <returns></returns>
        public Word FindRightWordByPatternListName(string PatternListName, Word StartWord, bool IgnoreStartWord)
        {
            int i = StartWord.Index;
            if(IgnoreStartWord){
                i++;
            }
            while(i < this.words.Count){
                Word w = this[i];
                if(w.Pattern != null){
                    if(w.Pattern.Parent != null){
                        if(w.Pattern.Parent.Name == PatternListName && w.Type != WordType.Space
                           && w.Type != WordType.Tab){
                            return w;
                        }
                    }
                }
                i++;
            }
            return null;
        }
        /// <summary>
        /// For public use only
        /// </summary>
        /// <param name="PatternList"></param>
        /// <param name="StartWord"></param>
        /// <param name="IgnoreStartWord"></param>
        /// <returns></returns>
        public Word FindLeftWordByPatternList(PatternList PatternList, Word StartWord, bool IgnoreStartWord)
        {
            int i = StartWord.Index;
            if(IgnoreStartWord){
                i--;
            }
            while(i >= 0){
                Word w = this[i];
                if(w.Pattern != null){
                    if(w.Pattern.Parent != null){
                        if(w.Pattern.Parent == PatternList && w.Type != WordType.Space && w.Type != WordType.Tab){
                            return w;
                        }
                    }
                }
                i--;
            }
            return null;
        }
        /// <summary>
        /// For public use only
        /// </summary>
        /// <param name="PatternListName"></param>
        /// <param name="StartWord"></param>
        /// <param name="IgnoreStartWord"></param>
        /// <returns></returns>
        public Word FindLeftWordByPatternListName(string PatternListName, Word StartWord, bool IgnoreStartWord)
        {
            int i = StartWord.Index;
            if(IgnoreStartWord){
                i--;
            }
            while(i >= 0){
                Word w = this[i];
                if(w.Pattern != null){
                    if(w.Pattern.Parent != null){
                        if(w.Pattern.Parent.Name == PatternListName && w.Type != WordType.Space
                           && w.Type != WordType.Tab){
                            return w;
                        }
                    }
                }
                i--;
            }
            return null;
        }
        /// <summary>
        /// For public use only
        /// </summary>
        /// <param name="spanDefinition"></param>
        /// <param name="StartWord"></param>
        /// <param name="IgnoreStartWord"></param>
        /// <returns></returns>
        public Word FindLeftWordByBlockType(SpanDefinition spanDefinition, Word StartWord, bool IgnoreStartWord)
        {
            int i = StartWord.Index;
            if(IgnoreStartWord){
                i--;
            }
            while(i >= 0){
                Word w = this[i];
                if(w.Span.spanDefinition == spanDefinition && w.Type != WordType.Space && w.Type != WordType.Tab){
                    return w;
                }
                i--;
            }
            return null;
        }
        /// <summary>
        /// For public use only
        /// </summary>
        /// <param name="spanDefinition"></param>
        /// <param name="StartWord"></param>
        /// <param name="IgnoreStartWord"></param>
        /// <returns></returns>
        public Word FindRightWordByBlockType(SpanDefinition spanDefinition, Word StartWord, bool IgnoreStartWord)
        {
            int i = StartWord.Index;
            if(IgnoreStartWord){
                i++;
            }
            while(i < this.words.Count){
                Word w = this[i];
                if(w.Span.spanDefinition == spanDefinition && w.Type != WordType.Space && w.Type != WordType.Tab){
                    return w;
                }
                i++;
            }
            return null;
        }
        /// <summary>
        /// For public use only
        /// </summary>
        /// <param name="BlockTypeName"></param>
        /// <param name="StartWord"></param>
        /// <param name="IgnoreStartWord"></param>
        /// <returns></returns>
        public Word FindLeftWordByBlockTypeName(string BlockTypeName, Word StartWord, bool IgnoreStartWord)
        {
            int i = StartWord.Index;
            if(IgnoreStartWord){
                i--;
            }
            while(i >= 0){
                Word w = this[i];
                if(w.Span.spanDefinition.Name == BlockTypeName && w.Type != WordType.Space && w.Type != WordType.Tab){
                    return w;
                }
                i--;
            }
            return null;
        }
        /// <summary>
        /// For public use only
        /// </summary>
        /// <param name="BlockTypeName"></param>
        /// <param name="StartWord"></param>
        /// <param name="IgnoreStartWord"></param>
        /// <returns></returns>
        public Word FindRightWordByBlockTypeName(string BlockTypeName, Word StartWord, bool IgnoreStartWord)
        {
            int i = StartWord.Index;
            if(IgnoreStartWord){
                i++;
            }
            while(i < this.words.Count){
                Word w = this[i];
                if(w.Span.spanDefinition.Name == BlockTypeName && w.Type != WordType.Space && w.Type != WordType.Tab){
                    return w;
                }
                i++;
            }
            return null;
        }
    }
}