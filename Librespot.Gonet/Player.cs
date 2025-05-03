namespace Librespot.Gonet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Player(GonetConfig Config, string? ConfigPath = null)
{
    private string DaemonPath = Path.Join(AppContext.BaseDirectory, "daemon");
    private Process _daemonProcess = new();
    public int? _apiPort = null;
    private ClientWebSocket _ws = new();

    private WebsocketHandler _websocketHandler => new(this);
    private HttpClient _httpClient = new();
    private string _baseHttpEndpoint = "";
    private string SaveConfig()
    {
        var serializer = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();
        var yaml = serializer.Serialize(Config);

        var location = ConfigPath == null ? Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "go-librespot") : ConfigPath;
        Directory.CreateDirectory(location);
        File.WriteAllText(
            Path.Join(location, "config.yml"),
            yaml
        );
        return ConfigPath != null ? ConfigPath : Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "go-librespot");
    }

    public async Task StartAsync()
    {
        var configPath = SaveConfig();
        var startinfo = new ProcessStartInfo()
        {
            FileName = DaemonPath,
            Arguments = $"--config_dir {configPath}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        _daemonProcess = new Process { StartInfo = startinfo };
        _daemonProcess.Start();
        _daemonProcess.BeginOutputReadLine();
        _daemonProcess.BeginErrorReadLine();
        _daemonProcess.OutputDataReceived += (s, e) => LibrespotReceievedData?.Invoke(s, e);
        _daemonProcess.ErrorDataReceived += (s, e) =>
        {
            LibrespotReceievedError?.Invoke(s, e);
            if (e?.Data?.Contains("to complete authentication visit the following link:") == true)
            {
                string? link = e?.Data?.Split(" ").Last();
                if (link != null)
                {
                    InteractiveAuthenticationRequested?.Invoke(s, new(link));
                }
            }
            if (e?.Data?.Contains("api server listening on") == true)
            {
                string? port = e?.Data?.Split(":").Last().TrimEnd('\'').TrimEnd('"');
                if (port != null) _apiPort = int.Parse(port);
                _baseHttpEndpoint = $"http://{Config.Server.Address}:{port}";
                _ = HandleWebSocketEvents();
            }
        };
        await _daemonProcess.WaitForExitAsync();
    }
    private async Task HandleWebSocketEvents()
    {
        try
        {
            _ws = new ClientWebSocket();
            var uri = new Uri($"ws://{Config.Server.Address}:{_apiPort}/events");
            await _ws.ConnectAsync(uri, CancellationToken.None);
            var buffer = new byte[1024];
            while (_ws.State == WebSocketState.Open)
            {
                var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                if (message is not null && message.Trim().Count() > 0)
                {
                    _websocketHandler.Handle(message);
                }
            }
        }
        catch (Exception Ex)
        {
            Console.WriteLine(Ex);
        }
    }

    internal void RaisePlayerActiveEvent(PlayerActiveEvent? e)
    {
        if (e is null) return;
        PlayerActive?.Invoke(this, e);
    }

    internal void RaisePlayerInactiveEvent(PlayerInactiveEvent? e)
    {
        if (e is null) return;
        PlayerInactive?.Invoke(this, e);
    }

    internal void RaisePlayerMetadataEvent(PlayerMetadataEvent? e)
    {
        if (e is null) return;
        PlayerMetadata?.Invoke(this, e);
    }

    internal void RaisePlayerWillPlayEvent(PlayerWillPlayEvent? e)
    {
        if (e is null) return;
        PlayerWillPlay?.Invoke(this, e);
    }

    internal void RaisePlayerPlayingEvent(PlayerPlayingEvent? e)
    {
        if (e is null) return;
        PlayerPlaying?.Invoke(this, e);
    }

    internal void RaisePlayerNotPlayingEvent(PlayerNotPlayingEvent? e)
    {
        if (e is null) return;
        PlayerNotPlaying?.Invoke(this, e);
    }

    internal void RaisePlayerPausedEvent(PlayerPausedEvent? e)
    {
        if (e is null) return;
        PlayerPaused?.Invoke(this, e);
    }

    internal void RaisePlayerStoppedEvent(PlayerStoppedEvent? e)
    {
        if (e is null) return;
        PlayerStopped?.Invoke(this, e);
    }

    internal void RaisePlayerSeekEvent(PlayerSeekEvent? e)
    {
        if (e is null) return;
        PlayerSeek?.Invoke(this, e);
    }

    internal void RaisePlayerVolumeEvent(PlayerVolumeEvent? e)
    {
        if (e is null) return;
        PlayerVolume?.Invoke(this, e);
    }

    internal void RaisePlayerShuffleContextEvent(PlayerShuffleContextEvent? e)
    {
        if (e is null) return;
        PlayerShuffleContext?.Invoke(this, e);
    }

    internal void RaisePlayerRepeatContextEvent(PlayerRepeatContextEvent? e)
    {
        if (e is null) return;
        PlayerRepeatContext?.Invoke(this, e);
    }

    internal void RaisePlayerRepeatTrackEvent(PlayerRepeatTrackEvent? e)
    {
        if (e is null) return;
        PlayerRepeatTrack?.Invoke(this, e);
    }

    public async Task<PlayerStatus?> StatusAsync()
    {
        var request = await (await _httpClient.GetAsync(_baseHttpEndpoint + "/status")).Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlayerStatus>(request);
    }

    public async Task<bool> PlayAsync(PlayCommandArgs args)
    {
        var content = new StringContent(JsonSerializer.Serialize(args), Encoding.UTF8, "application/json");
        var request = await _httpClient.PostAsync(_baseHttpEndpoint + "/player/play", content);
        return request.IsSuccessStatusCode;
    }

    public async Task<bool> SetPauseAsync(bool pause = true)
    {
        var url = pause == true ? _baseHttpEndpoint + "/player/pause" : _baseHttpEndpoint + "/player/resume";
        var request = await _httpClient.PostAsync(url, null);
        return request.IsSuccessStatusCode;
    }

    public async Task<bool> TogglePlayPauseAsync()
    {
        var request = await _httpClient.PostAsync(_baseHttpEndpoint + "/player/playpause", null);
        return request.IsSuccessStatusCode;
    }

    public async Task<bool> PlayNextAsync(PlayNextCommandArgs args)
    {
        var content = new StringContent(JsonSerializer.Serialize(args), Encoding.UTF8, "application/json");
        var request = await _httpClient.PostAsync(_baseHttpEndpoint + "/player/next", content);
        return request.IsSuccessStatusCode;
    }

    public async Task<bool> PlayPrevAsync()
    {
        var request = await _httpClient.PostAsync(_baseHttpEndpoint + "/player/prev", null);
        return request.IsSuccessStatusCode;
    }

    public event DataReceivedEventHandler? LibrespotReceievedData;
    public event DataReceivedEventHandler? LibrespotReceievedError;
    public event InteractiveAuthenticationRequestedEventHandler? InteractiveAuthenticationRequested;

    public event PlayerActiveEventHandler? PlayerActive;
    public event PlayerInactiveEventHandler? PlayerInactive;
    public event PlayerMetadataEventHandler? PlayerMetadata;
    public event PlayerWillPlayEventHandler? PlayerWillPlay;
    public event PlayerPlayingEventHandler? PlayerPlaying;
    public event PlayerNotPlayingEventHandler? PlayerNotPlaying;
    public event PlayerPausedEventHandler? PlayerPaused;
    public event PlayerStoppedEventHandler? PlayerStopped;
    public event PlayerSeekEventHandler? PlayerSeek;
    public event PlayerVolumeEventHandler? PlayerVolume;
    public event PlayerShuffleContextEventHandler? PlayerShuffleContext;
    public event PlayerRepeatContextEventHandler? PlayerRepeatContext;
    public event PlayerRepeatTrackEventHandler? PlayerRepeatTrack;
}

