using System.Collections.Generic;

namespace RetroMedieval.Modules.Storage.Sql;

public interface IStatement : IDatabaseInfo
{
    public List<DataParam> Parameters { get; set; }

    public IExecutor Insert<T>(T obj);
    public ICondition Select(params string[] columns);
    public ICondition Count();
    public ICondition Update(params (string, object)[] column_data);
    public ICondition Delete();
}