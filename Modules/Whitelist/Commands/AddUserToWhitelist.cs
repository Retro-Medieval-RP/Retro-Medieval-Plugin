using System.Collections.Generic;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Whitelist.Commands;

internal class AddUserToWhitelist : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (!ModuleLoader.Instance.GetModule<WhitelistModule>(out var whitelistModule))
        {
            Logger.LogError("Could not find module [WhitelistModule]!");
            return;   
        }

        if (command.Length < 1)
        {
            UnturnedChat.Say(caller, "Syntax Error: ", Color.red);
            UnturnedChat.Say(caller, Syntax, Color.red);
            return;
        }
        
        if (!ulong.TryParse(command[0], out var id))
        {
            UnturnedChat.Say(caller, "This User ID is not in the correct format for an ID.", Color.red);
            return;
        }

        if (!whitelistModule.GetConfiguration<WhiteListConfig>(out var config))
        {
            return;
        }
        
        config.WhitelistedUsers.Add(id);
        whitelistModule.SaveConfiguration(config);
        UnturnedChat.Say(caller, $"Added user ({id}) into the whitelist.");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "addwhitelist";
    public string Help => "Adds a user into the servers whitelist";
    public string Syntax => "addwhitelist <User ID>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}