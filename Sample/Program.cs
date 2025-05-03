using System.Text.Json;
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
player.PlayerActive += (_, _) =>
{
    Console.WriteLine("PLAYER JUST BECAME ACTIVE");
};
player.PlayerInactive += (_, _) =>
{
    Console.WriteLine("PLAYER IS INACTIVE");
};
player.PlayerMetadata += (_, e) =>
{
    Console.WriteLine("Metadata recieved: " + JsonSerializer.Serialize(e));
};

player.PlayerWillPlay += (_, e) =>
{
    Console.WriteLine("Player will play:" + JsonSerializer.Serialize(e));
};

player.PlayerPlaying += (_, e) =>
{
    Console.WriteLine("player playing:" + JsonSerializer.Serialize(e));
};

player.PlayerNotPlaying += (_, e) =>
{
    Console.WriteLine("player not playing:" + JsonSerializer.Serialize(e));
};

player.PlayerPaused += (_, e) =>
{
    Console.WriteLine("player was paused" + JsonSerializer.Serialize(e));
};

player.PlayerStopped += (_, e) =>
{
    Console.WriteLine("player stopped" + JsonSerializer.Serialize(e));
};
player.PlayerSeek += (_, e) =>
{
    Console.WriteLine("player seeked" + JsonSerializer.Serialize(e));
};
player.PlayerVolume += (_, e) =>
{
    Console.WriteLine("player VOLUME" + JsonSerializer.Serialize(e));
};
player.PlayerShuffleContext += (_, e) =>
{
    Console.WriteLine("player shuffle context" + JsonSerializer.Serialize(e));
};
player.PlayerRepeatContext += (_, e) =>
{
    Console.WriteLine("player repeat" + JsonSerializer.Serialize(e));
};
player.PlayerRepeatTrack += (_, e) =>
{
    Console.WriteLine("player repeat track" + JsonSerializer.Serialize(e));
};
await player.StartAsync();
