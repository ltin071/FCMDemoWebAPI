using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace FCMDemoWebAPI.Controllers
{
    public class DefaultController : ApiController
    {
        [System.Web.Http.Route("api/notification/send")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> SendNotification()
        {
            NameValueCollection requestBody = await Request.Content.ReadAsFormDataAsync();
            string fcmtoken = requestBody["fcmtoken"];
            string message = requestBody["message"];

            string returnMessage = SendPushNotification(message, fcmtoken);
            return Ok(returnMessage);
                

        }
        [Route("api/notification/broadcast")]
        [HttpPost]
        public async Task<IHttpActionResult> BroadcastNotification()
        {

            NameValueCollection requestBody = await Request.Content.ReadAsFormDataAsync();
            string message = requestBody["message"];

            
            string returnMessage = BroadcastPushNotification(message);

            return Ok(returnMessage);   

        }
        public static string SendPushNotification(string message, string fcmtoken)
        {
            string applicationID = ConfigurationManager.AppSettings["FCMAuthorizationKey"];
            string senderId = ConfigurationManager.AppSettings["FCMSenderID"];

            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create("https://fcm.googleapis.com/fcm/send");
            httpWebRequest.Method = "post";
            httpWebRequest.ContentType = "application/json";
            var data = new
            {
                to = fcmtoken,
                priority = "high",
                notification = new
                {
                    body = message
                },
                android = new
                {
                    header = new
                    {
                        priority = "high"
                    }
                }
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
            httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
            httpWebRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
            httpWebRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = httpWebRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = httpWebRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                        {
                            String sResponseFromServer = tReader.ReadToEnd();
                            string str = sResponseFromServer;
                            return str;
                        }
                    }
                }
            }
        }
        public static string BroadcastPushNotification(string message)
        {
            string applicationID = ConfigurationManager.AppSettings["FCMAuthorizationKey"];
            string senderId = ConfigurationManager.AppSettings["FCMSenderID"];

            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = "application/json";
            var data = new
            {
                to = "/topics/news",
                priority = "high",
                notification = new
                {
                    body = message
                }
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
            tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
            tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
            tRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                        {
                            String sResponseFromServer = tReader.ReadToEnd();
                            string str = sResponseFromServer;
                            return str;
                        }
                    }
                }
            }
        }
    }
}
