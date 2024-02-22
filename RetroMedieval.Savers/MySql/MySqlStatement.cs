using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Savers.MySql;

public class MySqlStatement(string table_name, string connection_string) : IStatement
{
    public string TableName { get; set; } = table_name;
    public string CurrentQueryString { get; set; } = "";
    public string ConnectionString { get; set; } = connection_string;

    public List<DataParam> Parameters { get; set; } = [];

    public IExecutor Insert<T>(T obj)
    {
        var columns = typeof(T).GetProperties()
            .Where(x => x.GetValue(obj) != null && !x.GetCustomAttributes<DatabaseIgnore>().Any());
        var column_data = columns.Select(x =>
        {
            if (x.GetCustomAttributes<DatabaseColumn>().Any())
            {
                return (x.GetCustomAttribute<DatabaseColumn>().ColumnName,
                        Value: x.GetValue(obj),
                        PropertyName: x.Name,
                        ProprtyType: x.PropertyType
                    );
            }

            return (ColumnName: x.Name,
                    Value: x.GetValue(obj),
                    PropertyName: x.Name,
                    ProprtyType: x.PropertyType
                );
        }).ToList();

        CurrentQueryString =
            $"INSERT INTO {TableName} ({string.Join(", ", column_data.Select(x => x.ColumnName))}) VALUES ({string.Join(", ", column_data.Select(x => "@" + x.PropertyName))})";
        Parameters.AddRange(
            column_data.Select(data => ConvertDataType(data.PropertyName, data.Value, data.ProprtyType)));
        
        return new MySqlExecutor(this, Parameters, []);
    }

    public ICondition Select(params string[] columns)
    {
        CurrentQueryString = $"SELECT {string.Join(", ", columns)} FROM {TableName}";
        return new MySqlCondition(this);
    }

    public ICondition Count()
    {
        CurrentQueryString = $"SELECT COUNT(*) AS counter FROM {TableName}";
        return new MySqlCondition(this);
    }

    public ICondition Update(params (string, object)[] column_data)
    {
        CurrentQueryString =
            $"UPDATE {TableName} SET {string.Join(" AND ", column_data.Select(x => x.Item1 + " = @" + x.Item1))}";
        Parameters.AddRange(column_data.Select(data => ConvertDataType(data.Item1, data.Item2, data.Item2.GetType())));

        return new MySqlCondition(this);
    }

    public ICondition Delete()
    {
        CurrentQueryString = $"DELETE FROM {TableName}";

        return new MySqlCondition(this);
    }

    private DataParam ConvertDataType(string property_name, object obj, Type prop_type)
    {
        if (Parameters.Any(x => x.ParamName == "@" + property_name))
        {
            property_name += Parameters.Count(x => x.ParamName == "@" + property_name);
        }

        if (prop_type == typeof(byte[]))
            return new DataParam("@" + property_name, (byte[])obj, DbType.Binary) { ParamType = prop_type };

        if (prop_type == typeof(Guid))
            return new DataParam("@" + property_name, ((Guid)obj).ToString()) { ParamType = prop_type };

        return new DataParam("@" + property_name, obj) { ParamType = prop_type };
    }
}