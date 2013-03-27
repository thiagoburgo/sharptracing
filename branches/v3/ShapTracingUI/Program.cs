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

namespace DrawEngine.SharpTracingUI {
    internal static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(MainForm.Instance);
            //Application.Run(new Test.TestPerlinNoise());
            //DrawEngine.SharpTracing.Plugin.ScriptingTemplate template =
            //    new DrawEngine.SharpTracing.Plugin.ScriptingTemplate();
            //template.Run();
        }
    }
}