using SDG.Framework.Utilities;
using SDG.Unturned;
using UnityEngine;

namespace TheLostLand.Utils;

public class BarricadeRaycaster
{
    public static bool RaycastBarricade(Player player, out Transform transform)
    {
        transform = null;
        if (!PhysicsUtility.raycast(new Ray(player.look.aim.position, player.look.aim.forward), out var hit, 3, RayMasks.BARRICADE_INTERACT))
        {
            return false;
        }
        transform = hit.transform;
        return true;
    }
}