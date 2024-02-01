using HarmonyLib;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using TheLostLand.Events.Unturned;

namespace TheLostLand.Patches.Death;

[HarmonyPatch(typeof(PlayerAnimator))]
[HarmonyPatch("tellGesture")]
internal class GesturePatch
{
    /// <summary>
    /// id | Gesture 
    /// 1  | Inventory Start
    /// 2  | Inventory Stop
    /// 3  | Pickup
    /// 4  | Punch Left
    /// 5  | Punch Right
    /// 6  | Surrender Start
    /// 7  | Surrender Stop
    /// 13 | Rest Start
    /// 14 | Rest Stop
    /// 11 | Arrest Start
    /// 12 | Arrest Stop
    /// 8  | Point
    /// 10 | Salute
    /// 15 | Facepalm 
    /// </summary>
    /// <param name="steamID"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool Prefix(CSteamID steam_id, byte id)
    {
        var allow = true;
        var player = UnturnedPlayer.FromCSteamID(steam_id);
        var gesture = EPlayerGesture.NONE;

        var player_current_gesture = player.Player.animator.gesture;
        
        switch (id)
        {
            case 1 when player_current_gesture == EPlayerGesture.NONE:
                gesture = EPlayerGesture.INVENTORY_START;
                break;
            case 2 when player_current_gesture == EPlayerGesture.INVENTORY_START:
            case 3:
            case 4:
            case 5:
                gesture = EPlayerGesture.NONE;
                break;
            case 6 when player_current_gesture == EPlayerGesture.NONE:
                gesture = EPlayerGesture.SURRENDER_START;
                break;
            case 7 when player_current_gesture == EPlayerGesture.SURRENDER_START:
                gesture = EPlayerGesture.NONE;
                break;
            case 13 when player_current_gesture == EPlayerGesture.NONE:
                gesture = EPlayerGesture.REST_START;
                break;
            case 14 when player_current_gesture == EPlayerGesture.REST_START:
                gesture = EPlayerGesture.NONE;
                break;
            case 11:
                gesture = EPlayerGesture.ARREST_START;
                break;
            case 12 when player_current_gesture == EPlayerGesture.ARREST_START:
            case 8 when player_current_gesture == EPlayerGesture.NONE:
            case 9 when player_current_gesture == EPlayerGesture.NONE:
            case 10 when player_current_gesture == EPlayerGesture.NONE:
            case 15 when player_current_gesture == EPlayerGesture.NONE:
                gesture = EPlayerGesture.NONE;
                break;
        }

        GestureEventEventPublisher.RaiseEvent(gesture, player, ref allow);
        return allow;
    }
}