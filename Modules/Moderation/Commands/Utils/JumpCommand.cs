using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Moderation.Commands.Utils;

internal class JumpCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        var player = caller as UnturnedPlayer;
        var eyePosition = GetEyePosition(10000, player);

        if (!eyePosition.HasValue)
        {
            UnturnedChat.Say(caller, $"There is nowhere to jump to (10000 meters is the max distance!)", Color.red);
            return;
        }
        var point = new Vector3();
        try
        {
            point = eyePosition.Value;
            point.y += 3;
        }
        catch
        {
            // ignore
        }

        if (player == null)
        {
            return;
        }
        
        var playerPosition = player.Position;
        player.Teleport(point, player.Rotation);

        UnturnedChat.Say(caller, $"You have jumped {(int)Vector3.Distance(playerPosition, point)} meters.");      
    }
    
    private static Vector3? GetEyePosition(float distance, UnturnedPlayer tempPlayer)
    {
        var masks = RayMasks.BLOCK_COLLISION & ~(1 << 0x15);
        var look = tempPlayer.Player.look ?? throw new ArgumentNullException("tempPlayer.Player.look");

        Physics.Raycast(look.aim.position, look.aim.forward, out var raycast, distance, masks);

        if (raycast.transform == null)
            return null;

        return raycast.point;
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "jump";
    public string Help => "This will teleport the user to where they are looking";
    public string Syntax => "jump";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}