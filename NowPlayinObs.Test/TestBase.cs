using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NowPlayinObs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowPlayinObs.Test;

public class TestBase
{
    public ServiceCollection Services { get; protected set; } = new ServiceCollection();

    protected IConfigurationRoot? _configurationRoot;

    [TestInitialize]
    public virtual async Task InitializeAsync()
    {
        _configurationRoot = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        Services.AddNowPlayin(_configurationRoot);

        Services.AddSingleton<IConfiguration>(_configurationRoot);

        await Task.CompletedTask;
    }
}
