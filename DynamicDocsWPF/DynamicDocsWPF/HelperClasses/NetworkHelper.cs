using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RestService;
using RestService.Model.Database;

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

        public List<string> GetTemplates()
        {
            return GetList(FileType.DocTemplate);
        }

        public List<string> GetProcesses()
        {
            return GetList(FileType.ProcessTemplate);
        }

        public FileMessage GetTemplateByName(string name)
        {
            return GetFileByName(FileType.DocTemplate, name);
        }

        public FileMessage GetProcessByName(string name)
        {
            return GetFileByName(FileType.ProcessTemplate, name);
        }

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