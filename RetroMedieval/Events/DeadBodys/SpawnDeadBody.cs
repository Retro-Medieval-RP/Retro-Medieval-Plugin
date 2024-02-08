using Rocket.Unturned.Player;

namespace RetroMedieval.Events.DeadBodys;

public class SpawnDeadBodyEventArgs
{
    public UnturnedPlayer Player { get; set; }
}

public static class SpawnDeadBodyEventPublisher
{
    public delegate void SpawnDeadBodyEventHandler(SpawnDeadBodyEventArgs e);

    public static event SpawnDeadBodyEventHandler SpawnDeadBodyEvent;

    internal static void RaiseEvent(UnturnedPlayer player) =>
        SpawnDeadBodyEvent?.Invoke(new SpawnDeadBodyEventArgs
        {
            Player = player
        });
}