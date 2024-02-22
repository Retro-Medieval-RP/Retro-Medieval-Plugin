using System.Collections.Generic;

namespace RetroMedieval.Modules.Storage.Sql;

public interface IExecutor
{
    public IQuery Query { get; set; }
    public List<DataParam> DataParams { get; set; }
    
    void ExecuteSql();

    T? QuerySql<T>();
}