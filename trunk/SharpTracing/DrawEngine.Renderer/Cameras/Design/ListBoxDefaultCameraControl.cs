using System;
using System.Windows.Forms;
using DrawEngine.Renderer.Collections.Design;

namespace DrawEngine.Renderer.Cameras.Design
{
    public partial class ListBoxDefaultCameraControl : UserControl
    {
        public ListBoxDefaultCameraControl()
        {
            this.InitializeComponent();
        }
        public ListBox ListBoxCameras
        {
            get { return this.listBox1; }
        }
        private void linkLblNewCamera_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ObjectChooseType oct = new ObjectChooseType(typeof(Camera));
            if(oct.ShowDialog() == DialogResult.OK){
                Camera camera = Activator.CreateInstance(oct.SelectedType, true) as Camera;
                if(camera != null){
                    UnifiedScenesRepository.CurrentEditingScene.Cameras.Add(camera);
                    this.listBox1.Items.Add(camera);
                }
            }
        }
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SendKeys.Send("{Enter}");
        }
    }
}