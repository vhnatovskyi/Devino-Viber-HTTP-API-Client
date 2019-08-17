using System;

namespace Devino.API.Models.Sms
{
    public struct StatusResponse
    {
        public MessageState State { get; set; }
        public DateTime? CreationDateUtc { get; set; }
        public DateTime? SubmittedDateUtc { get; set; }
        public DateTime? ReportedDateUtc { get; set; }
        public DateTime TimeStampUtc { get; set; }
        public string StateDescription { get; set; }
        public decimal? Price { get; set; }        
    }
}
