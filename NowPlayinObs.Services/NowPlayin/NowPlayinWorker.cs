using HtmlAgilityPack;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NowPlayinObs.Domain;
using System.Web;

namespace NowPlayinObs.Services;

public class NowPlayinWorker(
    ILogger<NowPlayinWorker> logger,
    IConfiguration configuration,
    NowPlayinService nowPlayinService,
    NowPlayinConfig nowPlayinConfig
    ) : BackgroundService
{
    private readonly ILogger<NowPlayinWorker> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly NowPlayinService _nowPlayinService = nowPlayinService;
    private readonly NowPlayinConfig _nowPlayinConfig = nowPlayinConfig;

    private HubConnection? _hubConnection;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var urlHub = _configuration.GetSection("Kestrel:Endpoints:Default:Url").Get<string>()!;

        _logger.LogInformation("Reading live list from: '{endpoint}'", _nowPlayinConfig.PlaylistUrl);

        _hubConnection = new HubConnectionBuilder()
         .WithUrl($"{urlHub}/nowplayinhub")
         .Build();

        await _hubConnection.StartAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var currentTrack = await _nowPlayinService.GetCurrentTrack();
                var nowPlayinTrack = await GetSeratoInfo();

                if (!currentTrack.Equals(nowPlayinTrack))
                    await _hubConnection.SendAsync("SetNowPlayin", nowPlayinTrack);

                await Task.Delay(_nowPlayinConfig.PollIntervall, stoppingToken);
            }
            catch (Exception ex) 
            {
                await Task.Delay(_nowPlayinConfig.ErrorIntervall, stoppingToken);
                _logger.LogError("test {test}", ex.ToString());
            }
            //await _hubConnection.SendAsync("SendMessage", "SYSTEM", $"The time is: {DateTime.Now}");            
        }
    }

    private async Task<TrackInfo> GetSeratoInfo()
    {
        var trackInfo = new TrackInfo();
        var web = new HtmlWeb();
        var doc = await web.LoadFromWebAsync(_nowPlayinConfig.PlaylistUrl);
        var content = doc.DocumentNode.SelectSingleNode("//div[@id='content']");

        if (content is not null)
        {
            var track = content.SelectSingleNode("//div[@class='playlist-track ']");

            if (track is not null)
            {
                var name = track.SelectSingleNode(".//div[@class='playlist-trackname']");
                var buffer = HttpUtility.HtmlDecode(name.InnerHtml);
                var items = buffer.Split(" - ");

                if(items.Length == 2)
                {
                    trackInfo.Status = "now playin";
                    trackInfo.Artist = items[0].Trim();
                    trackInfo.Title = items[1].Trim();
                }
            }
        }

        return trackInfo;
    }
}
