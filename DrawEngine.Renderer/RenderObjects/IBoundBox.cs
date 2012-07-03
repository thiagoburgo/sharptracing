namespace DrawEngine.Renderer.RenderObjects
{
    public interface IBoundBox
    {
        BoundBox BoundBox { get; set; }
        bool IsOverlap(BoundBox boundBox);
    }
}