using System;
using DrawEngine.Renderer.Shaders;
using DrawEngine.Renderer.Tracers;

namespace DrawEngine.Renderer
{
    [Serializable]
    public class SceneManager
    {
        private RayCasting rayCasting;
        private Scene scene;
        private Shader shader;
        public SceneManager(Scene scene, RayCasting rayCasting, Shader shader)
        {
            this.Shader = shader;
            this.Scene = scene;
            this.RayCasting = rayCasting;
        }
        public Shader Shader
        {
            get { return this.shader; }
            set { this.shader = value; }
        }
        public RayCasting RayCasting
        {
            get { return this.rayCasting; }
            set { this.rayCasting = value; }
        }
        public Scene Scene
        {
            get { return this.scene; }
            set { this.scene = value; }
        }
    }
}