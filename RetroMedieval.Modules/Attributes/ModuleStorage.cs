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

    internal override bool LoadedStorage(string data_path, string file_name)
    {
        if (StorageManager.Instance.Has(Name))
        {
            return false;
        }


        var store = new TStorage();
        StorageManager.Instance.Add(new Storage.Storage(Name, store));
        
        if (store.StorageType == StorageType.File)
        {
            if (!Directory.Exists(data_path))
            {
                Directory.CreateDirectory(data_path);
            }

            var file_path = Path.Combine(data_path, file_name);
            return store.Load(file_path);
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

    internal abstract bool LoadedStorage(string data_path, string file_name);
    internal abstract bool IsStorageOfType(Type t);
}