namespace WeifenLuo.WinFormsUI.Docking
{
    public partial class DockWindow
    {
        #region Nested type: SplitterControl
        private class SplitterControl : SplitterBase
        {
            protected override int SplitterSize
            {
                get { return Measures.SplitterSize; }
            }
            protected override void StartDrag()
            {
                DockWindow window = this.Parent as DockWindow;
                if(window == null){
                    return;
                }
                window.DockPanel.BeginDrag(window, window.RectangleToScreen(this.Bounds));
            }
        }
        #endregion
    }
}