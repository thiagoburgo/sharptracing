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
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

//Original Code From RayTracerFramework http://code.google.com/p/tumcgprakt/source/checkout

namespace DrawEngine.Renderer.SpatialSubdivision.Acceleration
{
    public abstract class KDTreeIntersectableManager<T> : IntersectableAccelerationStructure<T> where T : IIntersectable
    {
        #region Axis enum
        public enum Axis
        {
            X,
            Y,
            Z
        }
        #endregion

        public IList<T> Content;
        public int Height;
        public int MaxDesiredObjectsPerLeafCount;
        public int MaxHeight;
        public Node Root;
        // Properties
        private double weightDivisionQuality;
        private double weightSum;
        // Constructors
        protected KDTreeIntersectableManager() : this(new List<T>()) {}
        protected KDTreeIntersectableManager(IList<T> content) : base(content)
        {
            this.Root = new Leaf(content);
            this.Content = content;
            this.MaxDesiredObjectsPerLeafCount = 1;
            this.MaxHeight = 50;
            this.WeightDivisionQuality = 0.5d;
            this.WeightSum = 1 - this.weightDivisionQuality;
            this.Height = 1;
            //LeafesCount = 1;
        }
        public double WeightDivisionQuality
        {
            get { return this.weightDivisionQuality; }
            set
            {
                if(value < 0 || value > 1){
                    throw new Exception("WeightDivisionQuality must be between 0 and 1");
                }
                this.weightDivisionQuality = value;
                this.weightSum = 1 - value;
            }
        }
        public double WeightSum
        {
            get { return this.weightSum; }
            set
            {
                if(value < 0 || value > 1){
                    throw new Exception("WeightSum must be between 0 and 1");
                }
                this.weightSum = value;
                this.weightDivisionQuality = 1 - value;
            }
        }
        // Abstract methods
        protected abstract void SplitOnPlane(IList<T> splitContent, Axis axis, Point3D position,
                                             out IList<T> leftContent, out IList<T> rightContent);
        protected abstract Point3D CalculateMid(IList<T> content);
        // Methods
        public override void Optimize()
        {
            //LeafesCount = 1;
            this.Root = this.Optimize(new Leaf(this.Content), 1);
        }
        protected Node Optimize(Leaf leaf, int currentHeight)
        {
            if(currentHeight > this.Height){
                this.Height = currentHeight;
            }
            // Stop here if max leaf count is already satisfied or tree gets too high
            if(leaf.Content.Count <= this.MaxDesiredObjectsPerLeafCount || currentHeight++ == this.MaxHeight){
                return leaf;
            }
            // Find mid of all content (will be used as splitting position)
            Point3D mid = this.CalculateMid(leaf.Content);
            // Choose best splitting axis
            // This should be the one wich divides the content best
            // and does have the least content in both sides
            Axis splitAxis = Axis.X;
            double planePosition = mid.X;
            IList<T> leftContent, rightContent;
            this.SplitOnPlane(leaf.Content, Axis.X, mid, out leftContent, out rightContent);
            double currentDivisionQuality = leftContent.Count > rightContent.Count
                                                   ? (rightContent.Count) / leftContent.Count
                                                   : (leftContent.Count) / rightContent.Count;
            double currentSum = leftContent.Count + rightContent.Count;
            IList<T> alternativeLeftContent, alternativeRightContent;
            this.SplitOnPlane(leaf.Content, Axis.Y, mid, out alternativeLeftContent, out alternativeRightContent);
            double alternativeDivisionQuality = alternativeLeftContent.Count > alternativeRightContent.Count
                                                       ? (alternativeRightContent.Count)
                                                         / alternativeLeftContent.Count
                                                       : (alternativeLeftContent.Count)
                                                         / alternativeRightContent.Count;
            double alternativeSum = alternativeLeftContent.Count + alternativeRightContent.Count;
            if((this.WeightDivisionQuality * alternativeDivisionQuality / currentDivisionQuality
                + this.WeightSum * currentSum / alternativeSum) > 1d){
                leftContent = alternativeLeftContent;
                rightContent = alternativeRightContent;
                splitAxis = Axis.Y;
                planePosition = mid.Y;
                currentDivisionQuality = alternativeDivisionQuality;
                currentSum = alternativeSum;
            }
            this.SplitOnPlane(leaf.Content, Axis.Z, mid, out alternativeLeftContent, out alternativeRightContent);
            alternativeDivisionQuality = alternativeLeftContent.Count > alternativeRightContent.Count
                                                 ? (alternativeRightContent.Count) / alternativeLeftContent.Count
                                                 : (alternativeLeftContent.Count) / alternativeRightContent.Count;
            alternativeSum = alternativeLeftContent.Count + alternativeRightContent.Count;
            if((this.WeightDivisionQuality * alternativeDivisionQuality / currentDivisionQuality
                + this.WeightSum * currentSum / alternativeSum) > 1d){
                leftContent = alternativeLeftContent;
                rightContent = alternativeRightContent;
                splitAxis = Axis.Z;
                planePosition = mid.Z;
            }
            // Stop here if obj count could not be lowered anymore
            if(leftContent.Count == leaf.Content.Count || rightContent.Count == leaf.Content.Count){
                return leaf;
            }
            //LeafesCount++;
            // Recursivly optimize left side
            Node leftNode = this.Optimize(new Leaf(leftContent), currentHeight);
            // Recursivly optimize right side
            Node rightNode = this.Optimize(new Leaf(rightContent), currentHeight);
            // Create and return new inner node                        
            return new Inner(leftNode, rightNode, splitAxis, planePosition);
        }
        protected bool Traverse(Ray ray, Node node, double tMin, double tMax, out Intersection firstIntersection)
        {
            firstIntersection = new Intersection();
            firstIntersection.TMin = double.MaxValue;
            bool hit = false;
            if(node.IsLeaf){
                Leaf leaf = (Leaf)node;
                Intersection intersect_comp;
                for(int i = 0; i < leaf.Content.Count; i++){
                    T obj = leaf.Content[i];
                    if(obj.Visible && obj.FindIntersection(ray, out intersect_comp) && intersect_comp.TMin < firstIntersection.TMin) {
                        firstIntersection = intersect_comp;
                        hit = true;
                    }
                }
                return hit && firstIntersection.TMin <= tMax;
            }
            //if (node.IsLeaf)
            //{
            //    Leaf leaf = (Leaf)node;
            //    firstIntersection = new Intersection();
            //    Intersection currentIntersection;
            //    double currentT = double.PositiveInfinity;
            //    foreach (IIntersectable obj in leaf.Content)
            //    {
            //        if (obj.FindIntersection(ray, out currentIntersection))
            //        {
            //            if (currentIntersection.TMin < currentT)
            //            {
            //                currentT = currentIntersection.TMin;
            //                firstIntersection = currentIntersection;
            //            }
            //        }
            //    }
            //    //if (currentT > tMax)
            //    //    firstIntersection = null;
            //    return !(currentT > tMax);
            //}
            Inner inner = (Inner)node;
            Node near, far;
            double tSplit;
            this.CalculateInner(ray, inner, out near, out far, out tSplit);
            if((tSplit >= tMax) || (tSplit < 0d)){
                return this.Traverse(ray, near, tMin, tMax, out firstIntersection);
            } else if(tSplit < tMin){
                return this.Traverse(ray, far, tMin, tMax, out firstIntersection);
            } else{
                if(this.Traverse(ray, near, tMin, tSplit, out firstIntersection)){
                    if(firstIntersection.TMin <= tSplit){
                        return true;
                    }
                }
                return this.Traverse(ray, far, tSplit, tMax, out firstIntersection);
            }
        }
        //protected int Traverse(Ray ray, Node node, double tMin, double tMax, ref SortedList<double, RayIntersectionPoint> intersections)
        //{
        //    if (node.IsLeaf)
        //    {
        //        Leaf leaf = (Leaf)node;
        //        int numCurrentIntersections = 0;
        //        foreach (IIntersectable obj in leaf.Content)
        //        {
        //            numCurrentIntersections += obj.Intersect(ray, ref intersections);
        //        }
        //        return numCurrentIntersections;
        //    }
        //    Inner inner = (Inner)node;
        //    Node near, far;
        //    double tSplit;
        //    CalculateInner(ray, inner, out near, out far, out tSplit);
        //    if ((tSplit >= tMax) || (tSplit < 0d))
        //        return Traverse(ray, near, tMin, tMax, ref intersections);
        //    else if (tSplit < tMin)
        //        return Traverse(ray, far, tMin, tMax, ref intersections);
        //    else
        //    {
        //        int numCurrentIntersections = Traverse(ray, near, tMin, tSplit, ref intersections);
        //        return numCurrentIntersections + Traverse(ray, far, tSplit, tMax, ref intersections);
        //    }
        //}
        protected void CalculateInner(Ray ray, Inner inner, out Node near, out Node far, out double tSplit)
        {
            near = null;
            far = null;
            tSplit = 0d;
            switch(inner.Axis){
                case Axis.X:
                    if(ray.Origin.X > inner.PlanePosition){
                        near = inner.Right;
                        far = inner.Left;
                    } else{
                        near = inner.Left;
                        far = inner.Right;
                    }
                    if(ray.Direction.X == 0d){
                        tSplit = double.PositiveInfinity;
                    } else{
                        tSplit = (inner.PlanePosition - ray.Origin.X) / ray.Direction.X;
                    }
                    break;
                case Axis.Y:
                    if(ray.Origin.Y > inner.PlanePosition){
                        near = inner.Right;
                        far = inner.Left;
                    } else{
                        near = inner.Left;
                        far = inner.Right;
                    }
                    if(ray.Direction.Y == 0d){
                        tSplit = double.PositiveInfinity;
                    } else{
                        tSplit = (inner.PlanePosition - ray.Origin.Y) / ray.Direction.Y;
                    }
                    break;
                case Axis.Z:
                    if(ray.Origin.Z > inner.PlanePosition){
                        near = inner.Right;
                        far = inner.Left;
                    } else{
                        near = inner.Left;
                        far = inner.Right;
                    }
                    if(ray.Direction.Z == 0d){
                        tSplit = double.PositiveInfinity;
                    } else{
                        tSplit = (inner.PlanePosition - ray.Origin.Z) / ray.Direction.Z;
                    }
                    break;
            }
        }
        public bool Intersect(Ray ray)
        {
            Intersection dummy;
            return this.Traverse(ray, this.Root, 0d, double.PositiveInfinity, out dummy);
        }
        public override bool FindIntersection(Ray ray, out Intersection firstIntersection)
        {
            return this.Traverse(ray, this.Root, 0d, double.PositiveInfinity, out firstIntersection);
        }

        #region Nested type: Inner
        public class Inner : Node
        {
            public Axis Axis;
            public Node Left;
            public double PlanePosition;
            public Node Right;
            public Inner(Node left, Node right, Axis axis, double planePosition) : base(false)
            {
                this.Left = left;
                this.Right = right;
                this.Axis = axis;
                this.PlanePosition = planePosition;
            }
        }
        #endregion

        #region Nested type: Leaf
        public class Leaf : Node
        {
            public IList<T> Content;
            public Leaf() : base(true)
            {
                this.Content = new List<T>();
            }
            public Leaf(IList<T> content) : base(true)
            {
                this.Content = content;
            }
        }
        #endregion

        #region Nested type: Node
        public abstract class Node
        {
            public readonly bool IsLeaf;
            protected Node(bool isLeaf)
            {
                this.IsLeaf = isLeaf;
            }
        }
        #endregion
    }
}