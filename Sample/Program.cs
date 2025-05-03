using Librespot.Gonet;
var config = new GonetConfig()
{
    AudioBackend = AudioBackend.Pulseaudio,
    Credentials = new()
    {
        Type = CredentialType.Interactive,
    }
};
Player player = new(
    config,
    Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gonet")
);
Console.WriteLine("Starting Player ...");

player.LibrespotReceievedError += (_, s) =>
{
    Console.WriteLine(s.Data);
};
player.InteractiveAuthenticationRequested += (s, e) =>
{
    Console.WriteLine($"Authentication requested, please visit this link to complete : " + e.Link);
};
await player.StartAsync();
