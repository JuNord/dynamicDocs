using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using RestService;
using WebServerWPF.Model;
using Entry = WebServerWPF.Model.Entry;

namespace DynamicDocsWPF.HelperClasses
{
    public class NetworkHelper : NetworkBase
    {
        private string BaseUrl { get; }

        public NetworkHelper(string baseUrl) : base(baseUrl)
        {
        }
       
        #region GET
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

        public RunningProcess GetProcessInstanceById(int id)
        {
            var dataMessage = GetDataMessage(DataType.ProcessInstance, id);

            return dataMessage.DataType == DataType.ProcessInstance
                ? JsonConvert.DeserializeObject<RunningProcess>(dataMessage.Content)
                : null;
        }
        #endregion
        
        #region CREATE
        public UploadResult CreateUser(string email, string password)
        {
            var user = new User(email, HashHelper.Hash(password));
            
            return PostData(DataType.UserAccount, JsonConvert.SerializeObject(user));
        }
        
        public UploadResult CreateProcessInstance(string processTemplateId, string ownerId)
        {
            var runningProcess = new RunningProcess()
            {
                Declined = false,
                CurrentStep = 0,
                Owner_ID = ownerId,
                ProcessTemplate_ID = processTemplateId
            };
            
            return PostData(DataType.ProcessInstance,JsonConvert.SerializeObject(runningProcess));
        }
        
        public UploadResult UploadProcessTemplate(string filePath, bool forceOverwrite)
        {
            var process = XmlHelper.ReadXMLFromPath(filePath);
            var fileName = Path.GetFileName(filePath);
            var fileText = File.ReadAllText(filePath);
            return PostFile(new FileMessage(FileType.ProcessTemplate, process.Name, fileName, fileText, forceOverwrite));
        }
        
        public UploadResult UploadDocTemplate(string templateId, string filePath, bool forceOverwrite)
        {
            var fileName = Path.GetFileName(filePath);
            var fileBytes = Encoding.Default.GetString(File.ReadAllBytes(filePath));
            return PostFile(new FileMessage(FileType.DocTemplate, templateId, fileName, fileBytes, forceOverwrite));              
        }

        public void CreateEntry(Entry entry)
        {
            PostData(DataType.Entry, JsonConvert.SerializeObject(entry));
        }
        #endregion
        
        #region UPDATE
        public UploadResult ApproveProcess(int id)
        {
            return PostData(DataType.ProcessUpdate, JsonConvert.SerializeObject(new ProcessUpdate(id, false)));   
        }
        
        public UploadResult DeclineProcess(int id)
        {
            return PostData(DataType.ProcessUpdate, JsonConvert.SerializeObject(new ProcessUpdate(id, true))); 
        }
        
        #endregion
    }
}