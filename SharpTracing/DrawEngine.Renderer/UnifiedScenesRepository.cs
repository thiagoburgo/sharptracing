using DrawEngine.Renderer.Collections;

namespace DrawEngine.Renderer {
    public static class UnifiedScenesRepository {
        public static readonly NameableCollection<Scene> Scenes = new NameableCollection<Scene>();
        private static Scene currentEditingScene;
        private static readonly object syncObj = new object();

        public static Scene CurrentEditingScene {
            get { return currentEditingScene; }
            set {
                lock (syncObj) {
                    currentEditingScene = value;
                }
            }
        }
    }
}