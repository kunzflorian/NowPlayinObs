using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NowPlayinObs.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args)
                                    .AddNowPlayinClient();

await builder.Build().RunAsync();
