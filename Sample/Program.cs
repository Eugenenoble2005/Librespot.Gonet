using Librespot.Gonet;
var config = new GonetConfig()
{
    AudioBackend = AudioBackend.Pulseaudio,
    Credentials = new()
    {
        Type = CredentialType.Interactive,
    },
};
Player player = new(
    config,
    Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gonet")
);
await player.StartAsync();


