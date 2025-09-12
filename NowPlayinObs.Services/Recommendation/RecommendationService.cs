using Microsoft.Extensions.Logging;
using NowPlayinObs.Domain;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace NowPlayinObs.Services;

public class RecommendationService(
    ILogger<RecommendationService> logger,
    HttpClient httpClient
    ) : IRecommendationService
{
    private readonly ILogger<RecommendationService> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;
    private readonly List<Recommendation> _recommendations = [];

    public async Task<IEnumerable<Recommendation>> GetRecommendations()
    {
        if (_recommendations.Count <= 0)
            await InitializeAsync();
        
        return _recommendations;
    }

    protected async Task InitializeAsync()
    {
        try
        {
            var recommendations = await _httpClient.GetFromJsonAsync<Recommendation[]>("/Recommendation/index.json")!;

            _recommendations.Clear();
            if(recommendations is not null ) 
                _recommendations.AddRange(recommendations);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to initialize recommendations {ex}", ex.ToString());
        }
    }
}
