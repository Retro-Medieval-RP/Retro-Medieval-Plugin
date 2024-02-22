using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql.Exceptions;

namespace RetroMedieval.Savers.MySql;

public class MySqlCondition(IStatement statement) : ICondition
{
    public string TableName { get; set; } = statement.TableName;
    public string CurrentQueryString { get; set; } = statement.CurrentQueryString;
    public string ConnectionString { get; set; } = statement.ConnectionString;
    public List<(string, int)> FilterConditions { get; set; } = [];
    public List<DataParam> Parameters { get; set; } = statement.Parameters;

    public ICondition Where(params (string, object)[] conditions)
    {
        if (conditions.Length < 1)
        {
            throw new NoConditionValues();
        }

        FilterConditions.Add(new ($"WHERE {string.Join(" AND ", conditions.Select(x => x.Item1 + " = @" + x.Item1))}", 10));

        Parameters.AddRange(conditions.Select(data => ConvertDataType(data.Item1, data.Item2, data.Item2.GetType())));

        return this;
    }

    public ICondition OrderBy(params (string, OrderBy)[] columns)
    {
        FilterConditions.Add(new ($"ORDER BY {string.Join(", ", columns.Select(x => $"{x.Item1} {(x.Item2 == Modules.Storage.Sql.OrderBy.Ascending ? "ASC" : "DESC")}"))}", 1));
        return this;
    }

    public ICondition OrderBy(OrderBy order = Modules.Storage.Sql.OrderBy.Ascending, params string[] columns)
    {
        FilterConditions.Add(new ($"ORDER BY {string.Join(", ", columns)} {(order == Modules.Storage.Sql.OrderBy.Ascending ? "ASC" : "DESC")}", 1));
        return this;
    }

    public IExecutor Finalise() =>
        new MySqlExecutor(this, Parameters, FilterConditions);

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