using System;
using System.Data;
using System.Linq;
using System.Reflection;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql.Exceptions;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Savers.MySql.StatementsAndQueries;

public static class Queries
{
    public static IStatement Where(this IQuery query, params (string, object)[] condition_values)
    {
        if (condition_values.Length < 1)
        {
            throw new NoConditionValues();
        }
        
        query.FilterConditionString =
            $"WHERE {string.Join(" AND ", condition_values.Select(x => x.Item1 + " = @" + x.Item1))};";

        var data_params = condition_values.Select(data => ConvertDataType(data.Item1, data.Item2, data.Item2.GetType())).ToList();
        return new MySqlStatement(query) { Parameters = data_params};
    }
    
    public static IExecutor Count(this IQuery query, params (string, object)[] condition_values)
    {
        if (condition_values.Length < 1)
        {
            throw new NoConditionValues();
        }

        query.CurrentQueryString =
            $"SELECT COUNT({condition_values.Select(x => x.Item1).ToArray()[0]}) FROM {query.TableName} WHERE {string.Join(" AND ", condition_values.Select(x => x.Item1 + " = @" + x.Item1))};";

        var data_params = condition_values.Select(data => ConvertDataType(data.Item1, data.Item2, data.Item2.GetType())).ToList();
        return new MySqlExecutor(query, data_params);
    }

    public static IExecutor Insert<T>(this IQuery query, T obj)
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
            $"INSERT INTO {query.TableName} ({string.Join(", ", column_data.Select(x => x.ColumnName))}) VALUES ({string.Join(", ", column_data.Select(x => "@" + x.PropertyName))});";

        var data_params = column_data.Select(data => ConvertDataType(data.PropertyName, data.Value, data.ProprtyType)).ToList();
        return new MySqlExecutor(query, data_params);
    }

    public static DataParam ConvertDataType(string property_name, object obj, Type prop_type)
    {
        if (prop_type == typeof(byte[]))
            return new DataParam("@" + property_name, (byte[])obj, DbType.Binary) { ParamType = prop_type };

        if (prop_type == typeof(Guid))
            return new DataParam("@" + property_name, ((Guid)obj).ToString()) { ParamType = prop_type };

        return new DataParam("@" + property_name, obj) { ParamType = prop_type };
    }
}