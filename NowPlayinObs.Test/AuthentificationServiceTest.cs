using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NowPlayinObs.Authentificiation;
using NowPlayinObs.Services;

namespace NowPlayinObs.Test;

[TestClass]
public sealed class AuthentificationServiceTest : TestBase
{
    [TestMethod]
    public async Task GetClientCredentialsTokenAsync()
    {   
        var serviceProvider = Services.BuildServiceProvider();

        var authentificiationService = serviceProvider.GetRequiredService<AuthentificiationService>();
        var twitchConfig = serviceProvider.GetRequiredService<TwitchConfig>();
       

        var token = await authentificiationService.GetClientCredentialsTokenAsync();

        Assert.IsNotNull(token);
    }

    [TestMethod]
    public async Task GetAuthCodeTokenAsync()
    {
        var serviceProvider = Services.BuildServiceProvider();

        var authentificiationService = serviceProvider.GetRequiredService<AuthentificiationService>();
        var twitchConfig = serviceProvider.GetRequiredService<TwitchConfig>();

        var token = await authentificiationService.GetAuthCodeTokenAsync(code: "tsjaxovza3mzkuclgbryw9vgiqn2gu", scope: "user:write:chat user:bot");

        Assert.IsNotNull(token);
    }
}
