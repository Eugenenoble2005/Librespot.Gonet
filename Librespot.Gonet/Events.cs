namespace Librespot.Gonet;

public delegate void InteractiveAuthenticationRequestedEventHandler (object sender, InteractiveAuthenticationRequestedArgs e);
public class InteractiveAuthenticationRequestedArgs : EventArgs
{
    public string Link { get; }

    public InteractiveAuthenticationRequestedArgs(string link)
    {
        Link = link;
    }
}
