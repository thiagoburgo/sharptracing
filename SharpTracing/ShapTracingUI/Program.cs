using System;
using System.Windows.Forms;

namespace DrawEngine.SharpTracingUI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException 
                += (sender, e)=> MessageBox.Show(e.ExceptionObject.ToString());
            Application.Run(MainForm.Instance);
            //Application.Run(new Test.TestPerlinNoise());
            //DrawEngine.SharpTracing.Plugin.ScriptingTemplate template =
            //    new DrawEngine.SharpTracing.Plugin.ScriptingTemplate();
            //template.Run();
        }

        
    }
}