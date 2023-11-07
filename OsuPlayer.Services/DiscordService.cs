using System.Diagnostics;
using System.Reflection;
using System.Text;
using DiscordRPC;
using DiscordRPC.Message;
using Nein.Extensions;
using OsuPlayer.Interfaces.Service;
using Splat;
using ConsoleLogger = DiscordRPC.Logging.ConsoleLogger;
using LogLevel = DiscordRPC.Logging.LogLevel;

namespace OsuPlayer.Services;

public class DiscordService : OsuPlayerService, IDiscordService
{
    public override string ServiceName => "DISCORD_SERVICE";

    private const string ApplicationId = "506435812397940736";
    private const string DefaultImageKey = "logo";
    private readonly DiscordRpcClient _client;
    private readonly IProfileManagerService _profileManager;
    private readonly string _defaultOsuThumbnailUrl = "https://assets.ppy.sh/beatmaps/{0}/covers/list.jpg";
    private string _lastOsuThumbnailUrl = string.Empty;

    /// <summary>
    /// Default assets for the RPC including the logo
    /// </summary>
    private readonly Assets _defaultAssets;

    public DiscordService() : this(Locator.Current.GetRequiredService<IProfileManagerService>())
    {
    }

    public DiscordService(IProfileManagerService profileManager)
    {
        _profileManager = profileManager;
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
    public void Initialize()
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
    }

    ~DiscordService()
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

        if (url != _lastOsuThumbnailUrl)
        {
            // Discord can't accept URLs bigger than 256 bytes and throws an exception, so we check for that here
            if (Encoding.UTF8.GetByteCount(url) > 256)
            {
                return null;
            }

            LogToConsole($"Request => {url}");

            HttpResponseMessage response;

            try
            {
                using var client = new HttpClient();

                var req = new HttpRequestMessage(HttpMethod.Get, url);

                response = await client.SendAsync(req);
            }
            catch (Exception)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
                return null;

            _lastOsuThumbnailUrl = url;
        }

        return new()
        {
            LargeImageKey = url,
            LargeImageText = $"osu!player v{Assembly.GetEntryAssembly().ToVersionString()}"
        };
    }

    private Button[]? GetButtons()
    {
        return new Button[]
        {
            new()
            {
                Label = "osu!player GitHub",
                Url = "https://github.com/osu-player/osuplayer"
            }
        };
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