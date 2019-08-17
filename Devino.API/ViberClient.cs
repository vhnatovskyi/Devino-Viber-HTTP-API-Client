using Devino.API.Models.Viber;
using Devino.API.Models.Viber.Request;
using Devino.API.Models.Viber.Response;
using Nancy.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Devino.API
{
    public class ViberClient
    {
        #region Protected
        protected int livePeriod { get; set; }
        protected int sendingCount { get; set; }
        protected int liveTimeMessage { get; set; }

        protected string login { get; set; }
        protected string password { get; set; }

        protected string providerUrl { get; }
        protected string providerStatusUrl { get; }

        protected bool resendSms { get; } 
      
        protected string sourceSmsAddress { get; }
        protected string sourceViberAddress { get; }

        protected static Dictionary<ContentType, string> ContentTypes
        {
            get
            {
                Dictionary<ContentType, string> contentTypes = new Dictionary<ContentType, string>(3);
                contentTypes.Add(ContentType.TEXT, "text");
                contentTypes.Add(ContentType.BUTTON, "button");
                contentTypes.Add(ContentType.IMAGE, "image");
                return contentTypes;
            }
        }
        protected static Dictionary<PriorityType, string> PriorityTypes
        {
            get
            {
                Dictionary<PriorityType, string> priorityType = new Dictionary<PriorityType, string>(4);
                priorityType.Add(PriorityType.LOW, "low");
                priorityType.Add(PriorityType.NORMAL, "normal");
                priorityType.Add(PriorityType.HIGH, "high");
                priorityType.Add(PriorityType.REALTIME, "realtime");
                return priorityType;
            }
        }

        protected string Post(object body, string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{login}:{password}"));
            request.Headers.Add("Authorization", $"Basic {encoded}");
            request.ContentType = "application/json";
            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(new JavaScriptSerializer().Serialize(body));
            }
            try
            {
                using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (WebException exception)
            {
                if (exception.Response != null)
                {
                    Stream stream = exception.Response.GetResponseStream();
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string content = reader.ReadToEnd();
                            if(!string.IsNullOrEmpty(content))
                                throw new ViberApiException(new JavaScriptSerializer().Deserialize<ErrorResult>(content));
                        }
                    }
                }
                throw;
            }
        }
        protected SendingReplay GetSendingReplay(string model) =>
            new JavaScriptSerializer().Deserialize<SendingReplay>(model);
        protected StatusResponse GetStatusResponse(string model) =>
            new JavaScriptSerializer().Deserialize<StatusResponse>(model);
        protected MessageRequestBody BuildBody(string phone, string sourceAddress, string messageText,
            ContentType contentType = ContentType.TEXT, string type = "viber", PriorityType priority = PriorityType.LOW,
            string smsText = null, string smsSourceAddres = null, string comment = null, string buttonUrl = null,
            string buttonCaption = null, string imageUrl = null, bool resendSms = true)
        {
            MessageContent content = new MessageContent()
            {
                text = messageText,
                action = buttonUrl,
                caption = buttonCaption,
                imageUrl = imageUrl,
            };
            Message message = new Message()
            {
                address = phone,
                priority = PriorityTypes[priority],
                content = content,
                comment = comment,
                subject = sourceAddress,
                contentType = ContentTypes[contentType],
                validityPeriodSec = liveTimeMessage,
                type = type,
                smsText = smsText ?? content.text,
                smsSrcAddress = smsSourceAddres ?? sourceAddress,
                smsValidityPeriodSec = liveTimeMessage * 3,
            };
            List<Message> messages = new List<Message>() { message };
            MessageRequestBody body = new MessageRequestBody()
            {
                resendSms = resendSms,
                messages = messages,
            };
            return body;
        }

        #endregion Protected
        #region Public

            #region Constructor
        public ViberClient(string login, string password, string sourceViberAddress,
            string sourceSmsAddress = null, bool resendSMS = false,int livePeriod = 1, 
            int sendingCount = 3, int liveTimeMessage = 300,                            
            string providerUrl = "https://viber.devinotele.com:444/send",
            string providerStatusUrl = "https://viber.devinotele.com:444/status")
        {
            this.login = login;
            this.password = password;
            this.resendSms = resendSMS;
            this.livePeriod = livePeriod;
            this.sendingCount = sendingCount;
            this.liveTimeMessage = liveTimeMessage;
            this.providerUrl = providerUrl;
            this.providerStatusUrl = providerStatusUrl;
            this.sourceViberAddress = sourceViberAddress;
            this.sourceSmsAddress = sourceSmsAddress ?? sourceViberAddress;
        }
            #endregion Constructor

            #region Request

                #region SendMessage
        public virtual SendingReplay SendMessage(string phone, string messageText,
            ContentType contentType = ContentType.TEXT, PriorityType priority = PriorityType.LOW, string type = "viber",
            string smsText = null, string comment = null, string buttonUrl = null,
            string buttonCaption = null, string imageUrl = null) => 
            SendMessage(BuildBody(phone, sourceViberAddress, messageText, contentType, type, priority, smsText, sourceSmsAddress, comment, buttonUrl, buttonCaption, imageUrl, resendSms));
        public virtual SendingReplay SendMessage(Message message, bool resendSms = false) =>
            SendMessage(BuildBody(message, resendSms));
        public virtual SendingReplay SendMessage(MessageRequestBody body) => 
            GetSendingReplay(Post(body, providerUrl));
        public virtual SendingReplay SendMessage(Message message, out string resultResponse, bool resendSms = false) =>
            SendMessage(BuildBody(message, resendSms), out resultResponse);
        public virtual SendingReplay SendMessage(MessageRequestBody body, out string resultResponse)
        {
            resultResponse = Post(body, providerUrl);
            return GetSendingReplay(resultResponse);
        }
        #endregion SendMessage

                #region GetStatusMessage
        public virtual StatusResponse GetStatusMessage(long messageId) => 
            GetStatusMessage(new List<long>() { messageId });
        public virtual StatusResponse GetStatusMessage(List<long> messagesId) =>
            GetStatusMessage(BuildBody(messagesId));
        public virtual StatusResponse GetStatusMessage(StatusRequestBody body) =>
            GetStatusResponse(Post(body, providerStatusUrl));
        public virtual StatusResponse GetStatusMessage(long messageId, out string resultResponse) => 
            GetStatusMessage(new List<long>() { messageId }, out resultResponse);        
        public virtual StatusResponse GetStatusMessage(List<long> messagesId, out string resultResponse)
        {            
            return GetStatusMessage(BuildBody(messagesId), out resultResponse);
        }
        public virtual StatusResponse GetStatusMessage(StatusRequestBody body, out string resultResponse)
        {
            resultResponse = Post(body, providerStatusUrl);
            return GetStatusResponse(resultResponse);
        }
        #endregion GetStatusMessage

        #endregion Request

            #region BuildBody
        public MessageRequestBody BuildBody(Message message, bool resendSms) =>
           new MessageRequestBody() { messages = new List<Message>() { message }, resendSms = resendSms };
        public StatusRequestBody BuildBody(long message) =>
            new StatusRequestBody() { messages = new List<long>() { message } };
        public StatusRequestBody BuildBody(List<long> messages) =>
            new StatusRequestBody() { messages = messages };
        #endregion BuildBody

        #endregion Public
    }
}
