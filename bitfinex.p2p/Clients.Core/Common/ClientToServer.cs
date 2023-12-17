using BitFineX.Auction;
using BitFineX.Client;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;

namespace Clients.Core.Common
{
    public class ClientToServer : IClientToServer
    {
        public AuctionStateService.AuctionStateServiceClient _clientService;

        private readonly List<Client> clients = new();

        public ClientToServer()
        {
            clients.Add(new Client() { ClientID = "123", ClientName = "Client 1", ClientIp = "http://localhost:5215" });
            clients.Add(new Client() { ClientID = "234", ClientName = "Client 2", ClientIp = "http://localhost:5244" });
            clients.Add(new Client() { ClientID = "345", ClientName = "Client 3", ClientIp = "http://localhost:5112" });
        }

        public async Task<bool> SendCreateAuctionRequest(AuctionInformation auctionInformation, CancellationToken cancellationToken)
        {
            //send info to other clients
            foreach (var client in clients.Where(c => c.ClientID != auctionInformation.ClientId))
            {
                GrpcChannel grpcChannel = GrpcChannel.ForAddress(client.ClientIp);
                _clientService = new(grpcChannel);

                //Create auction service request
                AuctionServiceRequest request = new AuctionServiceRequest
                {
                    ClientId = auctionInformation.ClientId,
                    ClientName = auctionInformation.ClientName,
                    RequestCreatedDate = DateTime.UtcNow.ToTimestamp(),
                    CreateAuction = new CreateAuction
                    {
                        PictureId = auctionInformation.PictureId,
                        PictureName = auctionInformation.PictureName,
                        PicturePrice = auctionInformation.PicturePrice
                    }
                };

                var response = await _clientService.AuctionServiceAsync(request);
            }

            return true;
        }

        public async Task<bool> SendBidInformation(ReceiveBidsRequest request, CancellationToken cancellationToken)
        {

            foreach (var client in clients.Where(c => c.ClientID != request.ClientId))
            {
                GrpcChannel grpcChannel = GrpcChannel.ForAddress(client.ClientIp);
                _clientService = new(grpcChannel);

                //Create auction service request
                AuctionServiceRequest aucRequest = new AuctionServiceRequest
                {
                    ClientId = request.ClientId,
                    ClientName = request.ClientName,
                    RequestCreatedDate = DateTime.UtcNow.ToTimestamp(),
                    AuctionBidsRequest = new AuctionBidsRequest
                    {
                        BidClientId = request.ClientId,
                        BidPictureId = request.PictureId,
                        BidPicturePrice = request.PicturePrice,
                        IsCounterOffer = false
                    }
                };

                var response = await _clientService.AuctionServiceAsync(aucRequest);
            }

            return true;
        }
    }
}