using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;
using RetroMedieval.Modules.Storage.Sql;
using Rocket.Core.Logging;

namespace RetroMedieval.Savers.MySql;

public class MySqlExecutor(IQuery query, DynamicParameters? parameters = null) : IExecutor
{
    public IQuery Query { get; set; } = query;
    private DynamicParameters? Parameters { get; set; } = parameters;

    public void ExecuteSql()
    {
        using var conn = new MySqlConnection(Query.ConnectionString);

        try
        {
            if (Parameters == null)
            {
                conn.Execute(query.CurrentQueryString + " " + query.FilterConditionString + ";");
                return;
            }

            conn.Execute(query.CurrentQueryString + " " + query.FilterConditionString + ";", Parameters);
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

        if (typeof(T).GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            try
            {
                return Parameters == null
                    ? (T)conn.Query<T>(query.CurrentQueryString + " " + query.FilterConditionString + ";")
                    : (T)conn.Query<T>(query.CurrentQueryString + " " + query.FilterConditionString + ";", Parameters);
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
            return Parameters == null ? conn.QuerySingle<T>(query.CurrentQueryString + " " + query.FilterConditionString + ";") : conn.QuerySingle<T>(query.CurrentQueryString + " " + query.FilterConditionString + ";", Parameters);
        }
        catch (MySqlException ex)
        {
            Logger.LogError(
                $"Had an error when trying to execute: {Query.CurrentQueryString} {Query.FilterConditionString};");
            Logger.LogException(ex);
        }

        return default;
        
    }
}