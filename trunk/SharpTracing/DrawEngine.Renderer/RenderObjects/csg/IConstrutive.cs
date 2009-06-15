namespace DrawEngine.Renderer.RenderObjects.CSG
{
    public interface IConstrutive
    {
        Primitive BasePrimitive { get; set; }
        Primitive OperandPrimitive { get; set; }
    }
}