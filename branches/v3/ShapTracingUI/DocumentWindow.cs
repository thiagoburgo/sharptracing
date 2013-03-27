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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using DrawEngine.Renderer;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Cameras;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.Lights;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;
using DrawEngine.Renderer.RenderObjects.Design;
using DrawEngine.Renderer.Renderers;
using DrawEngine.Renderer.Tracers;
using TooboxUI.Components;
using WeifenLuo.WinFormsUI.Docking;
using Timer = System.Timers.Timer;

namespace DrawEngine.SharpTracingUI {
    public enum RenderType {
        [Description("Ray Tracer")]
        RayTracer,
        [Description("Photon Tracer")]
        PhotonTracer,
        [Description("Distributed Ray Tracer")]
        DistributedRayTracer
    }
    public enum StrategyType
    {
        Scanline = 2,
        Progressive = 4
    }

    public partial class DocumentWindow : DockContent {
        private Intersection current_intersection;
        private bool? intersected;
        private double msElapseds;
        private Point prevPoint;
        private Thread renderThread;
       
        
        private readonly Timer timer = new Timer(100);
        private RayCasting tracer;
        public DocumentWindow() : this(new Scene()) {}

        public DocumentWindow(Scene scene) {
            this.InitializeComponent();
            this.timer.Elapsed += this.timer_Elapsed;
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UnifiedScenesRepository.Scenes.Add(scene);
            this.TabText = scene.Name;
            UnifiedScenesRepository.CurrentEditingScene = UnifiedScenesRepository.Scenes[this.TabText];
            UnifiedScenesRepository.Scenes[this.TabText].OnNameChanged += this.scene_OnNameChanged;

            foreach (Object renderType in Enum.GetValues(typeof(RenderType)))
            {
                toolStripRenderer.Items.Add(renderType);    
            }
            toolStripRenderer.SelectedIndex = 0;
            foreach (Object strategy in Enum.GetValues(typeof(StrategyType)))
            {
                toolStripStrategy.Items.Add(strategy);
            }
            toolStripStrategy.SelectedIndex = 0;

        }

        public Scene Scene {
            get { return UnifiedScenesRepository.Scenes[this.TabText]; }
        }

        private void scene_OnNameChanged(INameable sender, string oldName) {
            this.TabText = sender.Name;
        }

        protected override string GetPersistString() {
            return this.GetType() + "," + "," + this.Text;
        }

        private void menuItem2_Click(object sender, EventArgs e) {
            MessageBox.Show(
                "This is to demostrate menu item has been successfully merged into the main form. Form Text=" +
                this.Text);
        }

        private void menuItemCheckTest_Click(object sender, EventArgs e) {
            this.menuItemCheckTest.Checked = !this.menuItemCheckTest.Checked;
        }

        private void DocumentWindow_DragEnter(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
        }

        private void DocumentWindow_DragDrop(object sender, DragEventArgs e) {
            this.Activate();
            ToolboxItem tItem = ((HostToolbox.HostItem) e.Data.GetData(typeof (HostToolbox.HostItem))).ToolboxItem;
            Assembly asm = Assembly.GetAssembly(typeof (Primitive));
            Object obj = asm.CreateInstance(tItem.TypeName, false);

            //Object obj = Activator.CreateInstance(Type.GetType(tItem.TypeName));
            if (obj == null) {
                return;
            }
            if (obj is Primitive) {
                Primitive p = (Primitive) obj;
                if (p is TriangleModel) {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter =
                        "All Know Files|*.ply;*.byu;*.obj;*.off;*.noff;*.cnoff|Ply Files|*.ply|Byu Files|*.byu|Wave Obj Files|*.obj|Off Files|*.off;*.noff;*.cnoff";
                    if (ofd.ShowDialog() == DialogResult.OK) {
                        TriangleModel model = p as TriangleModel;
                        model.Path = ofd.FileName;
                        LoadingModelDialog modelDlg = new LoadingModelDialog(model);
                        modelDlg.Show();
                    }
                }
                UnifiedScenesRepository.Scenes[this.TabText].Primitives.Add(p);
            } else if (obj is Scene) {
                DocumentWindow newForm = new DocumentWindow();
                obj = newForm.Scene;
                newForm.Show(this.DockPanel);
            } else if (obj is Material) {
                UnifiedScenesRepository.Scenes[this.TabText].Materials.Add((Material) obj);
            } else if (obj is Light) {
                UnifiedScenesRepository.Scenes[this.TabText].Lights.Add((Light) obj);
            } else if (obj is Camera) {
                UnifiedScenesRepository.Scenes[this.TabText].Cameras.Add((Camera) obj);
            }
            PropertyWindow.Instance.PropertyGrid.SelectedObject = obj;
            //PropertyWindow.Instance.PropertyGrid.SelectedObject = this.Tag;
            //Type type = Type.GetType(tItem.TypeName,false, true);   
            //PropertyWindow.Instance.PropertyGrid.SelectedObject =
            //Activator.CreateInstance(Type.GetType(tItem.TypeName), true);
        }

