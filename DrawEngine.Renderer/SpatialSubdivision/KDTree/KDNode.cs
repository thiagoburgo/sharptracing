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

namespace DrawEngine.Renderer.SpatialSubdivision.KDTree
{
    /// <summary>
    /// K-D Tree node class
    /// </summary>
    /// <typeparam name="T">Type of "value" node</typeparam>
    public class KDNode<T>
    {
        // these are seen by KDTree
        private readonly HPoint key;
        private bool deleted;
        private KDNode<T> left, right;
        private T value;
        private KDNode(HPoint key, T val)
        {
            this.key = key;
            this.value = val;
            this.left = null;
            this.right = null;
            this.deleted = false;
        }
        public HPoint Key
        {
            get { return this.key; }
        }
        public T Value
        {
            get { return this.value; }
        }
        public bool Deleted
        {
            get { return this.deleted; }
        }
        public KDNode<T> Right
        {
            get { return this.right; }
        }
        public KDNode<T> Left
        {
            get { return this.left; }
        }
        // Method ins translated from 352.ins.c of Gonnet & Baeza-Yates
        public static int Edit<T>(HPoint key, IEditor<T> editor, KDNode<T> t, int lev, int k)
        {
            KDNode<T> next_node;
            int next_lev = (lev + 1) % k;
            lock(t){
                if(key.Equals(t.key)){
                    bool was_deleted = t.deleted;
                    t.value = editor.Edit(t.deleted ? default(T) : t.value);
                    t.deleted = (t.value == null);
                    if(t.deleted == was_deleted){
                        return 0;
                    } else if(was_deleted){
                        return -1;
                    }
                    return 1;
                } else if(key.Coord[lev] > t.key.Coord[lev]){
                    next_node = t.right;
                    if(next_node == null){
                        t.right = Create(key, editor);
                        return t.right.deleted ? 0 : 1;
                    }
                } else{
                    next_node = t.left;
                    if(next_node == null){
                        t.left = Create(key, editor);
                        return t.left.deleted ? 0 : 1;
                    }
                }
            }
            return Edit(key, editor, next_node, next_lev, k);
        }
        public static KDNode<T> Create<T>(HPoint key, IEditor<T> editor)
        {
            KDNode<T> t = new KDNode<T>(key, editor.Edit(default(T)));
            if(Equals(t.value, default(T))){
                t.deleted = true;
            }
            return t;
        }
        public static bool Delete<T>(KDNode<T> t)
        {
            lock(t){
                if(!t.deleted){
                    t.deleted = true;
                    return true;
                }
            }
            return false;
        }
        // Method srch translated from 352.srch.c of Gonnet & Baeza-Yates
        public static KDNode<T> Search<T>(HPoint key, KDNode<T> t, int K)
        {
            for(int lev = 0; t != null; lev = (lev + 1) % K){
                if(!t.deleted && key.Equals(t.key)){
                    return t;
                } else if(key.Coord[lev] > t.key.Coord[lev]){
                    t = t.right;
                } else{
                    t = t.left;
                }
            }
            return null;
        }
        // Method rsearch translated from 352.range.c of Gonnet & Baeza-Yates
        public static void RangeSearch<T>(HPoint lowk, HPoint uppk, KDNode<T> t, int lev, int k, List<KDNode<T>> v)
        {
            if(t == null){
                return;
            }
            if(lowk.Coord[lev] <= t.key.Coord[lev]){
                RangeSearch(lowk, uppk, t.left, (lev + 1) % k, k, v);
            }
            if(!t.deleted){
                int j = 0;
                while(j < k && lowk.Coord[j] <= t.key.Coord[j] && uppk.Coord[j] >= t.key.Coord[j]){
                    j++;
                }
                if(j == k){
                    v.Add(t);
                }
            }
            if(uppk.Coord[lev] > t.key.Coord[lev]){
                RangeSearch(lowk, uppk, t.right, (lev + 1) % k, k, v);
            }
        }
        // Method Nearest Neighbor from Andrew Moore's thesis. Numbered
        // comments are direct quotes from there. NearestNeighborList solution
        // courtesy of Bjoern Heckel.
        public static void NearestNeighbors<T>(KDNode<T> kd, HPoint target, HRect hr, double max_dist_sqd, int lev,
                                               int K, NearestNeighborList<KDNode<T>> nnl, Predicate<T> checker,
                                               long timeout)
        {
            // 1. if kd is empty then set dist-sqd to infinity and exit.
            if(kd == null){
                return;
            }
            if((timeout > 0) && (timeout < DateTime.Now.TimeOfDay.TotalMilliseconds)){
                return;
            }
            // 2. s := split field of kd
            int s = lev % K;
            // 3. pivot := dom-elt field of kd
            HPoint pivot = kd.key;
            double pivot_to_target = HPoint.SqrDist(pivot, target);
            // 4. Cut hr into to sub-hyperrectangles left-hr and right-hr.
            // The cut plane is through pivot and perpendicular to the s
            // dimension.
            HRect left_hr = hr; // optimize by not cloning
            HRect right_hr = (HRect)hr.Clone();
            left_hr.Max.Coord[s] = pivot.Coord[s];
            right_hr.Min.Coord[s] = pivot.Coord[s];
            // 5. target-in-left := target_s <= pivot_s
            bool target_in_left = target.Coord[s] < pivot.Coord[s];
            KDNode<T> nearer_kd;
            HRect nearer_hr;
            KDNode<T> further_kd;
            HRect further_hr;
            // 6. if target-in-left then
            // 6.1. nearer-kd := left field of kd and nearer-hr := left-hr
            // 6.2. further-kd := right field of kd and further-hr := right-hr
            if(target_in_left){
                nearer_kd = kd.left;
                nearer_hr = left_hr;
                further_kd = kd.right;
                further_hr = right_hr;
            }
                    //
                    // 7. if not target-in-left then
                    // 7.1. nearer-kd := right field of kd and nearer-hr := right-hr
                    // 7.2. further-kd := left field of kd and further-hr := left-hr
            else{
                nearer_kd = kd.right;
                nearer_hr = right_hr;
                further_kd = kd.left;
                further_hr = left_hr;
            }
            // 8. Recursively call Nearest Neighbor with paramters
            // (nearer-kd, target, nearer-hr, max-dist-sqd), storing the
            // results in nearest and dist-sqd
            NearestNeighbors(nearer_kd, target, nearer_hr, max_dist_sqd, lev + 1, K, nnl, checker, timeout);
            // KDNode<T> nearest = nnl.getHighest();
            double dist_sqd;
            if(!nnl.IsCapacityReached()){
                dist_sqd = Double.MaxValue;
            } else{
                dist_sqd = nnl.MaxPriority;
            }
            // 9. max-dist-sqd := minimum of max-dist-sqd and dist-sqd
            max_dist_sqd = Math.Min(max_dist_sqd, dist_sqd);
            // 10. A nearer point could only lie in further-kd if there were some
            // part of further-hr within distance max-dist-sqd of
            // target.
            HPoint closest = further_hr.Closest(target);
            if(HPoint.SqrDist(closest, target) < max_dist_sqd){
                // 10.1 if (pivot-target)^2 < dist-sqd then
                if(pivot_to_target < dist_sqd){
                    // 10.1.1 nearest := (pivot, range-elt field of kd)
                    // nearest = kd;
                    // 10.1.2 dist-sqd = (pivot-target)^2
                    dist_sqd = pivot_to_target;
                    // add to nnl
                    if(!kd.deleted && ((checker == null) || checker.Invoke(kd.value))){
                        nnl.Insert(kd, dist_sqd);
                    }
                    // 10.1.3 max-dist-sqd = dist-sqd
                    // max_dist_sqd = dist_sqd;
                    if(nnl.IsCapacityReached()){
                        max_dist_sqd = nnl.MaxPriority;
                    } else{
                        max_dist_sqd = Double.MaxValue;
                    }
                }
                // 10.2 Recursively call Nearest Neighbor with parameters
                // (further-kd, target, further-hr, max-dist_sqd),
                // storing results in temp-nearest and temp-dist-sqd
                NearestNeighbors(further_kd, target, further_hr, max_dist_sqd, lev + 1, K, nnl, checker, timeout);
            }
        }
        // constructor is used only by class; other methods are static
        public override String ToString()
        {
            return this.ToString(0);
        }
        public String ToString(int depth)
        {
            String s = this.key + "  " + this.value + (this.deleted ? "*" : "");
            if(this.left != null){
                s = s + "\n" + pad(depth) + "L " + this.left.ToString(depth + 1);
            }
            if(this.right != null){
                s = s + "\n" + pad(depth) + "R " + this.right.ToString(depth + 1);
            }
            return s;
        }
        private static String pad(int n)
        {
            String s = "";
            for(int i = 0; i < n; ++i){
                s += " ";
            }
            return s;
        }
        private static void hrCopy(HRect hr_src, HRect hr_dst)
        {
            hpCopy(hr_src.Min, hr_dst.Min);
            hpCopy(hr_src.Max, hr_dst.Max);
        }
        private static void hpCopy(HPoint hp_src, HPoint hp_dst)
        {
            for(int i = 0; i < hp_dst.Coord.Length; ++i){
                hp_dst.Coord[i] = hp_src.Coord[i];
            }
        }
    }
}