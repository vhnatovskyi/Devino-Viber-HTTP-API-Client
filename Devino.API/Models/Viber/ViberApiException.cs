using System;
namespace Devino.API.Models.Viber
{
    public class ViberApiException:Exception
    {
        public ErrorResult Error { get; set; }
        public ViberApiException(ErrorResult error) : base(error.Message)
        {
            Error = error;
        }
    }
}
