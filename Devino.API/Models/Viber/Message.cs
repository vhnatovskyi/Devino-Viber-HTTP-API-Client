namespace Devino.API.Models.Viber
{
    public class Message
    {
        #region System
        public string type { get; set; }
        public string comment { get; set; }
        public string subject { get; set; }
        public string address { get; set; }
        public string priority { get; set; }
        public int validityPeriodSec { get; set; }
        public MessageContent content { get; set; }
        public string contentType { get; set; }
        #endregion System
        #region SMS
        public string smsText { get; set; }
        public string smsSrcAddress { get; set; }
        public int smsValidityPeriodSec { get; set; }
        #endregion SMS
    }
}
