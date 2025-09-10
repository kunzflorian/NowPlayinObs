namespace NowPlayinObs.Services;

public class NowPlayinConfig
{
    public string PlaylistUrl { get; set; } = string.Empty;
    public TimeSpan ErrorIntervall { get; set; } = new TimeSpan(0, 0, 15);
    public TimeSpan PollIntervall { get; set; } = new TimeSpan(0, 0, 15);
}
