using System.Windows.Shapes;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Steamworks;
using TheLostLand.Events.Zones;
using TheLostLand.Models.Zones;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Modules.Zones;

[ModuleInformation("Zones")]
[ModuleStorage<ZonesStorage>("ZonesStorage")]
public class ZonesModule : Module
{
    private Dictionary<CSteamID, Zone> PlayersInZones { get; set; } = [];

    public ZonesModule()
    {
        UnturnedPlayerEvents.OnPlayerUpdatePosition += OnPlayerMoved;
    }

    ~ZonesModule()
    {
        UnturnedPlayerEvents.OnPlayerUpdatePosition -= OnPlayerMoved;
    }

    private void OnPlayerMoved(UnturnedPlayer player, Vector3 position)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            Logger.LogError("Could not gather storage for Zones [ZonesStorage]");
            return;
        }

        var zones = storage.GetZones().ToList();
        if (!zones.Any(x => x.IsPointInZone(position)))
        {
            if (!PlayersInZones.ContainsKey(player.CSteamID))
            {
                return;
            }

            ZoneLeftEventPublisher.RaiseEvent(player.CSteamID, PlayersInZones[player.CSteamID]);
            PlayersInZones.Remove(player.CSteamID);
            return;
        }
        
        var zone = zones.First(x => x.IsPointInZone(position));
        ZoneEnteredEventPublisher.RaiseEvent(player.CSteamID, zone);
        PlayersInZones.Add(player.CSteamID, zone);
    }

    public void CreateZone(string zone)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            Logger.LogError("Could not gather storage for Zones [ZonesStorage]");
            return;
        }

        storage.CreateZone(zone);
    }

    public void DeleteZone(string zone)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            Logger.LogError("Could not gather storage for Zones [ZonesStorage]");
            return;
        }

        storage.DeleteZone(zone);
    }

    public void DeleteNode(string zone, int id)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            Logger.LogError("Could not gather storage for Zones [ZonesStorage]");
            return;
        }

        storage.DeleteNode(zone, id);
    }

    public void AddNode(string zone, Vector3 position)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            Logger.LogError("Could not gather storage for Zones [ZonesStorage]");
            return;
        }

        storage.AddNode(zone, position);
    }
}