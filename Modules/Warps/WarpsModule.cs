using System.Linq;
using JetBrains.Annotations;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using Warps.Models;
using Logger = Rocket.Core.Logging.Logger;

namespace Warps;

[ModuleInformation("Warps")]
[ModuleStorage<WarpsStorage>("Warps")]
internal class WarpsModule([NotNull] string directory) : Module(directory)
{
    public override void Load()
    {
    }

    public override void Unload()
    {
    }

    public void AddWarp(string warpName, Vector3 location, float rotation, [CanBeNull] UnturnedPlayer userToMessage = null)
    {
        if (!GetStorage<WarpsStorage>(out var warpsStorage))
        {
            Logger.LogError("Could not gather storage [WarpsStorage]");
            return;
        }
        
        var warp = new Warp
        {
            WarpName = warpName,
            LocationX = location.x,
            LocationY = location.y,
            LocationZ = location.z,
            Rotation = rotation
        };

        warpsStorage.AddWarp(warp);
        if (userToMessage != null)
        {
            UnturnedChat.Say(userToMessage, $"Warp ({warpName}) has been created.");
        }
    }

    public void RemoveWarp(string warpName, [CanBeNull] UnturnedPlayer userToMessage = null)
    {
        if (!GetStorage<WarpsStorage>(out var warpsStorage))
        {
            Logger.LogError("Could not gather storage [WarpsStorage]");
            return;
        }
        
        if (!warpsStorage.ContainsWarp(warpName))
        {
            Logger.LogWarning($"Warp ({warpName}) does not exist");
            if (userToMessage != null)
            {
                UnturnedChat.Say(userToMessage, $"Warp ({warpName}) does not exist");
            }
            return;
        }

        warpsStorage.RemoveWarp(warpName);
        if (userToMessage != null)
        {
            UnturnedChat.Say(userToMessage, $"Warp ({warpName}) has been deleted.");
        }
    }

    public void WarpUser(UnturnedPlayer player, string warpName)
    {
        if (!GetStorage<WarpsStorage>(out var warpsStorage))
        {
            Logger.LogError("Could not gather storage [WarpsStorage]");
            return;
        }

        if (!warpsStorage.ContainsWarp(warpName))
        {
            Logger.LogWarning($"Warp ({warpName}) does not exist");
            return;
        }

        var warp = warpsStorage.GetWarp(warpName);
        player.Teleport(new Vector3(warp.LocationX, warp.LocationY, warp.LocationZ), warp.Rotation);
        UnturnedChat.Say(player, "Warped to: " + warp.WarpName);
    }

    public void SendWarps(IRocketPlayer caller)
    {
        if (!GetStorage<WarpsStorage>(out var warpsStorage))
        {
            Logger.LogError("Could not gather storage [WarpsStorage]");
            return;
        }

        var warps = warpsStorage.StorageItem;
        UnturnedChat.Say(caller, "Warps:");
        UnturnedChat.Say(string.Join(", ", warps.Select(x => x.WarpName).Where(x => caller.HasPermission($"warp.{x}"))));
    }
}