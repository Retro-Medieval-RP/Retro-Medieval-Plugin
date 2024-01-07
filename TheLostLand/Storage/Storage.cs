using System.IO;
using Newtonsoft.Json;

namespace TheLostLand.Storage;

public abstract class Storage<T> where T : new()
{
    private string DataPath { get; set; }

    public void Save(T obj)
    {
        var obj_data = JsonConvert.SerializeObject(obj, Formatting.Indented);

        using var stream = new StreamWriter(DataPath, false);
        stream.Write(obj_data);
    }

    public T Read()
    {
        if (!File.Exists(DataPath))
        {
            return default;
        }

        string data_text;
        using (var stream = File.OpenText(DataPath))
        {
            data_text = stream.ReadToEnd();
        }

        return JsonConvert.DeserializeObject<T>(data_text);
    }
}