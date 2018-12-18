using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DynamicDocsWPF.Model;
using Newtonsoft.Json;
using RestService;
using RestService.Model;
using RestService.Model.Database;
using RestService.RestDTOs;

namespace DynamicDocsWPF.HelperClasses
{
    public class NetworkHelper : NetworkBase
    {
        public NetworkHelper(string baseUrl, User user) : base(baseUrl)
        {
            User = user;
        }

        public User User { get; set; }

        private string BaseUrl { get; }

        #region GET

        public string GetProcessTemplate(string id)
        {
            var request = new RequestGetProcessTemplate
            {
                Id = id
            };

            var reply = JsonConvert.DeserializeObject<ReplyGetProcessTemplate>(
                GetRequest(User, "ProcessTemplate", JsonConvert.SerializeObject(request))
            );

            return reply?.Text;
        }

        public ReplyGetDocTemplate GetDocTemplate(string id)
        {
            var request = new RequestGetDocTemplate
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
            var response = GetRequest(User, "AuthCheck");
            var reply = JsonConvert.DeserializeObject<ReplyGetAuthenticationResult>(
                response
            );

            return reply?.AuthorizationResult ?? AuthorizationResult.InvalidFormat;
        }

        public int GetPermissionLevel()
        {
            var request = new RequestGetPermissionLevel
            {
                Username = User.Email
            };

            var reply = JsonConvert.DeserializeObject<ReplyGetPermissionLevel>(
                GetRequest(User, "PermissionLevel", JsonConvert.SerializeObject(request))
            );

            return reply?.PermissionLevel ?? -1;
        }

        public List<DocTemplate> GetDocTemplates()
        {
            var reply = JsonConvert.DeserializeObject<ReplyGetDocTemplateList>(GetRequest(User, "DocTemplateList"));
            return reply?.DocTemplates;
        }

        public List<ProcessTemplate> GetProcessTemplates()
        {
            var reply = JsonConvert.DeserializeObject<ReplyGetProcessTemplateList>(GetRequest(User,
                "ProcessTemplateList"));
            return reply?.ProcessTemplates;
        }

        public List<PendingInstance> GetResponsibilities()
        {
            var reply =
                JsonConvert.DeserializeObject<ReplyGetResponsibilityList>(GetRequest(User, "ResponsibilityList"));
            return reply?.Responsibilities;
        }

        public ProcessInstance GetProcessInstanceById(int id)
        {
            var request = new RequestGetProcessInstance
            {
                Id = id
            };

            var reply = JsonConvert.DeserializeObject<ReplyGetProcessInstance>(
                GetRequest(User, "ProcessInstance", JsonConvert.SerializeObject(request))
            );

            return reply?.ProcessInstance;
        }

        public List<ProcessInstance> GetProcessInstances()
        {
            var reply = JsonConvert.DeserializeObject<ReplyGetProcessInstanceList>(
                GetRequest(User, "ProcessInstanceList")
            );

            return reply?.ProcessInstances;
        }

        public List<User> GetUsers()
        {
            var reply = JsonConvert.DeserializeObject<ReplyGetUserList>(
                GetRequest(User, "UserList")
            );

            return reply?.Users;
        }

        public List<Entry> GetEntries(int instanceId)
        {
            var request = new RequestGetEntryList
            {
                InstanceId = instanceId
            };

            var reply = JsonConvert.DeserializeObject<ReplyGetEntryList>(
                GetRequest(User, "Entries", JsonConvert.SerializeObject(request))
            );

            return reply?.Entries;
        }

        #endregion

        #region CREATE

        public UploadResult Register()
        {
            var request = new RequestPostUser
            {
                Email = User.Email,
                Password = User.Password
            };

            var reply = JsonConvert.DeserializeObject<ReplyPostUser>(
                PostRequest(null, "User", JsonConvert.SerializeObject(request))
            );

            return reply?.UploadResult ?? UploadResult.FailedOther;
        }