        private void DocumentWindow_Click(object sender, EventArgs e) {
            PropertyWindow.Instance.PropertyGrid.SelectedObject = UnifiedScenesRepository.Scenes[this.TabText];
            this.Activate();
        }

        private void panelRender_SizeChanged(object sender, EventArgs e) {
            int gapX = (this.Width - this.pictureView.Width) / 2;
            int gapY = (this.Height - this.pictureView.Height) / 2;
            if (gapX < 0) {
                gapX = 0;
            }
            if (gapY < 0) {
                gapY = 0;
            }
            this.pictureView.Location = new Point(gapX, gapY);
        }

        private void DocumentWindow_FormClosed(object sender, FormClosedEventArgs e) {
            this.tracer = null;
            UnifiedScenesRepository.Scenes.Remove(this.TabText);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void menuItemRender_Click(object sender, EventArgs e) {
            this.pictureView.Width = Convert.ToInt32(UnifiedScenesRepository.CurrentEditingScene.DefaultCamera.ResX);
            this.pictureView.Height = Convert.ToInt32(UnifiedScenesRepository.CurrentEditingScene.DefaultCamera.ResY);
            this.RenderScene();
            //this.timer.Stop();
        }

        public void RenderScene() {
            if (this.renderThread != null && this.renderThread.ThreadState == ThreadState.Running) {
                this.renderThread.Abort();
            }
            //System.Diagnostics.Process.GetCurrentProcess().PriorityBoostEnabled = true;
            //System.Diagnostics.Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            this.renderThread = new Thread(this.StartRender);
            this.renderThread.Start();
        }

        private void StartRender() {
            this.tracer.Scene = UnifiedScenesRepository.Scenes[this.TabText];
            this.pictureView.Image = new Bitmap((int) this.tracer.Scene.DefaultCamera.ResX,
                                                (int) this.tracer.Scene.DefaultCamera.ResY);
            Graphics g = Graphics.FromImage(this.pictureView.Image);
            timer.Start();
            this.msElapseds = 0;
            this.tracer.Render(g);
            g.Flush();
            if (this.pictureView.InvokeRequired) {
                this.pictureView.Invoke(new Action(this.pictureView.Refresh));
            }
            //this.pictureView.Refresh();
            this.timer.Stop();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e) {
            this.msElapseds += this.timer.Interval;
            if (this.statusBar.InvokeRequired) {
                this.lblTimeElapsed.Owner.Invoke(
                    new Action(
                        delegate { this.lblTimeElapsed.Text = TimeSpan.FromMilliseconds(this.msElapseds).ToString(); }));
            }
            if ((this.msElapseds % 500).NearZero()) {
                if (this.pictureView.InvokeRequired) {
                    this.pictureView.Invoke(new Action(this.pictureView.Refresh));
                }
            }
        }

        private void toolStripMenuItemStop_Click(object sender, EventArgs e) {
            if (this.renderThread != null && this.renderThread.ThreadState == ThreadState.Running) {
                this.renderThread.Abort();
                this.timer.Stop();
            }
        }

        private void menuItemCloseButThis_Click(object sender, EventArgs e) {
            IDockContent[] contents = this.DockHandler.DockPanel.Documents.ToArray();
            for (int i = 0; i < contents.Length; i++) {
                DockContent dc = (DockContent) contents[i];
                if (dc.TabText != this.TabText) {
                    dc.Close();
                }
            }
        }

        private void menuItemCloseAll_Click(object sender, EventArgs e) {
            IDockContent[] contents = this.DockHandler.DockPanel.Documents.ToArray();
            for (int i = 0; i < contents.Length; i++) {
                DockContent document = (DockContent) contents[i];
                document.Close();
            }
        }

        private void menuItemClose_Click(object sender, EventArgs e) {
            DockContent dc = null;
            bool found = false;
            foreach (IDockContent document in this.DockHandler.DockPanel.Documents) {
                dc = (document as DockContent);
                if (dc != null && dc.TabText == this.TabText) {
                    found = true;
                    break;
                }
            }
            if (found) {
                if (this.renderThread != null) {
                    this.renderThread.Abort();
                }
                if (this.Scene != null && this.Scene.Primitives != null) {
                    this.Scene.Primitives.Clear();
                }
                dc.Close();
            }
        }

        private void pictureView_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (this.tracer.Scene == null) {
                this.tracer.Scene = UnifiedScenesRepository.Scenes[this.TabText];
            }
            Ray ray = tracer.Scene.DefaultCamera.CreateRayFromScreen(e.X, e.Y);
            Intersection inter;
            if (this.Scene.FindIntersection(ray, out inter)) {
                PropertyWindow.Instance.PropertyGrid.SelectedObject = inter.HitPrimitive;
            }
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e) {
            FullScreenView fullScreen = new FullScreenView();
            //fullScreen.PictureBoxView = this.pictureView;
            //fullScreen.Tracer = tracer;
            fullScreen.ShowDialog();
        }

