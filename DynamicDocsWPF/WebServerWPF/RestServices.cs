using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using Newtonsoft.Json;
using RestService;
using WebServer;
using WebServerWPF.Model;

namespace WebServerWPF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestService : IRestService
    {
        private static DatabaseHelper _database = new DatabaseHelper();
        private const string TEMPLATE_PATH = "./Templates/";
        private const string PROCESS_PATH = "./Processes/";
        
        public FileMessage GetFile(string fileType, string name)
        {
            switch (Enum.Parse(typeof(FileType), fileType))
            {
                case FileType.ProcessTemplate:
                    break;
                case FileType.DocTemplate:
                    break;
                default: 
                    throw new ArgumentOutOfRangeException();
                    break;
            }
            return new FileMessage()
            {
                
            };
        }

        public UploadResult PostData(DataMessage message)
        {
            MainWindow.PostToLog("RECEIVED DATA");
            try
            {
                switch (message.DataType)
                {
                    case DataType.ProcessUpdate:
                        MainWindow.PostToLog("IS PROCESS UPDATE");
                        var update = JsonConvert.DeserializeObject<ProcessUpdate>(message.Content);
                        if (update.Accepted)
                        {
                            
                        }
                        break;
                    case DataType.ProcessTemplate:
                        
                        break;
                    default:
                        MainWindow.PostToLog("UNKNOWN");
                        break;
                }
            }
            catch (Exception e)
            {
                MainWindow.PostToLog(e.Message);
                return UploadResult.FAILED_OTHER;
            }

            return UploadResult.SUCCESS;
        }

        public UploadResult PostFile(FileMessage message)
        {
            try
            {                      
                if (!FileIsValid(message)) return UploadResult.FAILED_FILE_OR_TYPE_INVALID;
                var path = GetFiletypeDirectory(message);
                
                if (!File.Exists(path) || message.ForceOverWrite)
                {
                    var dir = Path.GetDirectoryName(path);
                    if (!Directory.Exists(dir)) 
                        Directory.CreateDirectory(dir);
                    
                    MainWindow.PostToLog("DIRECTORY: "+dir);
                    File.WriteAllBytes(path, Encoding.Default.GetBytes(message.Content));

                    switch (message.FileType)
                    {
                        case FileType.ProcessTemplate:
                            var process = XmlHelper.ReadXMLFromString(message.Content);
                            var processTemplate = new ProcessTemplate()
                            {
                                Process_ID = process.Name,
                                Description = process.Description,
                                FilePath = path
                            };
                            _database.AddProcessTemplate(processTemplate);
                            break;
                        case FileType.DocTemplate:
                            
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else return UploadResult.FAILED_FILEEXISTS;
            }
            catch (Exception e)
            {
                MainWindow.PostToLog(e.Message);
                return UploadResult.FAILED_OTHER;
            }

            return UploadResult.SUCCESS;
        }

        private static string GetFiletypeDirectory(FileMessage message)
        {
            switch (message.FileType)
            {
                case FileType.ProcessTemplate: return $"{PROCESS_PATH}{message.FileName}";                   
                case FileType.DocTemplate: return $"{TEMPLATE_PATH}{message.FileName}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public List<string> GetProcessList()
        {
            return new List<string>();
        }

        public List<string> GetTemplateList()
        {
            return new List<string>();
        }

        private static bool FileIsValid(FileMessage message)
        {
            return true;
        }
    }
}