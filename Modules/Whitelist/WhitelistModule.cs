using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using Rocket.Core.Logging;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Whitelist
{
    [ModuleInformation("Whitelist")]
    [ModuleConfiguration<WhiteListConfig>("WhitelistConfig")]
    public class WhitelistModule(string directory) : Module(directory)
    {
        public override void Load()
        {
            U.Events.OnPlayerConnected += OnPlayerConnected;
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            if (!GetConfiguration<WhiteListConfig>(out var config))
            {
                Logger.LogError("Could not gather configuration [WhiteListConfig]");
                return;
            }

            if (!config.WhitelistEnabled)
            {
                return;
            }

            if (config.WhitelistedUsers.Contains(player.CSteamID.m_SteamID))
            {
                return;
            }
            
            Provider.kick(player.CSteamID, string.IsNullOrEmpty(config.DeniedMessage) || string.IsNullOrWhiteSpace(config.DeniedMessage) ? "Server currently has whitelist enabled." : config.DeniedMessage);
        }

        public override void Unload()
        {
            U.Events.OnPlayerConnected -= OnPlayerConnected;
        }
    }
}