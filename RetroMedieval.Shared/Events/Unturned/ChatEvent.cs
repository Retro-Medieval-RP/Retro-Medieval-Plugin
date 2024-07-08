using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace RetroMedieval.Shared.Events.Unturned;

public class ChatEventEventArgs
{
    public CSteamID Sender { get; set; }
    public string IconURL { get; set; }
    public EChatMode ChatMode { get; set; }
    public Color Color { get; set; }
    public bool IsRich { get; set; }
    public string Text { get; set; }
}

public static class ChatEventEventPublisher
{
    public delegate void ChatEventEventHandler(ChatEventEventArgs e, ref bool allow);

    public static event ChatEventEventHandler ChatEventEvent;

    public static void RaiseEvent(ChatEventEventArgs args, ref bool allow) =>
        ChatEventEvent?.Invoke(args, ref allow);
}