        public ReplyPostProcessInstance CreateProcessInstance(string processTemplateId, string ownerId, string subject)
        {
            var processInstance = new ProcessInstance
            {
                Declined = false,
                CurrentStepIndex = 0,
                OwnerId = ownerId,
                TemplateId = processTemplateId,
                Subject = subject,
                Created = DateTime.Now.ToShortDateString()
            };
            var request = new RequestPostProcessInstance
            {
                ProcessInstance = processInstance
            };
            var reply = JsonConvert.DeserializeObject<ReplyPostProcessInstance>(
                PostRequest(User, "ProcessCreate", JsonConvert.SerializeObject(request))
            );
            return reply;
        }

        public UploadResult UploadProcessTemplate(string filePath, bool forceOverwrite)
        {
            var process = XmlHelper.ReadXmlFromPath(filePath);
            var request = new RequestPostProcessTemplate
            {
                Id = process.Name,
                Description = process.Description,
                ForceOverWrite = forceOverwrite,
                Text = File.ReadAllText(filePath)
            };
            var reply = JsonConvert.DeserializeObject<ReplyPostProcessTemplate>(
                PostRequest(User, "ProcessTemplate", JsonConvert.SerializeObject(request))
            );
            return reply?.UploadResult ?? UploadResult.FailedOther;
        }

        public UploadResult UploadDocTemplate(string templateId, string filePath, bool forceOverwrite)
        {
            var request = new RequestPostDocTemplate
            {
                Id = templateId,
                ForceOverWrite = forceOverwrite,
                Content = Encoding.Default.GetString(File.ReadAllBytes(filePath))
            };
            var reply = JsonConvert.DeserializeObject<ReplyPostDocTemplate>(
                PostRequest(User, "DocumentTemplate", JsonConvert.SerializeObject(request))
            );
            return reply?.UploadResult ?? UploadResult.FailedOther;
        }

        public UploadResult CreateEntry(Entry entry)
        {
            var request = new RequestPostEntry
            {
                Entry = entry
            };
            var reply = JsonConvert.DeserializeObject<ReplyPostEntry>(
                PostRequest(User, "Entry", JsonConvert.SerializeObject(request))
            );
            return reply?.UploadResult ?? UploadResult.FailedOther;
        }

        #endregion

        #region UPDATE

        public UploadResult PostEntryUpdate(Entry entry)
        {
            var request = new RequestPostEntryUpdate
            {
                Entry = entry
            };
            var reply = JsonConvert.DeserializeObject<ReplyPostEntryUpdate>(
                PostRequest(User, "EntryUpdate", JsonConvert.SerializeObject(request))
            );
            return reply?.UploadResult ?? UploadResult.FailedOther;
        }

        /// <summary>
        /// Sends a process update, setting if it was declined and if input is locked after this step.
        /// </summary>
        /// <param name="id">The id of the instance to be updated</param>
        /// <param name="declined">Is the instance declined now?</param>
        /// <param name="locks">Locks input forms from now.</param>
        /// <returns></returns>
        public UploadResult PostProcessUpdate(int id, bool declined, bool locks)
        {
            var request = new RequestPostProcessUpdate
            {
                Id = id,
                Declined = declined,
                Locks = locks
            };
            var reply = JsonConvert.DeserializeObject<ReplyPostProcessUpdate>(
                PostRequest(User, "ProcessUpdate", JsonConvert.SerializeObject(request))
            );
            return reply?.UploadResult ?? UploadResult.FailedOther;
        }

        public UploadResult PostPermissionChange(string email, int permissionLevel)
        {
            var request = new RequestPermissionChange
            {
                Email = email,
                PermissionLevel = permissionLevel
            };
            var reply = JsonConvert.DeserializeObject<ReplyPostProcessUpdate>(
                PostRequest(User, "Level", JsonConvert.SerializeObject(request))
            );
            return reply?.UploadResult ?? UploadResult.FailedOther;
        }

        #endregion
    }
}