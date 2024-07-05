using Rocket.Unturned.Player;

namespace DeadBodies.Events;

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