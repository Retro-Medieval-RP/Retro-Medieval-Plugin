using TheLostLand.Modules.Configuration;

namespace TheLostLand.Modules.Death;

public class DeathsConfiguration : IConfig
{
    public ushort ManID { get; set; }
    public double DespawnTime { get; set; }

    public void LoadDefaults()
    {
        ManID = 1409;
        DespawnTime = 600000;
    }
}