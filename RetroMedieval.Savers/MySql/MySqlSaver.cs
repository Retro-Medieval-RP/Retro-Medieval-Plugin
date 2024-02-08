using RetroMedieval.Modules.Storage;

namespace RetroMedieval.Savers.MySql;

internal class MySqlSaver<T> : IStorage<T>
{
    public string SavePath { get; }
    public StorageType StorageType => StorageType.Sql;
    public T StorageItem { get; }    
    
    public bool Load(string file_path)
    {
        throw new System.NotImplementedException();
    }

    public void Save()
    {
        throw new System.NotImplementedException();
    }
}