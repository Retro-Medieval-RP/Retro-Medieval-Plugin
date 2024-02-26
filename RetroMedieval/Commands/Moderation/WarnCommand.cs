using System.Collections.Generic;
using Rocket.API;

namespace RetroMedieval.Commands.Moderation;

internal class WarnCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "warn";
    public string Help => "Warns a user on the server.";
    public string Syntax => "warn <player name | player id> <reason>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}