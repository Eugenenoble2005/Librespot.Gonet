namespace Librespot.Gonet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

public class Player(GonetConfig Config, string? ConfigPath = null)
{
    private string DaemonPath = Path.Join(AppContext.BaseDirectory, "daemon");
    private Process _daemonProcess = new();
    public int? _apiPort = null;
    private ClientWebSocket _ws = new();
    private WebsocketHandler _websocketHandler => new(this);

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


