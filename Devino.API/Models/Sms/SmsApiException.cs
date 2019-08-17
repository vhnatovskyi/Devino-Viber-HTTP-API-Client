using System;

namespace Devino.API.Models.Sms
{
    public class SmsApiException:Exception
    {
        public ErrorResult Error { get; set; }
        public SmsApiException(ErrorResult error) : base(error.Desc)
        {
            Error = error;
        }
    }
}
