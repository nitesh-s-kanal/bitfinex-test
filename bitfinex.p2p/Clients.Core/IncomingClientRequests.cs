using BitFineX.Auction;
using BitFineX.Client;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clients.Core
{
    public class IncomingClientRequests : ClientStateService.ClientStateServiceBase
    {
        public AuctionStateService.AuctionStateServiceClient _server;
        private readonly List<Client> clients = new();

        public IncomingClientRequests()
        {
            clients.Add(new Client() { ClientID = "123", ClientName = "Client 1", ClientIp = "http://localhost:5215" });
            clients.Add(new Client() { ClientID = "234", ClientName = "Client 2", ClientIp = "http://localhost:5244" });
            clients.Add(new Client() { ClientID = "345", ClientName = "Client 3", ClientIp = "http://localhost:5112" });
        }

        public async override Task<ReceiveBidsResponse> ReceiveBids(ReceiveBidsRequest request, ServerCallContext context)
        {
            Console.WriteLine($"you received bids from {request.ClientName} for {request.PictureName} for {request.PicturePrice} $");
            Console.WriteLine("Do you want to accept it? Yes/No");

            string yesNo  = Console.ReadLine();
            if( yesNo != "Yes" ) 
            {
                foreach(Client client in clients )
                {
                    GrpcChannel grpcChannel = GrpcChannel.ForAddress(clients.First(c => c.ClientID != request.ClientId).ClientIp);
                    _server = new(grpcChannel);

                    AuctionServiceRequest compRequest = new ()
                    {
                        ClientId = request.ClientId,
                        ClientName = request.ClientName,
                        CompleteAuction = new CompleteAuction
                        {
                            ClientId = client.ClientID,
                            Accepted = true,
                            PictureId = request.PictureId
                        }
                    };

                    await _server.AuctionServiceAsync(compRequest);
                }
            }

            return new ReceiveBidsResponse();
        }

        public async override Task<CompleteAuctionResponse> CompleteAuction(CompleteAuctionRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Bid has been accepted for Client {clients.First(c => c.ClientID == request.ClientId).ClientName} for price {request.AcceptedPrice}");
            Console.WriteLine("Auction closed");

            return new CompleteAuctionResponse();
        }
    }
}
