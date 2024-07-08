using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using RetroMedieval.Shared.Events.Zones;
using RetroMedieval.Shared.Models.Zones;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Zones;

[ModuleInformation("Zones")]
[ModuleStorage<ZonesStorage>("ZonesList")]
public class ZonesModule([NotNull] string directory) : Module(directory)
{
    private Dictionary<UnturnedPlayer, Zone> PlayersInZones { get; } = [];

    public override void Load()
    {
        UnturnedPlayerEvents.OnPlayerUpdatePosition += OnPlayerMove;
    }

    public override void Unload()
    {
        UnturnedPlayerEvents.OnPlayerUpdatePosition -= OnPlayerMove;
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

            var allowActivation = true;
            ZoneEnterEventPublisher.RaiseEvent(ref player, ref zone, ref allowActivation);

            if (allowActivation)
            {
                PlayersInZones.Add(player, zone);
            }

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

    private bool IsInZone(Vector3 point) =>
        GetStorage<ZonesStorage>(out var storage) &&
        storage.GetZones().Any(zone => zone.IsInZone(point));

    public Zone GetZone(Vector3 point) =>
        GetStorage<ZonesStorage>(out var storage) ? storage.GetZones()?.FirstOrDefault(x => x.IsInZone(point)) : null;

    public bool CreateZone(string zoneName)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            return false;
        }

        storage.AddZone(zoneName);
        return true;
    }

    public bool DeleteZone(string zoneName)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            return false;
        }

        storage.RemoveZone(zoneName);
        return true;
    }

    public bool AddNode(string zoneName, Vector3 point, out int id)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            id = default;
            return false;
        }

        storage.AddNode(zoneName, new Node(point));

        var zone = storage.GetZone(zoneName);
        id = zone.Nodes.Count - 1;

        return true;
    }

    public bool RemoveNode(string zoneName, int id)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            return false;
        }

        storage.RemoveNode(zoneName, id);
        return true;
    }

    public bool Exists(string zoneName)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            return false;
        }

        return storage.Exists(zoneName);
    }

    public bool GetZone(string zoneName, out Zone zone)
    {
        if (!GetStorage<ZonesStorage>(out var storage))
        {
            zone = null;
            return false;
        }

        zone = storage.GetZone(zoneName);
        return true;
    }
}