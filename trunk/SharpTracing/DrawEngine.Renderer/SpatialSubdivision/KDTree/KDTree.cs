using System;
using System.Collections.Generic;

namespace DrawEngine.Renderer.SpatialSubdivision.KDTree
{
    /// <summary>
    /// KDTree is a public class supporting KD-tree insertion, deletion, equality search,
    /// range search, and nearest neighbor(s) using double-precision floating-point
    /// keys. Splitting dimension is chosen naively, by depth modulo K. Semantics are
    /// as follows:
    /// <list type="bullet">
    ///     <item><description>Two different keys containing identical numbers should retrieve the same value from a given KD-tree.
    ///  Therefore keys are cloned when a node is inserted.</description> </item>
    /// <item><description>As with Hashtables, values inserted into a KD-tree are <I>not</I> cloned.
    /// Modifying a value between insertion and retrieval will therefore modify the 
    /// value stored in the tree.</description> </item>
    /// </list>
    ///  : the Nearest Neighbor algorithm (Table 6.4) of
    ///<code>
    /// techreport{AndrewMooreNearestNeighbor,
    ///   author  = {Andrew Moore},
    ///   title   = {An introductory tutorial on kd-trees},
    ///   institution = {Robotics Institute, Carnegie Mellon University},
    ///   year    = {1991},
    ///   number  = {Technical Report No. 209, Computer Laboratory, 
    ///              University of Cambridge},
    ///   address = {Pittsburgh, PA}
    /// }
    /// </code>
    /// </summary>
    /// <typeparam name="T">Type of a kdtree values</typeparam>
    public class KDTree<T>
    {
        // K = number of dimensions
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly int m_K;
        // number of milliseconds
        private readonly long m_timeout;
        // count of nodes
        private int m_count;
        // root of KD-tree
        private KDNode<T> m_root;
        /// <summary>
        /// Creates a KD-tree with specified number of dimensions. 
        /// </summary>
        /// <param name="k">number of dimensions</param>
        public KDTree(int k) : this(k, 0) {}
        public KDTree(int k, long timeout)
        {
            this.m_timeout = timeout;
            this.m_K = k;
            this.m_root = null;
        }
        public int Count
        {
            get
            {
                /* added by MSL */
                return this.m_count;
            }
        }
        /// <summary>
        /// Insert a node in a KD-tree. Uses algorithm translated from 352.ins.c of
        /// Book{GonnetBaezaYates1991,                                   
        ///    author =    {G.H. Gonnet and R. Baeza-Yates},
        ///    title =     {Handbook of Algorithms and Data Structures},
        ///    publisher = {Addison-Wesley},
        ///    year =      {1991}
        ///}
        /// </summary>
        /// <param name="key">key for KD-tree node</param>
        /// <param name="value">value at that key</param>
        /// <exception cref="KeySizeException">if key.Length mismatches K</exception>
        /// <exception cref="KeyDuplicateException">if key already in tree</exception>
        public void Insert(double[] key, T value)
        {
            this.Edit(key, new Inserter<T>(value));
        }
        /// <summary>
        /// Edit a node in a KD-tree
        /// </summary>
        /// <param name="key">key for KD-tree node</param>
        /// <param name="editor">object to edit the value at that key</param>
        /// <exception cref="KeySizeException">if key.Length mismatches K</exception>
        /// <exception cref="KeyDuplicateException">if key already in tree</exception>
        public void Edit(double[] key, IEditor<T> editor)
        {
            if(key.Length != this.m_K){
                throw new KeySizeException();
            }
            lock(this){
                // the first insert has to be lock
                if(null == this.m_root){
                    this.m_root = KDNode<T>.Create(new HPoint(key), editor);
                    this.m_count = this.m_root.Deleted ? 0 : 1;
                    return;
                }
            }
            this.m_count += KDNode<T>.Edit(new HPoint(key), editor, this.m_root, 0, this.m_K);
        }
        /// <summary>
        ///Find KD-tree node whose key is identical to key. Uses algorithm
        ///translated from 352.srch.c of Gonnet & Baeza-Yates. 
        /// </summary>
        /// <param name="key">key for KD-tree node</param>
        /// <returns>Element associated to key</returns>
        /// <exception cref="KeySizeException">if key.Length mismatches K</exception>
        public T Search(double[] key)
        {
            if(key.Length != this.m_K){
                throw new KeySizeException();
            }
            KDNode<T> kd = KDNode<T>.Search(new HPoint(key), this.m_root, this.m_K);
            return (kd == null ? default(T) : kd.Value);
        }
        public void Delete(double[] key)
        {
            this.Delete(key, false);
        }
        /// <summary>
        /// Delete a node from a KD-tree. Instead of actually deleting node and
        /// rebuilding tree, marks node as deleted. Hence, it is up to the caller to
        /// rebuild the tree as needed for efficiency.
        /// </summary>
        /// <param name="key">key for KD-tree node</param>
        /// <param name="optional">if false and node not found, throw an exception</param>
        /// <exception cref="KeyMissingException">if no node in tree has key</exception>
        /// <exception cref="KeySizeException">if key.Length mismatches K</exception>
        public void Delete(double[] key, bool optional)
        {
            if(key.Length != this.m_K){
                throw new KeySizeException();
            }
            KDNode<T> t = KDNode<T>.Search(new HPoint(key), this.m_root, this.m_K);
            if(t == null){
                if(optional == false){
                    throw new KeyMissingException();
                }
            } else{
                if(KDNode<T>.Delete(t)){
                    this.m_count--;
                }
            }
        }
        /// <summary>
        /// Find KD-tree node whose key is nearest neighbor to key.
        /// </summary>
        /// <param name="key">key for KD-tree node</param>
        /// <returns>object at node nearest to key, or null on failure</returns>
        /// <exception cref="KeySizeException">if key.Length mismatches K</exception>
        public T Nearest(double[] key)
        {
            List<T> nbrs = this.Nearest(key, 1, null);
            return nbrs[0];
        }
        /// <summary>
        /// Find KD-tree nodes whose keys are <i>n</i> nearest neighbors to key.
        /// </summary>
        /// <param name="key">key for KD-tree node</param>
        /// <param name="n">number of nodes to return</param>
        /// <returns>objects at nodes nearest to key, or null on failure</returns>
        /// <exception cref="KeySizeException">if key.Length mismatches K</exception>
        public List<T> Nearest(double[] key, int n)
        {
            return this.Nearest(key, n, null);
        }
        /// <summary>
        /// Find KD-tree nodes whose keys are within a given Euclidean distance (less than or equals) of a given key.
        /// </summary>
        /// <param name="key">key for KD-tree node</param>
        /// <param name="dist">Euclidean distance</param>
        /// <returns>objects at nodes with distance of key, or null on failure</returns>
        /// <exception cref="KeySizeException">if key.Length mismatches K</exception>
        public List<T> NearestEuclidean(double[] key, double dist)
        {
            return this.nearestDistance(key, dist, new EuclideanDistance());
        }
        /// <summary>
        /// Find KD-tree nodes whose keys are within a given Hamming distance (less than or equals) of a given key.
        /// </summary>
        /// <param name="key">key for KD-tree node</param>
        /// <param name="dist">Hamming distance</param>
        /// <returns>objects at nodes with distance of key, or null on failure</returns>
        /// <exception cref="KeySizeException">if key.Length mismatches K</exception>
        public List<T> NearestHamming(double[] key, double dist)
        {
            return this.nearestDistance(key, dist, new HammingDistance());
        }
        /// <summary>
        /// Find KD-tree nodes whose keys are <I>n</I> nearest neighbors to key. Uses
        /// algorithm above. Neighbors are returned in ascending order of distance to
        /// key.
        /// </summary>
        /// <param name="key">key for KD-tree node</param>
        /// <param name="n">how many neighbors to find</param>
        /// <param name="checker">an optional object to filter matches</param>
        /// <returns>objects at node nearest to key, or null on failure</returns>
        /// <exception cref="KeySizeException">if key.Length mismatches K</exception>
        public List<T> Nearest(double[] key, int n, Predicate<T> checker)
        {
            if(n <= 0){
                return new List<T>();
            }
            NearestNeighborList<KDNode<T>> nnl = this.getNbrs(key, n, checker);
            n = nnl.Count;
            List<T> nbrs = new List<T>();
            for(int i = 0; i < n; ++i){
                KDNode<T> kd = nnl.RemoveHighest();
                nbrs.Add(kd.Value);
            }
            //nbrs.Reverse();
            return nbrs;
        }
        /// <summary>
        /// Range search in a KD-tree. Uses algorithm translated from 352.range.c of
        /// Gonnet & Baeza-Yates.
        /// </summary>
        /// <param name="lowk">lower-bounds for key</param>
        /// <param name="uppk">upper-bounds for key</param>
        /// <returns>array of Objects whose keys fall in range [lowk,uppk]</returns>
        /// <exception cref="KeySizeException">on mismatch among lowk.Length, uppk.Length, or K</exception>
        public List<T> Range(double[] lowk, double[] uppk)
        {
            if(lowk.Length != uppk.Length){
                throw new KeySizeException();
            } else if(lowk.Length != this.m_K){
                throw new KeySizeException();
            } else{
                List<KDNode<T>> found = new List<KDNode<T>>();
                KDNode<T>.RangeSearch(new HPoint(lowk), new HPoint(uppk), this.m_root, 0, this.m_K, found);
                List<T> o = new List<T>();
                foreach(KDNode<T> node in found){
                    o.Add(node.Value);
                }
                return o;
            }
        }
        public override String ToString()
        {
            return this.m_root.ToString(0);
        }
        private NearestNeighborList<KDNode<T>> getNbrs(double[] key)
        {
            return this.getNbrs(key, this.m_count, null);
        }
        private NearestNeighborList<KDNode<T>> getNbrs(double[] key, int n, Predicate<T> checker)
        {
            if(key.Length != this.m_K){
                throw new KeySizeException();
            }
            NearestNeighborList<KDNode<T>> nnl = new NearestNeighborList<KDNode<T>>(n);
            // initial call is with infinite hyper-rectangle and max distance
            HRect hr = HRect.InfiniteHRect(key.Length);
            double max_dist_sqd = Double.MaxValue;
            HPoint keyp = new HPoint(key);
            if(this.m_count > 0){
                long timeout = (this.m_timeout > 0) ? (CurrentTimeMillis() + this.m_timeout) : 0;
                KDNode<T>.NearestNeighbors(this.m_root, keyp, hr, max_dist_sqd, 0, this.m_K, nnl, checker, timeout);
            }
            return nnl;
        }
        private List<T> nearestDistance(double[] key, double dist, DistanceMetric metric)
        {
            NearestNeighborList<KDNode<T>> nnl = this.getNbrs(key);
            int n = nnl.Count;
            List<T> nbrs = new List<T>();
            for(int i = 0; i < n; ++i){
                KDNode<T> kd = nnl.RemoveHighest();
                HPoint p = kd.Key;
                //HACK metric.Distance(kd.Key.Coord, key) < dist changed to - metric.Distance(kd.Key.Coord, key) <= dist
                if(metric.Distance(kd.Key.Coord, key) <= dist){
                    nbrs.Add(kd.Value);
                }
            }
            //HACK Verificar se o reverse é necessario
            //nbrs.Reverse();
            return nbrs;
        }
        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }
}