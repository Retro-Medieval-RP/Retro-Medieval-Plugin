using System.Collections.Generic;
using RetroMedieval.Modules;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Whitelist.Commands;

internal class RemoveUserFromWhitelist : IRocketCommand
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

        if (config.WhitelistedUsers.Contains(id))
        {
            config.WhitelistedUsers.Remove(id);
            whitelistModule.SaveConfiguration(config);
            
            UnturnedChat.Say(caller, $"Successfully removed user ({id}) from the whitelist.");
            return;
        }
        
        UnturnedChat.Say(caller, $"Could not find id ({id}) in the whitelist.");
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "removewhitelist";
    public string Help => "Removes a user from the whitelist";
    public string Syntax => "removewhitelist <User ID>";
    public List<string> Aliases => [];
    public List<string> Permissions => [];
}