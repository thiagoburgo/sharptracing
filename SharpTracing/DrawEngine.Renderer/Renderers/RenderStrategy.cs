using System;
using System.Collections.Generic;
using DrawEngine.Renderer.BasicStructures;

namespace DrawEngine.Renderer.Renderers
{
    [Serializable]
    public abstract class RenderStrategy
    {
        private Scene scene;
        public RenderStrategy(Scene scene)
        {
            this.Scene = scene;
        }
        public Scene Scene
        {
            get { return this.scene; }
            set
            {
                if(value != null){
                    this.scene = value;
                } else{
                    throw new ArgumentNullException("Scene");
                }
            }
        }
        public abstract IEnumerable<Ray> GenerateRays();
    }
}