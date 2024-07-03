using System.IO;
using Newtonsoft.Json;
using RetroMedieval.Modules.Configuration;

namespace RetroMedieval.Modules.Storage;

internal class SavingConfiguration : IConfig
{
    public bool UseMySql { get; set; }
    public string Host { get; set; } = null!;
    public string Database { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int Port { get; set; }

    [JsonIgnore]
    public string ConnectionString => $"Server={Host};Port={Port};Database={Database};Uid={Username};Password={Password};";
    
    public void LoadDefaults()
    {
        UseMySql = true;
        Host = "";
        Database = "";
        Username = "";
        Password = "";
        Port = 3306;
    }
    
    internal static bool LoadedConfiguration(string dataPath)
    {
        if (ConfigurationManager.Instance.Has("SaverConfiguration"))
        {
            return false;
        }

        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        var filePath = Path.Combine(dataPath, "SaverConfiguration.json");

        if (File.Exists(filePath))
        {
            string dataText;
            using (var stream = File.OpenText(filePath))
            {
                dataText = stream.ReadToEnd();
            }

            ConfigurationManager.Instance.Add(
                new Configuration.Configuration("SaverConfiguration", JsonConvert.DeserializeObject<SavingConfiguration>(dataText)!));
            return true;
        }
        
        {
            var config = new SavingConfiguration();
            config.LoadDefaults();

            var objData = JsonConvert.SerializeObject(config, Formatting.Indented);

            using var stream = new StreamWriter(filePath, false);
            stream.Write(objData);

            ConfigurationManager.Instance.Add(new Configuration.Configuration("SaverConfiguration", config));
            
            return true;
        }
    }
}