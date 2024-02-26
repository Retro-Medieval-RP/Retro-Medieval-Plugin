using System.Collections.Generic;
using Rocket.API;

namespace RetroMedieval.Commands.Moderation;

internal class KickCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "kick";
    public string Help => "Kicks a user from the server.";
    public string Syntax => "kick <player name | player id> <reason>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}