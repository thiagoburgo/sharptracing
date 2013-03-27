using System.ComponentModel;
using System.Windows.Forms;

namespace DrawEngine.SharpTracingUI.Components {
    public partial class Frame : Button {
        public Frame() {
            this.InitializeComponent();
        }

        public Frame(IContainer container) {
            container.Add(this);
            this.InitializeComponent();
        }
    }
}