using System.Collections.Generic;
using System.Linq;
using CityZones.Models;
using RetroMedieval.Savers.Json;

namespace CityZones;

internal class CitiesStorage : JsonSaver<List<City>>
{
    public void AddCity(City city)
    {
        StorageItem.Add(city);
        Save();
    }

    public void RemoveCity(string zoneName)
    {
        StorageItem.RemoveAll(x => x.ZoneName == zoneName);
        Save();
    }

    public City GetCity(string zoneName) => StorageItem.First(x => x.ZoneName == zoneName);

    public bool CityExists(string zoneName) => StorageItem.Any(x => x.ZoneName == zoneName);
}