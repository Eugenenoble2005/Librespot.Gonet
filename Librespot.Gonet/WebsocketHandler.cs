namespace Librespot.Gonet;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class WebsocketHandler(Player player)
{
    public void Handle(string message)
    {
        using JsonDocument doc = JsonDocument.Parse(message);
        string? type = doc.RootElement.GetProperty("type").GetString();
        if (type is null) return;
        switch (type)
        {
            case "active":
                player.RaisePlayerActiveEvent(JsonSerializer.Deserialize<PlayerActiveEvent>(message));
                break;
            case "inactive":
                player.RaisePlayerInactiveEvent(JsonSerializer.Deserialize<PlayerInactiveEvent>(message));
                break;
            case "metadata":
                player.RaisePlayerMetadataEvent(JsonSerializer.Deserialize<PlayerMetadataEvent>(message));
                break;
            case "will_play":
                player.RaisePlayerWillPlayEvent(JsonSerializer.Deserialize<PlayerWillPlayEvent>(message));
                break;
            case "playing":
                player.RaisePlayerPlayingEvent(JsonSerializer.Deserialize<PlayerPlayingEvent>(message));
                break;
            case "not_playing":
                player.RaisePlayerNotPlayingEvent(JsonSerializer.Deserialize<PlayerNotPlayingEvent>(message));
                break;
            case "paused":
                player.RaisePlayerPausedEvent(JsonSerializer.Deserialize<PlayerPausedEvent>(message));
                break;
            case "stopped":
                player.RaisePlayerStoppedEvent(JsonSerializer.Deserialize<PlayerStoppedEvent>(message));
                break;
            case "seek":
                player.RaisePlayerSeekEvent(JsonSerializer.Deserialize<PlayerSeekEvent>(message));
                break;
            case "volume":
                player.RaisePlayerVolumeEvent(JsonSerializer.Deserialize<PlayerVolumeEvent>(message));
                break;
            case "shuffle_context":
                player.RaisePlayerShuffleContextEvent(JsonSerializer.Deserialize<PlayerShuffleContextEvent>(message));
                break;
            case "repeat_context":
                player.RaisePlayerRepeatContextEvent(JsonSerializer.Deserialize<PlayerRepeatContextEvent>(message));
                break;
            case "repeat_track":
                player.RaisePlayerRepeatTrackEvent(JsonSerializer.Deserialize<PlayerRepeatTrackEvent>(message));
                break;
        }
    }
}

public class PlayerActiveEvent
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("data")]
    public object? Data { get; set; }
}

public class PlayerInactiveEvent : PlayerActiveEvent { };

public class PlayerMetadataEventData
{
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("artist_names")]
    public List<string>? ArtistNames { get; set; }

    [JsonPropertyName("album_name")]
    public string? AlbumName { get; set; }

    [JsonPropertyName("album_cover_url")]
    public string? AlbumCoverUrl { get; set; }

    [JsonPropertyName("position")]
    public int? Position { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("track_number")]
    public int? TrackNumber { get; set; }

    [JsonPropertyName("disc_number")]
    public int? DiscNumber { get; set; }
}

public class PlayerMetadataEvent
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("data")]
    public PlayerMetadataEventData? Data { get; set; }
}

public class PlayerWillPlayEventData
{
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("play_origin")]
    public string? PlayOrigin { get; set; }
}

public class PlayerWillPlayEvent
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("data")]
    public PlayerWillPlayEventData? Data { get; set; }
}
public class PlayerSeekEventData
{
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("position")]
    public int? Position { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("play_origin")]
    public string? PlayOrigin { get; set; }
}

public class PlayerSeekEvent
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("data")]
    public PlayerSeekEventData? Data { get; set; }
}

public class PlayerPlayingEvent : PlayerWillPlayEvent;
public class PlayerNotPlayingEvent : PlayerWillPlayEvent;
public class PlayerPausedEvent : PlayerWillPlayEvent;
public class PlayerStoppedEvent : PlayerWillPlayEvent;

public class PlayerVolumeEventData
{
    [JsonPropertyName("value")]
    public int? Value { get; set; }

    [JsonPropertyName("max")]
    public int? Max { get; set; }
}

public class PlayerVolumeEvent
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("data")]
    public PlayerVolumeEventData? Data { get; set; }
}


public class PlayerShuffleContextEventData
{
    [JsonPropertyName("value")]
    public bool Value { get; set; }
}

public class PlayerShuffleContextEvent
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("data")]
    public PlayerShuffleContextEventData? Data { get; set; }
}

public class PlayerRepeatContextEvent : PlayerShuffleContextEvent;
public class PlayerRepeatTrackEvent : PlayerShuffleContextEvent;


