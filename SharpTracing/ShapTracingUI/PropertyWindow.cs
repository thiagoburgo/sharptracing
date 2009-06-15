using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DrawEngine.SharpTracingUI
{
    /// <summary>
    /// Summary description for DummyPropertyGrid.
    /// </summary>
    public partial class PropertyWindow : DockContent
    {
        private static readonly object padlock = new object();
        private static PropertyWindow instance;
        private ComboBox comboBox;
        private MenuItem menuItem1;
        private PropertyGrid propertyGrid;
        private PropertyWindow()
        {
            this.InitializeComponent();
            //propertyGrid.SelectedObjectsChanged += new EventHandler(propertyGrid_SelectedObjectsChanged);
        }
        public static PropertyWindow Instance
        {
            get
            {
                lock(padlock){
                    if(instance == null){
                        instance = new PropertyWindow();
                    }
                    return instance;
                }
            }
        }
        public PropertyGrid PropertyGrid
        {
            get { return this.propertyGrid; }
        }
        public ComboBox ComboBox
        {
            get { return this.comboBox; }
        }
        private void propertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
        {
            String name = ((Control)this.propertyGrid.SelectedObject).Name;
            this.comboBox.SelectedItem = name;
        }
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e) {}
        private void PropertyWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}