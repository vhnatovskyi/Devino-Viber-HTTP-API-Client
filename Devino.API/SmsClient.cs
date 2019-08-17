using Devino.API.Models.Sms;
using Nancy.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Devino.API
{
    public class SmsClient
    {
        #region Protected
        protected string login { get; set; }
        protected string password { get; set; }

        protected string providerUrl { get; set; } = "https://integrationapi.net/rest";

        public bool IsAuthorize
        {
            get => !string.IsNullOrEmpty(Get(HttpUtility.ParseQueryString(string.Empty), "/User/SessionId"));
        }
        #endregion Protected

        #region Public
        public SmsClient(string login, string password, string version = "v2")
        {
            this.login = login;
            this.password = password;
            this.providerUrl += $"/{version}";
        }
        #region Request
        public List<string> SendMessage(string sourceAddress, string destinationAddress, string data,
            DateTime? sendDate = null, int validity = 0)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["sourceAddress"] = sourceAddress;
            queryString["destinationAddress"] = destinationAddress;
            queryString["data"] = data;
            queryString["validity"] = validity.ToString();

            if (sendDate.HasValue)
            {
                queryString["sendDate"] = sendDate.Value.ToString("yyyy-MM-ddThh:mm:ss");
            }
            return new JavaScriptSerializer().Deserialize<List<string>>(Post(queryString.ToString(), "/Sms/Send"));
        }
        public List<string> SendMessage(string sourceAddress, List<string> destinationAddresses, string data,
            DateTime? sendDate = null, int validity = 0)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["sourceAddress"] = sourceAddress;
            queryString["data"] = data;
            queryString["validity"] = validity.ToString();
            if (sendDate.HasValue)
            {
                queryString["sendDate"] = sendDate.Value.ToString("yyyy-MM-ddThh:mm:ss");
            }
            var request = new StringBuilder();
            request.Append(queryString);
            foreach (var destinationAddress in destinationAddresses)
            {
                request.AppendFormat("&destinationAddresses={0}", HttpUtility.UrlEncode(destinationAddress));
            }
            return new JavaScriptSerializer().Deserialize<List<string>>(Post(request.ToString(), "/Sms/SendBulk"));
        }
        public List<string> SendMessageByTimeZone(string sourceAddress, string destinationAddress, string data,
            DateTime sendDate, int validity = 0)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["sourceAddress"] = sourceAddress;
            queryString["destinationAddress"] = destinationAddress;
            queryString["data"] = data;
            queryString["validity"] = validity.ToString();
            queryString["sendDate"] = sendDate.ToString("yyyy-MM-ddThh:mm:ss");
            return new JavaScriptSerializer().Deserialize<List<string>>(Post(queryString.ToString(), "/Sms/SendByTimeZone"));
        }
        public decimal GetBalance() => 
            new JavaScriptSerializer().Deserialize<decimal>(Get(HttpUtility.ParseQueryString(string.Empty), "/User/Balance"));        
        public StatusResponse GetStatusMessage(string messageId)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["messageId"] = messageId;
            return new JavaScriptSerializer().Deserialize<StatusResponse>(Get(queryString, "/Sms/State"));
        }
        public List<IncomingMessage> GetIncomingMessages(DateTime minDateUtc, DateTime maxDateUtc)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["minDateUTC"] = minDateUtc.ToString("yyyy-MM-ddThh:mm:ss");
            queryString["maxDateUTC"] = maxDateUtc.ToString("yyyy-MM-ddThh:mm:ss");
            return new JavaScriptSerializer().Deserialize<List<IncomingMessage>>(Get(queryString, "/Sms/In"));
        }
        #endregion Request
        #endregion Public

        #region Private
        private string Get(NameValueCollection query, string path)
        {
            query["login"] = login;
            query["password"] = password;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(providerUrl + path + "?" + query.ToString());
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            if (request.Proxy != null)
            {
                request.Proxy.Credentials = CredentialCache.DefaultCredentials;
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
            catch (WebException exeption)
            {
                if (exeption.Response != null)
                {
                    var contentStream = exeption.Response.GetResponseStream();
                    if (contentStream != null)
                    {
                        using (var reader = new StreamReader(contentStream))
                        {
                            var content = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(content))
                                throw new SmsApiException(new JavaScriptSerializer().Deserialize<ErrorResult>(content));
                            throw;
                        }
                    }
                }
                throw;
            }
        }
        private string Post(string query, string path)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(providerUrl + path + "?" + query.ToString());
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            if (request.Proxy != null)
            {
                request.Proxy.Credentials = CredentialCache.DefaultCredentials;
            }

            byte[] byteArr = Encoding.UTF8.GetBytes(query);
            request.ContentLength = byteArr.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(byteArr, 0, byteArr.Length);
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
            catch (WebException exeption)
            {
                if (exeption.Response != null)
                {
                    var contentStream = exeption.Response.GetResponseStream();
                    if (contentStream != null)
                    {
                        using (var reader = new StreamReader(contentStream))
                        {
                            var content = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(content))
                                throw new SmsApiException(new JavaScriptSerializer().Deserialize<ErrorResult>(content));
                            throw;
                        }
                    }
                }
                throw;
            }
        }
        #endregion Private
    }
}
