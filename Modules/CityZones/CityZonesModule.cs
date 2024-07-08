using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using Zones.Events;

namespace CityZones
{
    [ModuleInformation("CityZones")]
    [ModuleConfiguration<CityZonesConfiguration>("CityZonesConfiguration")]
    [ModuleStorage<CitiesStorage>("Cities")]
    public class CityZonesModule(string directory) : Module(directory)
    {
        public override void Load()
        {
            ZoneEnterEventPublisher.ZoneEnterEvent += OnZoneEnter;
        }

        public override void Unload()
        {
            ZoneEnterEventPublisher.ZoneEnterEvent -= OnZoneEnter;
        }

        private void OnZoneEnter(ZoneEnterEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}