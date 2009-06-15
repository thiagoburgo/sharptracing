using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DrawEngine.SharpTracingUI
{
    public partial class TreeViewSceneWindow : DockContent
    {
        private static readonly object padlock = new object();
        private static TreeViewSceneWindow instance;
        //public NameableCollection<Scene> Scenes
        //{
        //    get { return this.treeViewScene.Scenes; }            
        //}
        private TreeViewSceneWindow()
        {
            this.InitializeComponent();
            //this.DoubleBuffered = true;
        }
        public static TreeViewSceneWindow Instance
        {
            get
            {
                lock(padlock){
                    if(instance == null){
                        instance = new TreeViewSceneWindow();
                    }
                    return instance;
                }
            }
        }
        private void TreeViewSceneWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
        //public void AddScene(Scene scene) {
        //    //this.treeViewScene.AddScene(scene);
        //    /*
        //    //TODO Verificar se jah existe cena com esse nome e sugerir mudanca de nome
        //    TreeNode node = new TreeNode(scene.Name);
        //    node.Tag = scene;
        //    node.Name = scene.Name;
        //    this.treeViewScene.Nodes.Add(node);
        //    this.treeViewScene.Refresh();
        //    scene.OnNameChanged += new Scene.NameChange(scene_OnNameChanged);
        //    scene.OnPrimitiveAdded += new Scene.PrimitiveAdd(scene_OnPrimitiveAdded);
        //    */
        //}
        //private void scene_OnPrimitiveAdded(Scene sender, Primitive primitiveAdded) {
        //    RefreshScenesViewer(sender.Name);
        //}
        //private void scene_OnNameChanged(Scene sender, string oldName) {
        //    this.UpdateScene(oldName, sender);
        //    RefreshScenesViewer(sender.Name);
        //}
        //public Scene FindScene(string name) {
        //    TreeNode node = this.treeViewScene.Nodes[name];
        //    if (node != null) {
        //        return (Scene) node.Tag;
        //    }
        //    return null;
        //}
        //public void RefreshScenesViewer(String nameToFind) {
        //    TreeNode node = this.treeViewScene.Nodes[nameToFind];
        //    Scene newScene = node != null ? (Scene) node.Tag : null;
        //    if (newScene != null) {
        //        if (newScene.Lights != null
        //            && newScene.Lights.Count > 0) {
        //            TreeNode node2 = node.Nodes["Lights"];
        //            if (node2 == null) {
        //                node2 = new TreeNode("Lights", 1, 1);
        //                node2.Tag = newScene.Lights;
        //                node2.Name = "Lights";
        //                node.Nodes.Add(node2);
        //            }
        //            foreach (AbstractLight light in newScene.Lights) {
        //                //TODO VERIFICAR ISSO!
        //                //if (node2.Nodes[primitive.Name] == null)
        //                //{
        //                TreeNode node3 = new TreeNode(light.ToString());
        //                node3.Tag = light;
        //                //TODO dar nome as luzes ou nao
        //                node3.Name = light.ToString();
        //                node2.Nodes.Add(node3);
        //                //}
        //            }
        //        }
        //        if (newScene.Primitives != null
        //            && newScene.Primitives.Count > 0) {
        //            TreeNode node2 = node.Nodes["Primitives"];
        //            if (node2 == null) {
        //                node2 = new TreeNode("Primitives", 1, 1);
        //                node2.Tag = newScene.Lights;
        //                node2.Name = "Primitives";
        //                node.Nodes.Add(node2);
        //            }
        //            foreach (Primitive primitive in newScene.Primitives) {
        //                if (node2.Nodes[primitive.Name] == null) {
        //                    TreeNode node3 = new TreeNode(primitive.Name);
        //                    node3.Tag = primitive;
        //                    //TODO dar nome as luzes ou nao
        //                    node3.Name = primitive.Name;
        //                    node2.Nodes.Add(node3);
        //                }
        //            }
        //        }
        //    }
        //}
        //public void UpdateScene(string nameToFind, Scene newScene) {
        //    if (newScene != null) {
        //        if (this.treeViewScene.Nodes[newScene.Name] != null) {
        //            throw new Exception("Already exists an scene with this name! Choose Other!");
        //        }
        //        else if (this.treeViewScene.Nodes[nameToFind] != null) {
        //            TreeNode node = this.treeViewScene.Nodes[nameToFind];
        //            node.Tag = newScene;
        //            node.Text = newScene.Name;
        //            node.Name = newScene.Name;
        //        }
        //    }
        //}
        //private void treeViewScene_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
        //    if (e.Node.Level == 0) {
        //        e.Node.ContextMenuStrip = contextMenuStripScene;
        //    }
        //    else {
        //        e.Node.ContextMenuStrip = contextMenuPrimitive;
        //    }
        //}
    }
}