using System;
using System.Data;
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

    public static void Insert<T>(this IQuery query, T obj)
    {
        var columns = typeof(T).GetProperties()
            .Where(x => x.GetCustomAttributes<DatabaseColumn>().Any() && x.GetValue(obj) != null);
        var column_data = columns.Select(x =>
            (x.GetCustomAttribute<DatabaseColumn>().ColumnName,
                Value: x.GetValue(obj),
                PropertyName: x.Name,
                ProprtyType: x.PropertyType
            )).ToList();

        query.CurrentQueryString =
            $"INSERT INTO {query.TableName} ({string.Join(", ", column_data.Select(x => x.ColumnName))}) VALUES ({string.Join(", ", column_data.Select(x => "@" + x.PropertyName))})";

        using var conn = new MySqlConnection(query.ConnectionString);

        try
        {
            var data_params = new DynamicParameters();

            foreach (var data in column_data)
            {
                ConvertDataType(data.PropertyName, data.Value, data.ProprtyType, ref data_params);
            }

            conn.Execute(query.CurrentQueryString, data_params);
        }
        catch (MySqlException ex)
        {
            Logger.LogError(
                $"Had an error when trying to execute: {query.CurrentQueryString} {query.FilterConditionString};");
            Logger.LogException(ex);
        }
    }

    private static void ConvertDataType(string property_name, object obj, Type prop_type, ref DynamicParameters data_params)
    {
        if (prop_type == typeof(byte[]))
            data_params.Add("@" + property_name, obj as byte[], DbType.Binary);
        if (prop_type == typeof(Guid))
            data_params.Add("@" + property_name, obj.ToString());
        else
            data_params.Add("@" + property_name, obj);
    }
}