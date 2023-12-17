using BitFineX.Auction;
using BitFineX.Client;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Clients.Core
{
    public class ClientServer : AuctionStateService.AuctionStateServiceBase
    {
        public ClientStateService.ClientStateServiceClient _clientService;
        private readonly List<Client> clients = new();
        Dictionary<string, List<ClientAuctions>> clientAuctions = new();
        Dictionary<string, List<BidInformation>> bidsInfo = new ();
        public ClientServer() 
        {
            clients.Add(new Client() { ClientID = "123", ClientName = "Client 1", ClientIp = "http://localhost:5215" });
            clients.Add(new Client() { ClientID = "234", ClientName = "Client 2", ClientIp = "http://localhost:5244" });
            clients.Add(new Client() { ClientID = "345", ClientName = "Client 3", ClientIp = "http://localhost:5112" });
        }

        public async override Task<AuctionServiceResponse> AuctionService(AuctionServiceRequest request, ServerCallContext context)
        {
            if(request.AuctionTransactionTypeCase == AuctionServiceRequest.AuctionTransactionTypeOneofCase.CreateAuction)
            {
                Console.WriteLine($"Auction has been created by {request.ClientName} for {request.CreateAuction.PictureName} for a price of {request.CreateAuction.PicturePrice} $");
                Console.WriteLine("Do you want to Bid? Yes/No");
                string bid = Console.ReadLine();

                if(bid == "Yes")
                {
                    Console.WriteLine("Enter Bid price");
                    var bidPrice = Console.ReadLine();

                    double price = 0;

                    while (!double.TryParse(bidPrice, out price))
                    {
                        Console.WriteLine("Please enter a valid amount");
                        bidPrice = Console.ReadLine();
                    }

                    // grpc call to the auction owner

                    GrpcChannel grpcChannel = GrpcChannel.ForAddress(clients.First(c => c.ClientID == request.ClientId).ClientIp);
                    _clientService = new(grpcChannel);

                    ReceiveBidsRequest bidReq = new()
                    {
                        ClientId = request.ClientId,
                        ClientName = request.ClientName,
                        PictureId = request.CreateAuction.PictureId,
                        PictureName = request.CreateAuction.PictureName,
                        PicturePrice = price,
                        BiddingTime = DateTime.UtcNow.ToTimestamp()
                    };

                    await _clientService.ReceiveBidsAsync(bidReq);
                }

            }

            if(request.AuctionTransactionTypeCase == AuctionServiceRequest.AuctionTransactionTypeOneofCase.AuctionBidsRequest)
            {
                var auctionOwner = string.Empty;

                foreach (var cl in clientAuctions)
                {
                    if(cl.Value.Any(c => c.PictureId == request.AuctionBidsRequest.BidPictureId))
                    {
                        auctionOwner = cl.Key; break;
                    }
                }

                var latestBid = bidsInfo[request.AuctionBidsRequest.BidPictureId];

                // grpc call to the auction owner

                GrpcChannel grpcChannel = GrpcChannel.ForAddress(clients.First(c => c.ClientID == auctionOwner).ClientIp);
                _clientService = new(grpcChannel);

                ReceiveBidsRequest bidReq = new()
                {
                    ClientId = request.AuctionBidsRequest.BidClientId,
                    ClientName = clients.First(c => c.ClientID == request.AuctionBidsRequest.BidClientId).ClientName,
                    PictureId = request.AuctionBidsRequest.BidPictureId,
                    PictureName = latestBid.First().PictureName,
                    PicturePrice = request.AuctionBidsRequest.BidPicturePrice,
                    BiddingTime = DateTime.UtcNow.ToTimestamp()
                };

                await _clientService.ReceiveBidsAsync(bidReq);

            }

            if(request.AuctionTransactionTypeCase == AuctionServiceRequest.AuctionTransactionTypeOneofCase.CompleteAuction)
            {
                Console.WriteLine($"Bid for {clients.First(c => c.ClientID == request.CompleteAuction.ClientId).ClientName} has been accepted");
            }

            return new AuctionServiceResponse();
        }
    }
}