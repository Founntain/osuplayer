﻿using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using Nein.Extensions;
using OsuPlayer.Network.Online;

namespace OsuPlayer.Network.Discord;

public class DiscordClient
{
    private const string ApplicationId = "506435812397940736";
    private readonly DiscordRpcClient _client;
    private const string DefaultImageKey = "logo";
    private readonly string _defaultOsuThumbnailUrl = "https://assets.ppy.sh/beatmaps/{0}/covers/list.jpg";

    /// <summary>
    /// Default assets for the RPC including the logo
    /// </summary>
    private readonly Assets _defaultAssets;

    public DiscordClient()
    {
        _client = new DiscordRpcClient(ApplicationId);

        _defaultAssets = new Assets
        {
            LargeImageKey = "logo",
            LargeImageText = $"osu!player v{Assembly.GetEntryAssembly().ToVersionString()}"
        };
    }

    /// <summary>
    /// Initializes the Discord Client and prepares all events
    /// </summary>
    /// <returns>itself</returns>
    public DiscordClient? Initialize()
    {
        _client.Logger = new ConsoleLogger
        {
            Level = LogLevel.Warning
        };

        _client.OnReady += Client_OnReady;
        _client.OnPresenceUpdate += Client_OnPresenceUpdate;

        _client.Initialize();

        _client.SetPresence(new RichPresence
        {
            Details = "osu!player",
            State = "doing nothing...",
            Assets = new Assets
            {
                LargeImageKey = DefaultImageKey,
                LargeImageText = "osu!player"
            }
        });

        return this;
    }

    ~DiscordClient()
    {
        DeInitialize();
    }

    /// <summary>
    /// Needs to be called to dispose the client properly.
    /// </summary>
    public void DeInitialize()
    {
        _client.Dispose();
    }

    /// <summary>
    /// Update the current RPC
    /// </summary>
    /// <param name="details">Text of the first line</param>
    /// <param name="state">Text of the second line</param>
    /// <param name="beatmapSetId">Optional beatmapset ID</param>
    /// <param name="assets">Optional assets to use</param>
    /// <param name="durationLeft">Optional duration left that is displayed in the RPC</param>
    public async Task UpdatePresence(string details, string state, int beatmapSetId = 0, Assets? assets = null, TimeSpan? durationLeft = null)
    {
        if (assets == null && beatmapSetId != 0)
        {
            assets = await TryToGetThumbnail(beatmapSetId);
        }

        var timestamps = durationLeft == null ? null : Timestamps.FromTimeSpan(durationLeft.Value);
        
        _client.SetPresence(new RichPresence
        {
            Details = details,
            State = state,
            Assets = assets ?? _defaultAssets,
            Buttons = GetButtons(),
            Timestamps = timestamps
        });
    }

    private async Task<Assets?> TryToGetThumbnail(int beatmapSetId)
    {
        var url = string.Format(_defaultOsuThumbnailUrl, beatmapSetId);

        // Discord can't accept URLs bigger than 256 bytes and throws an exception, so we check for that here
        if (Encoding.UTF8.GetByteCount(url) > 256)
        {
            return null;
        }

        var osuApi = new WebRequestBase(url);

        var thumbnailResponse = await osuApi.GetRequestWithResponseObj<object>(string.Empty);

        if (thumbnailResponse.StatusCode != HttpStatusCode.OK)
            return null;

        return new()
            {
                LargeImageKey = url,
                LargeImageText = $"osu!player v{Assembly.GetEntryAssembly().ToVersionString()}"
            };
    }

    private Button[]? GetButtons()
    {
        var buttons = new List<Button>
        {
            new()
            {
                Label = "osu!player GitHub",
                Url = "https://github.com/osu-player/osuplayer"
            }
        };

        if (ProfileManager.User is null || string.IsNullOrWhiteSpace(ProfileManager.User.Name)) return buttons.ToArray();

        buttons.Add(new Button
            {
                Label = $"{ProfileManager.User.Name}'s profile",
                Url = $"https://stats.founntain.dev/user/{Uri.EscapeDataString(ProfileManager.User.Name)}"
            }
        );

        return buttons.ToArray();
    }

    private void Client_OnReady(object sender, ReadyMessage args)
    {
        Debug.WriteLine("Discord client ready...");
    }

    private void Client_OnPresenceUpdate(object sender, PresenceMessage args)
    {
        Debug.WriteLine("Discord Presence updated...");
    }
}