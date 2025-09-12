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

        services.AddSingleton<NowPlayinService>();

        var config = configuration.GetSection("NowPlayin").Get<NowPlayinConfig>();
        if (config is not null)
            services.AddSingleton(config);
        else
            throw new Exception("config not found");

        services.AddHostedService<NowPlayinWorker>();

        services.AddHttpClient();   
        services.ConfigureHttpClientDefaults(o => {
            o.ConfigureHttpClient(c =>
                c.BaseAddress = new Uri(configuration.GetSection("Kestrel:Endpoints:Default:Url").Get<string>()!));
        });

        return services;
    }

    public static WebApplication UseNowPlayin(this WebApplication app)
    {
        app.UseResponseCompression();
        app.MapHub<NowPlayinHub>("/nowplayinhub");
        return app;
    }
}
