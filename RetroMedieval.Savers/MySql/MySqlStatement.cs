using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql.Tables.Attributes;

namespace RetroMedieval.Savers.MySql;

public class MySqlStatement(string tableName, string connectionString) : IStatement
{
    public string TableName { get; set; } = tableName;
    public string CurrentQueryString { get; set; } = "";
    public string ConnectionString { get; set; } = connectionString;

    public List<DataParam> Parameters { get; set; } = [];

    public IExecutor Insert<T>(T obj)
    {
        var columns = typeof(T).GetProperties()
            .Where(x => x.GetValue(obj) != null && !x.GetCustomAttributes<DatabaseIgnore>().Any());
        var columnData = columns.Select(x =>
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
            $"INSERT INTO {TableName} ({string.Join(", ", columnData.Select(x => x.ColumnName))}) VALUES ({string.Join(", ", columnData.Select(x => "@" + x.PropertyName))})";
        Parameters.AddRange(
            columnData.Select(data => ConvertDataType(data.PropertyName, data.Value, data.ProprtyType)));
        
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

    public ICondition Update(params (string, object)[] columnData)
    {
        CurrentQueryString =
            $"UPDATE {TableName} SET {string.Join(" AND ", columnData.Select(x => x.Item1 + " = @" + x.Item1))}";
        Parameters.AddRange(columnData.Select(data => ConvertDataType(data.Item1, data.Item2, data.Item2.GetType())));

        return new MySqlCondition(this);
    }

    public ICondition Delete()
    {
        CurrentQueryString = $"DELETE FROM {TableName}";

        return new MySqlCondition(this);
    }

    private DataParam ConvertDataType(string propertyName, object obj, Type propType)
    {
        if (Parameters.Any(x => x.ParamName == "@" + propertyName))
        {
            propertyName += Parameters.Count(x => x.ParamName == "@" + propertyName);
        }

        if (propType == typeof(byte[]))
            return new DataParam("@" + propertyName, (byte[])obj, DbType.Binary) { ParamType = propType };

        if (propType == typeof(Guid))
            return new DataParam("@" + propertyName, ((Guid)obj).ToString()) { ParamType = propType };

        return new DataParam("@" + propertyName, obj) { ParamType = propType };
    }
}