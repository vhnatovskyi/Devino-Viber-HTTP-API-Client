using System;

namespace Devino.API.Models.Sms
{
    public class IncomingMessage
    {
        public string ID { get; set; }
        public string Data { get; set; }
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public DateTime CreationDateUtc { get; set; }
    }
}
