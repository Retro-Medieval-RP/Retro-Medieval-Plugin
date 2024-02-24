using System.Collections.Generic;

namespace RetroMedieval.Modules.Storage.Sql;

public interface ICondition : IDatabaseInfo
{
    /// <summary>
    /// String -> Condition Name
    /// String -> Filter String
    /// Int -> Filter Priority
    /// </summary>
    public Dictionary<string, (string, int)> FilterConditions { get; set; }
    public List<DataParam> Parameters { get; set; }

    public ICondition Where(params (string, object)[] conditions);
    public ICondition OrderBy(params (string, OrderBy)[] columns);
    public ICondition OrderBy(OrderBy order = Sql.OrderBy.Ascending, params string[] columns);
    
    public IExecutor Finalise();
}