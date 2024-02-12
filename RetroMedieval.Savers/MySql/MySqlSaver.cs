using System.Collections;
using System.Collections.Generic;
using Dapper;
using MySql.Data.MySqlClient;
using RetroMedieval.Modules.Storage;
using RetroMedieval.Modules.Storage.Sql;
using RetroMedieval.Savers.MySql.Queries;
using RetroMedieval.Savers.MySql.Tables;
using Rocket.Core.Logging;

namespace RetroMedieval.Savers.MySql;

public class MySqlSaver<T> : ISqlStorage<T>
{
    public string SavePath { get; private set; } = "";
    public StorageType StorageType => StorageType.Sql;

    private MySqlConnection Connection => new(SavePath);
    
    private string TableName { get; set; } = "";

    public bool Load(string file_path)
    {
        SavePath = file_path;
        using var connection = Connection;

        try
        {
            var ddl = TableGenerator.GenerateDDL(typeof(T), out var table_name);
            TableName = table_name;
            
            connection.Execute(ddl);
            return true;
        }
        catch (MySqlException ex)
        {
            Logger.LogError("An error has occured while trying to execute or generate ddl for: " + typeof(T).Name);
            Logger.LogException(ex);
            return false;
        }
    }

    public IQuery StartQuery() => 
        new MySqlQuery(TableName, SavePath);
}