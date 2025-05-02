namespace Librespot.Gonet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Player(GonetConfig Config, string? ConfigPath = null)
{
    public string SaveConfig()
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
}

