using System.Collections.Generic;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Moderation.Commands;

internal class RemoveWarnCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }

        if (!ModuleLoader.Instance.GetModule<ModerationModule>(out var moderationModule))
        {
            Logger.LogError("Could not find module [ModerationModule]!");
            return;
        }

        moderationModule.RemoveWarn(caller, command[0]);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "removewarn";
    public string Help => "Removes a warn from a user on the server.";
    public string Syntax => "removewarn <warn id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}