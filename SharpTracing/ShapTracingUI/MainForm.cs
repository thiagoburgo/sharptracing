using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DrawEngine.Renderer;
using DrawEngine.Renderer.RenderObjects.Design;
using TooboxUI.Components;
using WeifenLuo.WinFormsUI.Docking;
using DrawEngine.Renderer.Util;
using System.Linq;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.SharpTracingUI
{
    public partial class MainForm : Form
    {
        private static MainForm instance;
        private readonly FrameViewForm frameViewWindow = FrameViewForm.Instance;
        private readonly PropertyWindow propertyWindow = PropertyWindow.Instance;
        private readonly ScriptingForm scriptingFormWindow = ScriptingForm.Instance;
        private readonly ToolBoxWindow toolBoxWindow = ToolBoxWindow.Instance;
        private readonly TreeViewSceneWindow treeViewSceneWindow = TreeViewSceneWindow.Instance;
        //private TreeViewMaterialWindow treeViewMaterialWindow = DrawEngine.Renderer.Materials.TreeViewMaterialWindow.Instance;
        private MainForm()
        {
            this.InitializeComponent();
            //Extender.SetSchema(this.dockPanel, Extender.Schema.FromBase);
            this.treeViewSceneWindow.Show(this.dockPanel);
            this.toolBoxWindow.Show(this.dockPanel);
            this.frameViewWindow.Show(this.dockPanel);
            this.propertyWindow.Show(this.dockPanel);
            this.scriptingFormWindow.Show(this.dockPanel);
            this.toolStripRenderType.SelectedIndex = 0;
        }
        public static MainForm Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainForm();
                }
                return instance;
            }
        }
        public DockPanel DockPanel
        {
            get { return this.dockPanel; }
        }
        private void ShowNewForm(object sender, EventArgs e)
        {
            DocumentWindow childForm =
                    new DocumentWindow(
                            (RenderType)Enum.Parse(typeof(RenderType), this.toolStripRenderType.SelectedItem.ToString()));
            childForm.Show(this.dockPanel);
        }
        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Scene Files (*.xml, *.scn)|*.xml;*.scn";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {

                //System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(Scene));
                //Scene loaded = (Scene)xml.Deserialize(File.Open(FileName, FileMode.Open));
                ObjectXMLSerializer<Scene>.OnDeserialized += (scene =>
                {
                    IEnumerable<TriangleModel> models =
                              from p in scene.Primitives
                              where p is TriangleModel
                              select p as TriangleModel;
                    foreach (TriangleModel model in models)
                    {
                        LoadingModelDialog modelDlg = new LoadingModelDialog(model);
                        modelDlg.Show();
                    }
                });
                Scene loaded = Scene.Load(openFileDialog.FileName);
                if (UnifiedScenesRepository.Scenes.ContainsName(loaded.Name))
                {
                    DialogResult dr =
                            MessageBox.Show(
                                    "Alredy exists an scene opened with the same name!\n Press \"Yes\" to overwrite current scene or \"No\" to rename loaded scene!",
                                    "Overwrite scene?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
                                    MessageBoxDefaultButton.Button2);
                    if (dr == DialogResult.Yes)
                    {
                        UnifiedScenesRepository.Scenes[loaded.Name] = loaded;
                        this.SelectDocument(loaded.Name);
                        return;
                    }
                    else if (dr == DialogResult.Cancel)
                    {
                        return;
                    }
                    loaded.Name = null;
                    loaded = null;
                }
                DocumentWindow childForm = new DocumentWindow(loaded,
                                                              (RenderType)
                                                              Enum.Parse(typeof(RenderType),
                                                                         this.toolStripRenderType.SelectedItem.ToString()));
                childForm.Show(this.dockPanel);
            }
        }
        public void SelectDocument(String tabText)
        {
            foreach (IDockContent doc in this.DockPanel.Documents)
            {
                if (doc is DocumentWindow && (doc as DocumentWindow).TabText == tabText)
                {
                    (doc as DocumentWindow).Activate();
                }
            }
        }
        private void SaveDialog(Scene scene)
        {
            if (UnifiedScenesRepository.Scenes.Contains(scene))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Scene Files (*.xml)|*.xml;*.scn";
                if (scene != null)
                {
                    saveFileDialog.FileName = scene.Name;
                }
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    UnifiedScenesRepository.Scenes[scene.Name].Name =
                            Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                    scene.Save(saveFileDialog.FileName);
                }
            }
        }
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveDialog(UnifiedScenesRepository.CurrentEditingScene);
        }
        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DocumentWindow document in this.dockPanel.Documents)
            {
                this.SaveDialog(UnifiedScenesRepository.Scenes[document.TabText]);
            }
        }
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            this.SaveDialog(UnifiedScenesRepository.CurrentEditingScene);
        }
        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptingForm scriptingForm = (this.dockPanel.ActiveDocument as ScriptingForm);
            if (scriptingForm != null)
            {
                scriptingForm.SyntaxBox.Cut();
                this.cutToolStripMenuItem.Enabled = scriptingForm.SyntaxBox.CanCopy && scriptingForm.SyntaxBox.Enabled;
            }
        }
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptingForm scriptingForm = (this.dockPanel.ActiveDocument as ScriptingForm);
            if (scriptingForm != null)
            {
                scriptingForm.SyntaxBox.Copy();
                this.copyToolStripMenuItem.Enabled = scriptingForm.SyntaxBox.CanCopy;
            }
        }
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptingForm scriptingForm = (this.dockPanel.ActiveDocument as ScriptingForm);
            if (scriptingForm != null)
            {
                scriptingForm.SyntaxBox.Paste();
                this.pasteToolStripMenuItem.Enabled = scriptingForm.SyntaxBox.CanPaste;
            }

        }
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptingForm scriptingForm = (this.dockPanel.ActiveDocument as ScriptingForm);
            if (scriptingForm != null)
            {
                scriptingForm.SyntaxBox.Redo();
                this.undoToolStripMenuItem.Enabled = scriptingForm.SyntaxBox.CanRedo;
            }

        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptingForm scriptingForm = (this.dockPanel.ActiveDocument as ScriptingForm);
            if (scriptingForm != null)
            {
                scriptingForm.SyntaxBox.Undo();
                this.undoToolStripMenuItem.Enabled = scriptingForm.SyntaxBox.CanUndo;
            }

        }
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptingForm scriptingForm = (this.dockPanel.ActiveDocument as ScriptingForm);
            if (scriptingForm != null)
            {
                scriptingForm.SyntaxBox.SelectAll();
                this.undoToolStripMenuItem.Enabled = scriptingForm.SyntaxBox.CanSelect;
            }

        }
        private void formatCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptingForm scriptingForm = (this.dockPanel.ActiveDocument as ScriptingForm);
            if (scriptingForm != null)
            {
                scriptingForm.FormatCode();
            }
        }
        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.toolStrip.Visible = this.toolBarToolStripMenuItem.Checked;
        }
        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.statusStrip.Visible = this.statusBarToolStripMenuItem.Checked;
        }
        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }
        private void TileVerticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }
        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }
        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.ArrangeIcons);
        }
        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in this.MdiChildren)
            {
                childForm.Close();
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Form childForm in this.MdiChildren)
            {
                childForm.Close();
                childForm.Dispose();
            }
            this.Dispose();
            Application.Exit();
        }
        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            if (this.dockPanel.ActiveDocument is DockContent)
            {
                UnifiedScenesRepository.CurrentEditingScene =
                        UnifiedScenesRepository.Scenes[((DockContent)this.dockPanel.ActiveDocument).TabText];
            }
        }
        private void renderToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.dockPanel.ActiveDocument != null)
            {
                if (this.dockPanel.ActiveDocument is ScriptingForm)
                {
                    ((ScriptingForm)this.dockPanel.ActiveDocument).Run();
                }
                else if (this.dockPanel.ActiveDocument is DocumentWindow)
                {
                    ((DocumentWindow)this.dockPanel.ActiveDocument).RenderScene();
                }
            }
        }
        private void dockPanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
        }
        private void dockPanel_DragDrop(object sender, DragEventArgs e)
        {
            ToolboxItem tItem = ((HostToolbox.HostItem)e.Data.GetData(typeof(HostToolbox.HostItem))).ToolboxItem;
            Assembly asm = Assembly.GetAssembly(typeof(Scene));
            //Object obj = Activator.CreateInstance(Type.GetType(tItem.TypeName), true);
            Object obj = asm.CreateInstance(tItem.TypeName, false,
                                            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                                            null, null, null);
            if (obj is Scene)
            {
                DocumentWindow newForm =
                        new DocumentWindow(
                                (RenderType)
                                Enum.Parse(typeof(RenderType), this.toolStripRenderType.SelectedItem.ToString()));

                newForm.Show(this.dockPanel);
            }
        }
        private void toolStripRenderType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.dockPanel.ActiveDocument != null && (this.dockPanel.ActiveDocument is DocumentWindow))
            {
                ((DocumentWindow)this.dockPanel.ActiveDocument).RenderType =
                        (RenderType)Enum.Parse(typeof(RenderType), this.toolStripRenderType.SelectedItem.ToString());
            }
        }
    }
}