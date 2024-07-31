using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetroMedieval.Modules.Storage.Sql;

public interface IExecutor
{
    public IDatabaseInfo DatabaseInfo { get; set; }
    public Dictionary<string, (string, int)> FilterConditions { get; set; }
    public List<DataParam> DataParams { get; set; }
    
    bool ExecuteSql();

    IEnumerable<T> Query<T>() where T : new();
    T QuerySingle<T>() where T : new();
    Task<IEnumerable<T>> QueryAsync<T>() where T : new();
    Task<T> QuerySingleAsync<T>() where T : new();
}