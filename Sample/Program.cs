using Librespot.Gonet;

var config = new GonetConfig()
{
    Credentials = new()
    {
        Type = CredentialType.Zeroconf,
    },
    AudioBackend = AudioBackend.Pipewire,
    DeviceName = "Godhand"
};

Player player = new(config);
var configpath = player.SaveConfig();
