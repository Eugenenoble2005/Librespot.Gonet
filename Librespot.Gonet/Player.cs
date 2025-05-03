namespace Librespot.Gonet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Diagnostics;

public class Player(GonetConfig Config, string? ConfigPath = null)
{
    private string DaemonPath = Path.Join(AppContext.BaseDirectory, "daemon");
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
        var process = new Process { StartInfo = startinfo };
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.OutputDataReceived += (s, e) => LibrespotReceievedData?.Invoke(s, e);
        process.ErrorDataReceived += (s, e) =>
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
        };
        await process.WaitForExitAsync();
    }

    public event DataReceivedEventHandler? LibrespotReceievedData;
    public event DataReceivedEventHandler? LibrespotReceievedError;
    public event InteractiveAuthenticationRequestedEventHandler? InteractiveAuthenticationRequested;
}


