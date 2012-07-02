using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WeifenLuo.WinFormsUI.Docking
{
    public class DockContentCollection : ReadOnlyCollection<IDockContent>
    {
        private static List<IDockContent> _emptyList = new List<IDockContent>(0);
        private DockPane m_dockPane = null;
        internal DockContentCollection() : base(new List<IDockContent>()) {}
        internal DockContentCollection(DockPane pane) : base(_emptyList)
        {
            this.m_dockPane = pane;
        }
        private DockPane DockPane
        {
            get { return this.m_dockPane; }
        }
        public new IDockContent this[int index]
        {
            get
            {
                if(this.DockPane == null){
                    return this.Items[index] as IDockContent;
                } else{
                    return this.GetVisibleContent(index);
                }
            }
        }
        public new int Count
        {
            get
            {
                if(this.DockPane == null){
                    return base.Count;
                } else{
                    return this.CountOfVisibleContents;
                }
            }
        }
        private int CountOfVisibleContents
        {
            get
            {
#if DEBUG
				if (DockPane == null)
					throw new InvalidOperationException();
#endif
                int count = 0;
                foreach(IDockContent content in this.DockPane.Contents){
                    if(content.DockHandler.DockState == this.DockPane.DockState){
                        count++;
                    }
                }
                return count;
            }
        }
        internal int Add(IDockContent content)
        {
#if DEBUG
			if (DockPane != null)
				throw new InvalidOperationException();
#endif
            if(this.Contains(content)){
                return this.IndexOf(content);
            }
            this.Items.Add(content);
            return this.Count - 1;
        }
        internal void AddAt(IDockContent content, int index)
        {
#if DEBUG
			if (DockPane != null)
				throw new InvalidOperationException();
#endif
            if(index < 0 || index > this.Items.Count - 1){
                return;
            }
            if(this.Contains(content)){
                return;
            }
            this.Items.Insert(index, content);
        }
        public new bool Contains(IDockContent content)
        {
            if(this.DockPane == null){
                return this.Items.Contains(content);
            } else{
                return (this.GetIndexOfVisibleContents(content) != -1);
            }
        }
        public new int IndexOf(IDockContent content)
        {
            if(this.DockPane == null){
                if(!this.Contains(content)){
                    return -1;
                } else{
                    return this.Items.IndexOf(content);
                }
            } else{
                return this.GetIndexOfVisibleContents(content);
            }
        }
        internal void Remove(IDockContent content)
        {
            if(this.DockPane != null){
                throw new InvalidOperationException();
            }
            if(!this.Contains(content)){
                return;
            }
            this.Items.Remove(content);
        }
        private IDockContent GetVisibleContent(int index)
        {
#if DEBUG
			if (DockPane == null)
				throw new InvalidOperationException();
#endif
            int currentIndex = -1;
            foreach(IDockContent content in this.DockPane.Contents){
                if(content.DockHandler.DockState == this.DockPane.DockState){
                    currentIndex++;
                }
                if(currentIndex == index){
                    return content;
                }
            }
            throw (new ArgumentOutOfRangeException());
        }
        private int GetIndexOfVisibleContents(IDockContent content)
        {
#if DEBUG
			if (DockPane == null)
				throw new InvalidOperationException();
#endif
            if(content == null){
                return -1;
            }
            int index = -1;
            foreach(IDockContent c in this.DockPane.Contents){
                if(c.DockHandler.DockState == this.DockPane.DockState){
                    index++;
                    if(c == content){
                        return index;
                    }
                }
            }
            return -1;
        }
    }
}