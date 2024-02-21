using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql.Exceptions;
using Rocket.Core.Logging;

namespace RetroMedieval.Savers.MySql;

public class MySqlExecutor(IQuery query, List<DataParam>? parameters = null) : IExecutor
{
    public IQuery Query { get; set; } = query;
    public List<DataParam>? DataParams { get; set; } = parameters;

    public void ExecuteSql()
    {
        using var conn = new MySqlConnection(Query.ConnectionString);

        try
        {
            if (DataParams == null)
            {
                conn.Execute(query.CurrentQueryString + " " + query.FilterConditionString + ";");
                return;
            }

            if (DataParams.Count > 0)
            {
                conn.Execute(query.CurrentQueryString + " " + query.FilterConditionString + ";", ConvertParams());
            }
            else
            {
                throw new NoDataParamsGiven();
            }
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
                return DataParams == null
                    ? (T)conn.Query<T>(query.CurrentQueryString + " " + query.FilterConditionString + ";")
                    : (T)conn.Query<T>(query.CurrentQueryString + " " + query.FilterConditionString + ";",
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
            return DataParams == null
                ? conn.QuerySingle<T>(query.CurrentQueryString + " " + query.FilterConditionString + ";")
                : conn.QuerySingle<T>(query.CurrentQueryString + " " + query.FilterConditionString + ";",
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

        if (DataParams == null)
        {
            return params_out;
        }

        foreach (var param in DataParams)
        {
            params_out.Add(param.ParamName, param.ParamObject, param.ParamDbType);
        }

        return params_out;
    }
}