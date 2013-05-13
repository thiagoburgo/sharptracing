using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawEngine.Renderer.Collections
{
    public class FixedSizedQueue<T> : ConcurrentQueue<T>
    {
        public int MaxSize { get; private set; }

        public FixedSizedQueue(int maxSize)
        {
            this.MaxSize = maxSize;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (this)
            {
                while (base.Count > this.MaxSize)
                {
                    T outObj;
                    base.TryDequeue(out outObj);
                }
            }
        }
    }
}
