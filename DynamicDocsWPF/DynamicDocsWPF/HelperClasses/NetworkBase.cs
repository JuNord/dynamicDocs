using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using RestService;
using RestService.Model.Database;

namespace DynamicDocsWPF.HelperClasses
{
    public class NetworkBase
    {
        protected NetworkBase(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        private string BaseUrl { get; }

        protected string GetRequest(User user, string url, string message = null)
        {
            try
            {
                HttpWebRequest httpWebRequest;
                if(!string.IsNullOrWhiteSpace(message)) 
                    httpWebRequest = (HttpWebRequest) WebRequest.Create($"{BaseUrl}/{url}/{message}");
                else httpWebRequest = (HttpWebRequest) WebRequest.Create($"{BaseUrl}/{url}");
                httpWebRequest.Method = "GET";
                var encoded = Convert.ToBase64String(
                                Encoding.GetEncoding("ISO-8859-1")
                                    .GetBytes($"{user.Email}:{user.Password}"));
                httpWebRequest.Headers.Add("Authorization","Basic " + encoded);

                var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    using (var responseStream =
                        new StreamReader(httpWebResponse.GetResponseStream() ?? throw new HttpException()))
                    {
                        return responseStream.ReadToEnd();
                    }
            }
            catch (HttpException)
            {
            }

            return null;  
        }

        protected string PostRequest(User user, string url, string message = null)
        {
                try
                {
                    var postData = JsonConvert.SerializeObject(message);
                    var bytes = Encoding.UTF8.GetBytes(postData);

                    var requestString = $"{BaseUrl}/url";
                    var httpWebRequest = (HttpWebRequest) WebRequest.Create(requestString);
                    httpWebRequest.Method = "POST";
                    httpWebRequest.ContentLength = bytes.Length;
                    httpWebRequest.ContentType = "application/json";
                    var encoded = Convert.ToBase64String(
                        Encoding.GetEncoding("ISO-8859-1")
                            .GetBytes($"{user.Email}:{user.Password}"));
                    httpWebRequest.Headers.Add("Authorization","Basic " + encoded);

                    using (var requestStream = httpWebRequest.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                    }

                    var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

                    if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                        using (var responseStream =
                            new StreamReader(httpWebResponse.GetResponseStream() ?? throw new HttpException()))
                        {
                            return responseStream.ReadToEnd();
                        }
                }
                catch (HttpException)
                {
                }

            return null;
        }
    }
}