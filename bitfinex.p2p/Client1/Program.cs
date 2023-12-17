using Client1;
using Clients.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IClientToServer, ClientToServer>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
