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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Alsing.Windows.Forms;
using DrawEngine.PluginEngine;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using Microsoft.CSharp;
using WeifenLuo.WinFormsUI.Docking;

namespace DrawEngine.SharpTracingUI {
    public partial class ScriptingForm : DockContent {
        private static ScriptingForm instance;
        private Thread tRun;

        private ScriptingForm() {
            this.InitializeComponent();
            Stream stream =
                Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("DrawEngine.SharpTracingUI.ScriptingTemplate.cs");
            if (stream != null) {
                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream);
                String text = reader.ReadToEnd();
                this.syntaxBoxControl1.Document.Text = text;
                stream.Flush();
                stream.Close();
                reader.Close();
            }
        }

        public static ScriptingForm Instance {
            get {
                if (instance == null) {
                    instance = new ScriptingForm();
                }
                return instance;
            }
        }

        public SyntaxBoxControl SyntaxBox {
            get { return this.syntaxBoxControl1; }
        }

        public void FormatCode() {
            TextReader r = new StringReader(this.syntaxBoxControl1.Document.Text);
            IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, r);
            parser.Parse();
            List<ISpecial> specials = parser.Lexer.SpecialTracker.RetrieveSpecials();
            try {
                if (parser.Errors.Count == 0) {
                    CSharpOutputVisitor ov = new CSharpOutputVisitor();
                    SpecialNodesInserter.Install(specials, ov);
                    foreach (INode c in parser.CompilationUnit.Children) {
                        c.AcceptVisitor(ov, null);
                    }
                    this.syntaxBoxControl1.Document.Text = ov.Text;
                }
            } catch (Exception) {}
        }

        public void Run() {
            CSharpCodeProvider cs = new CSharpCodeProvider(new Dictionary<string, string> {{"CompilerVersion", "v3.5"}});
            if (this.tRun != null) {
                this.tRun.Abort();
            }
            CompilerParameters cp = new CompilerParameters();
            // Generate an executable instead of 
            // a class library.
            cp.GenerateInMemory = true;
            // Specify the assembly file name to generate.
            //cp.OutputAssembly = "Scripting.dll";
            cp.CompilerOptions = "/t:library";
            cp.IncludeDebugInformation = false;
            // Set whether to treat all warnings as errors.
            cp.TreatWarningsAsErrors = false;
            cp.ReferencedAssemblies.Add("mscorlib.dll");
            cp.ReferencedAssemblies.Add("System.Drawing.dll");
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("DrawEngine.Renderer.dll");
            cp.ReferencedAssemblies.Add("DrawEngine.Renderer.Animator.dll");
            cp.ReferencedAssemblies.Add("DrawEngine.PluginEngine.dll");
            cp.ReferencedAssemblies.Add("WeifenLuo.WinFormsUI.Docking.dll");
            cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            cp.ReferencedAssemblies.Add("DrawEngine.SharpTracingUI.exe");
            // Invoke compilation of the source file.
            CompilerResults cr = cs.CompileAssemblyFromSource(cp, this.syntaxBoxControl1.Document.Text);
            if (cr.Errors.Count > 0) {
                StringBuilder sb = new StringBuilder();
                foreach (CompilerError ce in cr.Errors) {
                    sb.AppendFormat("Linha: {0}, Coluna: {1} - {2}" + Environment.NewLine, ce.Line, ce.Column,
                                    ce.ErrorText);
                }
                MessageBox.Show(sb.ToString());
            } else {
                //Assembly loadeInDomain = domain.Load(File.ReadAllBytes("Scripting.dll"));
                //IPluggable pluggable = loadeInDomain.CreateInstance("ScriptingTemplate") as IPluggable;
                //this.TabText = pluggable.Name;
                //tRun = new Thread(new ThreadStart(pluggable.Run));
                //tRun.Start();
                Type[] types = cr.CompiledAssembly.GetExportedTypes();
                var result = types.First(t => ((Type) t).GetInterface("IPluggable") != null);
                IPluggable pluggable = cr.CompiledAssembly.CreateInstance(result.FullName) as IPluggable;
                this.TabText = pluggable.Name;
                this.tRun = new Thread(pluggable.Run);
                this.tRun.Start();
            }
        }
    }
}