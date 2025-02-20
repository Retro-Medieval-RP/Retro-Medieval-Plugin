using HarmonyLib;
using RetroMedieval.Shared.Events.Unturned;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace RetroMedieval.Patches;

[HarmonyPatch(typeof(ChatManager))]
[HarmonyPatch("receiveChatMessage")]
internal class PlayerChatPatch
{
    public static bool Prefix(CSteamID speakerSteamID, string iconURL, EChatMode mode, Color color, bool isRich, string text, ChatManager __instance)
    {
        try
        {
            var allow = true;
            var args = new ChatEventArgs
            {
                Sender = speakerSteamID,
                IconURL = iconURL,
                ChatMode = mode,
                Color = color,
                IsRich = isRich,
                Text = text
            };

            ChatEventPublisher.RaiseEvent(args, ref allow);
            return allow;
        }
        catch
        {
            return true;
        }
    }
}