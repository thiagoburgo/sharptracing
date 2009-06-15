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
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(MainForm.Instance);
            //Application.Run(new Test.TestPerlinNoise());
            //DrawEngine.SharpTracing.Plugin.ScriptingTemplate template =
            //    new DrawEngine.SharpTracing.Plugin.ScriptingTemplate();
            //template.Run();
        }
    }
}