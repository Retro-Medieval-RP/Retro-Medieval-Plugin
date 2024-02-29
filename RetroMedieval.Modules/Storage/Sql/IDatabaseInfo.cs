namespace RetroMedieval.Modules.Storage.Sql;

public interface IDatabaseInfo
{
    public string TableName { get; set; }
    public string CurrentQueryString { get; set; }
    public string ConnectionString { get; set; }
}