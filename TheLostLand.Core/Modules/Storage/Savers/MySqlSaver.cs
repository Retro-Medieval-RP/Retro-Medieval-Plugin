namespace TheLostLand.Core.Modules.Storage.Savers;

public class MySqlSaver<T> : IStorage<T> where T : class, new()
{
    public string SavePath { get; private set; }
    
    public T StorageItem { get; private set; }

    public virtual bool Load(string file_path)
    {
        SavePath = file_path;
        return true;
    }

    public virtual void Save()
    {
    }
}