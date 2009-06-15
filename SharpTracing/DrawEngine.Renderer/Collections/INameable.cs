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