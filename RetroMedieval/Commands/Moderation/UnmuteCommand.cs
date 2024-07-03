using System.Collections.Generic;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Moderation;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace RetroMedieval.Commands.Moderation;

internal class UnmuteCommand : IRocketCommand
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

        moderationModule.Unmute(caller, command[0]);
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "unmute";
    public string Help => "Unmutes a user on the server.";
    public string Syntax => "unmute <mute id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}