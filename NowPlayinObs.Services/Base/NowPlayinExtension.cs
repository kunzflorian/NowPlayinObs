using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NowPlayinObs.Hubs;

namespace NowPlayinObs.Services;

public static class NowPlayinExtension
{
    public static IServiceCollection AddNowPlayin(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR();
        services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                ["application/octet-stream"]);
        });

        var config = configuration.GetSection("NowPlayin").Get<NowPlayinConfig>();
        if (config is not null)
            services.AddSingleton(config);
        else
            throw new Exception("config not found");


        services.AddTwitch(configuration);

        services.AddSingleton<NowPlayinService>();
        services.AddHostedService<NowPlayinWorker>();

        services.AddSingleton<IRecommendationService, RecommendationService>();
        services.AddHostedService<RecommendationWorker>();

        // default client for hub
        services.AddHttpClient();
        services.ConfigureHttpClientDefaults(o =>
        {
            o.ConfigureHttpClient(c =>
                c.BaseAddress = new Uri(configuration.GetSection("Kestrel:Endpoints:Default:Url").Get<string>()!));
        });

        return services;
    }

    private static IServiceCollection AddTwitch(this IServiceCollection services, IConfiguration configuration)
    {
        var configTwitch = configuration.GetSection("Twitch").Get<TwitchConfig>();
        if (configTwitch is not null)
        {
            services.AddSingleton(configTwitch);

            services.AddHttpClient(
                TwitchConfig.HttpClientAuth,
                client =>
                {
                    client.BaseAddress = new Uri(configTwitch!.BaseAuthUrl);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    // Add a user-agent default request header.
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-docs");
                }
            ).AddAsKeyed();
            services.AddHttpClient(
                TwitchConfig.HttpClientService,
                client =>
                {
                    client.BaseAddress = new Uri(configTwitch.BaseApiUrl!);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("client-id", configTwitch.ClientId);
                    // Add a user-agent default request header.
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-docs");
                }
            ).AddAsKeyed();

            services.AddScoped<AuthentificiationService>();
        }
        else
            throw new Exception("configTwitch not found");

        return services;
    }

    public static WebApplication UseNowPlayin(this WebApplication app)
    {
        app.UseResponseCompression();
        app.MapHub<NowPlayinHub>(NowPlayinHubDefaults.NOWPLAYIN_HUB);
        app.MapHub<RecommendationHub>(NowPlayinHubDefaults.RECOMMENDATION_HUB);
        return app;
    }
}
