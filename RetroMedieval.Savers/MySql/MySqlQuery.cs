using RetroMedieval.Modules.Storage.Sql;

namespace RetroMedieval.Savers.MySql;

public class MySqlQuery(
    string table_name,
    string connection_string)
    : IQuery
{
    public string TableName { get; set; } = table_name;
    public string CurrentQueryString { get; set; } = "";
    public string FilterConditionString { get; set; } = "";
    public string ConnectionString { get; set; } = connection_string;
}