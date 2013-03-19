using System.Xml.Serialization;
using DrawEngine.Renderer.BasicStructures;

namespace DrawEngine.Renderer.RenderObjects.EnvironmentMaps
{
    [XmlInclude(typeof(CubeMap)), XmlInclude(typeof(SphereMap))]
    public abstract class EnvironmentMap
    {
        public abstract RGBColor GetColor(Ray ray);
    }
}