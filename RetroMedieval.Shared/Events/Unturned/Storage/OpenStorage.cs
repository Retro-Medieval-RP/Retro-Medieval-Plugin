using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RetroMedieval.Shared.Events.Unturned.Storage
{
    public class OpenStorageEventArgs
    {
        public UnturnedPlayer Player { get; set; }
        public InteractableStorage StorageOpened { get; set; }
    }

    public static class OpenStorageEventPublisher
    {
        public delegate void OpenStorageEventHandler(OpenStorageEventArgs e, ref bool allow);

        public static event OpenStorageEventHandler OpenStorageEvent;

        public static void RaiseEvent(UnturnedPlayer player, InteractableStorage storage, ref bool allow) =>
            OpenStorageEvent?.Invoke(new OpenStorageEventArgs
            {
                Player = player,
                StorageOpened = storage,
            }, ref allow);
    }
}

