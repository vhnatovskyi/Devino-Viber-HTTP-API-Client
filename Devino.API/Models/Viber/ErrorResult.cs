using System.Runtime.Serialization;

namespace Devino.API.Models.Viber
{
    [DataContract]
    public class ErrorResult
    {
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public string Error { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public long TimeStamp { get; set; }
    }
}
