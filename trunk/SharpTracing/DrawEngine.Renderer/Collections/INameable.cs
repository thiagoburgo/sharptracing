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
using System.Collections.Generic;
using System.ComponentModel;

namespace DrawEngine.Renderer.Collections
{
    public delegate void NameChangedEventHandler(INameable sender, String oldName);

    public delegate void NameChangingEventHandler(INameable sender, CancelNameChageEventArgs e);

    public class CancelNameChageEventArgs : CancelEventArgs
    {
        private string newName;
        public CancelNameChageEventArgs(string newName) : base()
        {
            this.newName = newName;
        }
        public CancelNameChageEventArgs(string newName, bool cancel) : base(cancel)
        {
            this.newName = newName;
        }
        public string NewName
        {
            get { return this.newName; }
            set { this.newName = value; }
        }
    }

    public interface INameable : IComparer<INameable>
    {
        #region EVENTOS
        event NameChangedEventHandler OnNameChanged;
        event NameChangingEventHandler OnNameChanging;
        #endregion

        String Name { get; set; }
    }
}