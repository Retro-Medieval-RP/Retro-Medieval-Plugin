using System.Collections.Generic;
using Rocket.API;

namespace RetroMedieval.Commands.Moderation;

internal class BanCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "ban";
    public string Help => "Bans a user from a server.";
    public string Syntax => "ban <player name | player id> <reason> <time span>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}