using System;

public interface IPluggable : IDisposable
{
    string Description { get; }
    string Name { get; }
    void Run();
}