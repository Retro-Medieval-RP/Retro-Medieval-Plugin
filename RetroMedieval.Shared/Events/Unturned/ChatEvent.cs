using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace RetroMedieval.Shared.Events.Unturned;

public class ChatEventArgs
{
    public CSteamID Sender { get; set; }
    public string IconURL { get; set; }
    public EChatMode ChatMode { get; set; }
    public Color Color { get; set; }
    public bool IsRich { get; set; }
    public string Text { get; set; }
}

public static class ChatEventPublisher
{
    public delegate void ChatEventEventHandler(ChatEventArgs e, ref bool allow);

    public static event ChatEventEventHandler ChatEventEvent;

    public static void RaiseEvent(ChatEventArgs args, ref bool allow) =>
        ChatEventEvent?.Invoke(args, ref allow);
}