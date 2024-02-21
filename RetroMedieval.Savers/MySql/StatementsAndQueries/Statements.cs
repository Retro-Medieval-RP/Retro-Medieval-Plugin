using System;
using System.Data;
using System.Linq;
using RetroMedieval.Modules.Storage.Sql;

namespace RetroMedieval.Savers.MySql.StatementsAndQueries;

public static class Statements
{
    public static IExecutor Select(this IStatement statement, params string[] columns)
    {
        statement.CurrentQueryString = $"SELECT {string.Join(" AND ", columns)} FROM {statement.TableName}";
        
        return new MySqlExecutor(statement, []);
    }
    
    public static IExecutor Update(this IStatement statement, params (string, object)[] columns)
    {
        statement.CurrentQueryString = $"UPDATE {statement.TableName} SET {string.Join(" AND ", columns.Select(x => x.Item1 + " = @" + x.Item1))}";
        statement.Parameters.AddRange(columns.Select(data => ConvertDataType(data.Item1, data.Item2, data.Item2.GetType())));
        
        return new MySqlExecutor(statement, statement.Parameters);
    }

    public static IExecutor Delete(this IStatement statement, params (string, object)[] columns)
    {
        statement.CurrentQueryString = $"DELETE FROM {statement.TableName}";
        return new MySqlExecutor(statement, []);
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