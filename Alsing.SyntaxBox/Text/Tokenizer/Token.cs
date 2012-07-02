using System;

namespace Alsing.Text
{
    public sealed class Token
    {
        public Token(string text, object[] tags)
        {
            if(tags == null){
                tags = new object[0];
            }
            this.Text = text;
            this.Tags = tags;
        }
        public string Text { get; private set; }
        public object[] Tags { get; private set; }
        public override string ToString()
        {
            return this.Text;
        }
        public bool HasTag(object tag)
        {
            return Array.IndexOf(this.Tags, tag) >= 0;
        }
    }
}