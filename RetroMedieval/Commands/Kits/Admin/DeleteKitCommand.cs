using System.Collections.Generic;
using Rocket.API;

namespace RetroMedieval.Commands.Kits.Admin;

internal class DeleteKitCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both; 
    public string Name => "deletekit";
    public string Help => "Deletes a kit from the server.";
    public string Syntax => "deletekit <kit name>";
    public List<string> Aliases => [ "dkit" ];
    public List<string> Permissions => [];
}