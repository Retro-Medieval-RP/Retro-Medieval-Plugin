using System.Collections.Generic;
using RetroMedieval.Utils;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Moderation.Commands.Utils;

internal class WreckVehicleCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        var player = caller as UnturnedPlayer;
        var result = Raycaster.RayCastPlayer(player, RayMasks.VEHICLE);
        if (!result.RaycastHit)
        {
            return;
        }

        if (result.Vehicle == null)
        {
            UnturnedChat.Say(caller, "Could not find vehicle from raycast.", Color.red);
            return;
        }
        
        VehicleManager.askVehicleDestroy(result.Vehicle);
        UnturnedChat.Say(caller, $"Successfully destroyed vehicle: {result.Vehicle.asset.FriendlyName}");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "wv";
    public string Help => "Removes the vehicle that the user is looking at";
    public string Syntax => "wv";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}