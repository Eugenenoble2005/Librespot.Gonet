namespace Librespot.Gonet;

public delegate void InteractiveAuthenticationRequestedEventHandler(object sender, InteractiveAuthenticationRequestedArgs e);
public class InteractiveAuthenticationRequestedArgs : EventArgs
{
    public string Link { get; }

    public InteractiveAuthenticationRequestedArgs(string link)
    {
        Link = link;
    }
}

public delegate void PlayerActiveEventHandler(object sender, PlayerActiveEvent e);
public delegate void PlayerInactiveEventHandler(object sender, PlayerInactiveEvent e);
public delegate void PlayerMetadataEventHandler(object sender, PlayerMetadataEvent e);
public delegate void PlayerWillPlayEventHandler(object sender, PlayerWillPlayEvent e);
public delegate void PlayerPlayingEventHandler(object sender, PlayerPlayingEvent e);
public delegate void PlayerNotPlayingEventHandler(object sender, PlayerNotPlayingEvent e);
public delegate void PlayerPausedEventHandler(object sender, PlayerPausedEvent e);
public delegate void PlayerStoppedEventHandler(object sender, PlayerStoppedEvent e);

public delegate void PlayerSeekEventHandler(object sender, PlayerSeekEvent e);
public delegate void PlayerVolumeEventHandler(object sender, PlayerVolumeEvent e);
public delegate void PlayerShuffleContextEventHandler(object sender, PlayerShuffleContextEvent e);
public delegate void PlayerRepeatContextEventHandler(object sender, PlayerRepeatContextEvent e);
public delegate void PlayerRepeatTrackEventHandler(object sender, PlayerRepeatTrackEvent e);

