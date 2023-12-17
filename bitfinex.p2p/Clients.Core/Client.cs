using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clients.Core
{
    public class Client
    {
        public string ClientID { get; set; }
        public string ClientName { get; set; }
        public string ClientIp { get; set; }
    }

    public class Picture
    {
        public string PictureID { get; set; }
        public string PictureName { get; set; }
    }

    public class ClientAuctions
    {
        public string PictureId { get; set; }
        public string PictureName { get; set; }
        public double PicturePrice { get; set; }
    }

    public class BidInformation
    {
        public string ClientID { get; set; }
        public string ClientName { get; set;}
        public string PictureName { get; set;}
        public double PicturePrice { get; set;}
        public bool IsCounter { get; set; }
        public Timestamp BidDate { get; set; }
    }
}
