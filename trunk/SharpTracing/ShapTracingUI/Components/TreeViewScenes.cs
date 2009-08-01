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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using DrawEngine.Renderer;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.Lights;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.RenderObjects;
using System.Linq;
using WeifenLuo.WinFormsUI.Docking;

namespace DrawEngine.SharpTracingUI {
    public partial class TreeViewScenes : TreeView {
        public TreeViewScenes() {
            UnifiedScenesRepository.Scenes.CollectionChanged += this.Scenes_CollectionChanged;
            this.InitializeComponent();
        }

        void Scenes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs<Scene> e) {
            NameableCollection<Scene> scenes = sender as NameableCollection<Scene>;
            if(scenes != null){
                foreach(Scene scene in scenes){
                    scene.OnNameChanged -= this.nameable_OnNameChanged;

                    scene.Lights.CollectionChanged -= this.Nameable_CollectionChanged;
                    scene.Primitives.CollectionChanged -= this.Nameable_CollectionChanged;
                    scene.Materials.CollectionChanged -= this.Nameable_CollectionChanged;

                    foreach(INameable nameable in scene.Lights) {
                        nameable.OnNameChanged -= this.nameable_OnNameChanged;
                    }
                    foreach(INameable nameable in scene.Primitives) {
                        nameable.OnNameChanged -= this.nameable_OnNameChanged;
                    }
                    foreach(INameable nameable in scene.Materials) {
                        nameable.OnNameChanged -= this.nameable_OnNameChanged;
                    }
                    //********************************************
                    scene.OnNameChanged += this.nameable_OnNameChanged;

                    scene.Lights.CollectionChanged += this.Nameable_CollectionChanged;
                    scene.Primitives.CollectionChanged += this.Nameable_CollectionChanged;
                    scene.Materials.CollectionChanged += this.Nameable_CollectionChanged;
                    foreach(INameable nameable in scene.Lights){
                        nameable.OnNameChanged += this.nameable_OnNameChanged;
                    }
                    foreach(INameable nameable in scene.Primitives){
                        nameable.OnNameChanged += this.nameable_OnNameChanged;
                    }
                    foreach(INameable nameable in scene.Materials){
                        nameable.OnNameChanged += this.nameable_OnNameChanged;
                    }
                }
            }

            #region MyRegion
            //foreach(Scene scene in e.NewItems) {
            //    switch(e.Action) {
            //        case NotifyCollectionChangedAction.Add:
            //        case NotifyCollectionChangedAction.Replace:
            //            scene.OnNameChanged += this.nameable_OnNameChanged;

            //            scene.Lights.CollectionChanged += Nameable_CollectionChanged;
            //            scene.Primitives.CollectionChanged += Nameable_CollectionChanged;
            //            scene.Materials.CollectionChanged += Nameable_CollectionChanged;
            //            foreach(INameable nameable in scene.Lights){
            //                nameable.OnNameChanged += this.nameable_OnNameChanged;
            //            }
            //            foreach(INameable nameable in scene.Primitives) {
            //                nameable.OnNameChanged += this.nameable_OnNameChanged;
            //            }
            //            foreach(INameable nameable in scene.Materials) {
            //                nameable.OnNameChanged += this.nameable_OnNameChanged;
            //            }

            //            break;
            //        case NotifyCollectionChangedAction.Remove:
            //        case NotifyCollectionChangedAction.Clear:
            //            scene.OnNameChanged -= this.nameable_OnNameChanged;

            //            scene.Lights.CollectionChanged -= Nameable_CollectionChanged;
            //            scene.Primitives.CollectionChanged -= Nameable_CollectionChanged;
            //            scene.Materials.CollectionChanged -= Nameable_CollectionChanged;

            //            foreach(INameable nameable in scene.Lights) {
            //                nameable.OnNameChanged -= this.nameable_OnNameChanged;
            //            }
            //            foreach(INameable nameable in scene.Primitives) {
            //                nameable.OnNameChanged -= this.nameable_OnNameChanged;
            //            }
            //            foreach(INameable nameable in scene.Materials) {
            //                nameable.OnNameChanged -= this.nameable_OnNameChanged;
            //            }
            //            break;
            //    }
            //} 
            #endregion
            this.Refresh();
        }
        void Nameable_CollectionChanged<T>(object sender, NotifyCollectionChangedEventArgs<T> e) {
            this.Refresh();
        }
        void nameable_OnNameChanged(INameable sender, string oldName) {
            this.Refresh();
        }
        public NameableCollection<Scene> Scenes {
            get { return UnifiedScenesRepository.Scenes; }
        }
       
