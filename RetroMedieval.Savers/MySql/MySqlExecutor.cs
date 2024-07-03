using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using MySql.Data.MySqlClient;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql.Tables.Attributes;
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
            var command = new MySqlCommand(SqlString, conn);
            conn.Open();

            if (DataParams.Count < 1)
            {
                command.ExecuteNonQuery();
                conn.Close();
                return true;
            }

            foreach (var param in ConvertParams())
            {
                command.Parameters.Add(param);
            }

            command.ExecuteNonQuery();
            conn.Close();
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

    public IEnumerable<T> Query<T>() where T : new()
    {
        using var conn = new MySqlConnection(DatabaseInfo.ConnectionString);
        var output = new List<T>();

        try
        {
            var command = new MySqlCommand(SqlString, conn);
            conn.Open();
            
            if (DataParams.Count < 1)
            {
                var reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var fieldName = reader.GetName(i);
                        var o = new T();

                        foreach (var prop in o.GetType().GetProperties())
                        {
                            if (prop.GetCustomAttributes().All(x => x.GetType() != typeof(DatabaseColumn)))
                            {
                                continue;
                            }

                            var column = prop
                                .GetCustomAttributes()
                                .First(x => x.GetType() == typeof(DatabaseColumn)) as DatabaseColumn;

                            if (fieldName == column?.ColumnName)
                            {
                                prop.SetValue(o, reader[i]);
                            }
                        }
                        
                        output.Add(o);
                    }
                }

                conn.Close();
                return output;
            }
            
            foreach (var param in ConvertParams())
            {
                command.Parameters.Add(param);
            }
            
            var readerParams = command.ExecuteReader();
            while (readerParams.Read())
            {
                for (var i = 0; i < readerParams.FieldCount; i++)
                {
                    var fieldName = readerParams.GetName(i);
                    var o = new T();

                    foreach (var prop in o.GetType().GetProperties())
                    {
                        if (prop.GetCustomAttributes().All(x => x.GetType() != typeof(DatabaseColumn)))
                        {
                            continue;
                        }

                        var column = prop
                            .GetCustomAttributes()
                            .First(x => x.GetType() == typeof(DatabaseColumn)) as DatabaseColumn;

                        if (fieldName == column?.ColumnName)
                        {
                            prop.SetValue(o, readerParams[i]);
                        }
                    }
                    
                    output.Add(o);
                }
            }
            conn.Close();
            return output;
        }
        catch (MySqlException ex)
        {
            Logger.LogError(
                $"Had an error when trying to execute: {SqlString}");
            Logger.LogException(ex);
        }

        return output;
    }

    public T QuerySingle<T>() where T : new()
    {
        using var conn = new MySqlConnection(DatabaseInfo.ConnectionString);
        var output = new List<T>();

        try
        {
            var command = new MySqlCommand(SqlString, conn);
            conn.Open();
            
            if (DataParams.Count < 1)
            {
                var reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var fieldName = reader.GetName(i);
                        var o = new T();

                        foreach (var prop in o.GetType().GetProperties())
                        {
                            if (prop.GetCustomAttributes().All(x => x.GetType() != typeof(DatabaseColumn)))
                            {
                                continue;
                            }

                            var column = prop
                                .GetCustomAttributes()
                                .First(x => x.GetType() == typeof(DatabaseColumn)) as DatabaseColumn;

                            if (fieldName == column?.ColumnName)
                            {
                                prop.SetValue(o, reader[i]);
                            }
                        }
                        
                        output.Add(o);
                    }
                }

                conn.Close();
                return output.Any() ? output.First() : new T();
            }
            
            foreach (var param in ConvertParams())
            {
                command.Parameters.Add(param);
            }
            
            var readerParams = command.ExecuteReader();
            while (readerParams.Read())
            {
                for (var i = 0; i < readerParams.FieldCount; i++)
                {
                    var fieldName = readerParams.GetName(i);
                    var o = new T();

                    foreach (var prop in o.GetType().GetProperties())
                    {
                        if (prop.GetCustomAttributes().All(x => x.GetType() != typeof(DatabaseColumn)))
                        {
                            continue;
                        }

                        var column = prop
                            .GetCustomAttributes()
                            .First(x => x.GetType() == typeof(DatabaseColumn)) as DatabaseColumn;

                        if (fieldName == column?.ColumnName)
                        {
                            prop.SetValue(o, readerParams[i]);
                        }
                    }
                    
                    output.Add(o);
                }
            }
            conn.Close();
            return output.Any() ? output.First() : new T();
        }
        catch (MySqlException ex)
        {
            Logger.LogError(
                $"Had an error when trying to execute: {SqlString}");
            Logger.LogException(ex);
        }

        return new T();
    }

    private IEnumerable<MySqlParameter> ConvertParams() =>
        DataParams.Select(param =>
        {
            var p = new MySqlParameter(param.ParamName, param.ParamObject)
            {
                DbType = param.ParamDbType ?? DbType.String
            };

            return p;
        });
}