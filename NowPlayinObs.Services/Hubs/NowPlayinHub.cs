using Microsoft.AspNetCore.SignalR;
using NowPlayinObs.Domain;
using NowPlayinObs.Services;

namespace NowPlayinObs.Hubs;

public class NowPlayinHub(
    NowPlayinService nowPlayinService
) : Hub
{
    private readonly NowPlayinService _nowPlayinService = nowPlayinService;

    public async Task SetNowPlayin(TrackInfo trackInfo)
    {
        await _nowPlayinService.SetCurrentTrack(trackInfo);
        await Clients.All.SendAsync("ReceiveNowPlayin", trackInfo);
    }

    public async Task GetNowPlayin()
    {
        var currentTrack = await _nowPlayinService.GetCurrentTrack();
        await Clients.Caller.SendAsync("ReceiveNowPlayin", currentTrack);
    }
}