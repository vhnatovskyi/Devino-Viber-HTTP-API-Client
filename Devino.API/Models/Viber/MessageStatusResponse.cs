using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Devino.API.Models.Viber
{
    [DataContract]
    public class MessageStatusResponse:MessageStatus
    {
        [DataMember]
        public string error { get; set; }
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string statusAt { get; set; }
        [DataMember]
        public List<SmsState> smsStates { get; set; }
    }
}
