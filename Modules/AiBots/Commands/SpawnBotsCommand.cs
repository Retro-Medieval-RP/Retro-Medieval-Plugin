using System.Collections.Generic;
using AiBots.Bot;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Player;
using Steamworks;

namespace AiBots.Commands;

internal class SpawnBotsCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<AiBotsModule>(out var module))
        {
            return;
        }

        if (!module.GetStorage<BotsStorage>(out var storage))
        {
            return;
        }
        
        foreach (var data in storage.StorageItem)
        {
            var bot = ModuleLoader.Instance.ServerGameObject.AddComponent<BotAi>();
            bot.Prepare(new CSteamID(data.Id), data.Position);
            bot.Player.teleportToLocationUnsafe(data.Position, 80f);
            module.ActiveBots.Add(bot);
            UnturnedPlayer.FromPlayer(bot.Player);
            bot.Player.life.serverModifyHealth(100);
            foreach (var num in bot.Layout)
            {
                UnturnedPlayer.FromPlayer(bot.Player).GiveItem(num, 1);
            }
            bot.Hunter = true;
        }
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "spawnbots";
    public string Help => "Spawns all the bots";
    public string Syntax => "spawnbots";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}