using System;

namespace TheLostLand.Core.Modules.Attributes;

public class ModuleStorage<TStorage>(string name) : ModuleStorage(name)
{
    internal override bool LoadedStorage(string data_path, string file_name)
    {
        return false;
    }

    internal override bool IsStorageOfType(Type t) => t == typeof(TStorage);
}

[AttributeUsage(AttributeTargets.Class)]
public abstract class ModuleStorage(string name) : Attribute
{
    internal string Name { get; } = name;
    
    internal abstract bool LoadedStorage(string data_path, string file_name);
    internal abstract bool IsStorageOfType(Type t);
}