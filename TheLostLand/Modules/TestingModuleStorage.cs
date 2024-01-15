using System;
using System.IO;
using Newtonsoft.Json;
using TheLostLand.Core.Modules.Storage;

namespace TheLostLand.Modules;

internal class TestingModuleStorage : IStorage
{
    private string FilePath { get; set; }
    private string StorageExampleData { get; set; }
    
    public bool Load(string file_path)
    {
        FilePath = file_path;
        
        if (File.Exists(FilePath))
        {
            string data_text;
            using (var stream = File.OpenText(FilePath))
            {
                data_text = stream.ReadToEnd();
            }
            StorageExampleData = JsonConvert.DeserializeObject<String>(data_text);
            return true;
        }

        return false;
    }

    public void Save()
    {
        var obj_data = JsonConvert.SerializeObject(StorageExampleData, Formatting.Indented);

        using var stream = new StreamWriter(FilePath, false);
        stream.Write(obj_data);
    }
}