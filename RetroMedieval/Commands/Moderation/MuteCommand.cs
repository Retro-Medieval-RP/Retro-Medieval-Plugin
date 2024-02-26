using System.Collections.Generic;
using Rocket.API;

namespace RetroMedieval.Commands.Moderation;

internal class MuteCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "mute";
    public string Help => "Mutes a user on a server.";
    public string Syntax => "mute <player name | player id> <reason> <time span>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}