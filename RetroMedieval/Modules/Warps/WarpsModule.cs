using System.Linq;
using JetBrains.Annotations;
using RetroMedieval.Models.Warps;
using RetroMedieval.Modules.Attributes;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RetroMedieval.Modules.Warps;

[ModuleInformation("Warps")]
[ModuleStorage<WarpsStorage>("Warps")]
internal class WarpsModule : Module
{
    public override void Load()
    {
    }

    public override void Unload()
    {
    }

    public void AddWarp(string warp_name, Vector3 location, float rotation, [CanBeNull] UnturnedPlayer user_to_message = null)
    {
        if (!GetStorage<WarpsStorage>(out var warps_storage))
        {
            Logger.LogError("Could not gather storage [WarpsStorage]");
            return;
        }
        
        var warp = new Warp
        {
            WarpName = warp_name,
            LocationX = location.x,
            LocationY = location.y,
            LocationZ = location.z,
            Rotation = rotation
        };

        warps_storage.AddWarp(warp);
        if (user_to_message != null)
        {
            UnturnedChat.Say(user_to_message, $"Warp ({warp_name}) has been created.");
        }
    }

    public void RemoveWarp(string warp_name, [CanBeNull] UnturnedPlayer user_to_message = null)
    {
        if (!GetStorage<WarpsStorage>(out var warps_storage))
        {
            Logger.LogError("Could not gather storage [WarpsStorage]");
            return;
        }
        
        if (!warps_storage.ContainsWarp(warp_name))
        {
            Logger.LogWarning($"Warp ({warp_name}) does not exist");
            if (user_to_message != null)
            {
                UnturnedChat.Say(user_to_message, $"Warp ({warp_name}) does not exist");
            }
            return;
        }

        warps_storage.RemoveWarp(warp_name);
        if (user_to_message != null)
        {
            UnturnedChat.Say(user_to_message, $"Warp ({warp_name}) has been deleted.");
        }
    }

    public void WarpUser(UnturnedPlayer player, string warp_name)
    {
        if (!GetStorage<WarpsStorage>(out var warps_storage))
        {
            Logger.LogError("Could not gather storage [WarpsStorage]");
            return;
        }

        if (!warps_storage.ContainsWarp(warp_name))
        {
            Logger.LogWarning($"Warp ({warp_name}) does not exist");
            return;
        }

        var warp = warps_storage.GetWarp(warp_name);
        player.Teleport(new Vector3(warp.LocationX, warp.LocationY, warp.LocationZ), warp.Rotation);
        UnturnedChat.Say(player, "Warped to: " + warp.WarpName);
    }

    public void SendWarps(IRocketPlayer caller)
    {
        if (!GetStorage<WarpsStorage>(out var warps_storage))
        {
            Logger.LogError("Could not gather storage [WarpsStorage]");
            return;
        }

        var warps = warps_storage.StorageItem;
        UnturnedChat.Say(caller, "Warps:");
        UnturnedChat.Say(string.Join(", ", warps.Select(x => x.WarpName).Where(x => caller.HasPermission($"warp.{x}"))));
    }
}