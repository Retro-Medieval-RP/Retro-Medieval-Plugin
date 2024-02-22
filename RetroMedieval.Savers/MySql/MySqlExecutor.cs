using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql.Exceptions;
using Rocket.Core.Logging;

namespace RetroMedieval.Savers.MySql;

public class MySqlExecutor(IQuery query, List<DataParam> parameters) : IExecutor
{
    public IQuery Query { get; set; } = query;
    public List<DataParam> DataParams { get; set; } = parameters;

    public void ExecuteSql()
    {
        using var conn = new MySqlConnection(Query.ConnectionString);

        try
        {
            if (DataParams.Count < 1)
            {
                conn.Execute(Query.CurrentQueryString + " " + Query.FilterConditionString + ";");
                return;
            }

            conn.Execute(Query.CurrentQueryString + " " + Query.FilterConditionString + ";", ConvertParams());
        }
        catch (MySqlException ex)
        {
            Logger.LogError(
                $"Had an error when trying to execute: {Query.CurrentQueryString} {Query.FilterConditionString};");
            Logger.LogException(ex);
        }
    }

    public T? QuerySql<T>()
    {
        using var conn = new MySqlConnection(Query.ConnectionString);

        if (typeof(T).GetInterfaces()
            .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            try
            {
                return DataParams.Count < 1
                    ? (T)conn.Query<T>(Query.CurrentQueryString + " " + Query.FilterConditionString + ";")
                    : (T)conn.Query<T>(Query.CurrentQueryString + " " + Query.FilterConditionString + ";",
                        ConvertParams());
            }
            catch (MySqlException ex)
            {
                Logger.LogError(
                    $"Had an error when trying to execute: {Query.CurrentQueryString} {Query.FilterConditionString};");
                Logger.LogException(ex);
            }

            return default;
        }

        try
        {
            return DataParams.Count < 1
                ? conn.QuerySingle<T>(Query.CurrentQueryString + " " + Query.FilterConditionString + ";")
                : conn.QuerySingle<T>(Query.CurrentQueryString + " " + Query.FilterConditionString + ";",
                    ConvertParams());
        }
        catch (MySqlException ex)
        {
            Logger.LogError(
                $"Had an error when trying to execute: {Query.CurrentQueryString} {Query.FilterConditionString};");
            Logger.LogException(ex);
        }

        return default;
    }

    private DynamicParameters ConvertParams()
    {
        var params_out = new DynamicParameters();
        
        foreach (var param in DataParams)
        {
            params_out.Add(param.ParamName, param.ParamObject, param.ParamDbType);
        }

        return params_out;
    }
}