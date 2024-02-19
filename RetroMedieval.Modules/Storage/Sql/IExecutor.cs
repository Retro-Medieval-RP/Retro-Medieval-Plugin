namespace RetroMedieval.Modules.Storage.Sql;

public interface IExecutor
{
    public IQuery Query { get; set; }
    
    void ExecuteSql();

    T? QuerySql<T>();
}