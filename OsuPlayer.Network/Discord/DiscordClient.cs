using System.Diagnostics;
using System.Reflection;
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
                LargeImageKey = "logo",
                LargeImageText = "osu!player"
            }
        });

        _client.Invoke();

        return this;
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
    /// <param name="assets">Assets to use</param>
    public void UpdatePresence(string details, string state, Assets? assets = null)
    {
        _client.SetPresence(new RichPresence
        {
            Details = details,
            State = state,
            Assets = assets ?? _defaultAssets,
            Timestamps = Timestamps.Now,
            Buttons = GetButtons()
        });

        _client.Invoke();
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