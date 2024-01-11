using System;
using System.IO;
using Newtonsoft.Json;
using TheLostLand.Core.Storage;

namespace TheLostLand.Core.Modules.Attributes;

public class ModuleStorage<TStorage>(string storage_name) : ModuleStorage(storage_name) where TStorage : class, IStorage, new()
{
    public TStorage Storage => StorageData as TStorage;

    internal override void LoadStorage(string data_path)
    {
        var file_path = Path.Combine(data_path, StorageName + ".json");

        if (!Directory.Exists(data_path))
        {
            Directory.CreateDirectory(data_path);
        }

        if (File.Exists(file_path))
        {
            string json;

            using (var stream = File.OpenText(file_path))
            {
                json = stream.ReadToEnd();
            }

            StorageData = JsonConvert.DeserializeObject(json);
            return;
        }

        StorageData = new TStorage();

        Directory.CreateDirectory(data_path);

        var json_save = JsonConvert.SerializeObject(StorageData, Formatting.Indented);
        using (var stream = new StreamWriter(file_path, false))
        {
            stream.Write(json_save);
        }
    }

    public override bool StorageType(Type type) => type == typeof(TStorage);
}

[AttributeUsage(AttributeTargets.Class)]
public abstract class ModuleStorage(string storage_name) : Attribute
{
    protected string StorageName { get; set; } = storage_name;
    protected object StorageData { get; set; }

    internal abstract void LoadStorage(string data_path);
    public abstract bool StorageType(Type type);
}