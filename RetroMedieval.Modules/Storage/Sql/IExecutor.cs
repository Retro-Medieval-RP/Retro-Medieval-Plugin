using System.Collections.Generic;

namespace RetroMedieval.Modules.Storage.Sql;

public interface IExecutor
{
    public IDatabaseInfo DatabaseInfo { get; set; }
    public Dictionary<string, (string, int)> FilterConditions { get; set; }
    public List<DataParam> DataParams { get; set; }
    
    bool ExecuteSql();

    IEnumerable<T> Query<T>() where T : new();
    T QuerySingle<T>() where T : new();
}