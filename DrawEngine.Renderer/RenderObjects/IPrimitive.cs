using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrawEngine.Renderer.Collections;
using DrawEngine.Renderer.BasicStructures;
using System.ComponentModel;
using DrawEngine.Renderer.Materials.Design;
using System.Drawing.Design;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Algebra;

namespace DrawEngine.Renderer.RenderObjects
{
    public interface IPrimitive : IIntersectable, IBoundBox
    {
        [Editor(typeof(MaterialSelectorEditor), typeof(UITypeEditor)),
         DefaultValue(null),
         TypeConverter(typeof(ExpandableObjectConverter))]
        Material Material { get; set; }
        
    }
}
