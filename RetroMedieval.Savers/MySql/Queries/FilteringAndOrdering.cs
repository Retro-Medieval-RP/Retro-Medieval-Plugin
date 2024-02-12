using System.Linq;
using RetroMedieval.Modules.Storage.Sql;

namespace RetroMedieval.Savers.MySql.Queries;

public static class FilteringAndOrdering
{
    public static IStatement Where(this IQuery query, params (string, object)[] condition_values)
    {
        query.FilterConditionString = $"WHERE {string.Join(" AND ", condition_values.Select(x => x.Item1 + " = " + x.Item2))}";
        return new MySqlStatements(query.TableName, query.CurrentQueryString, query.FilterConditionString, query.ConnectionString);
    }
}