        //public TreeViewScenes(NameableCollection<Scene> scenes)
        //{
        //    UnifiedScenesRepository.Scenes = scenes;
        //    this.InitializeComponent();
        //    this.Refresh();
        //}
        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e) {
            if(e.Node.Level == 0) {
                e.Node.ContextMenuStrip = this.contextMenuStripScene;
            }
            else {
                e.Node.ContextMenuStrip = this.contextMenuPrimitive;
            }
            base.OnNodeMouseClick(e);
        }
        public override void Refresh() {
            this.Nodes.Clear();
            if(UnifiedScenesRepository.Scenes == null) {
                return;
            }
            TreeNode scenesNode = new TreeNode("Scenes(" + UnifiedScenesRepository.Scenes.Count + ")", 1, 1);
            scenesNode.Name = "Scenes";
            scenesNode.Tag = UnifiedScenesRepository.Scenes;
            scenesNode.Expand();
            this.Nodes.Add(scenesNode);
            foreach(Scene newScene in UnifiedScenesRepository.Scenes) {
                TreeNode newSceneNode = new TreeNode(newScene.Name, 0, 0);
                newSceneNode.Name = newScene.Name;
                newSceneNode.Tag = newScene;
                scenesNode.Nodes.Add(newSceneNode);
                if(newScene.Primitives != null) {
                    TreeNode primitivesNode = new TreeNode("Primitives(" + newScene.Primitives.Count + ")", 1, 1);
                    primitivesNode.Name = "Primitives";
                    primitivesNode.Tag = newScene.Primitives;
                    primitivesNode.Expand();
                    newSceneNode.Nodes.Add(primitivesNode);
                    foreach(Primitive newPrimitive in newScene.Primitives) {
                        TreeNode newPrimitiveNode = new TreeNode(newPrimitive.Name, 0, 0);
                        newPrimitiveNode.Name = newPrimitive.Name;
                        newPrimitiveNode.Tag = newPrimitive;
                        //newSceneNode.Expand();
                        primitivesNode.Nodes.Add(newPrimitiveNode);
                    }
                }
                if(newScene.Lights != null) {
                    TreeNode lightsNode = new TreeNode("Lights(" + newScene.Lights.Count + ")", 1, 1);
                    lightsNode.Name = "Lights";
                    lightsNode.Tag = newScene.Lights;
                    lightsNode.Expand();
                    newSceneNode.Nodes.Add(lightsNode);
                    foreach(Light newLight in newScene.Lights) {
                        TreeNode newLightNode = new TreeNode(newLight.Name, 0, 0);
                        newLightNode.Name = newLight.Name;
                        newLightNode.Tag = newLight;
                        //newSceneNode.Expand();
                        lightsNode.Nodes.Add(newLightNode);
                    }
                }
                if(newScene.Materials != null) {
                    TreeNode materialsNode = new TreeNode("Materials(" + newScene.Materials.Count + ")", 1, 1);
                    materialsNode.Name = "Materials";
                    materialsNode.Tag = newScene.Materials;
                    materialsNode.Expand();
                    newSceneNode.Nodes.Add(materialsNode);
                    foreach(Material newMaterial in newScene.Materials) {
                        TreeNode newMaterialNode = new TreeNode(newMaterial.Name, 0, 0);
                        newMaterialNode.Name = newMaterial.Name;
                        newMaterialNode.Tag = newMaterial;
                        //newSceneNode.Expand();
                        materialsNode.Nodes.Add(newMaterialNode);
                    }
                }
            }
            base.Refresh();
            this.ExpandAll();
        }
        public void AddScene(Scene scene) {
            UnifiedScenesRepository.Scenes.Add(scene);
            this.Refresh();
            ////TODO Verificar se jah existe cena com esse nome e sugerir mudanca de nome
            //TreeNode node = new TreeNode(scene.Name);
            //node.Tag = scene;
            //node.Name = scene.Name;
            //this.treeViewScene.Nodes.Add(node);
            //this.treeViewScene.Refresh();
            //scene.OnNameChanged += new Scene.NameChange(scene_OnNameChanged);
            //scene.OnPrimitiveAdded += new Scene.PrimitiveAdd(scene_OnPrimitiveAdded);
        }
        private static Scene GetSceneFromNode(TreeNode node) {
            if(node != null) {
                if((node.Tag as Scene) != null) {
                    return (node.Tag as Scene);
                }
                while(node.Parent != null) {
                    if((node.Parent.Tag as Scene) != null) {
                        return (node.Parent.Tag as Scene);
                    }
                    node = node.Parent;
                }
            }
            return null;
        }
        private void TreeViewScenes_AfterSelect(object sender, TreeViewEventArgs e) {
            Scene scn = GetSceneFromNode(e.Node);
            if(scn != null) {
                UnifiedScenesRepository.CurrentEditingScene = scn;
                Form mainForm = (Form)FromHandle(Process.GetCurrentProcess().MainWindowHandle);
                foreach(IDockContent doc in ((MainForm)mainForm).DockPanel.Documents) {
                    if(doc is DocumentWindow
                       && (doc as DocumentWindow).TabText == UnifiedScenesRepository.CurrentEditingScene.Name) {
                        (doc as DocumentWindow).Activate();
                    }
                }
            }
            if(!(e.Node.Tag is IEnumerable)) {
                PropertyWindow.Instance.PropertyGrid.SelectedObject = e.Node.Tag;
            }
        }
        private void contextMenuPrimitive_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            if(this.SelectedNode != null && this.SelectedNode.Tag != null) {
                this.SelectedNode.Tag = null;
                UnifiedScenesRepository.CurrentEditingScene.Primitives.Remove(this.SelectedNode.Tag as Primitive);
                this.Refresh();
            }
        }
    }
}