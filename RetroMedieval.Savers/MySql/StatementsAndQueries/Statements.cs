using System.Linq;
using RetroMedieval.Modules.Storage.Sql;

namespace RetroMedieval.Savers.MySql.StatementsAndQueries;

public static class Statements
{
    public static IExecutor SelectMultiple(this IStatement statement, params string[] columns)
    {
        statement.CurrentQueryString = $"SELECT {string.Join(" AND ", columns)} FROM {statement.TableName}";
        return new MySqlExecutor(statement);
    }

    public static IExecutor SelectSingle(this IStatement statement, string column)
    {
        statement.CurrentQueryString = $"SELECT {column} FROM {statement.TableName}";
        return new MySqlExecutor(statement);
    }

    public static IExecutor Update(this IStatement statement, params (string, object)[] columns)
    {
        statement.CurrentQueryString = $"UPDATE {statement.TableName} SET {string.Join(" AND ", columns.Select(x => x.Item1 + " = " + x.Item2))}";
        return new MySqlExecutor(statement);
    }

    public static IExecutor Delete(this IStatement statement, params (string, object)[] columns)
    {
        statement.CurrentQueryString = $"DELETE FROM {statement.TableName}";
        return new MySqlExecutor(statement);
    }
}