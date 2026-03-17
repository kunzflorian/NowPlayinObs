using HtmlAgilityPack;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NowPlayinObs.Domain;
using NowPlayinObs.Hubs;
using System;
using System.Text.Json.Nodes;
using System.Web;

namespace NowPlayinObs.Services;

public class NowPlayinWorker(
    ILogger<NowPlayinWorker> logger,
    IConfiguration configuration,
    NowPlayinService nowPlayinService,
    NowPlayinConfig nowPlayinConfig,
    HttpClient httpClient
    ) : BackgroundService
{
    private readonly ILogger<NowPlayinWorker> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly NowPlayinService _nowPlayinService = nowPlayinService;
    private readonly NowPlayinConfig _nowPlayinConfig = nowPlayinConfig;
    private readonly HttpClient _httpClient = httpClient;
    private HubConnection? _hubConnection;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var urlHub = _configuration.GetSection("Kestrel:Endpoints:Default:Url").Get<string>()!;

        _logger.LogInformation("Reading live list from: '{endpoint}'", _nowPlayinConfig.PlaylistUrl);

        _hubConnection = new HubConnectionBuilder()
         .WithUrl($"{urlHub}{NowPlayinHubDefaults.NOWPLAYIN_HUB}")
         .Build();

        await _hubConnection.StartAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var currentTrack = await _nowPlayinService.GetCurrentTrack();
                var nowPlayinTrack = _nowPlayinConfig.Source switch
                {
                    NowPlayinSource.SeratoLive => await GetSeratoInfo(),
                    NowPlayinSource.NowPlayingApp => await GetNowPlayingAppInfo(),
                    _ => null
                };

                if (!currentTrack.Equals(nowPlayinTrack))
                    await _hubConnection.SendAsync("SetNowPlayin", nowPlayinTrack);

                await Task.Delay(_nowPlayinConfig.PollIntervall, stoppingToken);
            }
            catch (Exception ex) 
            {
                await Task.Delay(_nowPlayinConfig.ErrorIntervall, stoppingToken);
                _logger.LogError("Publishing new track failed {ex}", ex.ToString());
            }
        }

        await _hubConnection.StopAsync();
    }

    private async Task<TrackInfo> GetNowPlayingAppInfo()
    {
        var response = await httpClient.GetAsync(_nowPlayinConfig.PlaylistUrl);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");

        var json = await response.Content.ReadAsStringAsync();

        var root = JsonNode.Parse(json)!;
        var currentTrack = root["currentTrack"];       

        var title = (string)currentTrack!["title"]!;
        var artist = (string)currentTrack["artist"]!;
        //var label = (string)currentTrack["label"]!;

        var trackInfo = new TrackInfo()
        {
            Status = NowPlayinHubDefaults.TITLE,
            Title = title,
            Artist = artist,
        };

        return trackInfo;
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
                    trackInfo.Status = NowPlayinHubDefaults.TITLE;
                    trackInfo.Artist = items[0].Trim();
                    trackInfo.Title = items[1].Trim();
                }
            }
        }

        return trackInfo;
    }
}
