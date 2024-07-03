using HarmonyLib;
using RetroMedieval.Events.Unturned;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace RetroMedieval.Patches;

[HarmonyPatch(typeof(ChatManager))]
[HarmonyPatch("receiveChatMessage")]
internal class PlayerChatPatch
{
    public static bool Prefix(CSteamID speakerSteamID, string iconURL, EChatMode mode, Color color, bool isRich, string text, ChatManager instance)
    {
        var allow = true;
        var args = new ChatEventEventArgs()
        {
            Sender = speakerSteamID,
            IconURL = iconURL,
            ChatMode = mode,
            Color = color,
            IsRich = isRich,
            Text = text
        };

        ChatEventEventPublisher.RaiseEvent(args, ref allow);
        return allow;
    }
}