using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using RetroMedieval.Modules.Storage.Sql;
using Rocket.Core.Logging;

namespace RetroMedieval.Savers.MySql;

public class MySqlExecutor(
    IDatabaseInfo info,
    List<DataParam> parameters,
    Dictionary<string, (string, int)> filterConditions) : IExecutor
{
    public IDatabaseInfo DatabaseInfo { get; set; } = info;
    public Dictionary<string, (string, int)> FilterConditions { get; set; } = filterConditions;
    public List<DataParam> DataParams { get; set; } = parameters;

    public string FilterConditionString => string.Join(" ",
        FilterConditions.Select(x => x.Value).OrderByDescending(x => x.Item2).Select(x => x.Item1));

    public string SqlString => DatabaseInfo.CurrentQueryString +
                               (FilterConditions.Count > 0 ? " " + FilterConditionString : "") + ";";

    public bool ExecuteSql()
    {
        using var conn = new MySqlConnection(DatabaseInfo.ConnectionString);

        try
        {
            if (DataParams.Count < 1)
            {
                conn.Execute(SqlString);
                return true;
            }

            conn.Execute(SqlString, ConvertParams());
            return true;
        }
        catch (MySqlException ex)
        {
            Logger.LogError(
                $"Had an error when trying to execute: {SqlString}");
            Logger.LogException(ex);
            return false;
        }
    }

    public async Task<IEnumerable<T>> Query<T>() where T : new()
    {
        return await QueryAsync<T>();
        
        using var conn = new MySqlConnection(DatabaseInfo.ConnectionString);
        
        try
        {
            return DataParams.Count < 1
                ? conn.Query<T>(SqlString)
                : conn.Query<T>(SqlString,
                    ConvertParams());
        }
        catch (MySqlException ex)
        {
            Logger.LogError(
                $"Had an error when trying to execute: {SqlString}");
            Logger.LogException(ex);
        }

        return new List<T>();
    }
    
    public async Task<IEnumerable<T>> QueryAsync<T>() where T : new()
    {
        await using var conn = new MySqlConnection(DatabaseInfo.ConnectionString);
        
        try
        {
            return DataParams.Count < 1
                ? await conn.QueryAsync<T>(SqlString)
                : await conn.QueryAsync<T>(SqlString,
                    ConvertParams());
        }
        catch (MySqlException ex)
        {
            Logger.LogError(
                $"Had an error when trying to execute: {SqlString}");
            Logger.LogException(ex);
        }

        return new List<T>();
    }

    public async Task<T> QuerySingle<T>() where T : new()
    {
        return await QuerySingleAsync<T>();
        
        using var conn = new MySqlConnection(DatabaseInfo.ConnectionString);
        
        try
        {
            return DataParams.Count < 1
                ? conn.QuerySingle<T>(SqlString)
                : conn.QuerySingle<T>(SqlString,
                    ConvertParams());
        }
        catch (MySqlException ex)
        {
            Logger.LogError(
                $"Had an error when trying to execute: {SqlString}");
            Logger.LogException(ex);
        }

        return new T();
    }

    public async Task<T> QuerySingleAsync<T>() where T : new()
    {
        await using var conn = new MySqlConnection(DatabaseInfo.ConnectionString);
        
        try
        {
            return DataParams.Count < 1
                ? await conn.QuerySingleAsync<T>(SqlString)
                : await conn.QuerySingleAsync<T>(SqlString,
                    ConvertParams());
        }
        catch (MySqlException ex)
        {
            Logger.LogError(
                $"Had an error when trying to execute: {SqlString}");
            Logger.LogException(ex);
        }

        return new T();
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