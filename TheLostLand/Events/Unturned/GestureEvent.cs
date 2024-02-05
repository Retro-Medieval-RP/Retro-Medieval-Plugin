﻿using Rocket.Unturned.Player;
using SDG.Unturned;

namespace TheLostLand.Events.Unturned;

public class GestureEventEventArgs
{
    public EPlayerGesture Gesture { get; set; }
    public UnturnedPlayer Player { get; set; }
}

public static class GestureEventEventPublisher
{
    public delegate void GestureEventEventHandler(GestureEventEventArgs e, ref bool allow);

    public static event GestureEventEventHandler GestureEventEvent;

    internal static void RaiseEvent(EPlayerGesture gesture, UnturnedPlayer player, ref bool allow) =>
        GestureEventEvent?.Invoke(new GestureEventEventArgs
        {
            Gesture = gesture,
            Player = player
        }, ref allow);
}

