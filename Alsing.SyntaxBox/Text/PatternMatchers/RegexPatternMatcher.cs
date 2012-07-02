// *
// * Copyright (C) 2008 Roger Alsing : http://www.rogeralsing.com
// *
// * This library is free software; you can redistribute it and/or modify it
// * under the terms of the GNU Lesser General Public License 2.1 or later, as
// * published by the Free Software Foundation. See the included license.txt
// * or http://www.gnu.org/copyleft/lesser.html for details.
// *
// *
using System;
using System.Text.RegularExpressions;

namespace Alsing.Text.PatternMatchers
{
    /// <summary>
    /// Pattern matcher that allows regex patterns to be matched
    /// </summary>
    public class RegexPatternMatcher : PatternMatcherBase
    {
        private Regex regEx;
        public RegexPatternMatcher()
        {
            this.PatternChanged += this.RegexPatternMatcher_PatternChanged;
        }
        public RegexPatternMatcher(string pattern) : this()
        {
            this.Pattern = pattern;
        }

        #region PUBLIC PROPERTY PATTERN (+PATTERNCHANGED EVENT)
        private string pattern;
        /// <summary>
        /// Gets or Sets the <c>Pattern</c> property
        /// </summary>
        public string Pattern
        {
            get { return this.pattern; }
            set
            {
                //Ignore same value
                if(this.pattern == value){
                    return;
                }
                //Set the new value
                this.pattern = value;
                //Raise the changed event
                this.OnPatternChanged(EventArgs.Empty);
            }
        }

        #region PUBLIC EVENT PATTERNCHANGED
        /// <summary>
        /// Fires when the 'Pattern' Property changes
        /// </summary>
        public event EventHandler PatternChanged = null;
        /// <summary>
        /// Raises the <c>PatternChanged</c> Event
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnPatternChanged(EventArgs e)
        {
            if(this.PatternChanged != null){
                this.PatternChanged(this, e);
            }
        }
        #endregion //END PUBLIC EVENT PATTERNCHANGED

        #endregion //END PUBLIC PROPERTY PATTERN (+PATTERNCHANGED EVENT)

        //perform the match
        public override int Match(string textToMatch, int matchAtIndex)
        {
            if(this.regEx == null){
                return 0;
            }
            Match match = this.regEx.Match(textToMatch, matchAtIndex);
            if(match.Success){
                return match.Length;
            }
            return 0;
        }
        private void RegexPatternMatcher_PatternChanged(object sender, EventArgs e)
        {
            const RegexOptions options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline;
            this.regEx = new Regex(string.Format(@"\G({0})", this.pattern), options);
        }
    }
}