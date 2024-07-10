using System.Collections.Generic;
using CityZones.Models;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace CityZones.Commands;

internal class AddCityCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<CityZonesModule>(out var module))
        {
            Logger.LogError("Could not get module [CityZonesModule]");
            return;
        }
        
        if (command.Length < 2)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }

        var zoneName = command[0];

        if (module.DoesCityExist(zoneName))
        {
            UnturnedChat.Say(caller, $"City for zone ({zoneName}) already exists.", Color.red);
            return;
        }
        
        var welcomeMessage = command[1];
        var territoryMessage = command.Length < 3 ? "" : command[2];

        var city = new City
        {
            ZoneName = zoneName,
            WelcomeMessage = welcomeMessage,
            TerritoryMessage = territoryMessage
        };
        
        module.AddCity(city);
        UnturnedChat.Say(caller, $"Added new city for zone ({zoneName}) onto map.");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "addcity";
    public string Help => "Adds a city into the storage that will display the UI when entered";
    public string Syntax => "addcity <Zone Name> <Welcome Message> [Territory Message]";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}