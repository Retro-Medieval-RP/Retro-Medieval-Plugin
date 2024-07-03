using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.CloathingDequip;

public class HatDequippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort DequippedClothingID { get; set; }
}

public static class HatDequippedEventPublisher
{
    public delegate void HatDequippedEventHandler(HatDequippedEventArgs e);

    public static event HatDequippedEventHandler HatDequippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort id) =>
        HatDequippedEvent?.Invoke(new HatDequippedEventArgs()
        {
            Player = player,
            DequippedClothingID = id
        });
}