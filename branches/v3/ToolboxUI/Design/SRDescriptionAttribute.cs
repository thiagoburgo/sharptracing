using System;
using System.ComponentModel;
using ToolBoxUI.Components.Properties;

namespace TooboxUI.Components.Design {
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SRDescriptionAttribute : DescriptionAttribute {
        // Methods
        private bool replaced;
        public SRDescriptionAttribute(string description) : base(description) {}
        // Properties
        public override string Description {
            get {
                if (!this.replaced) {
                    this.replaced = true;
                    base.DescriptionValue = Resources.ResourceManager.GetString(base.Description);
                }
                return base.Description;
            }
        }

        // Fields
    }
}