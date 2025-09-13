using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using NowPlayinObs.Domain;
using NowPlayinObs.Services;

namespace NowPlayinObs.Hubs;

public class RecommendationHub(
    IRecommendationService recommendationService
) : Hub
{
    private readonly IRecommendationService _recommendationService = recommendationService;

    public async Task<IEnumerable<Recommendation>> GetRecommendations()
    {
        return await _recommendationService.GetRecommendationsAsync();
    }

    public async Task RequestShowItemByIndex(int index)
    {
        await Clients.Others.SendAsync("ReceiveShowItemByIndex", index);
    }
}
