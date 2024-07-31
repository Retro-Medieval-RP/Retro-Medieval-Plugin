using System.Collections.Generic;
using System.Linq;
using PlayerStatus.Models;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace PlayerStatus
{
    [ModuleInformation("PlayerStatus")]
    [ModuleConfiguration<StatusConfiguration>("PlayerStatusConfiguration")]
    public class PlayerStatusModule(string directory) : Module(directory)
    {
        private Dictionary<CSteamID, ImageChangeRange> LastHealthRange { get; } = [];
        private Dictionary<CSteamID, ImageChangeRange> LastStaminaRange { get; } = [];
        private Dictionary<CSteamID, ImageChangeRange> LastHungerRange { get; } = [];
        private Dictionary<CSteamID, ImageChangeRange> LastWaterRange { get; } = [];
        
        public override void Load()
        {
            U.Events.OnPlayerConnected += OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerUpdateHealth += OnHealthChange;
            UnturnedPlayerEvents.OnPlayerUpdateStamina += OnStaminaChange;
            UnturnedPlayerEvents.OnPlayerUpdateFood += OnFoodChange;
            UnturnedPlayerEvents.OnPlayerUpdateWater += OnWaterChange;
        }

        public override void Unload()
        {
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerUpdateHealth -= OnHealthChange;
            UnturnedPlayerEvents.OnPlayerUpdateStamina -= OnStaminaChange;
            UnturnedPlayerEvents.OnPlayerUpdateFood -= OnFoodChange;
            UnturnedPlayerEvents.OnPlayerUpdateWater -= OnWaterChange;
        }
        
        private void OnHealthChange(UnturnedPlayer player, byte health) => 
            UpdateHealth(player);

        private void OnStaminaChange(UnturnedPlayer player, byte stamina) => 
            UpdateStamina(player);

        private void OnFoodChange(UnturnedPlayer player, byte food) =>
            UpdateHunger(player);

        private void OnWaterChange(UnturnedPlayer player, byte water) =>
            UpdateWater(player);

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            if (!GetConfiguration<StatusConfiguration>(out var config))
            {
                return;
            }
            
            player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowLifeMeters);
            player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons);
            
            EffectManager.sendUIEffect(config.UIID, 5567, player.Player.channel.GetOwnerTransportConnection(), false);
            UpdateHealth(player);
            UpdateStamina(player);
            UpdateHunger(player);
            UpdateWater(player);
        }

        private void UpdateHealth(UnturnedPlayer player)
        {
            if (!GetConfiguration<StatusConfiguration>(out var config))
            {
                return;
            }
            
            var range = config.Health.Ranges
                .Where(x => player.Player.life.health <= x.MaxValue)
                .First(x => player.Player.life.health >= x.MinValue);

            if (LastHealthRange.ContainsKey(player.CSteamID))
            {
                if (LastHealthRange[player.CSteamID] == range)
                {
                    return;
                }
                
                LastHealthRange[player.CSteamID] = range;
            }
            else
            {
                LastHealthRange.Add(player.CSteamID, range);
            }
            
            EffectManager.sendUIEffectImageURL(5567, player.Player.channel.GetOwnerTransportConnection(), false, config.Health.ChildName, range.ImageURL, true, true);
        }

        private void UpdateStamina(UnturnedPlayer player)
        {
            if (!GetConfiguration<StatusConfiguration>(out var config))
            {
                return;
            }
            
            var range = config.Stamina.Ranges
                .Where(x => player.Player.life.stamina <= x.MaxValue)
                .First(x => player.Player.life.stamina >= x.MinValue);
            
            if (LastStaminaRange.ContainsKey(player.CSteamID))
            {
                if (LastStaminaRange[player.CSteamID] == range)
                {
                    return;
                }
                
                LastStaminaRange[player.CSteamID] = range;
            }
            else
            {
                LastStaminaRange.Add(player.CSteamID, range);
            }
            
            EffectManager.sendUIEffectImageURL(5567, player.Player.channel.GetOwnerTransportConnection(), false, config.Stamina.ChildName, range.ImageURL, true, true);
        }

        private void UpdateHunger(UnturnedPlayer player)
        {
            if (!GetConfiguration<StatusConfiguration>(out var config))
            {
                return;
            }
            
            var range = config.Hunger.Ranges
                .Where(x => player.Player.life.food <= x.MaxValue)
                .First(x => player.Player.life.food >= x.MinValue);

            if (LastHungerRange.ContainsKey(player.CSteamID))
            {
                if (LastHungerRange[player.CSteamID] == range)
                {
                    return;
                }
                
                LastHungerRange[player.CSteamID] = range;
            }
            else
            {
                LastHungerRange.Add(player.CSteamID, range);
            }

            EffectManager.sendUIEffectImageURL(5567, player.Player.channel.GetOwnerTransportConnection(), false, config.Hunger.ChildName, range.ImageURL, true, true);
        }

        private void UpdateWater(UnturnedPlayer player)
        {
            if (!GetConfiguration<StatusConfiguration>(out var config))
            {
                return;
            }
            
            var range = config.Water.Ranges
                .Where(x => player.Player.life.water <= x.MaxValue)
                .First(x => player.Player.life.water >= x.MinValue);
            
            if (LastWaterRange.ContainsKey(player.CSteamID))
            {
                if (LastWaterRange[player.CSteamID] == range)
                {
                    return;
                }
                
                LastWaterRange[player.CSteamID] = range;
            }
            else
            {
                LastWaterRange.Add(player.CSteamID, range);
            }
            
            EffectManager.sendUIEffectImageURL(5567, player.Player.channel.GetOwnerTransportConnection(), false, config.Water.ChildName, range.ImageURL, true, true);
        }
    }
}