        private void DocumentWindow_FormClosing(object sender, FormClosingEventArgs e) {
            if (this.renderThread != null && this.renderThread.ThreadState == ThreadState.Running) {
                this.renderThread.Abort();
            }
            if (UnifiedScenesRepository.CurrentEditingScene != null) {
                UnifiedScenesRepository.CurrentEditingScene.Primitives.Clear();
                UnifiedScenesRepository.Scenes.Remove(UnifiedScenesRepository.CurrentEditingScene);
            }
            PropertyWindow.Instance.PropertyGrid.SelectedObject = null;
            this.Dispose();
        }

        private void pictureView_MouseDown(object sender, MouseEventArgs e) {
            this.tracer.Scene = UnifiedScenesRepository.Scenes[this.TabText];
            Ray ray = this.tracer.Scene.DefaultCamera.CreateRayFromScreen(e.X, e.Y);
            this.intersected = this.Scene.FindIntersection(ray, out this.current_intersection);
            this.prevPoint = new Point(e.X, e.Y);
        }

        private void pictureView_MouseUp(object sender, MouseEventArgs e) {
            if (this.intersected != null && !this.intersected.Value) {
                this.RenderScene();
            }
            this.intersected = null;
        }

        private void pictureView_MouseMove(object sender, MouseEventArgs e) {
            if (this.intersected.GetValueOrDefault()) {
                IPrimitive primitive = this.current_intersection.HitPrimitive;
                //Scene scene = UnifiedScenesRepository.Scenes[this.TabText];
                if (primitive is ITransformable3D) {
                    //Ray ray = scene.DefaultCamera.CreateRayFromScreen(e.X, e.Y);
                    float transX = (e.X - this.prevPoint.X);
                    float transY = (e.Y - this.prevPoint.Y);
                    ((ITransformable3D) primitive).Translate(transX, -transY, 0);
                    this.RenderScene();
                    this.prevPoint.X = e.X;
                    this.prevPoint.Y = e.Y;
                }
            }
        }

        private void toolStripAddFrame_Click(object sender, EventArgs e) {
            FrameViewForm.Instance.FrameView.AddFrame(new Bitmap(this.pictureView.Image));
        }

        private void toolStripRenderer_SelectedIndexChanged(object sender, EventArgs e) {
            RenderType renderType = this.toolStripRenderer.SelectedItem != null ? (RenderType)this.toolStripRenderer.SelectedItem : RenderType.RayTracer;
            StrategyType strategyType = this.toolStripStrategy.SelectedItem != null ? (StrategyType)this.toolStripStrategy.SelectedItem : StrategyType.Progressive;

            RenderStrategy strategy = new ProgressiveRenderStrategy();
            switch (strategyType) {
                case StrategyType.Scanline:
                    strategy = new ScanlineRenderStrategy();
                    break;
                case StrategyType.Progressive:
                    strategy = new ProgressiveRenderStrategy();
                    break;
            }
            switch (renderType)
            {
                case RenderType.RayTracer:
                    this.tracer = new RayTracer(UnifiedScenesRepository.Scenes[this.TabText], strategy);
                    break;
                case RenderType.PhotonTracer:
                    this.tracer = new PhotonTracer(UnifiedScenesRepository.Scenes[this.TabText], strategy, 500000);
                    break;
                case RenderType.DistributedRayTracer:
                    this.tracer = new DistributedRayTracer(UnifiedScenesRepository.Scenes[this.TabText], strategy);
                    break;
            }
        }
    }
}