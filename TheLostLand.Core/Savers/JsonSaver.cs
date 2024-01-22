using System.IO;
using Newtonsoft.Json;
using TheLostLand.Core.Modules.Storage;

namespace TheLostLand.Core.Savers
{
    public class JsonSaver<T> : IStorage<T> where T : new()
    {
        public string SavePath { get; private set; }
        public T StorageItem { get; protected set; }

        public virtual bool Load(string file_path)
        {
            SavePath = file_path;

            if (File.Exists(SavePath))
            {
                string data_text;
                using (var stream = File.OpenText(SavePath))
                {
                    data_text = stream.ReadToEnd();
                }

                StorageItem = JsonConvert.DeserializeObject<T>(data_text);
                return true;
            }

            StorageItem = new T();
            Save();

            return true;
        }

        public virtual void Save()
        {
            var obj_data = JsonConvert.SerializeObject(StorageItem, Formatting.Indented);

            using var stream = new StreamWriter(SavePath, false);
            stream.Write(obj_data);
        }
    }
}