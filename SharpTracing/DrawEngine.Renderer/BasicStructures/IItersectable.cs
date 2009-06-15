namespace DrawEngine.Renderer.BasicStructures
{
    public interface IIntersectable
    {
        //bool Intersect(Ray ray);
        bool FindIntersection(Ray ray, out Intersection intersection);
    }
}