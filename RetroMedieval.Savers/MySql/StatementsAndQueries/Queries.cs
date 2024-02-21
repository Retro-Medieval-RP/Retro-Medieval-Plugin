using System;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql.Exceptions;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Savers.MySql.StatementsAndQueries;

public static class Queries
{
    public static IExecutor Count(this IQuery query, params (string, object)[] condition_values)
    {
        if (condition_values.Length < 1)
        {
            throw new NoConditionValues();
        }
        
        query.CurrentQueryString = $"SELECT COUNT({condition_values.Select(x => x.Item1).ToArray()[0]}) FROM {query.TableName} WHERE {string.Join(" AND ", condition_values.Select(x => x.Item1 + " = @" + x.Item1))};";
        var data_params = new DynamicParameters();
        
        foreach (var param in condition_values.Select(data =>
                     ConvertDataType(data.Item1, data.Item2, data.Item2.GetType())))
        {
            data_params.Add(param.ParamName, param.ParamObject, param.ParamDbType);
        }

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
            $"INSERT INTO {query.TableName} ({string.Join(", ", column_data.Select(x => x.ColumnName))}) VALUES ({string.Join(", ", column_data.Select(x => "@" + x.PropertyName))})";

        var data_params = new DynamicParameters();

        foreach (var param in column_data.Select(data =>
                     ConvertDataType(data.PropertyName, data.Value, data.ProprtyType)))
        {
            data_params.Add(param.ParamName, param.ParamObject, param.ParamDbType);
        }

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

    public class DataParam(string name, object obj, DbType? type = null)
    {
        public string ParamName { get; } = name;
        public object ParamObject { get; } = obj;
        public DbType? ParamDbType { get; } = type;
        public Type ParamType { get; set; } = null!;

        public override bool Equals(object? obj) =>
            obj is DataParam param && Equals(param);

        protected bool Equals(DataParam other) =>
            ParamName == other.ParamName && ParamObject.Equals(other.ParamObject) && ParamDbType == other.ParamDbType &&
            ParamType == other.ParamType;

        public override int GetHashCode()
        {
            unchecked
            {
                var hash_code = ParamName.GetHashCode();
                hash_code = (hash_code * 397) ^ ParamObject.GetHashCode();
                hash_code = (hash_code * 397) ^ ParamDbType.GetHashCode();
                hash_code = (hash_code * 397) ^ ParamType.GetHashCode();
                return hash_code;
            }
        }
    }
}