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