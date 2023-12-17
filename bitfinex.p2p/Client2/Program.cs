using Client2;
using Clients.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IClientToServer, ClientToServer>();
        services.AddHostedService<Worker>();
        services.AddGrpcClient<ClientServer>();
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(5002); // Specify the port number here
        });
        webBuilder.UseStartup<ClientServer>();
    })
    .Build();

await host.RunAsync();