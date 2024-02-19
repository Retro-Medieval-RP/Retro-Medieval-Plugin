namespace RetroMedieval.Modules.Storage;

public interface IFileStorage<T> : IStorage
{
    public T? StorageItem { get; }
    void Save();
}

public interface ISqlStorage<T> : IStorage
{
}

public interface IStorage
{
    public string SavePath { get; }
    public StorageType StorageType { get; }
    bool Load(string file_path);
}