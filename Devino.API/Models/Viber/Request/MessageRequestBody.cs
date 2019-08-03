using System.Collections.Generic;

namespace Devino.API.Models.Viber.Request
{
    public class MessageRequestBody
    {
        public bool resendSms { get; set; }
        public List<Message> messages { get; set; }
    }
}
