using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class NetworkHelper : NetworkBase
    {
        public User User { get; set; }

        
        public NetworkHelper(string baseUrl, User user) : base(baseUrl)
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
        
        public RunningProcess GetProcessInstanceById(int id)
        {
            var request = new RequestGetProcessInstance()
            {
                Id = id
            };

            var reply = JsonConvert.DeserializeObject<ReplyGetProcessInstance>(
                GetRequest(User, "ProcessInstance", JsonConvert.SerializeObject(request))
            );

            return reply.ProcessInstance;
        }

        public List<Entry> GetEntries(int instanceId)
        {
            var request = new RequestGetEntryList()
            {
                InstanceId = instanceId
            };

            var reply = JsonConvert.DeserializeObject<ReplyGetEntryList>(
                GetRequest(User, "ProcessTemplate", JsonConvert.SerializeObject(request))
            );

            return reply.Entries;
        }
        
        
        #endregion

        #region CREATE

        public UploadResult Register()
        {
            var request = new RequestPostUser()
            {
                User = User
            };

            var reply = JsonConvert.DeserializeObject<ReplyPostUser>(
                PostRequest(User, "User", JsonConvert.SerializeObject(request))
            );

            return reply.UploadResult;
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
            var request = new RequestPostProcessInstance()
            {
                RunningProcess = runningProcess
            };
            var reply = JsonConvert.DeserializeObject<ReplyPostUser>(
                 PostRequest(User,"ProcessCreate", JsonConvert.SerializeObject(request))
                );
            return reply.UploadResult;
        }

        public UploadResult UploadProcessTemplate(string filePath, bool forceOverwrite)
        {
            var process = XmlHelper.ReadXMLFromPath(filePath);
            var request = new RequestPostProcessTemplate()
            {
                Id = process.Name,
                Description = process.Description,
                ForceOverWrite = forceOverwrite,
                Text = File.ReadAllText(filePath)
            };
            var reply = JsonConvert.DeserializeObject<ReplyPostProcessTemplate>(
              PostRequest(User, "ProcessTemplate", JsonConvert.SerializeObject(request))
                );
            return reply.UploadResult;
        }

        public UploadResult UploadDocTemplate(string templateId, string filePath, bool forceOverwrite)
        {
            var request = new RequestPostDocTemplate()
            {
                Id = templateId,
                ForceOverWrite = forceOverwrite,
                Content = Encoding.Default.GetString(File.ReadAllBytes(filePath))
            };
            var reply = JsonConvert.DeserializeObject<ReplyPostDocTemplate>(
                PostRequest(User, "DocumentTemplate", JsonConvert.SerializeObject(request))
            );
            return reply.UploadResult;
        }

        public UploadResult CreateEntry(Entry entry)
        {
            var request = new RequestPostEntry()
            {
                Entry = entry
            };
            var reply = JsonConvert.DeserializeObject<ReplyPostEntry>(
                PostRequest(User, "Entry", JsonConvert.SerializeObject(request))
            );
            return reply.UploadResult;
        }

        #endregion

        #region UPDATE

        public UploadResult PostProcessUpdate(int id, bool declined)
        {
            var request = new RequestPostProcessUpdate()
            {
                Id = id,
                Declined = declined
            };
            var reply = JsonConvert.DeserializeObject<ReplyPostProcessUpdate>(
                PostRequest(User, "ProcessUpdate", JsonConvert.SerializeObject(request))
            );
            return reply.UploadResult;
        }

        #endregion
    }
}









































