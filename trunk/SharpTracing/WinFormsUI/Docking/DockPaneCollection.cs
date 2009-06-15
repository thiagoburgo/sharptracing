using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WeifenLuo.WinFormsUI.Docking
{
    public class DockPaneCollection : ReadOnlyCollection<DockPane>
    {
        internal DockPaneCollection() : base(new List<DockPane>()) {}
        internal int Add(DockPane pane)
        {
            if(this.Items.Contains(pane)){
                return this.Items.IndexOf(pane);
            }
            this.Items.Add(pane);
            return this.Count - 1;
        }
        internal void AddAt(DockPane pane, int index)
        {
            if(index < 0 || index > this.Items.Count - 1){
                return;
            }
            if(this.Contains(pane)){
                return;
            }
            this.Items.Insert(index, pane);
        }
        internal void Dispose()
        {
            for(int i = this.Count - 1; i >= 0; i--){
                this[i].Close();
            }
        }
        internal void Remove(DockPane pane)
        {
            this.Items.Remove(pane);
        }
    }
}