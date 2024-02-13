using System.Linq;
using System.Reflection;
using Dapper;
using MySql.Data.MySqlClient;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql.Tables.Attributes;
using Rocket.Core.Logging;

namespace RetroMedieval.Savers.MySql.StatementsAndQueries;

public static class Queries
{
    public static IStatement Where(this IQuery query, params (string, object)[] condition_values)
    {
        query.FilterConditionString =
            $"WHERE {string.Join(" AND ", condition_values.Select(x => x.Item1 + " = " + x.Item2))}";
        return new MySqlStatements(query.TableName, query.CurrentQueryString, query.FilterConditionString,
            query.ConnectionString);
    }

    public static void Insert(this IQuery query, params (string, object)[] columns)
    {
        query.CurrentQueryString =
            $"INSERT INTO {query.TableName} ({string.Join(", ", columns.Select(x => x.Item1))}) VALUES ({string.Join(", ", columns.Select(x => x.Item2))})";

        Logger.Log($"Query String: {query.CurrentQueryString} {query.FilterConditionString};");
        using var conn = new MySqlConnection(query.ConnectionString);

        try
        {
            conn.Execute(query.CurrentQueryString);
        }
        catch (MySqlException ex)
        {
            Logger.LogError($"Had an error when trying to execute: {query.CurrentQueryString} {query.FilterConditionString};");
            Logger.LogException(ex);
        }
    }

    public static void Insert<T>(this IQuery query, T obj)
    {
        var columns = typeof(T).GetProperties().Where(x => x.GetCustomAttributes<DatabaseColumn>().Any() && x.GetValue(obj) != null);
        var column_data = columns.Select(x => (x.GetCustomAttribute<DatabaseColumn>().ColumnName, x.GetValue(obj)));
        
        query.CurrentQueryString = $"INSERT INTO {query.TableName} ({string.Join(", ", column_data.Select(x => x.Item1))}) VALUES ({'"'}{string.Join($"{'"'} , {'"'}", column_data.Select(x => x.Item2))}{'"'})";

        using var conn = new MySqlConnection(query.ConnectionString);

        try
        {
            conn.Execute(query.CurrentQueryString);
        }
        catch (MySqlException ex)
        {
            Logger.LogError($"Had an error when trying to execute: {query.CurrentQueryString} {query.FilterConditionString};");
            Logger.LogException(ex);
        }
    }

    public static TOutput? Select<TOutput>(this IQuery statement, params string[] columns) where TOutput : class
    {
        statement.CurrentQueryString = $"SELECT {string.Join(" AND ", columns)} FROM {statement.TableName}";

        using var conn = new MySqlConnection(statement.ConnectionString);

        try
        {
            return (TOutput)conn.Query<TOutput>(statement.CurrentQueryString + ";");
        }
        catch (MySqlException ex)
        {
            Logger.LogError($"Had an error when trying to execute: {statement.CurrentQueryString};");
            Logger.LogException(ex);
        }

        return null;
    }
}