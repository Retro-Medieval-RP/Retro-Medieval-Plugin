using System;

namespace TheLostLand.Core.Modules.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class ModuleStorage(string name) : Attribute
{
    internal abstract bool LoadedStorage(string data_path, string file_name);

    internal string Name { get; } = name;
    
    internal abstract bool IsStorageOfType(Type t);
}