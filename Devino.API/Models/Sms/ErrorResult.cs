using System.Runtime.Serialization;

namespace Devino.API.Models.Sms
{
    [DataContract]
    public class ErrorResult
    {
        [DataMember]
        public int Code { get; set; }
        [DataMember]
        public string Desc { get; set; }
    }
}
