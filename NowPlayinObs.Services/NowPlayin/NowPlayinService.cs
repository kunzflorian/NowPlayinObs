using NowPlayinObs.Domain;

namespace NowPlayinObs.Services;

public class NowPlayinService
{
    private TrackInfo _currentTrack = new TrackInfo();

    public delegate void CurrentTrackChangedEvent(TrackInfo trackInfo);

    public CurrentTrackChangedEvent? CurrentTrackChanged { get; set; }

    public async Task<TrackInfo> GetCurrentTrack()
    {
        await Task.CompletedTask;
        return _currentTrack;
    }

    public async Task SetCurrentTrack(TrackInfo trackInfo)
    {
        await Task.CompletedTask;
        _currentTrack = trackInfo;
        CurrentTrackChanged?.Invoke(_currentTrack);
    }
}
