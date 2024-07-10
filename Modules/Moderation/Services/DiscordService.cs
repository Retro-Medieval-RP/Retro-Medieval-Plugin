using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Moderation.Models;
using Moderation.Models.Discord;
using Newtonsoft.Json;
using RetroMedieval.Modules;
using Rocket.Core.Logging;
using SDG.Unturned;

namespace Moderation.Services;

internal class DiscordService
{
    public void SendMessage(string content, ModerationActionType messageType)
    {
        if (!ModuleLoader.Instance.GetModule<ModerationModule>(out var moderationModule))
        {
            Logger.LogError("Could not find module [ModerationModule]!");
            return;
        }

        if (!moderationModule.GetConfiguration<ModerationConfiguration>(out var config))
        {
            Logger.LogError("Could not find configuration [ModerationConfiguration]!");
            return;
        }
        
        if (!config.UseWebhooks)
        {
            return;
        }

        if (ThreadUtil.gameThread == Thread.CurrentThread)
        {
            Logger.LogWarning("Discord webhook is being sent on the game thread. This will cause lag. Report to developer!");
        }

        var webhook = config.Webhooks.FirstOrDefault(x => x.WebhookType == messageType.ToString());

        if (webhook == null || string.IsNullOrEmpty(webhook.WebhookUrl))
        {
            return;
        }

        try
        {
            using var wc = new WebClient();
            var msg = new DiscordWebhookMessage(new Embed(content, Convert.ToInt32(webhook.WebhookColor.Trim('#'), 16)));
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            wc.UploadString(new Uri(webhook.WebhookUrl), JsonConvert.SerializeObject(msg));
        }
        catch (Exception e)
        {
            Logger.LogError($"Error sending discord webhook for {messageType}: {e.Message}");
        }
    }

    private static string FormatMessage(string value, string[] args, ModerationActionType messageType)
    {
        value = value.Replace("{name}", args[0])
            .Replace("{steamid}", args[1])
            .Replace("{punisher}", args[2])
            .Replace("{servername}", Provider.serverName);

        if (messageType != ModerationActionType.Unban)
        {
            value = value.Replace("{reason}", args[3]);
        }
        if (messageType == ModerationActionType.Ban)
        {
            value = value.Replace("{duration}", args[4]);
        }

        return value;
    }

    public void SendMessage(string[] args, ModerationActionType messageType)
    {
        if (!ModuleLoader.Instance.GetModule<ModerationModule>(out var moderationModule))
        {
            Logger.LogError("Could not find module [ModerationModule]!");
            return;
        }

        if (!moderationModule.GetConfiguration<ModerationConfiguration>(out var config))
        {
            Logger.LogError("Could not find configuration [ModerationConfiguration]!");
            return;
        }
        
        if (!config.UseWebhooks)
        {
            return;
        }

        if (ThreadUtil.gameThread == Thread.CurrentThread)
        {
            Logger.LogWarning("Discord webhook is being sent on the game thread. This will cause lag. Report to developer!");
        }

        var webhook = config.Webhooks.FirstOrDefault(x => x.WebhookType == messageType.ToString());

        if (webhook == null || string.IsNullOrEmpty(webhook.WebhookUrl))
        {
            return;
        }

        var array = webhook.MessageFormat.Split([": ", ", "], StringSplitOptions.RemoveEmptyEntries);
        var num = 0;

        var fields = new List<Field>();
        while (num < array.Length - 1)
        {
            var arr = array.Skip(num).Take(2).ToArray();

            var value = FormatMessage(arr[1], args, messageType);

            fields.Add(new Field(arr[0], value, webhook.Inline));
            num += 2;
        }

        Footer footer = null;
        if (config.ServerFooterEnabled)
        {
            footer = new Footer
            {
                text = Provider.serverName,
                icon_url = Provider.configData.Browser.Icon
            };
        }

        var embed = new Embed(fields, Convert.ToInt32(webhook.WebhookColor.Trim('#'), 16), footer);

        if (!string.IsNullOrEmpty(webhook.Title))
        {
            embed.title = FormatMessage(webhook.Title, args, messageType);
        }

        if (!string.IsNullOrEmpty(webhook.Url))
        {
            embed.url = FormatMessage(webhook.Url, args, messageType);
        }

        var msg = new DiscordWebhookMessage(embed);

        try
        {
            using var wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            wc.UploadString(new Uri(webhook.WebhookUrl), JsonConvert.SerializeObject(msg));
        }
        catch (Exception e)
        {
            Logger.LogError($"Error sending discord webhook for {messageType}: {e.Message}");
        }
    }
}