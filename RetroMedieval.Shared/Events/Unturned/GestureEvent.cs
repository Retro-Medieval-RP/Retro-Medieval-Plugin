﻿using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Shared.Events.Unturned;

public class GestureEventArgs
{
    public EPlayerGesture Gesture { get; set; }
    public UnturnedPlayer Player { get; set; }
}

public static class GestureEventPublisher
{
    public delegate void GestureEventEventHandler(GestureEventArgs e, ref bool allow);

    public static event GestureEventEventHandler GestureEventEvent;

    public static void RaiseEvent(EPlayerGesture gesture, UnturnedPlayer player, ref bool allow) =>
        GestureEventEvent?.Invoke(new GestureEventArgs
        {
            Gesture = gesture,
            Player = player
        }, ref allow);
}

