using Clients.Core;
using Clients.Core.Common;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting.Server;

namespace Client1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IClientToServer _server;
        private readonly string clientId = "123";
        private readonly string clientName = "Client 1";

        public Worker(ILogger<Worker> logger, IClientToServer clientToServer)
        {
            _logger = logger;
            _server = clientToServer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Welcome Client 1");

                Console.WriteLine("Do you want to create an Auction? Yes/No");
                string yesNo = Console.ReadLine();

                if (yesNo == "Yes")
                {
                    Console.WriteLine("Enter picture name");
                    var picName = Console.ReadLine();

                    Console.WriteLine("Enter picture price");
                    var picPrice = Console.ReadLine();

                    double price = 0;

                    while (!double.TryParse(picPrice, out price))
                    {
                        Console.WriteLine("Please enter a valid amount");
                        picPrice = Console.ReadLine();
                    }


                    AuctionInformation auctionInformation = new AuctionInformation
                    {
                        ClientId = clientId,
                        ClientName = clientName,
                        PictureId = Guid.NewGuid().ToString(),
                        PictureName = picName,
                        PicturePrice = price
                    };

                    //Create a request and send to server
                    bool isCreated = await _server.SendCreateAuctionRequest(auctionInformation, stoppingToken);

                    if (isCreated)
                    {
                        Console.WriteLine($"Auction for the picture {picName} has been create for a price of {price}");
                    }

                    break;
                }

                await Task.Delay(10000000, stoppingToken);
            }
        }
    }
}