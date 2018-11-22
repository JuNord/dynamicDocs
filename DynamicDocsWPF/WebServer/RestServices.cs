using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using RestService;

namespace WebServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestService : IRestService
    {
        private const string TEMPLATE_PATH = "./Templates/";
        private const string PROCESS_PATH = "./Processes/";
        
        public FileMessage GetFile(string fileType, string name)
        {
            switch (Enum.Parse(typeof(FileType), fileType))
            {
                case FileType.Process:
                    break;
                case FileType.Template:
                    break;
                default: 
                    throw new ArgumentOutOfRangeException();
                    break;
            }
            return new FileMessage()
            {
                
            };
        }

        public UploadResult PostFile(FileMessage message)
        {
            try
            {
                var path = "";
                
                if (!FileIsValid(message)) return UploadResult.FAILED_FILE_OR_TYPE_INVALID;
                
                switch (message.FileType)
                {
                    case FileType.Process:
                        path = $"{PROCESS_PATH}{message.FileName}"; 
                        break;   
                    case FileType.Template:
                        path = $"{TEMPLATE_PATH}{message.FileName}"; 
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (!File.Exists(path) || message.ForceOverWrite)
                {
                    File.WriteAllBytes(path, Encoding.Default.GetBytes(message.Content));
                }
                else return UploadResult.FAILED_FILEEXISTS;
            }
            catch (Exception)
            {
                return UploadResult.FAILED_OTHER;
            }

            return UploadResult.SUCCESS;
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