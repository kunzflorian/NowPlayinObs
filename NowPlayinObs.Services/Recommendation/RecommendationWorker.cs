using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NowPlayinObs.Domain;
using NowPlayinObs.Hubs;

namespace NowPlayinObs.Services;

public class RecommendationWorker(
    ILogger<RecommendationWorker> logger,
    IConfiguration configuration,
    IRecommendationService recommendationService,
    NowPlayinConfig nowPlayinConfig
) : BackgroundService
{
    private readonly ILogger<RecommendationWorker> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly IRecommendationService _recommendationService = recommendationService;
    private readonly NowPlayinConfig _nowPlayinConfig = nowPlayinConfig;

    private HubConnection? _hubConnection;
    private int _currentIndex = -1;
    private IEnumerable<Recommendation> _recommendations = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var urlHub = _configuration.GetSection("Kestrel:Endpoints:Default:Url").Get<string>()!;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{urlHub}{NowPlayinHubDefaults.RECOMMENDATION_HUB}")
            .Build();

        await _hubConnection.StartAsync(stoppingToken);

        _recommendations = await _hubConnection.InvokeAsync<IEnumerable<Recommendation>>("GetRecommendations", stoppingToken);

        _currentIndex = _recommendations.Count();        

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _currentIndex--;

                if (_currentIndex < 0)
                    _currentIndex = _recommendations.Count() - 1;

                await _hubConnection.SendAsync("RequestShowItemByIndex", _currentIndex, stoppingToken);

                await Task.Delay(_nowPlayinConfig.RecommendationsIntervall, stoppingToken);
            }
            catch (Exception ex)
            {
                await Task.Delay(_nowPlayinConfig.RecommendationsIntervall, stoppingToken);
                _logger.LogError("RequestShowItemByIndex failed {ex}", ex.ToString());
            }
        }

        await _hubConnection.StopAsync(stoppingToken);
    }
}
