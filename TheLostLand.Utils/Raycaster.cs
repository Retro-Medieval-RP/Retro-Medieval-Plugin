using Rocket.Unturned.Player;
using UnityEngine;

namespace TheLostLand.Utils;

public static class Raycaster
{
    public static RaycastResult RayCastPlayer(UnturnedPlayer player, int raycasts, int max_distance = 100) => 
        Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out var ray, max_distance, raycasts) ? new RaycastResult(ray, true) : new RaycastResult(ray, false);
}