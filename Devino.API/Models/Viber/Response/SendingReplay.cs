using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Devino.API.Models.Viber.Response
{
    [DataContract]
    public class SendingReplay
    {
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public List<MessageStatus> messages { get; set; }
    }
}
