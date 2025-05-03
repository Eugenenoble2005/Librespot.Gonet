using System.Runtime.Serialization;

namespace Librespot.Gonet;

public record GonetConfig
{
    public string DeviceName { get; set; } = "Librespot.Gonet";

    public DeviceType DeviceType { get; set; } = DeviceType.Computer;

    public string? ClientToken { get; set; }

    public AudioBackend AudioBackend { get; set; } = AudioBackend.Alsa;

    public GonetCredentials? Credentials { get; set; }
}

public record GonetCredentials
{
    public CredentialType Type { get; set; } = CredentialType.Zeroconf;
    public CredentialTypeInteractive? Interactive { get; set; }
    public CredentialTypeSpotifyToken? SpotifyToken { get; set; }
    public CredentialTypeZeroconf? Zeroconf { get; set; }
}

public enum CredentialType
{
    [EnumMember(Value = "interactive")]
    Interactive,
    [EnumMember(Value = "spotify_token")]
    SpotifyToken,
    [EnumMember(Value = "zeroconf")]
    Zeroconf
}

public record CredentialTypeInteractive
{
    [EnumMember(Value = "callback_port")]
    public int CallbackPort;
}

public record CredentialTypeSpotifyToken
{
    public string? Username { get; set; }
    public string? AccessToken { get; set; }
}

public record CredentialTypeZeroconf
{
    public bool? PersistCredentials { get; set; }
}
public enum DeviceType
{
    [EnumMember(Value = "computer")]
    Computer,
    [EnumMember(Value = "tablet")]
    Tablet,
    [EnumMember(Value = "smartphone")]
    Smartphone,
    [EnumMember(Value = "speaker")]
    Speaker,
}

public enum AudioBackend
{
    [EnumMember(Value = "alsa")]
    Alsa,
    [EnumMember(Value = "pulseaudio")]
    Pulseaudio,
    [EnumMember(Value = "pipe")]
    Pipewire
}


