namespace DrawEngine.Renderer.SpatialSubdivision.KDTree
{
    public interface IEditor<T>
    {
        T Edit(T current);
    }

    public abstract class BaseEditor<T> : IEditor<T>
    {
        protected readonly T val;
        protected BaseEditor(T val)
        {
            this.val = val;
        }

        #region IEditor<T> Members
        public abstract T Edit(T current);
        #endregion
    }

    public class Inserter<T> : BaseEditor<T>
    {
        public Inserter(T val) : base(val) {}
        public override T Edit(T current)
        {
            if(Equals(current, default(T))){
                return this.val;
            }
            throw new KeyDuplicateException();
        }
    }

    public class OptionalInserter<T> : BaseEditor<T>
    {
        public OptionalInserter(T val) : base(val) {}
        public override T Edit(T current)
        {
            return (Equals(current, default(T))) ? this.val : current;
        }
    }

    public class Replacer<T> : BaseEditor<T>
    {
        public Replacer(T val) : base(val) {}
        public override T Edit(T current)
        {
            return this.val;
        }
    }
}