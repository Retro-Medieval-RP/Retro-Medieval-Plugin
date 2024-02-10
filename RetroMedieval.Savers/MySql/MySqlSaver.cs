using RetroMedieval.Modules.Storage;
using RetroMedieval.Savers.MySql.Tables;

namespace RetroMedieval.Savers.MySql;

public class MySqlSaver<T> : IStorage<T>
{
    public string SavePath { get; }
    public StorageType StorageType => StorageType.Sql;
    public T StorageItem { get; }    
    
    public bool Load(string file_path)
    {
        return true;
    }

    public string GetStorageDDL()
    {
        var ddl = TableGenerator.GenerateDDL(typeof(T));
        return ddl;
    }
    
    public void Save()
    {
    }
}