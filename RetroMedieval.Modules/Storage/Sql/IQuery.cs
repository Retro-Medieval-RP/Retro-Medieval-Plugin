namespace RetroMedieval.Modules.Storage.Sql;

public interface IQuery
{
    public string TableName { get; set; }
    public string CurrentQueryString { get; set; }
    public string FilterConditionString { get; set; }
    
    public string ConnectionString { get; set; }
}