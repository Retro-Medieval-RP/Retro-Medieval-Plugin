using Rocket.Unturned.Player;
using UnityEngine;

namespace RetroMedieval.Utils;

public static class Raycaster
{
    public static RaycastResult RayCastPlayer(UnturnedPlayer player, int raycasts, int maxDistance = 100) => 
        Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out var ray, maxDistance, raycasts) ? new RaycastResult(ray, true) : new RaycastResult(ray, false);
}