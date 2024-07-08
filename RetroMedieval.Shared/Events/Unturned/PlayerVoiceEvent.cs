using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned;

public class PlayerVoiceEventEventArgs
{
    public UnturnedPlayer Sender { get; set; }
    public UnturnedPlayer Listener { get; set; }
}

public static class PlayerVoiceEventEventPublisher
{
    public delegate void PlayerVoiceEventEventHandler(PlayerVoiceEventEventArgs e, ref bool allow);

    public static event PlayerVoiceEventEventHandler PlayerVoiceEventEvent;

    public static void RaiseEvent(UnturnedPlayer sender, UnturnedPlayer listener, ref bool allow) =>
        PlayerVoiceEventEvent?.Invoke(new PlayerVoiceEventEventArgs()
        {
            Sender = sender,
            Listener = listener
        }, ref allow);
}