using System.Collections.Generic;
using System.Linq;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using TheLostLand.Core.Modules;
using TheLostLand.Core.Modules.Attributes;
using TheLostLand.Events.Zones;
using TheLostLand.Models.Zones;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace TheLostLand.Modules.Zones;

[ModuleInformation("Zones")]
[ModuleStorage<ZonesStorage>("ZonesList")]
public class ZonesModule : Module
{
    private Dictionary<UnturnedPlayer, Zone> PlayersInZones { get; } = [];

    public override void Load()
    {
        UnturnedPlayerEvents.OnPlayerUpdatePosition += OnPlayerMove;
    }

    private void OnPlayerMove(UnturnedPlayer player, Vector3 position)
    {
        if (IsInZone(position))
        {
            if (PlayersInZones.ContainsKey(player))
            {
                return;
            }

            var zone = GetZone(position);
            PlayersInZones.Add(player, zone);

            ZoneEnterEventPublisher.RaiseEvent(player, zone);
            return;
        }

        if (!PlayersInZones.ContainsKey(player))
        {
            return;
        }

        {
            var zone = PlayersInZones[player];
            PlayersInZones.Remove(player);

            ZoneLeftEventPublisher.RaiseEvent(player, zone);
        }
    }

    public override void Unload()
    {
        UnturnedPlayerEvents.OnPlayerUpdatePosition -= OnPlayerMove;
    }

    private bool IsInZone(Vector3 point) =>
        GetStorage<ZonesStorage>(out var storage) &&
        storage.GetZones().Select(zone => zone.IsInZone(point)).FirstOrDefault();

    private Zone GetZone(Vector3 point) =>
        GetStorage<ZonesStorage>(out var storage) ? storage.GetZones()?.FirstOrDefault(x => x.IsInZone(point)) : null;

    public bool CreateZone(string zone_name)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            return false;
        }

        storage.AddZone(zone_name);
        return true;
    }

    public bool DeleteZone(string zone_name)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            return false;
        }

        storage.RemoveZone(zone_name);
        return true;
    }

    public bool AddNode(string zone_name, Vector3 point, out int id)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            id = default;
            return false;
        }

        storage.AddNode(zone_name, new Node(point));

        var zone = storage.GetZone(zone_name);
        id = zone.Nodes.Count - 1;

        return true;
    }

    public bool RemoveNode(string zone_name, int id)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            return false;
        }
        
        storage.RemoveNode(zone_name, id);
        return true;
    }
}