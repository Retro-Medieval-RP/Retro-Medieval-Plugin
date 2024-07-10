using System.Collections.Generic;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace CityZones.Commands;

internal class RemoveCityCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<CityZonesModule>(out var module))
        {
            Logger.LogError("Could not get module [CityZonesModule]");
            return;
        }
        
        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }

        var zoneName = command[0];

        if (!module.DoesCityExist(zoneName))
        {
            UnturnedChat.Say(caller, $"A city does not exist on the zone: {zoneName}", Color.red);
            return;
        }

        module.RemoveCity(zoneName);
        UnturnedChat.Say(caller, $"Removed city for zone: {zoneName}");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "removecity";
    public string Help => "Removes a city from a zone";
    public string Syntax => "removecity <Zone Name>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}