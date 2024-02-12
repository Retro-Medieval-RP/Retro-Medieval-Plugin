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
    public bool SSL { get; set; }

    [JsonIgnore]
    public string ConnectionString => $"Server={Host};Port={Port};Database={Database};Uid={Username};Password={Password};SSL={SSL}";
    
    public void LoadDefaults()
    {
        UseMySql = true;
        Host = "";
        Database = "";
        Username = "";
        Password = "";
        Port = 3306;
        SSL = false;
    }
    
    internal static bool LoadedConfiguration(string data_path)
    {
        if (ConfigurationManager.Instance.Has("SaverConfiguration"))
        {
            return false;
        }

        if (!Directory.Exists(data_path))
        {
            Directory.CreateDirectory(data_path);
        }

        var file_path = Path.Combine(data_path, "SaverConfiguration.json");

        if (File.Exists(file_path))
        {
            string data_text;
            using (var stream = File.OpenText(file_path))
            {
                data_text = stream.ReadToEnd();
            }

            ConfigurationManager.Instance.Add(
                new Configuration.Configuration("SaverConfiguration", JsonConvert.DeserializeObject<SavingConfiguration>(data_text)!));
            return true;
        }
        
        {
            var config = new SavingConfiguration();
            config.LoadDefaults();

            var obj_data = JsonConvert.SerializeObject(config, Formatting.Indented);

            using var stream = new StreamWriter(file_path, false);
            stream.Write(obj_data);

            ConfigurationManager.Instance.Add(new Configuration.Configuration("SaverConfiguration", config));
            
            return true;
        }
    }
}