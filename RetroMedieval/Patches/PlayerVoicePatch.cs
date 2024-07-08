using HarmonyLib;
using RetroMedieval.Shared.Events.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Patches;

[HarmonyPatch(typeof(PlayerVoice))]
[HarmonyPatch("handleRelayVoiceCulling_Proximity")]
internal class PlayerVoicePatch
{
    public static bool Prefix(PlayerVoice speaker, PlayerVoice listener, PlayerVoice __instance)
    {
        var allow = true;
        PlayerVoiceEventEventPublisher.RaiseEvent(UnturnedPlayer.FromPlayer(speaker.player), UnturnedPlayer.FromPlayer(listener.player), ref allow);
        return allow;
    }
}