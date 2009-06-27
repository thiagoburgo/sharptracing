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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Alsing.SourceCode
{
    /// <summary>
    /// TextStyles are used to describe the apperance of text.
    /// </summary>
    [Editor(typeof(TextStyleUIEditor), typeof(UITypeEditor))]
    public class TextStyle : ICloneable
    {
        /// <summary>
        /// Name of the style
        /// </summary>
        public string Name;

        #region PUBLIC PROPERTY BOLD
        private bool _Bold;
        [Category("Font"), Description("Gets or Sets if the style uses a BOLD font")]
        public bool Bold
        {
            get { return this._Bold; }
            set
            {
                this._Bold = value;
                this.OnChange();
            }
        }
        #endregion

        #region PUBLIC PROPERTY ITALIC
        private bool _Italic;
        [Category("Font"), Description("Gets or Sets if the style uses an ITALIC font")]
        public bool Italic
        {
            get { return this._Italic; }
            set
            {
                this._Italic = value;
                this.OnChange();
            }
        }
        #endregion

        #region PUBLIC PROPERTY UNDERLINE
        private bool _Underline;
        [Category("Font"), Description("Gets or Sets if the style uses an UNDERLINED font")]
        public bool Underline
        {
            get { return this._Underline; }
            set
            {
                this._Underline = value;
                this.OnChange();
            }
        }
        #endregion

        #region PUBLIC PROPERTY FORECOLOR
        private Color _ForeColor = Color.Black;
        [Category("Color"), Description("Gets or Sets the fore color of the style")]
        public Color ForeColor
        {
            get { return this._ForeColor; }
            set
            {
                this._ForeColor = value;
                this.OnChange();
            }
        }
        #endregion

        #region PUBLIC PROPERTY BACKCOLOR
        private Color _BackColor = Color.Transparent;
        [Category("Color"), Description("Gets or Sets the background color of the style")]
        public Color BackColor
        {
            get { return this._BackColor; }
            set
            {
                this._BackColor = value;
                this.OnChange();
            }
        }
        #endregion

        /// <summary>
        /// Gets or Sets if the style uses a Bold font
        /// </summary>
        /// <summary>
        /// Gets or Sets if the style uses an Italic font
        /// </summary>
        /// <summary>
        /// Gets or Sets if the style uses an Underlined font
        /// </summary>
        /// <summary>
        /// Gets or Sets the ForeColor of the style
        /// </summary>
        /// <summary>
        /// Gets or Sets the BackColor of the style
        /// </summary>
        /// <summary>
        /// Default constructor
        /// </summary>
        public TextStyle()
        {
            this.ForeColor = Color.Black;
            this.BackColor = Color.Transparent;
        }
        /// <summary>
        /// Returns true if no color have been assigned to the backcolor
        /// </summary>
        [Browsable(false)]
        public bool Transparent
        {
            get { return (this.BackColor.A == 0); }
        }
        public event EventHandler Change = null;
        protected virtual void OnChange()
        {
            if(this.Change != null){
                this.Change(this, EventArgs.Empty);
            }
        }
        public override string ToString()
        {
            if(this.Name == null){
                return "TextStyle";
            }
            return this.Name;
        }

        #region Implementation of ICloneable
        public object Clone()
        {
            var ts = new TextStyle{
                                          //TODO: verify if this actually works
                                          BackColor = this.BackColor,
                                          Bold = this.Bold,
                                          ForeColor = this.ForeColor,
                                          Italic = this.Italic,
                                          Underline = this.Underline,
                                          Name = this.Name
                                  };
            return ts;
        }
        #endregion
    }
}