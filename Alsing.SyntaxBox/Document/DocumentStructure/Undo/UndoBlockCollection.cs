// *
// * Copyright (C) 2008 Roger Alsing : http://www.RogerAlsing.com
// *
// * This library is free software; you can redistribute it and/or modify it
// * under the terms of the GNU Lesser General Public License 2.1 or later, as
// * published by the Free Software Foundation. See the included license.txt
// * or http://www.gnu.org/copyleft/lesser.html for details.
// *
// *
using System;
using System.Collections;
using T = Alsing.SourceCode.UndoBlock;

namespace Alsing.SourceCode
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UndoBlockCollection : IList, ICloneable
    {
        private const int DefaultMinimumCapacity = 16;
        private UndoBlock[] m_array = new UndoBlock[DefaultMinimumCapacity];
        private int m_count;
        private int m_version;
        /// <summary>
        /// 
        /// </summary>
        public string Name = "UndoAction";
        // Construction
        /// <summary>
        /// 
        /// </summary>
        public UndoBlockCollection() {}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public UndoBlockCollection(UndoBlockCollection collection)
        {
            AddRange(collection);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        public UndoBlockCollection(UndoBlock[] array)
        {
            AddRange(array);
        }
        /// <summary>
        /// 
        /// </summary>
        public UndoBlock this[int index]
        {
            get
            {
                this.ValidateIndex(index); // throws
                return this.m_array[index];
            }
            set
            {
                this.ValidateIndex(index); // throws
                ++this.m_version;
                this.m_array[index] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Capacity
        {
            get { return this.m_array.Length; }
            set
            {
                if(value < this.m_count){
                    value = this.m_count;
                }
                if(value < DefaultMinimumCapacity){
                    value = DefaultMinimumCapacity;
                }
                if(this.m_array.Length == value){
                    return;
                }
                ++this.m_version;
                var temp = new UndoBlock[value];
                // for (int i=0; i < m_count; ++i) temp[i] = m_array[i];
                Array.Copy(this.m_array, 0, temp, 0, this.m_count);
                this.m_array = temp;
            }
        }

        #region ICloneable Members
        object ICloneable.Clone()
        {
            return (this.Clone());
        }
        #endregion

        // Operations (type-safe ICollection)

        #region IList Members
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return this.m_count; }
        }
        bool ICollection.IsSynchronized
        {
            get { return this.m_array.IsSynchronized; }
        }
        object ICollection.SyncRoot
        {
            get { return this.m_array.SyncRoot; }
        }
        void ICollection.CopyTo(Array array, int start)
        {
            this.CopyTo((UndoBlock[])array, start);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this.GetEnumerator());
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            ++this.m_version;
            this.m_array = new UndoBlock[DefaultMinimumCapacity];
            this.m_count = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this.ValidateIndex(index); // throws
            ++this.m_version;
            this.m_count--;
            // for (int i=index; i < m_count; ++i) m_array[i] = m_array[i+1];
            Array.Copy(this.m_array, index + 1, this.m_array, index, this.m_count - index);
            if(this.NeedsTrimming()){
                this.Trim();
            }
        }
        bool IList.IsFixedSize
        {
            get { return false; }
        }
        bool IList.IsReadOnly
        {
            get { return false; }
        }
        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (UndoBlock)value; }
        }
        int IList.Add(object item)
        {
            return this.Add((UndoBlock)item);
        }
        /* redundant w/ type-safe method
    void IList.Clear()
    {
    this.Clear();
    }
     */
        bool IList.Contains(object item)
        {
            return this.Contains((UndoBlock)item);
        }
        int IList.IndexOf(object item)
        {
            return this.IndexOf((UndoBlock)item);
        }
        void IList.Insert(int position, object item)
        {
            this.Insert(position, (UndoBlock)item);
        }
        void IList.Remove(object item)
        {
            this.Remove((UndoBlock)item);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        public void CopyTo(UndoBlock[] array)
        {
            this.CopyTo(array, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="start"></param>
        public void CopyTo(UndoBlock[] array, int start)
        {
            if(this.m_count > array.GetUpperBound(0) + 1 - start){
                throw new ArgumentException("Destination array was not long enough.");
            }
            // for (int i=0; i < m_count; ++i) array[start+i] = m_array[i];
            Array.Copy(this.m_array, 0, array, start, this.m_count);
        }
        // Operations (type-safe IList)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int Add(UndoBlock item)
        {
            if(this.NeedsGrowth()){
                this.Grow();
            }
            ++this.m_version;
            this.m_array[this.m_count] = item;
            return this.m_count++;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(UndoBlock item)
        {
            return ((this.IndexOf(item) == - 1) ? false : true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(UndoBlock item)
        {
            for(int i = 0; i < this.m_count; ++i){
                if(this.m_array[i] == (item)){
                    return i;
                }
            }
            return - 1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="item"></param>
        public void Insert(int position, UndoBlock item)
        {
            this.ValidateIndex(position, true); // throws
            if(this.NeedsGrowth()){
                this.Grow();
            }
            ++this.m_version;
            // for (int i=m_count; i > position; --i) m_array[i] = m_array[i-1];
            Array.Copy(this.m_array, position, this.m_array, position + 1, this.m_count - position);
            this.m_array[position] = item;
            this.m_count++;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Remove(UndoBlock item)
        {
            int index = this.IndexOf(item);
            if(index < 0){
                throw new ArgumentException(
                        "Cannot remove the specified item because it was not found in the specified Collection.");
            }
            this.RemoveAt(index);
        }
        // Operations (type-safe IEnumerable)
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
        // Operations (type-safe ICloneable)
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public UndoBlockCollection Clone()
        {
            var tc = new UndoBlockCollection();
            tc.AddRange(this);
            tc.Capacity = this.m_array.Length;
            tc.m_version = this.m_version;
            return tc;
        }
        // Public helpers (just to mimic some nice features of ArrayList)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(UndoBlockCollection collection)
        {
            // for (int i=0; i < collection.Count; ++i) Add(collection[i]);
            ++this.m_version;
            this.Capacity += collection.Count;
            Array.Copy(collection.m_array, 0, this.m_array, this.m_count, collection.m_count);
            this.m_count += collection.Count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        public void AddRange(UndoBlock[] array)
        {
            // for (int i=0; i < array.Length; ++i) Add(array[i]);
            ++this.m_version;
            this.Capacity += array.Length;
            Array.Copy(array, 0, this.m_array, this.m_count, array.Length);
            this.m_count += array.Length;
        }
        // Implementation (helpers)
        private void ValidateIndex(int index)
        {
            this.ValidateIndex(index, false);
        }
        private void ValidateIndex(int index, bool allowEqualEnd)
        {
            int max = (allowEqualEnd) ? (this.m_count) : (this.m_count - 1);
            if(index < 0 || index > max){
                throw new ArgumentOutOfRangeException(
                        "Index was out of range.  Must be non-negative and less than the size of the collection.", index,
                        "Specified argument was out of the range of valid values.");
            }
        }
        private bool NeedsGrowth()
        {
            return (this.m_count >= this.Capacity);
        }
        private void Grow()
        {
            if(this.NeedsGrowth()){
                this.Capacity = this.m_count * 2;
            }
        }
        private bool NeedsTrimming()
        {
            return (this.m_count <= this.Capacity / 2);
        }
        private void Trim()
        {
            if(this.NeedsTrimming()){
                this.Capacity = this.m_count;
            }
        }
        // Implementation (ICollection)
        /* redundant w/ type-safe method
    int ICollection.Count
    {
    get
    { return m_count; }
    }
     */
        // Nested enumerator class

        #region Nested type: Enumerator
        /// <summary>
        /// 
        /// </summary>
        public class Enumerator : IEnumerator
        {
            private readonly UndoBlockCollection m_collection;
            private readonly int m_version;
            private int m_index;
            // Construction
            public Enumerator(UndoBlockCollection tc)
            {
                this.m_collection = tc;
                this.m_index = - 1;
                this.m_version = tc.m_version;
            }
            // Operations (type-safe IEnumerator)
            /// <summary>
            /// 
            /// </summary>
            public UndoBlock Current
            {
                get { return this.m_collection[this.m_index]; }
            }

            #region IEnumerator Members
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if(this.m_version != this.m_collection.m_version){
                    throw new InvalidOperationException(
                            "Collection was modified; enumeration operation may not execute.");
                }
                ++this.m_index;
                return (this.m_index < this.m_collection.Count) ? true : false;
            }
            /// <summary>
            /// 
            /// </summary>
            public void Reset()
            {
                if(this.m_version != this.m_collection.m_version){
                    throw new InvalidOperationException(
                            "Collection was modified; enumeration operation may not execute.");
                }
                this.m_index = - 1;
            }
            // Implementation (IEnumerator)
            object IEnumerator.Current
            {
                get { return (this.Current); }
            }
            #endregion

            /* redundant w/ type-safe method
      bool IEnumerator.MoveNext()
      {
      return this.MoveNext();
      }
       */
            /* redundant w/ type-safe method
      void IEnumerator.Reset()
      {
      this.Reset();
      }
       */
        }
        #endregion
    }
}