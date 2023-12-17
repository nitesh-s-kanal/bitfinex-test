using BitFineX.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clients.Core.Common
{
    public interface IClientToServer
    {
        public Task<bool> SendCreateAuctionRequest(AuctionInformation auctionInformation, CancellationToken cancellationToken);

        public Task<bool> SendBidInformation(ReceiveBidsRequest request, CancellationToken cancellationToken);
    }
}
