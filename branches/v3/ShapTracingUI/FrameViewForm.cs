using DrawEngine.SharpTracingUI.Components;
using WeifenLuo.WinFormsUI.Docking;

namespace DrawEngine.SharpTracingUI {
    public partial class FrameViewForm : DockContent {
        private static FrameViewForm instance;

        private FrameViewForm() {
            this.InitializeComponent();
        }

        public static FrameViewForm Instance {
            get {
                if (instance == null) {
                    instance = new FrameViewForm();
                }
                return instance;
            }
        }

        public FrameView FrameView {
            get { return this.frameView1; }
        }
    }
}