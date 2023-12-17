using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clients.Core
{
    public class AuctionInformation
    {
        public string ClientId {  get; set; }
        public string ClientName { get; set; }
        public string PictureId { get; set; }
        public string PictureName { get; set; }
        public double PicturePrice { get; set; }
    }
}
