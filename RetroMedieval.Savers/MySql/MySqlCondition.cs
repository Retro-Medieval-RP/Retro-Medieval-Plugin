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
    public Dictionary<string, (string, int)> FilterConditions { get; set; } = [];
    public List<DataParam> Parameters { get; set; } = statement.Parameters;

    public ICondition Where(params (string, object)[] conditions)
    {
        if (conditions.Length < 1)
        {
            throw new NoConditionValues();
        }

        if (FilterConditions.TryGetValue("WHERE", out var storedWhere))
        {
            storedWhere.Item1 += $" AND {string.Join(" AND ", conditions.Select(x => x.Item1 + " = @" + x.Item1))}";

            Parameters.AddRange(
                conditions.Select(data => ConvertDataType(data.Item1, data.Item2, data.Item2.GetType())));
        }
        else
        {
            FilterConditions.Add("WHERE",
                new($"WHERE {string.Join(" AND ", conditions.Select(x => x.Item1 + " = @" + x.Item1))}", 10));

            Parameters.AddRange(
                conditions.Select(data => ConvertDataType(data.Item1, data.Item2, data.Item2.GetType())));
        }

        return this;
    }

    public ICondition WhereStartsWith(params (string, string)[] conditions)
    {
        if (conditions.Length < 1)
        {
            throw new NoConditionValues();
        }

        if (FilterConditions.TryGetValue("WHERE", out var storedWhere))
        {
            storedWhere.Item1 +=
                $" AND {string.Join(" AND ", conditions.Select(x => x.Item1 + " = " + x.Item2 + "%"))}";
        }
        else
        {
            FilterConditions.Add("WHERE",
                new($"WHERE {string.Join(" AND ", conditions.Select(x => x.Item1 + " = " + x.Item2 + "%"))}", 10));
        }

        return this;
    }

    public ICondition WhereEndsWith(params (string, string)[] conditions)
    {
        if (conditions.Length < 1)
        {
            throw new NoConditionValues();
        }

        if (FilterConditions.TryGetValue("WHERE", out var storedWhere))
        {
            storedWhere.Item1 +=
                $" AND {string.Join(" AND ", conditions.Select(x => x.Item1 + " = %" + x.Item2))}";
        }
        else
        {
            FilterConditions.Add("WHERE",
                new($"WHERE {string.Join(" AND ", conditions.Select(x => x.Item1 + " = %" + x.Item2))}", 10));
        }

        return this;
    }

    public ICondition WhereContains(params (string, string)[] conditions)
    {
        if (conditions.Length < 1)
        {
            throw new NoConditionValues();
        }

        if (FilterConditions.TryGetValue("WHERE", out var storedWhere))
        {
            storedWhere.Item1 +=
                $" AND {string.Join(" AND ", conditions.Select(x => x.Item1 + " = %" + x.Item2 + "%"))}";
        }
        else
        {
            FilterConditions.Add("WHERE",
                new($"WHERE {string.Join(" AND ", conditions.Select(x => x.Item1 + " = %" + x.Item2 + "%"))}", 10));
        }

        return this;
    }

    public ICondition OrderBy(params (string, OrderBy)[] columns)
    {
        if (FilterConditions.ContainsKey("ORDER BY"))
        {
            throw new OrderBySortAlreadyGiven();
        }
        
        FilterConditions.Add("ORDER BY",
            new(
                $"ORDER BY {string.Join(", ", columns.Select(x => $"{x.Item1} {(x.Item2 == Modules.Storage.Sql.OrderBy.Ascending ? "ASC" : "DESC")}"))}",
                1));
        return this;
    }

    public ICondition OrderBy(OrderBy order = Modules.Storage.Sql.OrderBy.Ascending, params string[] columns)
    {
        if (FilterConditions.ContainsKey("ORDER BY"))
        {
            throw new OrderBySortAlreadyGiven();
        }
        
        FilterConditions.Add("ORDER BY",
            new(
                $"ORDER BY {string.Join(", ", columns)} {(order == Modules.Storage.Sql.OrderBy.Ascending ? "ASC" : "DESC")}",
                1));
        return this;
    }

    public IExecutor Finalise() =>
        new MySqlExecutor(this, Parameters, FilterConditions);

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