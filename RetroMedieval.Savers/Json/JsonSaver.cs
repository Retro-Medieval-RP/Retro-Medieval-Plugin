using System.IO;
using Newtonsoft.Json;
using RetroMedieval.Modules.Storage;

namespace RetroMedieval.Savers.Json;

public class JsonSaver<T> : IFileStorage<T> where T : new()
{
    public string SavePath { get; private set; } = "";
    public StorageType StorageType => StorageType.File;
    public T? StorageItem { get; protected set; }

    public virtual bool Load(string filePath)
    {
        SavePath = filePath;

        if (File.Exists(SavePath))
        {
            string dataText;
            using (var stream = File.OpenText(SavePath))
            {
                dataText = stream.ReadToEnd();
            }

            StorageItem = JsonConvert.DeserializeObject<T>(dataText);
            return true;
        }

        StorageItem = new T();
        Save();

        return true;
    }

    public bool Unload(string filePath)
    {
        Save();
        return true;
    }

    public virtual void Save()
    {
        var objData = JsonConvert.SerializeObject(StorageItem, Formatting.Indented);

        using var stream = new StreamWriter(SavePath, false);
        stream.Write(objData);
    }
}