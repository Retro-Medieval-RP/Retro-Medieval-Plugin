using MySql.Data.MySqlClient;
using RetroMedieval.Modules.Storage;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql.Tables;
using Rocket.Core.Logging;

namespace RetroMedieval.Savers.MySql;

public class MySqlSaver<T> : ISqlStorage<T>
{
    public string SavePath { get; private set; } = "";
    public StorageType StorageType => StorageType.Sql;

    private MySqlConnection Connection => new(SavePath);
    
    private string TableName { get; set; } = "";

    public bool Load(string filePath)
    {
        SavePath = filePath;
        using var connection = Connection;

        try
        {
            var ddl = TableGenerator.GenerateDdl(typeof(T), out var tableName);
            TableName = tableName;

            var command = new MySqlCommand(ddl, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            return true;
        }
        catch (MySqlException ex)
        {
            Logger.LogError("An error has occured while trying to execute or generate ddl for: " + typeof(T).Name);
            Logger.LogException(ex);
            return false;
        }
    }

    public IStatement StartQuery() => 
        new MySqlStatement(TableName, SavePath);
}