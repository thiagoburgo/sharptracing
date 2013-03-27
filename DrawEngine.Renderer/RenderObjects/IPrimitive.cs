using System.ComponentModel;
using System.Drawing.Design;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Materials.Design;

namespace DrawEngine.Renderer.RenderObjects {
    public interface IPrimitive : IIntersectable, IBoundBox {
        [Editor(typeof (MaterialSelectorEditor), typeof (UITypeEditor)), DefaultValue(null),
         TypeConverter(typeof (ExpandableObjectConverter))]
        Material Material { get; set; }
    }
}