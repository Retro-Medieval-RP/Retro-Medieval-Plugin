using Rocket.Unturned.Player;

namespace RetroMedieval.Shared.Events.Unturned.CloathingDequip;

public class VestDequippedEventArgs
{
    public UnturnedPlayer Player { get; set; }
    public ushort DequippedClothingID { get; set; }
}

public static class VestDequippedEventPublisher
{
    public delegate void VestDequippedEventHandler(VestDequippedEventArgs e);

    public static event VestDequippedEventHandler VestDequippedEvent;

    public static void RaiseEvent(UnturnedPlayer player, ushort id) =>
        VestDequippedEvent?.Invoke(new VestDequippedEventArgs()
        {
            Player = player,
            DequippedClothingID = id
        });
}