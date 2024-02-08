namespace RetroMedieval.Modules.Storage;

public interface IStorage<T> : IStorage
{
    public T StorageItem { get; }
}

public interface IStorage
{
    public string SavePath { get; }
    public StorageType StorageType { get; }
    bool Load(string file_path);
    void Save();
}