// *
// * Copyright (C) 2008 Roger Alsing : http://www.RogerAlsing.com
// *
// * This library is free software; you can redistribute it and/or modify it
// * under the terms of the GNU Lesser General Public License 2.1 or later, as
// * published by the Free Software Foundation. See the included license.txt
// * or http://www.gnu.org/copyleft/lesser.html for details.
// *
// *
using System.Drawing;

namespace Alsing.Windows.Forms.FormatLabel
{
    public enum TextEffect
    {
        None = 0,
        Outline,
        ShadowRB,
        ShadowLB,
        ShadowRT,
        ShadowLT,
    }

    public class Element
    {
        protected string _Tag = "";
        protected string _TagName = "";
        public Color BackColor = Color.Black;
        public TextEffect Effect = 0;
        public Color EffectColor = Color.Black;
        public Font Font;
        public Color ForeColor = Color.Black;
        public Element Link;
        public bool NewLine;
        public string Text = "";
        public Word[] words;
        public string TagName
        {
            get { return this._TagName; }
        }
        public string Tag
        {
            get { return this._Tag; }
            set
            {
                this._Tag = value.ToLowerInvariant();
                this._Tag = this._Tag.Replace("\t", " ");
                if(this._Tag.IndexOf(" ") >= 0){
                    this._TagName = this._Tag.Substring(0, this._Tag.IndexOf(" "));
                } else{
                    this._TagName = this._Tag;
                }
            }
        }
    }
}