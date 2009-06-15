using System;
using System.Windows.Forms;

namespace PhotonVisualization
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new Form1());
            // Make the form.
            RenderForm frm = new RenderForm();
            // Initialize Direct3D.
            if(frm.InitializeGraphics()){
                frm.Show();
                // While the form is valid,
                // render the scene and process messages.
                while(frm.Created){
                    frm.Render();
                    Application.DoEvents();
                }
            }
        }
    }
}