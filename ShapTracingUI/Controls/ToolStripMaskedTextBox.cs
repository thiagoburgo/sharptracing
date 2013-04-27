using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DrawEngine.SharpTracingUI.Controls
{
    //Declare a class that inherits from ToolStripControlHost.
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripMaskedTextBox : CustomToolStripControlHost
    {
        // Call the base constructor passing in a MaskedTextBox instance.
        public ToolStripMaskedTextBox() : base(CreateControlInstance()) { }

        public MaskedTextBox MaskedTextBox
        {
            get
            {
                return Control as MaskedTextBox;
            }
        }


        private static Control CreateControlInstance()
        {
            return new MaskedTextBox(){Width = 25, Height = 25, Size = new Size(25, 25)};
        }
    }

    public class CustomToolStripControlHost : ToolStripControlHost
    {
        public CustomToolStripControlHost()
            : base(new Control())
        {
        }
        public CustomToolStripControlHost(Control c)
            : base(c)
        {
        }
    }
}
