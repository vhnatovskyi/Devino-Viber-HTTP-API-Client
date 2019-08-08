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
        #region ProtectedVariable
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

        protected virtual Dictionary<ContentType, string> ContentTypes
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
        protected virtual Dictionary<PriorityType, string> PriorityTypes
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
        #endregion ProtectedVariable
        #region Public
        /// <summary>
        /// </summary>
        /// <param name="login">Login devino</param>
        /// <param name="password">Password devino</param>
        /// <param name="sourceViberAddress">Name from whom Viber Message will come.</param>
        /// <param name="sourceSmsAddress">Name from whom sms Message will come. If sourceSmsAddress = null then even sourceViberAddres</param>
        /// <param name="resendSMS">Forward SMS true = send, false - not send!</param>
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
        #region Request
        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone">Phone number for send message</param>
        /// <param name="messageText">Message for send viber</param>
        /// <param name="contentType">Type message TEXT, BUTTON, IMAGE</param>
        /// <param name="priority">Set up priority message delivery LOW, NORMAL, HIGT or REALTIME</param>
        /// <param name="smsText">SMS text message if smsText even null then even viber message</param>
        /// <param name="buttonUrl">Button action URL address in message</param>
        /// <param name="buttonCaption">Button name in message</param>
        /// <param name="imageUrl">URL image in message</param>
        /// <returns></returns>
        public virtual SendingReplay SendMessage(string phone, string messageText,
            ContentType contentType = ContentType.TEXT, PriorityType priority = PriorityType.LOW, string type = "viber",
            string smsText = null, string comment = null, string buttonUrl = null,
            string buttonCaption = null, string imageUrl = null)
        {
            string result = string.Empty;
            SendingReplay response = null;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(providerUrl);
            request.Method = "POST";
            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{login}:{password}"));
            request.Headers.Add("Authorization", $"Basic {encoded}");
            request.ContentType = "application/json";
            MessageRequestBody body = BuildBody(phone, sourceViberAddress, messageText, contentType, type, priority, smsText, sourceSmsAddress, comment, buttonUrl, buttonCaption, imageUrl, resendSms);
            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(new JavaScriptSerializer().Serialize(body));
            }
            using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                response = new JavaScriptSerializer().Deserialize<SendingReplay>(result);
            }
            return response;
        }


        public virtual StatusResponse GetStatusMessage(long messageId)
        {
            return GetStatusMessage(messageId, out string result);
        }
        public virtual StatusResponse GetStatusMessage(List<long> messagesId)
        {
            return GetStatusMessage(messagesId, out string result);
        }
        public virtual StatusResponse GetStatusMessage(long messageId, out string resultResponse)
        {
            return GetStatusMessage(new List<long>() { messageId }, out resultResponse);
        }
        public virtual StatusResponse GetStatusMessage(List<long> messagesId, out string resultResponse)
        {
            StatusResponse response = null;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(providerStatusUrl);
            request.Method = "POST";
            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{login}:{password}"));
            request.Headers.Add("Authorization", $"Basic {encoded}");
            request.ContentType = "application/json";

            StatusRequestBody body = BuildBody(messagesId);

            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(new JavaScriptSerializer().Serialize(body));
            }

            using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    resultResponse = streamReader.ReadToEnd();
                }
                response = new JavaScriptSerializer().Deserialize<StatusResponse>(resultResponse);
            }
            return response;
        }
        #endregion Request
        #endregion Public
        #region Protected
        protected MessageRequestBody BuildBody(string phone, string sourceAddress, string messageText, ContentType contentType = ContentType.TEXT, string type = "viber", PriorityType priority = PriorityType.LOW,
            string smsText = null, string smsSourceAddres = null, string comment = null, string buttonUrl = null, string buttonCaption = null, string imageUrl = null, bool resendSms = true)
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

            List<Message> messages = new List<Message>();
            messages.Add(message);

            MessageRequestBody body = new MessageRequestBody()
            {
                resendSms = resendSms,
                messages = messages,
            };
            return body;
        }

        protected StatusRequestBody BuildBody(List<long> messages)
        {
            StatusRequestBody body = new StatusRequestBody()
            {
                messages = messages,
            };
            return body;
        }
        #endregion Protected
    }
}
