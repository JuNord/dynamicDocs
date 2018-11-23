using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using RestService;

namespace DynamicDocsWPF.Networking
{
    public class NetworkHelper
    {
        private string BaseUrl { get; }

        public NetworkHelper(string baseUrl)
        {
            BaseUrl = baseUrl;
        }
        
        public UploadResult PostFile(FileMessage message)
        {
            try
            {
                var postData = JsonConvert.SerializeObject(message);
                var bytes = Encoding.UTF8.GetBytes(postData);

                var httpWebRequest = (HttpWebRequest) WebRequest.Create(BaseUrl+"/Template");
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = bytes.Length;
                httpWebRequest.ContentType = "application/json";

                using (var requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var responseStream = new StreamReader(httpWebResponse.GetResponseStream() ?? throw new HttpException()))
                    {
                        return JsonConvert.DeserializeObject<UploadResult>(responseStream.ReadToEnd());
                    }
                }
            }
            catch (HttpException)
            {
            }

            return UploadResult.FAILED_OTHER;
        }


        public List<string> GetTemplates() => GetList(FileType.Template);
        public List<string> GetProcesses() => GetList(FileType.Process);
        public FileMessage GetTemplateByName(string name) => GetFileByName(FileType.Template, name);
        public FileMessage GetProcessByName(string name) => GetFileByName(FileType.Process, name);

        private FileMessage GetFileByName(FileType fileType, string name)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create($"{BaseUrl}/{Enum.GetName(typeof(FileType), fileType)}/{name}");
            httpWebRequest.Method = "GET";
            var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

            if (httpWebResponse.StatusCode != HttpStatusCode.OK) return null;
         
            using (var responseStream = new StreamReader(httpWebResponse.GetResponseStream() ?? throw new HttpException()))
            {
                return JsonConvert.DeserializeObject<FileMessage>(responseStream.ReadToEnd());
            }
        }
        
        private List<string> GetList(FileType fileType)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create($"{BaseUrl}/{Enum.GetName(typeof(FileType), fileType)}s");
            httpWebRequest.Method = "GET";
            var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

            if (httpWebResponse.StatusCode != HttpStatusCode.OK) return null;
         
            using (var responseStream = new StreamReader(httpWebResponse.GetResponseStream() ?? throw new HttpException()))
            {
                return JsonConvert.DeserializeObject<List<string>>(responseStream.ReadToEnd());
            }
        }
    }
}