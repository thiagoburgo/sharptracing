// Bjoern Heckel's solution to the KD-Tree n-nearest-neighbor problem
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

namespace DrawEngine.Renderer.SpatialSubdivision.KDTree {
    public class NearestNeighborList<T> where T : class {
        private readonly int m_Capacity;
        private readonly PriorityQueue<NeighborEntry<T>> m_Queue;
        // constructor
        public NearestNeighborList(int capacity) {
            this.m_Capacity = capacity;
            this.m_Queue = new PriorityQueue<NeighborEntry<T>>(this.m_Capacity);
        }

        public double MaxPriority {
            get {
                NeighborEntry<T> p = this.m_Queue.Peek();
                return (p == null) ? double.PositiveInfinity : p.Value;
            }
        }

        public T Highest {
            get {
                NeighborEntry<T> p = this.m_Queue.Peek();
                return (p == null) ? null : p.Data;
            }
        }

        public bool IsEmpty {
            get { return this.m_Queue.Count == 0; }
        }

        public int Count {
            get { return this.m_Queue.Count; }
        }

        public bool Insert(T obj, double priority) {
            if (this.IsCapacityReached()) {
                if (priority > this.MaxPriority) {
                    // do not insert - all elements in queue have lower priority
                    return false;
                }
                this.m_Queue.Enqueue(new NeighborEntry<T>(obj, priority));
                // remove object with highest priority
                this.m_Queue.Dequeue();
            } else {
                this.m_Queue.Enqueue(new NeighborEntry<T>(obj, priority));
            }
            return true;
        }

        public bool IsCapacityReached() {
            return this.m_Queue.Count >= this.m_Capacity;
        }

        public T RemoveHighest() {
            // remove object with highest priority
            NeighborEntry<T> p = this.m_Queue.Dequeue();
            return (p == null) ? null : p.Data;
        }
    }

    public sealed class NeighborEntry<T> : IComparable<NeighborEntry<T>> {
        private readonly T data;
        private readonly double value;

        public NeighborEntry(T data, double value) {
            this.data = data;
            this.value = value;
        }

        public T Data {
            get { return this.data; }
        }

        public double Value {
            get { return this.value; }
        }

        #region IComparable<NeighborEntry<T>> Members

        public int CompareTo(NeighborEntry<T> t) {
            //note that the positions are reversed!
            return t.value.CompareTo(this.value);
        }

        #endregion
    }
}