using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DrawEngine.Renderer;
using DrawEngine.Renderer.Cameras;
using DrawEngine.Renderer.Lights;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.RenderObjects;
using DrawEngine.Renderer.RenderObjects.CSG;
using TooboxUI.Components;
using WeifenLuo.WinFormsUI.Docking;

namespace DrawEngine.SharpTracingUI
{
    public partial class ToolBoxWindow : DockContent
    {
        private static readonly object padlock = new object();
        private static ToolBoxWindow instance;
        private readonly HostToolbox toolbox1;
        private ToolBoxWindow()
        {
            this.InitializeComponent();
            this.toolbox1 = new HostToolbox(false){
                                    Dock = DockStyle.Fill, 
                                    AllowToolboxItems = true
                            };
            Assembly ass = Assembly.GetAssembly(typeof(Primitive));
          
            
            foreach(Type type in ass.GetExportedTypes().OrderBy(t => t.Name)) {
                if(!type.IsAbstract){
                    if(type.IsSubclassOf(typeof(Primitive))){
                        if(type.GetInterface(typeof(IConstrutive).FullName) == null){
                            this.toolbox1.AddToolboxItem(type, "Primitives");
                        }
                    }
                    if(type.IsSubclassOf(typeof(Material))){
                        this.toolbox1.AddToolboxItem(type, "Materials");
                    }
                    if(type.IsSubclassOf(typeof(Light))){
                        this.toolbox1.AddToolboxItem(type, "Lights");
                    }
                    if(type.IsSubclassOf(typeof(Camera))){
                        this.toolbox1.AddToolboxItem(type, "Cameras");
                    }
                }
            }
            this.toolbox1.AddToolboxItem(typeof(Scene), "Scene");
            this.Controls.Add(this.toolbox1);
        }
        public HostToolbox ToolBox
        {
            get { return this.toolbox1; }
        }
        public static ToolBoxWindow Instance
        {
            get
            {
                lock(padlock){
                    if(instance == null){
                        instance = new ToolBoxWindow();
                    }
                    return instance;
                }
            }
        }
        private void ToolBoxWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}