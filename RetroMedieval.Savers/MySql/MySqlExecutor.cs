using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;
using RetroMedieval.Modules.Storage.Sql;
using Rocket.Core.Logging;

namespace RetroMedieval.Savers.MySql;

public class MySqlExecutor(IDatabaseInfo info, List<DataParam> parameters, Dictionary<string, (string, int)> filter_conditions) : IExecutor
{
    public IDatabaseInfo DatabaseInfo { get; set; } = info;
    public Dictionary<string, (string, int)> FilterConditions { get; set; } = filter_conditions;
    public List<DataParam> DataParams { get; set; } = parameters;
    public string FilterConditionString => string.Join(" ", FilterConditions.Select(x => x.Value).OrderByDescending(x => x.Item2).Select(x => x.Item1));
    public string SqlString => DatabaseInfo.CurrentQueryString + (FilterConditions.Count > 0 ? " " + FilterConditionString : "") + ";";

    public void ExecuteSql()
    {
        using var conn = new MySqlConnection(DatabaseInfo.ConnectionString);

        try
        {
            if (DataParams.Count < 1)
            {
                conn.Execute(SqlString);
                return;
            }

            conn.Execute(SqlString, ConvertParams());
        }
        catch (MySqlException ex)
        {
            Logger.LogError(
                $"Had an error when trying to execute: {SqlString}");
            Logger.LogException(ex);
        }
    }

    public T? QuerySql<T>()
    {
        using var conn = new MySqlConnection(DatabaseInfo.ConnectionString);

        if (typeof(T).GetInterfaces()
            .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            try
            {
                return DataParams.Count < 1
                    ? (T)conn.Query<T>(SqlString)
                    : (T)conn.Query<T>(SqlString,
                        ConvertParams());
            }
            catch (MySqlException ex)
            {
                Logger.LogError(
                    $"Had an error when trying to execute: {SqlString}");
                Logger.LogException(ex);
            }

            return default;
        }

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