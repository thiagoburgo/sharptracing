/**
 * Criado por: Thiago Burgo Belo (thiagoburgo@gmail.com)
 * SharpTracing é um projeto feito inicialmente para disciplina
 * Computação Gráfica da UFPE e depois melhorado nos tempos livres
 * sinta-se a vontade para copiar modificar e mandar correções e 
 * sugestões. Mantenha os créditos!
 * **************************************************************
 * SharpTracing is a project originally created to discipline 
 * Computer Graphics of UFPE and was improved in my free time.
 * Feel free to copy, modify and  give fixes 
 * suggestions. Keep the credits!
 */

using System;
using System.Windows.Forms;
using DrawEngine.Renderer.Collections.Design;

namespace DrawEngine.Renderer.Materials.Design {
    public partial class SamplerTreeViewEditorControl : UserControl {
        private string selectedName;

        public SamplerTreeViewEditorControl()
        {
            this.InitializeComponent();
            this.Refresh();
            this.treeViewMaterials.ExpandAll();
        }

        public Material SelectedMaterial {
            get {
                if (UnifiedScenesRepository.CurrentEditingScene.Materials.ContainsName(this.selectedName)) {
                    return UnifiedScenesRepository.CurrentEditingScene.Materials[this.selectedName];
                }
                return null;
            }
        }

        //public void AddMaterial(AbstractMaterial material) {
        //    UnifiedScenesRepository.CurrentEditingScene.Materials.Add(material);
        //    TreeNode node = new TreeNode(material.Name);
        //    node.Name = material.Name;
        //    node.Tag = material;            
        //    this.treeViewMaterials.Nodes["materials"].Nodes.Add(node);
        //    this.treeViewMaterials.ExpandAll();
        //}
        //public void AddMaterials(NameableCollection<AbstractMaterial> materials)
        //{
        //    foreach (AbstractMaterial material in materials)
        //    {
        //        this.AddMaterial(material);
        //    }
        //    this.treeViewMaterials.ExpandAll();
        //}
        public override void Refresh() {
            this.treeViewMaterials.Nodes["materials"].Nodes.Clear();
            foreach (Material material in UnifiedScenesRepository.CurrentEditingScene.Materials) {
                TreeNode node = new TreeNode(material.Name);
                node.Name = material.Name;
                node.Tag = material;
                this.treeViewMaterials.Nodes["materials"].Nodes.Add(node);
            }
            this.treeViewMaterials.ExpandAll();
            base.Refresh();
        }

        //public void RemoveMaterial(AbstractMaterial material) {
        //    TreeNode node = new TreeNode(material.ToString());
        //    this.treeViewMaterials.Nodes["materials"].Nodes.Remove(node);
        //}	
        private void treeViewMaterials_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
            SendKeys.Send("{Enter}");
            this.selectedName = e.Node.Name;
        }

        private void linkLblNewMaterial_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            //GenericCollectionForm<AbstractMaterial> form = new GenericCollectionForm<AbstractMaterial>();
            //form.Objects = Treevi
            ObjectChooseType choose = new ObjectChooseType(typeof (Material));
            if (choose.ShowDialog() == DialogResult.OK) {
                Material material = Activator.CreateInstance(choose.SelectedType, true) as Material;
                if (material != null) {
                    UnifiedScenesRepository.CurrentEditingScene.Materials.Add(material);
                }
            }
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
        }

        private void treeViewMaterials_VisibleChanged(object sender, EventArgs e) {
            this.Refresh();
        }

        private void treeViewMaterials_AfterSelect(object sender, TreeViewEventArgs e) {
            this.selectedName = e.Node.Name;
        }
    }
}