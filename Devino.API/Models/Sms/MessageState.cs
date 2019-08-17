namespace Devino.API.Models.Sms
{
    public enum MessageState
    {
        Sent = -1,
        InQueue = -2,
        Deleted = -97,
        Stopped = -98,
        Delivered = 0,
        WrongSourceAddress = 10,
        WrongDestinationAddress = 11,
        InvalidDestinationAddress = 41,
        RejectedBySmsCenter = 42,
        ValidityExpired = 46,
        Rejected = 69,
        Unknown = 99,
        StateExpired = 255
    }
}
