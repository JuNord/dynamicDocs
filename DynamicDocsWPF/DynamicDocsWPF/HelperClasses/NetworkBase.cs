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
        
        protected DataMessage GetDataMessage(DataType dataType, int id)
        {
            var httpWebRequest =
                (HttpWebRequest) WebRequest.Create($"{BaseUrl}/{Enum.GetName(typeof(DataType), dataType)}/{id}");
            httpWebRequest.Method = "GET";
            var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

            if (httpWebResponse.StatusCode != HttpStatusCode.OK) return null;

            using (var responseStream =
                new StreamReader(httpWebResponse.GetResponseStream() ?? throw new HttpException()))
            {
                return JsonConvert.DeserializeObject<DataMessage>(responseStream.ReadToEnd());
            }
        }

        protected FileMessage GetFileById(string id, FileType fileType, User user)
        {
            try
            {
                var request = new FileRequest()
                {
                    Id = id,
                    FileType = fileType
                };
                var message = new DataMessage()
                {
                    DataType = DataType.FileRequest,
                    Content = JsonConvert.SerializeObject(request),
                    User = user
                };
                var postData = JsonConvert.SerializeObject(message);
                var bytes = Encoding.UTF8.GetBytes(postData);

                var httpWebRequest = (HttpWebRequest) WebRequest.Create($"{BaseUrl}/getfile");
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = bytes.Length;
                httpWebRequest.ContentType = "application/json";

                using (var requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    using (var responseStream =
                        new StreamReader(httpWebResponse.GetResponseStream() ?? throw new HttpException()))
                    {
                        return JsonConvert.DeserializeObject<FileMessage>(responseStream.ReadToEnd());
                    }
            }
            catch (HttpException)
            {
            }

            return null;
        }

        protected List<string> GetList(FileType fileType)
        {
            var httpWebRequest =
                (HttpWebRequest) WebRequest.Create($"{BaseUrl}/{Enum.GetName(typeof(FileType), fileType)}s");
            httpWebRequest.Method = "GET";
            var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

            if (httpWebResponse.StatusCode != HttpStatusCode.OK) return null;

            using (var responseStream =
                new StreamReader(httpWebResponse.GetResponseStream() ?? throw new HttpException()))
            {
                return JsonConvert.DeserializeObject<List<string>>(responseStream.ReadToEnd());
            }
        }
        
        protected DataMessage GetDataList(DataType dataType, User user)
        {
            try
            {
                var message = new DataMessage()
                {
                    DataType = dataType,
                    Content = null,
                    User = user
                };
                var postData = JsonConvert.SerializeObject(message);
                var bytes = Encoding.UTF8.GetBytes(postData);

                var httpWebRequest = (HttpWebRequest) WebRequest.Create($"{BaseUrl}/getlist");
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = bytes.Length;
                httpWebRequest.ContentType = "application/json";

                using (var requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    using (var responseStream =
                        new StreamReader(httpWebResponse.GetResponseStream() ?? throw new HttpException()))
                    {
                        return JsonConvert.DeserializeObject<DataMessage>(responseStream.ReadToEnd());
                    }
            }
            catch (HttpException)
            {
            }

            return null;
        }

        protected UploadResult PostFile(FileMessage message)
        {
            try
            {
                var postData = JsonConvert.SerializeObject(message);
                var bytes = Encoding.UTF8.GetBytes(postData);

                var httpWebRequest = (HttpWebRequest) WebRequest.Create($"{BaseUrl}/fileMessage");
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = bytes.Length;
                httpWebRequest.ContentType = "application/json";

                using (var requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    using (var responseStream =
                        new StreamReader(httpWebResponse.GetResponseStream() ?? throw new HttpException()))
                    {
                        return JsonConvert.DeserializeObject<UploadResult>(responseStream.ReadToEnd());
                    }
            }
            catch (HttpException)
            {
            }

            return UploadResult.FAILED_OTHER;
        }

        protected UploadResult PostData(User user, DataType type, string content)
        {
            try
            {
                var message = new DataMessage
                {
                    User = user,
                    DataType = type,
                    Content = content
                };
                var postData = JsonConvert.SerializeObject(message);
                var bytes = Encoding.UTF8.GetBytes(postData);

                var requestString = $"{BaseUrl}/dataMessage";
                var httpWebRequest = (HttpWebRequest) WebRequest.Create(requestString);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = bytes.Length;
                httpWebRequest.ContentType = "application/json";

                using (var requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    using (var responseStream =
                        new StreamReader(httpWebResponse.GetResponseStream() ?? throw new HttpException()))
                    {
                        return JsonConvert.DeserializeObject<UploadResult>(responseStream.ReadToEnd());
                    }
            }
            catch (HttpException)
            {
            }

            return UploadResult.FAILED_OTHER;
        }
    }
}