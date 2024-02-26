using System.Collections.Generic;
using Rocket.API;

namespace RetroMedieval.Commands.Moderation;

internal class RemoveWarnCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "removewarn";
    public string Help => "Removes a warn from a user on the server.";
    public string Syntax => "removewarn <warn id>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}