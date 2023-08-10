using OnlineShopping.Libraries.Models;
using System.Net;


namespace OnlineShopping.Libraries.Services
{
    public class SmsService : ISMSService
    {
        public const int TypeSmsSending = 5;
        private readonly SmsConfiguration _smsConfiguration;
        public SmsService(SmsConfiguration smsConfiguration)
        {
            _smsConfiguration = smsConfiguration;
        }

        public string GetUserInfor()
        {
            String url = _smsConfiguration.RootUrl + "/user/info";
            NetworkCredential myCreds = new NetworkCredential(_smsConfiguration.AccessToken, ":x");
            WebClient client = new WebClient();
            client.Credentials = myCreds;
            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            return reader.ReadToEnd();
        }

        public string SendSMS(Sms sms)
        {          
                String url = _smsConfiguration.RootUrl + "/sms/send";
                if (sms.To.Count <= 0)
                    return "No recerver!";
                if (sms.Content.Equals(""))
                    return "No SMS content";
                NetworkCredential myCreds = new NetworkCredential(_smsConfiguration.AccessToken, ":x");
                WebClient client = new WebClient();
                client.Credentials = myCreds;
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                string builder = "{\"to\":[";

                for (int i = 0; i < sms.To.Count; i++)
                {
                    builder += "\"" + sms.To[i] + "\"";
                    if (i < sms.To.Count - 1)
                    {
                        builder += ",";
                    }
                }
                builder += "], \"content\": \"" + Uri.EscapeDataString(sms.Content) + "\", \"type\":" + TypeSmsSending + ", \"sender\": \"" + _smsConfiguration.DeviceID + "\"}";

                String json = builder.ToString();
                return client.UploadString(url, json);           
        }
    }
}
