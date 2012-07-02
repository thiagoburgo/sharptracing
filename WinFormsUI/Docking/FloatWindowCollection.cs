using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WeifenLuo.WinFormsUI.Docking
{
    public class FloatWindowCollection : ReadOnlyCollection<FloatWindow>
    {
        internal FloatWindowCollection() : base(new List<FloatWindow>()) {}
        internal int Add(FloatWindow fw)
        {
            if(this.Items.Contains(fw)){
                return this.Items.IndexOf(fw);
            }
            this.Items.Add(fw);
            return this.Count - 1;
        }
        internal void Dispose()
        {
            for(int i = this.Count - 1; i >= 0; i--){
                this[i].Close();
            }
        }
        internal void Remove(FloatWindow fw)
        {
            this.Items.Remove(fw);
        }
        internal void BringWindowToFront(FloatWindow fw)
        {
            this.Items.Remove(fw);
            this.Items.Add(fw);
        }
    }
}