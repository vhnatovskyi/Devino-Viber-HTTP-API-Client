using System.Runtime.Serialization;

namespace Devino.API.Models.Viber
{
    [DataContract]
    public class SmsState
    {
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public string status { get; set; }
    }
}
