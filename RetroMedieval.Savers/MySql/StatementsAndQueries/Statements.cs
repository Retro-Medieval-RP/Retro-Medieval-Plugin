using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;
using RetroMedieval.Modules.Storage.Sql;
using Rocket.Core.Logging;

namespace RetroMedieval.Savers.MySql.StatementsAndQueries;

public static class Statements
{
    public static TOutput? Select<TOutput>(this IStatement statement, params string[] columns) where TOutput : class
    {
        statement.CurrentQueryString = $"SELECT {string.Join(" AND ", columns)} FROM {statement.TableName}";
        
        using var conn = new MySqlConnection(statement.ConnectionString);
        
        try
        {
            return (TOutput)conn.Query<TOutput>(statement.CurrentQueryString + " " + statement.FilterConditionString + ";");
        }
        catch (MySqlException ex)
        {
            Logger.LogError($"Had an error when trying to execute: {statement.CurrentQueryString} {statement.FilterConditionString};");
            Logger.LogException(ex);
        }

        return null;
    }
    
    public static void Update(this IStatement statement, params (string, object)[] columns)
    {
        statement.CurrentQueryString = $"UPDATE {statement.TableName} SET {string.Join(" AND ", columns.Select(x => x.Item1 + " = " + x.Item2))}";

        using var conn = new MySqlConnection(statement.ConnectionString);
        
        try
        {
            conn.Execute(statement.CurrentQueryString + " " + statement.FilterConditionString + ";");
        }
        catch (MySqlException ex)
        {
            Logger.LogError($"Had an error when trying to execute: {statement.CurrentQueryString} {statement.FilterConditionString};");
            Logger.LogException(ex);
        }
    }
    
    public static void Delete(this IStatement statement, params (string, object)[] columns)
    {
        statement.CurrentQueryString = $"DELETE FROM {statement.TableName}";
        
        using var conn = new MySqlConnection(statement.ConnectionString);
        
        try
        {
            conn.Execute(statement.CurrentQueryString + " " + statement.FilterConditionString + ";");
        }
        catch (MySqlException ex)
        {
            Logger.LogError($"Had an error when trying to execute: {statement.CurrentQueryString} {statement.FilterConditionString};");
            Logger.LogException(ex);
        }
    }

    public static void Insert(this IQuery query, params (string, object)[] columns)
    {
        query.CurrentQueryString = $"INSERT INTO {query.TableName} ({string.Join(", ", columns.Select(x => x.Item1))}) VALUES ({string.Join(", ", columns.Select(x => x.Item2))})";
        
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
}