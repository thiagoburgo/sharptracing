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
using System.Text;
 using DrawEngine.PluginEngine;
 using DrawEngine.Renderer;
 using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Cameras;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.Lights;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.RenderObjects;
using DrawEngine.Renderer.RenderObjects.Design;
using DrawEngine.Renderer.Tracers;
using DrawEngine.SharpTracingUI;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

public class ScriptingTemplate : IPluggable {

    #region IPluggable Members

    public void Run() {
        try {
            Scene scene = UnifiedScenesRepository.Scenes["DistributedRT"];
            //Scene scene = Scene.Load(@"D:\Models & Textures\HandAnimation.xml");
            //Scene scene = Scene.Load(@"c:\temp\Scene1.xml");

            if(scene != null) {
                RayTracer tracer = new RayTracer(scene);
                //scene.DefaultCamera = scene.Cameras["MinhaCamera"];
                foreach(Primitive primi in scene.Primitives) {
                    if(primi is TriangleModel) {
                        ((TriangleModel)primi).Load();
                    }
                }
                using(Bitmap bFrame = new Bitmap((int)scene.DefaultCamera.ResX, (int)scene.DefaultCamera.ResY)) {
                    Point3D eye = scene.DefaultCamera.Eye;
                    float degreesToRotate = 0.5f;
                    int count = 0;
                    for(float i = 0; i < 360.0; i += degreesToRotate) {
                        //2PI = 360
                        //xpi = 0.1 = 0.2PI = x * 360 = x = 0.2/360
                        eye.RotateAxisY(((2f * (float)Math.PI) * degreesToRotate / 360.0f));
                        scene.DefaultCamera.Eye = eye;

                        tracer.Render(Graphics.FromImage(bFrame));
                        //FrameViewForm.Instance.FrameView.AddFrame(bFrame);

                        bFrame.Save(@"d:\temp\frames\frame_" + count + ".png", ImageFormat.Png);
                        count++;
                    }
                }
            }
        }
        catch(Exception ex) {

            MessageBox.Show("Erro de Execução: " + ex.Message);

        }

    }

    public string Description {
        get { return "Your Description"; }
    }

    public string Name {
        get { return "Your Name"; }
    }

    #endregion

    #region IDisposable Members

    public void Dispose() {

    }

    #endregion
}

