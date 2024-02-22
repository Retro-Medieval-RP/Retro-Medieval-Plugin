using System.Collections.Generic;
using RetroMedieval.Modules.Storage.Sql;

namespace RetroMedieval.Savers.MySql;

public class MySqlStatement(
    string table_name,
    string current_query_string,
    string filter_condition_string,
    string connection_string)
    : IStatement
{
    public MySqlStatement(IQuery query) : this(query.TableName, query.CurrentQueryString, query.FilterConditionString, query.ConnectionString)
    {
    }

    public string TableName { get; set; } = table_name;
    public string CurrentQueryString { get; set; } = current_query_string;
    public string FilterConditionString { get; set; } = filter_condition_string;
    public string ConnectionString { get; set; } = connection_string;
    public List<DataParam> Parameters { get; set; } = [];
}