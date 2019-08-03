using System.Runtime.Serialization;

namespace Devino.API.Models.Viber
{
    [DataContract]
    public class MessageStatus
    {
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public long providerId { get; set; }
    }
}
