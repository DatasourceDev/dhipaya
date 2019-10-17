using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Text;
using System.Collections.Specialized;

namespace Dhipaya.Extensions
{
    public sealed class RequestUtil
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(MvcApplication));

        public static string SendRequest(string url)
        {
            string result = "";
            using (var client = new WebClient())
            {
                try
                {
                    client.Encoding = Encoding.UTF8;
                    client.Proxy = null;
                    result = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    //log.Error("Send Request To Web service: " + ex.Message);
                    return result;
                }
            }
            return result;
        }

        //public static string PostRequest(string endpoint, MidtransModel m, string authValue)
        //{
        //    // Create string to hold JSON response
        //    string jsonResponse = string.Empty;
        //    using (var client = new WebClient())
        //    {
        //        try
        //        {
        //            client.Headers["Content-Type"] = "application/json";
        //            client.Headers["Accept"] = "application/json";
        //            client.Headers["Authorization"] = string.Format("Basic {0}", authValue);
        //            client.Encoding = Encoding.UTF8;
        //            client.Proxy = null;
        //            var uri = new Uri(endpoint);
        //            JsonSerializerSettings settings = new JsonSerializerSettings();
        //            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        //            var data = JsonConvert.SerializeObject(m, settings);
        //            jsonResponse = client.UploadString(uri, "POST", data);
        //        }
        //        catch (WebException ex)
        //        {
        //            // Http Error
        //            if (ex.Status == WebExceptionStatus.ProtocolError)
        //            {
        //                HttpWebResponse wrsp = (HttpWebResponse)ex.Response;
        //                var statusCode = (int)wrsp.StatusCode;
        //                var msg = wrsp.StatusDescription;
        //                log.Error("Post Request To Midtrans service: " + msg);
        //                //throw new HttpException(statusCode, msg);
        //            }
        //            else
        //            {
        //                //throw new HttpException(500, ex.Message);
        //            }
        //        }
        //    }

        //    return jsonResponse;
        //}


        public static string PostRequest(string endpoint, object oj, string auth = "", string authtype = "")
        {
            string jsonResponse = string.Empty;
            using (var client = new WebClient())
            {
                try
                {
                    client.Headers["Content-Type"] = "application/json";
                    client.Headers["Accept"] = "application/json";
                    if (!string.IsNullOrEmpty(authtype) && !string.IsNullOrEmpty(auth))
                    {
                        if (authtype == "Basic")
                            client.Headers["Authorization"] = string.Format("Basic {0}", auth);
                        else if (authtype == "Bearer")
                            client.Headers["Authorization"] = string.Format("Bearer {0}", auth);
                    }
                    client.Encoding = Encoding.UTF8;
                    client.Proxy = null;
                    var uri = new Uri(endpoint);
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    var data = JsonConvert.SerializeObject(oj, settings);
                    jsonResponse = client.UploadString(uri, "POST", data);
                }
                catch (WebException ex)
                {
                    // Http Error
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse wrsp = (HttpWebResponse)ex.Response;
                        var statusCode = (int)wrsp.StatusCode;
                        var msg = wrsp.StatusDescription;
                        //log.Error("Post request to service error: " + msg);
                    }
                    else
                    {

                    }
                }
            }

            return jsonResponse;
        }

        public static string GetRequest(string endpoint, string auth, string authtype = "")
        {
            string result = "";
            using (var client = new WebClient())
            {
                try
                {
                    client.Headers["Content-Type"] = "application/json";
                    client.Headers["Accept"] = "application/json";
                    if (!string.IsNullOrEmpty(authtype) && !string.IsNullOrEmpty(auth))
                    {
                        if (authtype == "Basic")
                            client.Headers["Authorization"] = string.Format("Basic {0}", auth);
                        else if (authtype == "Bearer")
                            client.Headers["Authorization"] = string.Format("Bearer {0}", auth);
                    }
                    client.Encoding = Encoding.UTF8;
                    client.Proxy = null;
                    result = client.DownloadString(endpoint);
                }
                catch (Exception ex)
                {
                    //log.Error("Send Request To Web service: " + ex.Message);
                    return result;
                }
            }
            return result;
        }
    }
}