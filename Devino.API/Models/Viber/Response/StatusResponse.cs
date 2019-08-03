using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Devino.API.Models.Viber.Response
{
    [DataContract]
    public class StatusResponse
    {
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public List<MessageStatusResponse> messages { get; set; }
    }
}
