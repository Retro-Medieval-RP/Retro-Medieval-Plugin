using Rocket.Unturned.Player;

namespace RetroMedieval.Events.Unturned;

public class PlayerJoinEventEventArgs
{
    public UnturnedPlayer Player { get; set; }
}

public static class PlayerJoinEventEventPublisher
{
    public delegate void PlayerJoinEventEventHandler(PlayerJoinEventEventArgs e, ref bool allow);

    public static event PlayerJoinEventEventHandler PlayerJoinEventEvent;

    internal static void RaiseEvent(UnturnedPlayer player, ref bool allow) =>
        PlayerJoinEventEvent?.Invoke(new PlayerJoinEventEventArgs() { Player = player }, ref allow);
}