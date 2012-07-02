using DrawEngine.Renderer.Collections;

namespace DrawEngine.Renderer
{
    public static class UnifiedScenesRepository
    {
        private readonly static NameableCollection<Scene> scenes = new NameableCollection<Scene>();
        private static Scene currentEditingScene;
        public static NameableCollection<Scene> Scenes
        {
            get { return scenes; }
        }
        public static Scene CurrentEditingScene
        {
            get { return currentEditingScene; }
            set { currentEditingScene = value; }
        }
    }
}