using System;
using System.IO;
using RetroMedieval.Modules.Storage;

namespace RetroMedieval.Modules.Attributes;

public class ModuleStorage<TStorage>(string name) : ModuleStorage(name) where TStorage : class, IStorage, new()
{
    public TStorage Storage => GetStorage();

    private TStorage GetStorage()
    {
        StorageManager.Instance.Get((x => x.StorageName == Name), out var storage);
        return (TStorage)storage.Store;
    }

    internal override bool LoadedStorage(string dataPath, string fileName)
    {
        if (StorageManager.Instance.Has(Name))
        {
            return false;
        }


        var store = new TStorage();
        StorageManager.Instance.Add(new Storage.Storage(Name, store));
        
        if (store.StorageType == StorageType.File)
        {
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            var filePath = Path.Combine(dataPath, fileName);
            return store.Load(filePath);
        }

        var path = StorageManager.GetSavingConfig();
        return store.Load(path.ConnectionString);
    }

    internal override bool IsStorageOfType(Type t) => t == typeof(TStorage);
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class ModuleStorage(string name) : Attribute
{
    internal string Name { get; } = name;

    internal abstract bool LoadedStorage(string dataPath, string fileName);
    internal abstract bool IsStorageOfType(Type t);
}