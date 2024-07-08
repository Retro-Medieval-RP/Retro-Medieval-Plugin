using RetroMedieval.Modules.Configuration;

namespace CityZones;

internal class CityZonesConfiguration : IConfig
{
    public ushort ID { get; set; }
    
    public void LoadDefaults()
    {
        ID = 17100;
    }
}