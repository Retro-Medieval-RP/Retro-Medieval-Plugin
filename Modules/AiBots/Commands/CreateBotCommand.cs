using System.Collections.Generic;
using AiBots.Bot;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace AiBots.Commands;

internal class CreateBotCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }
        
        var unturnedPlayer = caller as UnturnedPlayer;
        var data = new BotData
        {
            Id = ulong.Parse(command[0]),
            Position = unturnedPlayer!.Position
        };

        if (!ModuleLoader.Instance.GetModule<AiBotsModule>(out var module))
        {
            Logger.LogError("Could not get module [AiBotsModule]");
            return;
        }

        if (!module.GetStorage<BotsStorage>(out var storage))
        {
            Logger.LogError("Could not get storage [BotsStorage]");
            return;
        }
        
        storage.AddBotData(data);
        
        UnturnedChat.Say(unturnedPlayer, "Done!");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "createbot";
    public string Help => "Creates a bot";
    public string Syntax => "createbot <id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}