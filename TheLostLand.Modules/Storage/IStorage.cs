namespace TheLostLand.Modules.Storage;

public interface IStorage<T> : IStorage
{
    public T StorageItem { get; }
}

public interface IStorage
{
    public string SavePath { get; }
    bool Load(string file_path);
    void Save();
}