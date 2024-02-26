using System.Collections.Generic;
using Rocket.API;

namespace RetroMedieval.Commands.Moderation;

internal class UnmuteCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "unmute";
    public string Help => "Unmutes a user on the server.";
    public string Syntax => "unmute <player name | player id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}