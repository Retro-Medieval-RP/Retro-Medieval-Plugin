using TheLostLand.Core.Modules.Storage;

namespace TheLostLand.Savers.MySql;

public class MySqlSaver<T> : IStorage<T>
{
    public string SavePath { get; private set; }
    public T StorageItem { get; }
    
    public bool Load(string file_path)
    {
        SavePath = file_path;
        return true;
    }

    public void Save()
    {
    }

}