public class PlayerStatus
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    [JsonPropertyName("device_type")]
    public string? DeviceType { get; set; } //TODO: should resolve to enum

    [JsonPropertyName("device_name")]
    public string? DeviceName { get; set; }

    [JsonPropertyName("play_origin")]
    public string? PlayOrigin { get; set; }

    [JsonPropertyName("stopped")]
    public bool Stopped { get; set; }

    [JsonPropertyName("paused")]
    public bool Paused { get; set; }

    [JsonPropertyName("buffering")]
    public bool Buffering { get; set; }

    [JsonPropertyName("volume")]
    public int Volume { get; set; }

    [JsonPropertyName("volume_steps")]
    public int Volume_steps { get; set; }

    [JsonPropertyName("repeat_context")]
    public bool RepeatContext { get; set; }

    [JsonPropertyName("repeat_track")]
    public bool RepeatTrack { get; set; }

    [JsonPropertyName("shuffle_context")]
    public bool ShuffleContext { get; set; }

    [JsonPropertyName("track")]
    public Track? Track { get; set; }
}

public class Track
{
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("artist_names")]
    public List<string>? ArtistNames { get; set; }

    [JsonPropertyName("album_name")]
    public string? AlbumName { get; set; }

    [JsonPropertyName("album_cover_url")]
    public string? AlbumCoverUrl { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("track_number")]
    public int track_number { get; set; }

    [JsonPropertyName("disc_number")]
    public int DiscNumber { get; set; }
}

public record PlayCommandArgs
{
    [JsonPropertyName("uri")]
    public required string Uri { get; set; }

    [JsonPropertyName("skip_to_uri")]
    public string? SkipToUri { get; set; }

    [JsonPropertyName("paused")]
    public bool StartPaused { get; set; } = false;
}

public record PlayNextCommandArgs
{
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }
}
