using System;
using System.Collections.Generic;
using Alsing.Text.PatternMatchers;

namespace Alsing.Text
{
    public class Tokenizer
    {
        private readonly TokenTree tree;
        public Tokenizer()
        {
            this.tree = new TokenTree();
        }
        public bool IsImmutable { get; private set; }
        public string Text { get; set; }
        public Tokenizer AddPattern(IPatternMatcher matcher, bool caseSensitive, bool needsSeparators,
                                    params object[] tags)
        {
            this.ThrowIfImmutable();
            this.tree.AddPattern(matcher, caseSensitive, needsSeparators, tags);
            return this;
        }
        public Tokenizer AddToken(string token, bool caseSensitive, bool needsSeparators, params object[] tags)
        {
            this.ThrowIfImmutable();
            this.tree.AddToken(token, caseSensitive, needsSeparators, tags);
            return this;
        }
        public Token[] Tokenize()
        {
            if(this.Text == null){
                throw new ArgumentNullException("Text");
            }
            this.MakeImmutable();
            var tokens = new List<Token>();
            int index = 0;
            while(index < this.Text.Length){
                MatchResult match = this.tree.Match(this.Text, index);
                if(match.Found){
                    string dummyText = this.Text.Substring(index, match.Index - index);
                    var dummyToken = new Token(dummyText, null);
                    tokens.Add(dummyToken);
                    var realToken = new Token(match.GetText(), match.Tags);
                    index = match.Index + match.Length;
                    tokens.Add(realToken);
                } else{
                    string dummyText = this.Text.Substring(index);
                    var dummyToken = new Token(dummyText, null);
                    tokens.Add(dummyToken);
                    index = this.Text.Length;
                }
            }
            return tokens.ToArray();
        }
        private void ThrowIfImmutable()
        {
            if(this.IsImmutable){
                throw new Exception("Tokens can not be added to an immutable tokenizer");
            }
        }
        public void MakeImmutable()
        {
            this.IsImmutable = true;
        }
    }
}