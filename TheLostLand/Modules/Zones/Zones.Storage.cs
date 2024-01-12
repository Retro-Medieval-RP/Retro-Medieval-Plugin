using TheLostLand.Core.Storage;
using TheLostLand.Models.Zones;

namespace TheLostLand.Modules.Zones;

public class ZonesStorage : IStorage
{
    private List<Zone> Zones { get; set; } = [];
    
    public void Save()
    {
    }

    public void Load()
    {
    }

    public IEnumerable<Zone> GetZones() => Zones;
}