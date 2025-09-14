using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowPlayinObs;

public class TwitchConfig
{
    public string BaseApiUrl { get; set; } = "https://api.twitch.tv/";
    public string BaseAuthUrl { get; set; } = "https://id.twitch.tv";
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }

    public const string HttpClientAuth = "TwitchHttpClientAuth";
    public const string HttpClientService = "TwitchHttpClientService";
}
