using NowPlayinObs.Domain;

namespace NowPlayinObs.Services;

public interface IRecommendationService
{
    //public Task InitializeAsync();
    public Task<IEnumerable<Recommendation>> GetRecommendations();
}
