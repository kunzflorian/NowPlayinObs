using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NowPlayinObs.Authentification;
using NowPlayinObs.Authentificiation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace NowPlayinObs.Services;

public class AuthentificiationService(
    ILogger<AuthentificiationService> logger,
    IConfiguration configuration,
    [FromKeyedServices(TwitchConfig.HttpClientAuth)]
    HttpClient client,
    TwitchConfig twitchConfig
)
{
    private readonly ILogger<AuthentificiationService> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly HttpClient _client = client;
    private readonly TwitchConfig _twitchConfig = twitchConfig;
    private readonly string _pathToken = "oauth2/token";
    private readonly string _pathDevice = "oauth2/device";

    public async Task<ClientCredentialsToken> GetClientCredentialsTokenAsync()
    {
        var request = new ClientCredentialsRequest()
        {
            ClientId = _twitchConfig.ClientId!,
            ClientSecret = _twitchConfig.ClientSecret!
        };

        var json = JsonSerializer.Serialize(request);
        var uri = $"{_client.BaseAddress}{_pathToken}";
        var jsonString = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

        var response = await _client.PostAsync(uri, jsonString);

        if (response.IsSuccessStatusCode)
        {   
            var token = await response.Content.ReadFromJsonAsync<ClientCredentialsToken>();
            return token!;
        }
        else
        {
            string tmp = await response.Content.ReadAsStringAsync();
            throw new Exception($"Auth failed {tmp}");
        }
    }

    public async Task<AuthCodeToken> GetAuthCodeTokenAsync(string code, string scope)
    {
        var request = new AuthCodeRequest()
        {
            ClientId = _twitchConfig.ClientId!,
            ClientSecret = _twitchConfig.ClientSecret!,
            Scope = scope,

            Code = code,

            RedirectUrl = $"{_configuration!.GetSection("Kestrel:Endpoints:Default:Url").Get<string>()!}/signin-twitch"
        };

        var json = JsonSerializer.Serialize(request);
        var uri = $"{_client.BaseAddress}{_pathToken}";
        var jsonString = new StringContent(json, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

        var response = await _client.PostAsync(uri, jsonString);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadFromJsonAsync<AuthCodeToken>();
            return token!;
        }
        else
        {
            string tmp = await response.Content.ReadAsStringAsync();
            throw new Exception($"Auth failed {tmp}");
        }
    }

    public async Task<DeviceCode> GetDeviceCodeAsync(string scope)
    {
        var uri = $"{_client.BaseAddress}{_pathDevice}?client_id={_twitchConfig.ClientId}&scopes={Uri.EscapeDataString(scope)}";
        var jsonString = new StringContent("{}", Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));

        var response = await _client.PostAsync(uri, jsonString);

        if (response.IsSuccessStatusCode)
        {
            var deviceCode = await response.Content.ReadFromJsonAsync<DeviceCode>();
            return deviceCode!;
        }
        else
        {
            var status = await response.Content.ReadFromJsonAsync<StatusResponse>();
            throw new Exception($"Auth failed {status}");
        }
    }
}
