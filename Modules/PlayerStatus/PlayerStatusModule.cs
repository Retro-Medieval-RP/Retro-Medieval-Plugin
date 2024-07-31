﻿using System.Linq;
using RetroMedieval.Modules;
using RetroMedieval.Modules.Attributes;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace PlayerStatus
{
    [ModuleInformation("PlayerStatus")]
    [ModuleConfiguration<StatusConfiguration>("PlayerStatusConfiguration")]
    public class PlayerStatusModule(string directory) : Module(directory)
    {
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
            
            var range = config.Health.Ranges.First(x =>
                x.MaxValue <= player.Player.life.health && player.Player.life.health >= x.MinValue);
            
            EffectManager.sendUIEffectImageURL(5567, player.Player.channel.GetOwnerTransportConnection(), false, config.Health.ChildName, range.ImageURL, true, true);
        }

        private void UpdateStamina(UnturnedPlayer player)
        {
            if (!GetConfiguration<StatusConfiguration>(out var config))
            {
                return;
            }
            
            var range = config.Stamina.Ranges.First(x =>
                x.MaxValue <= player.Player.life.stamina && player.Player.life.stamina >= x.MinValue);
            
            EffectManager.sendUIEffectImageURL(5567, player.Player.channel.GetOwnerTransportConnection(), false, config.Health.ChildName, range.ImageURL, true, true);
        }

        private void UpdateHunger(UnturnedPlayer player)
        {
            if (!GetConfiguration<StatusConfiguration>(out var config))
            {
                return;
            }
            
            var range = config.Hunger.Ranges.First(x =>
                x.MaxValue <= player.Player.life.food && player.Player.life.food >= x.MinValue);
            
            EffectManager.sendUIEffectImageURL(5567, player.Player.channel.GetOwnerTransportConnection(), false, config.Health.ChildName, range.ImageURL, true, true);
        }

        private void UpdateWater(UnturnedPlayer player)
        {
            if (!GetConfiguration<StatusConfiguration>(out var config))
            {
                return;
            }
            
            var range = config.Water.Ranges.First(x =>
                x.MaxValue <= player.Player.life.water && player.Player.life.water >= x.MinValue);
            
            EffectManager.sendUIEffectImageURL(5567, player.Player.channel.GetOwnerTransportConnection(), false, config.Health.ChildName, range.ImageURL, true, true);
        }
    }
}