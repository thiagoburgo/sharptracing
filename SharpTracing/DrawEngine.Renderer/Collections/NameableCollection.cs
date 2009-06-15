using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using DrawEngine.Renderer.Collections.Design;

namespace DrawEngine.Renderer.Collections
{
    [Serializable, Editor(typeof(CustomCollectionEditor), typeof(UITypeEditor))]
    public class NameableCollection<T> : NotifyList<T> where T : INameable
    {
        public NameableCollection() : base() {}
        public NameableCollection(IEnumerable<T> list) : base(list) {}
        public NameableCollection(int capacity) : base(capacity) {}
        public override T this[int index]
        {
            get { return base[index]; }
            set
            {
                if(String.IsNullOrEmpty(value.Name)){
                    value.Name = this.CreateName(value.GetType());
                }
                if(!this.ContainsName(value.Name)){
                    base[index] = value;
                } else{
                    throw new DuplicateNameException("Alredy exists a object with name \"" + value.Name + "\"!");
                }
            }
        }
        public T this[string name]
        {
            get
            {
                int index = this.IndexOf(name);
                return index > -1 ? base[index] : default(T);
            }
            set
            {
                if(String.IsNullOrEmpty(value.Name)){
                    value.Name = this.CreateName(value.GetType());
                }
                int index = this.IndexOf(name);
                if(index > -1){
                    base[index] = value;
                } else{
                    throw new DuplicateNameException("Alredy exists a object with name \"" + value.Name + "\"!");
                }
            }
        }
        public void Remove(string name)
        {
            int index = this.IndexOf(name);
            if(index > -1){
                this.RemoveAt(index);
            }
        }
        public override void Add(T item)
        {
            if(String.IsNullOrEmpty(item.Name)){
                item.Name = this.CreateName(item.GetType());
            }
            if(!this.ContainsName(item.Name)){
                base.Add(item);
            } else{
                throw new DuplicateNameException("Alredy exists a object with name \"" + item.Name + "\"!");
            }
        }
        public override void Insert(int index, T item)
        {
            if(String.IsNullOrEmpty(item.Name)){
                item.Name = this.CreateName(item.GetType());
            }
            if(!this.ContainsName(item.Name)){
                base.Insert(index, item);
            } else{
                throw new DuplicateNameException("Alredy exists a object with name \"" + item.Name + "\"!");
            }
        }
        public string CreateName(Type type)
        {
            int uniqueID = 1;
            // Create a basic type name string.
            string baseName = type.Name + uniqueID;
            // Continue to increment uniqueID numeral until a 
            // unique ID is located.
            while(this.ContainsName(baseName)){
                baseName = type.Name + uniqueID++;
            }
            return baseName;
        }
        public bool ContainsName(String name)
        {
            if(!String.IsNullOrEmpty(name)){
                foreach(var item in this){
                    if(item.Name == name){
                        return true;
                    }
                }
            }
            return false;
        }
        public int IndexOf(string name)
        {
            if(!String.IsNullOrEmpty(name)){
                for(int i = 0; i < this.Count; i++){
                    if(this[i].Name == name){
                        return i;
                    }
                }
            }
            return -1;
        }
    }

    //[Serializable, Editor(typeof(CustomCollectionEditor), typeof(UITypeEditor))]
    //public class NameableCollection<T> :  IList<T> where T : INameable
    //{
    //    #region Delegates
    //    public delegate void NameableEventHandler(INameable nameable);
    //    #endregion
    //    private Dictionary<string, T> nameables;
    //    public NameableCollection()
    //    {
    //        this.nameables = new Dictionary<string, T>();
    //    }
    //    public NameableCollection(int capacity)
    //    {
    //        this.nameables = new Dictionary<string, T>(capacity);
    //    }
    //    public NameableCollection(IEnumerable<T> collection)
    //    {
    //        this.nameables = new Dictionary<string, T>();
    //        foreach(T item in collection){
    //            this.nameables.Add(item.Name, item);
    //        }
    //    }
    //    public T this[string name]
    //    {
    //        get { return this.nameables[name]; }
    //        set { this.nameables[name] = value; }
    //    }
    //    #region IList<T> Members
    //    public int IndexOf(T item)
    //    {
    //        int i = 0;
    //        foreach(T newItem in this.nameables.Values){
    //            if(item.Name == newItem.Name){
    //                return i;
    //            }
    //            i++;
    //        }
    //        return -1;
    //    }
    //    public void Insert(int index, T item)
    //    {
    //        this[index] = item;
    //    }
    //    public void RemoveAt(int index)
    //    {
    //        this.nameables.Remove(this[index].Name);
    //    }
    //    public T this[int index]
    //    {
    //        get
    //        {
    //            int i = 0;
    //            foreach(T item in this.nameables.Values){
    //                if(i == index){
    //                    return item;
    //                }
    //                i++;
    //            }
    //            throw new IndexOutOfRangeException();
    //        }
    //        set
    //        {
    //            int i = 0;
    //            foreach(string nameItem in this.nameables.Keys){
    //                if(i == index){
    //                    this.nameables[nameItem] = value;
    //                    break;
    //                }
    //                i++;
    //            }
    //            throw new IndexOutOfRangeException();
    //        }
    //    }
    //    public void Add(T item)
    //    {
    //        if(string.IsNullOrEmpty(item.Name)){
    //            item.Name = NameFactory.CreateName(this, item.GetType());
    //        }
    //        if(!this.nameables.ContainsKey(item.Name)){
    //            this.nameables.Add(item.Name, item);
    //            if(this.OnNameableAdded != null){
    //                this.OnNameableAdded(item);
    //            }
    //        }
    //    }
    //    public void Clear()
    //    {
    //        this.nameables.Clear();
    //    }
    //    public bool Contains(T item)
    //    {
    //        return (item != null && this.nameables.ContainsValue(item));
    //    }
    //    public void CopyTo(T[] array, int arrayIndex)
    //    {
    //        this.nameables.Values.CopyTo(array, arrayIndex);
    //    }
    //    public int Count
    //    {
    //        get { return this.nameables.Count; }
    //    }
    //    public bool IsReadOnly
    //    {
    //        get { return false; }
    //    }
    //    public bool Remove(T item)
    //    {
    //        return this.Remove(item.Name);
    //    }
    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        return this.nameables.Values.GetEnumerator();
    //    }
    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this.nameables.Values.GetEnumerator();
    //    }
    //    #endregion
    //    public event NameableEventHandler OnNameableAdded;
    //    public event NameableEventHandler OnNameableRemoved;
    //    public override string ToString()
    //    {
    //        String shortName = typeof(T).Name.Replace(typeof(T).Assembly.FullName + ".", "");
    //        return shortName + "s";
    //    }
    //    public bool ContainsName(String name)
    //    {
    //        return (!String.IsNullOrEmpty(name) && this.nameables.ContainsKey(name));
    //    }
    //    public bool Remove(string name)
    //    {
    //        T removedItem;
    //        bool removed = this.nameables.TryGetValue(name, out removedItem);
    //        removed = removed && this.nameables.Remove(name);
    //        if(removed && this.OnNameableRemoved != null){
    //            this.OnNameableRemoved(removedItem);
    //        }
    //        return removed;
    //    }
    //}
}