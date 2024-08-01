using System.Collections.Generic;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace AiBots.Commands;

internal class RemoveBotCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }

        if (!ulong.TryParse(command[0], out var id))
        {
            UnturnedChat.Say(caller, "Could not get the id parsed from the args.", Color.red);
            return;
        }

        if (!ModuleLoader.Instance.GetModule<AiBotsModule>(out var module))
        {
            Logger.LogError("Could not find module [AiBotsModule]");
            return;
        }

        if (!module.GetStorage<BotsStorage>(out var storage))
        {
            Logger.LogError("Could not get storage [BotsStorage]");
            return;
        }
        
        storage.RemoveBot(id);
        UnturnedChat.Say(caller, "Removed the bot.");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "removebot";
    public string Help => "Removes a bot from the server.";
    public string Syntax => "removebot <id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}