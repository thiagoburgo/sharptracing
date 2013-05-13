using System;

namespace DrawEngine.PluginEngine {
    public interface IPluggable : IDisposable {
        string Description { get; }
        string Name { get; }
        void Run();
    }
}