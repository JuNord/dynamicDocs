using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using RestService;
using WebServer.Model;
using Entry = RestService.Entry;

namespace DynamicDocsWPF.HelperClasses
{
    public class NetworkHelper
    {
        private string BaseUrl { get; }

        public NetworkHelper(string baseUrl)
        {
            BaseUrl = baseUrl;
        }
       
        public List<string> GetTemplates() => GetList(FileType.DocTemplate);
        public List<string> GetProcesses() => GetList(FileType.ProcessTemplate);
        public FileMessage GetTemplateByName(string name) => GetFileByName(FileType.DocTemplate, name);
        public FileMessage GetProcessByName(string name) => GetFileByName(FileType.ProcessTemplate, name);

        public Entry GetEntryById(int id)
        {
            var dataMessage = GetDataMessage(DataType.Entry, id);

            return dataMessage.DataType == DataType.Entry 
                ? JsonConvert.DeserializeObject<Entry>(dataMessage.Content) 
                : null;
        }

        public ProcessInstance GetProcessInstanceById(int id)
        {
            var dataMessage = GetDataMessage(DataType.ProcessInstance, id);

            return dataMessage.DataType == DataType.ProcessInstance
                ? JsonConvert.DeserializeObject<ProcessInstance>(dataMessage.Content)
                : null;
        }

        public void PostProcessInstance(ProcessInstance processInstance)
        {
            var message = new DataMessage()
            {
                DataType = DataType.ProcessInstance,
                Content = JsonConvert.SerializeObject(processInstance)
            };
            
            PostData(message);
        }
        
        public UploadResult UploadProcessTemplate(string filePath, bool forceOverwrite)
        {
            var process = XmlHelper.ReadXMLFromPath(filePath);

            return PostFile(new FileMessage()
            {
                FileName = Path.GetFileName(filePath),
                FileType = FileType.ProcessTemplate,
                ID = process.Name,
                Content = File.ReadAllText(filePath),
                ForceOverWrite = forceOverwrite
            });
        }
        
        public UploadResult UploadDocTemplate(string templateId, string filePath, bool forceOverwrite)
        {
            var process = XmlHelper.ReadXMLFromPath(filePath);
            
            return PostFile(new FileMessage()
            {
                FileName = Path.GetFileName(filePath),
                FileType = FileType.ProcessTemplate,
                ID = process.Name,
                Content = File.ReadAllText(filePath),
                ForceOverWrite = forceOverwrite
            });
        }

        public void PostEntry(Entry entry)
        {
            var message = new DataMessage()
            {
                DataType = DataType.Entry,
                Content = JsonConvert.SerializeObject(entry)
            };
            
            PostData(message);
        }

        public void PostProcessUpdate(ProcessUpdate processUpdate)
        {
            var message = new DataMessage()
            {
                DataType = DataType.Entry,
                Content = JsonConvert.SerializeObject(processUpdate)
            };

            PostData(message);
        }
        
        private DataMessage GetDataMessage(DataType dataType, int id)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create($"{BaseUrl}/{Enum.GetName(typeof(DataType), dataType)}/{id}");
            httpWebRequest.Method = "GET";
            var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

            if (httpWebResponse.StatusCode != HttpStatusCode.OK) return null;
         
            using (var responseStream = new StreamReader(httpWebResponse.GetResponseStream() ?? throw new HttpException()))
            {
                return JsonConvert.DeserializeObject<DataMessage>(responseStream.ReadToEnd());
            }
        }
        
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
        
        public UploadResult PostFile(FileMessage message)
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
        
        public UploadResult PostData(DataMessage message)
        {
            try
            {
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
    }
}