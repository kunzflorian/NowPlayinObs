using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using NowPlayinObs.Domain;
using NowPlayinObs.Hubs;

namespace NowPlayinObs.Services;

public class RecommendationClientService(
    NavigationManager navigation
) : IRecommendationService
{
    private readonly NavigationManager _navigation = navigation;
    private HubConnection? _hubConnection;

    public async Task<IEnumerable<Recommendation>> GetRecommendationsAsync()
    {
        List<Recommendation> recommendations = [];
   
        _hubConnection ??= new HubConnectionBuilder()
                .WithUrl(_navigation.ToAbsoluteUri(NowPlayinHubDefaults.RECOMMENDATION_HUB))
                .Build();
        await _hubConnection.StartAsync();
        var x = await _hubConnection.InvokeAsync<IEnumerable<Recommendation>>("GetRecommendations");

        recommendations.AddRange(
            x
        );
       
        return recommendations;
    }
}
