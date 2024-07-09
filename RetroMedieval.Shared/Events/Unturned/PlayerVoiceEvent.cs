using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned;

public class PlayerVoiceEventArgs
{
    public UnturnedPlayer Sender { get; set; }
    public UnturnedPlayer Listener { get; set; }
}

public static class PlayerVoiceEventPublisher
{
    public delegate void PlayerVoiceEventEventHandler(PlayerVoiceEventArgs e, ref bool allow);

    public static event PlayerVoiceEventEventHandler PlayerVoiceEventEvent;

    public static void RaiseEvent(UnturnedPlayer sender, UnturnedPlayer listener, ref bool allow) =>
        PlayerVoiceEventEvent?.Invoke(new PlayerVoiceEventArgs()
        {
            Sender = sender,
            Listener = listener
        }, ref allow);
}