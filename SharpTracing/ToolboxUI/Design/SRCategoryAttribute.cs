using System;
using System.ComponentModel;
using ToolBoxUI.Components.Properties;

namespace TooboxUI.Components.Design
{
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SRCategoryAttribute : CategoryAttribute
    {
        public SRCategoryAttribute(string category) : base(category) {}
        protected override string GetLocalizedString(string value)
        {
            return Resources.ResourceManager.GetString(value);
        }
    }
}