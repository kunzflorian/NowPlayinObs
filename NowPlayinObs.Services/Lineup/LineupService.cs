using Microsoft.Extensions.Logging;
using NowPlayinObs.Domain;
using System.Net.Http.Json;

namespace NowPlayinObs.Services;

public class LineupService (
    ILogger<LineupService> logger,
    HttpClient httpClient
    ) : ILineupService
{
    private readonly ILogger<LineupService> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;
    private readonly List<Slot> _slots = [];

    public async Task<IEnumerable<Slot>> GetSlotsAsync()
    {
        if (_slots.Count <= 0)
            await InitializeAsync();

        return _slots;
    }

    protected async Task InitializeAsync()
    {
        try
        {
            var slots = await _httpClient.GetFromJsonAsync<Slot[]>("/Lineup/index.json")!;

            _slots.Clear();
            if (slots is not null)
                _slots.AddRange(slots);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to initialize lineup {ex}", ex.ToString());
        }
    }
}
