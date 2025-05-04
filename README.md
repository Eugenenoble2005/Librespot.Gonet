## Librespot.Gonet
Very simple event driven c# wrapper around the go-librespot api (https://github.com/devgianlu/go-librespot)

# Building
First clone recursively , install go and dotnet and build librespot-go
```
git clone https://github.com/Eugenenoble2005/Librespot.Gonet --recursive
cd Librespot.Gonet/go-librespot && go build ./cmd/daemon
cd .. && cp go-librespot/daemon Librespot.Gonet
```

# Usage
You can create a new spotify device easily now from c#
```
using Librespot.Gonet;
var config = new GonetConfig()
{
    AudioBackend = AudioBackend.Pulseaudio,
    Credentials = new()
    {
        Type = CredentialType.Interactive, //for interactive authentication
    
    },
};
Player player = new(config);
player.InteractiveAuthenticationRequested += (_, e) =>
{
    Console.WriteLine($"Librespot go requires authentication, please visit {e.Link} to authenticate");
};
await player.StartAsync();
```

You can also choose to simple create a simple device that will be available on the network to other spotify clients by setting credential type to zeroconf or omitting it entirely
```
var config = new GonetConfig()
{
    DeviceName = "MyDevice",
    AudioBackend = AudioBackend.Pulseaudio,
    DeviceType = DeviceType.Tablet,
};
```
See [GonetConfig.cs](https://github.com/Eugenenoble2005/Librespot.Gonet/blob/main/Librespot.Gonet/GonetConfig.cs) and [configschema.json](https://github.com/devgianlu/go-librespot/blob/master/config_schema.json) for more config options.

# Events
While the player is active, several events are emitted from librespot that you can subscribe to:
```
player.PlayerMetadata += (_, e) =>
{
    Console.WriteLine($"Playing {e.Data.Name} from {e.Data.ArtistNames[0]}");
};

//Paused
player.PlayerPaused += (_, e) =>
{
    Console.WriteLine("Player was paused");
};

//Player is active
player.PlayerActive += (_, e) =>
{
    Console.WriteLine("Player has become active");
};

player.PlayerVolume += (_, e) =>
{
    Console.WriteLine("Player has been paused");
};

player.DaemonExited += (_, e) =>
{
    Console.WriteLine("Librespot native daemon has been exited");
};


await player.StartAsync();

```

# Controlling
You can also control librespot at runtime.
```
_ = player.StartAsync();

await player.SetPauseAsync(true); // pause
await player.SetPauseAsync(false); //play
await player.TogglePlayPauseAsync(); //toggle play pause

await player.SeekAsync(new() { Position = 70000 }); //seek to 1:10

await player.PlayAsync(new() { Uri = " spotify:track:3n3Ppam7vgaVa1iaRUc9Lp" });
await player.PlayNextAsync(new()); //play next in queue
await player.PlayPrevAsync();
await player.AddToQueueAsync("spotify:track:3n3Ppam7vgaVa1iaRUc9Lp"); //add to queue
```
See the http api for more commands: [api-spec.yml](https://github.com/devgianlu/go-librespot/blob/master/api-spec.yml)






