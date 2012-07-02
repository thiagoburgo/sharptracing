using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal class DummyControl : Control
    {
        public DummyControl()
        {
            this.SetStyle(ControlStyles.Selectable, false);
        }
    }
}