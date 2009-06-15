using System;
using System.Windows.Forms;

namespace DrawEngine.Renderer.Lights.Design
{
    public partial class LightCollectionForm : Form
    {
        private LightDictionary lights = new LightDictionary();
        private Type type;
        public LightCollectionForm()
        {
            this.type = typeof(Light);
            this.InitializeComponent();
        }
        public LightDictionary Lights
        {
            get { return this.lights; }
            set
            {
                if(value != null){
                    this.lights = value;
                    foreach(Light lg in this.lights.Values){
                        this.listObjects.Items.Add(lg);
                    }
                }
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            LightChooseType clt = new LightChooseType();
            if(clt.ShowDialog() == DialogResult.OK){
                this.listObjects.Items.Add(this.CreateInstance(clt.SelectedType));
            }
        }
        private Object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type, true);
        }
        private void listObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ArrayList list = new ArrayList();
            //list.AddRange(listObjects.SelectedItems);
            //this.propertyGridItems.SelectedObjects = list.ToArray();
            this.propertyGridItems.SelectedObject = this.listObjects.SelectedItem;
        }
        private void propertyGridItems_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            foreach(Object obj in ((PropertyGrid)s).SelectedObjects){
                this.listObjects.Items[this.listObjects.Items.IndexOf(obj)] = obj;
            }
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            //this.lights.AddRange();
            foreach(Light lg in this.listObjects.Items){
                this.lights.Add(lg.Name, lg);
            }
            this.DialogResult = DialogResult.OK;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            this.listObjects.Items.RemoveAt(this.listObjects.SelectedIndex);
        }
    }
}