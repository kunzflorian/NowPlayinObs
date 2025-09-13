using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NowPlayinObs.Services;
using System.Runtime.CompilerServices;

namespace NowPlayinObs.Client;

public static class NowPlayinObsClientExtensions
{
    public static WebAssemblyHostBuilder AddNowPlayinClient(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddBlazorBootstrap();

        builder.Services.AddScoped(sp =>
            new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddScoped<IRecommendationService, RecommendationClientService>();

        return builder;
    }
}
