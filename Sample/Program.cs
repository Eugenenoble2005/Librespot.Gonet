using System.Text.Json;
using Librespot.Gonet;
var config = new GonetConfig()
{
    AudioBackend = AudioBackend.Pulseaudio,
    Credentials = new()
    {
        Type = CredentialType.Interactive,
    },
    Server = new()
    {
        Port = 3000
    }
};
Player player = new(
    config,
    Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gonet")
);

Console.WriteLine("Starting Player ...");

_ = player.StartAsync();

while (true)
{
    ConsoleKeyInfo keyinfo = Console.ReadKey(true);
    switch (keyinfo.Key)
    {
        case ConsoleKey.S:
            Console.WriteLine(JsonSerializer.Serialize(await player.StatusAsync()));
            break;
        case ConsoleKey.P:
            var result = await player.PlayAsync(new()
            {
                Uri = "spotify:track:1w7cgGZR86yWz1pA2puVJD" //heated by Beyonce,
            });
            Console.WriteLine(result);
            break;
        case ConsoleKey.A:
            Console.WriteLine(await player.SetPauseAsync(false));
            break;
        case ConsoleKey.Z:
            Console.WriteLine(await player.SetPauseAsync(true));
            break;
        case ConsoleKey.Spacebar:
            Console.WriteLine(await player.TogglePlayPauseAsync());
            break;
        case ConsoleKey.N:
            Console.WriteLine(await player.PlayNextAsync(new()));
            break;
        case ConsoleKey.B:
            Console.WriteLine(await player.PlayPrevAsync());
            break;
    }
}
