using System.Collections.Generic;
using Rocket.API;

namespace RetroMedieval.Commands.Moderation;

internal class UnbanCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "unban";
    public string Help => "Unbans a user from a server.";
    public string Syntax => "unban <player name | player id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}