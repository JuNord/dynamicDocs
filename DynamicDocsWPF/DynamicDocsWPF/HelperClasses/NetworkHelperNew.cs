using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using DynamicDocsWPF.Model;
using Newtonsoft.Json;
using RestService;
using RestService.Model.Database;
using WebServerWPF.RestDots;
using WebServerWPF.RestDTOs;

namespace DynamicDocsWPF.HelperClasses
{
    public class NetworkHelperNew : NetworkBase
    {
        public User User { get; set; }

        
        public NetworkHelperNew(string baseUrl, User user) : base(baseUrl)
        {
            User = user;
        }

        private string BaseUrl { get; }

        #region GET

        public string GetProcessTemplate(string id)
        {
            var request = new RequestGetProcessTemplate()
            {
                Id = id
            };

            var reply = JsonConvert.DeserializeObject<ReplyGetProcessTemplate>(
                GetRequest(User, "ProcessTemplate", JsonConvert.SerializeObject(request))
            );

            return reply.Text;
        }
        
        public ReplyGetDocTemplate GetDocTemplate(string id)
        {
            var request = new RequestGetDocTemplate()
            {
                Id = id
            };

            var reply = JsonConvert.DeserializeObject<ReplyGetDocTemplate>(
                GetRequest(User, "DocumentTemplate", JsonConvert.SerializeObject(request))
            );

            return reply;
        }
        
        public AuthorizationResult CheckAuthorization()
        {
            var reply = JsonConvert.DeserializeObject<ReplyGetAuthenticationResult>(
                    GetRequest(User, "AuthCheck")
                );

            return reply.AuthorizationResult;
        }
        
        public int GetPermissionLevel()
        {
            var request = new RequestGetPermissionLevel()
            {
                Username = User.Email
            };
            
            var reply = JsonConvert.DeserializeObject<ReplyGetPermissionLevel>(
                GetRequest(User, "PermissionLevel", JsonConvert.SerializeObject(request))
            );

            return reply.PermissionLevel;
        }

        public List<DocTemplate> GetDocTemplates()
        {
            var reply = JsonConvert.DeserializeObject<ReplyGetDocTemplateList>(GetRequest(User, "DocTemplateList"));
            return reply.DocTemplates;
        }

        public List<ProcessTemplate> GetProcessTemplates()
        {
            var reply = JsonConvert.DeserializeObject<ReplyGetProcessTemplateList>(GetRequest(User, "ProcessTemplateList"));
            return reply.ProcessTemplates;
        }
        
        public RunningProcess GetProcessById(int id)
        {
            var request = new RequestGetProcessInstance()
            {
                Id = id
            };

            var reply = JsonConvert.DeserializeObject<ReplyGetProcessInstance>(
                GetRequest(User, "ProcessTemplate", JsonConvert.SerializeObject(request))
            );

            return reply.ProcessInstance;
        }

        public List<Entry> GetEntries(int processId)
        {
            var request = new RequestGetEntryList()
            {
                Id = id
            };

            var reply = JsonConvert.DeserializeObject<ReplyGetProcessInstance>(
                GetRequest(User, "ProcessTemplate", JsonConvert.SerializeObject(request))
            );

            return reply.ProcessInstance;
        }

        public RunningProcess GetProcessInstanceById(int id)
        {
            var dataMessage = GetDataMessage(DataType.ProcessInstance, id);

            return dataMessage.DataType == DataType.ProcessInstance
                ? JsonConvert.DeserializeObject<RunningProcess>(dataMessage.Content)
                : null;
        }
        
        public string GetRequest(User user, string url, string message = null)
        {
            try
            {
                HttpWebRequest httpWebRequest;
                if(!string.IsNullOrWhiteSpace(message)) 
                    httpWebRequest = (HttpWebRequest) WebRequest.Create($"{BaseUrl}/{url}/{message}");
                else httpWebRequest = (HttpWebRequest) WebRequest.Create($"{BaseUrl}/{url}");
                httpWebRequest.Method = "GET";

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
   
        #endregion

        #region CREATE

        public static UploadResult CreateUser(string baseUrl, string email, string password)
        {
            var user = new User(email, HashHelper.Hash(password));

            return new NetworkHelper(baseUrl, new User()).PostData(user, DataType.UserAccount, "");
        }

        public UploadResult CreateProcessInstance(string processTemplateId, string ownerId)
        {
            var runningProcess = new RunningProcess
            {
                Declined = false,
                CurrentStep = 0,
                Owner_ID = ownerId,
                Template_ID = processTemplateId
            };

            return PostData(User,DataType.ProcessInstance, JsonConvert.SerializeObject(runningProcess));
        }

        public UploadResult UploadProcessTemplate(string filePath, bool forceOverwrite)
        {
            var process = XmlHelper.ReadXMLFromPath(filePath);
            var fileName = Path.GetFileName(filePath);
            var fileText = File.ReadAllText(filePath);
            return PostFile(new FileMessage(User, FileType.ProcessTemplate, process.Name, fileName, fileText, forceOverwrite));
        }

        public UploadResult UploadDocTemplate(string templateId, string filePath, bool forceOverwrite)
        {
            var fileName = Path.GetFileName(filePath);
            var fileBytes = Encoding.Default.GetString(File.ReadAllBytes(filePath));
            return PostFile(new FileMessage(User, FileType.DocTemplate, templateId, fileName, fileBytes, forceOverwrite));
        }

        public void CreateEntry(Entry entry)
        {
            PostData(User,DataType.Entry, JsonConvert.SerializeObject(entry));
        }

        #endregion

        #region UPDATE

        public UploadResult ApproveProcess(int id)
        {
            return PostData(User,DataType.ProcessUpdate, JsonConvert.SerializeObject(new ProcessUpdate(id, false)));
        }

        public UploadResult DeclineProcess(int id)
        {
            return PostData(User,DataType.ProcessUpdate, JsonConvert.SerializeObject(new ProcessUpdate(id, true)));
        }

        #endregion
    }
}