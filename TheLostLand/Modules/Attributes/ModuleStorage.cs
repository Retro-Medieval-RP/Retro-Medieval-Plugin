using System.IO;
using Newtonsoft.Json;

namespace TheLostLand.Modules.Attributes;

public class ModuleStorage<TStorage>(string storage_name) : ModuleStorage(storage_name) where TStorage : class
{
    protected TStorage Storage => StorageData as TStorage;
}

[AttributeUsage(AttributeTargets.Class)]
public class ModuleStorage : Attribute
{
    private string StorageName { get; set; }
    protected object StorageData { get; private set; }

    protected ModuleStorage(string storage_name)
    {
        StorageName = storage_name;
    }

    internal void LoadStorage(string data_path)
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

        StorageData = new object();

        Directory.CreateDirectory(data_path);

        var json_save = JsonConvert.SerializeObject(StorageData, Formatting.Indented);
        using (var stream = new StreamWriter(file_path, false))
        {
            stream.Write(json_save);
        }
    }
}