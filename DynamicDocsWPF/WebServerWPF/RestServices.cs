using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using MySql.Data.MySqlClient;
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
            FileMessage message = null;
            switch (Enum.Parse(typeof(FileType), fileType))
            {
                case FileType.ProcessTemplate:
                    break;
                case FileType.DocTemplate:
                    break;
                default: 
                    throw new ArgumentOutOfRangeException();
                    
            }

            return message;
        }

        public UploadResult PostData(DataMessage message)
        {
            MainWindow.PostToLog("RECEIVED DATA");
            try
            {
                switch (message.DataType)
                {
                    case DataType.ProcessUpdate:
                        MainWindow.PostToLog("Received a process update.");
                        var update = JsonConvert.DeserializeObject<ProcessUpdate>(message.Content);
                        if (update.Declined)
                        {
                            MainWindow.PostToLog("The process was declined.");
                            _database.DeclineRunningProcess(update.ID);
                        }
                        else
                        {
                            MainWindow.PostToLog("The process was approved.");
                            _database.ApproveRunningProcess(update.ID);
                        }
                        break;
                    case DataType.ProcessInstance:
                        MainWindow.PostToLog("Received request to create a new Process Instance.");
                        var instance = JsonConvert.DeserializeObject<RunningProcess>(message.Content);
                        MainWindow.PostToLog("Registering in Database...");
                        _database.AddRunningProcess(instance);
                        MainWindow.PostToLog("Done");
                        break;
                    case DataType.UserAccount:
                        MainWindow.PostToLog("Received request to register a new user.");
                        var user = JsonConvert.DeserializeObject<User>(message.Content);
                        MainWindow.PostToLog("Registering in Database...");
                        _database.AddUser(user);
                        MainWindow.PostToLog("Done!");
                        break;
                    case DataType.Entry:
                        MainWindow.PostToLog("Received request to register a new user.");
                        var entry = JsonConvert.DeserializeObject<Entry>(message.Content);
                        MainWindow.PostToLog("Registering in Database...");
                        _database.AddEntry(entry);
                        MainWindow.PostToLog("Done!");
                        break;
                    default:
                        MainWindow.PostToLog("UNKNOWN");
                        break;
                }
            }
            catch (Exception e)
            {
                MainWindow.PostToLog(e.ToString());
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

                    switch (message.FileType)
                    {
                        case FileType.ProcessTemplate:
                            MainWindow.PostToLog("Received Processtemplate...");
                            var process = XmlHelper.ReadXMLFromString(message.Content);
                            var processTemplate = new ProcessTemplate()
                            {
                                Process_ID = process.Name,
                                Description = process.Description, 
                                FilePath = path
                            };
                            MainWindow.PostToLog("Registering in Database...");
                            _database.AddProcessTemplate(processTemplate);
                            MainWindow.PostToLog("Done!");
                            break;
                        case FileType.DocTemplate:
                            MainWindow.PostToLog("Received Doctemplate...");
                            var docTemplate = new DocTemplate()
                            {
                                DocTemplate_ID = message.ID,
                                FilePath = path
                            };
                            MainWindow.PostToLog("Registering in Database...");
                            _database.AddDocTemplate(docTemplate);
                            MainWindow.PostToLog("Done!");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    File.WriteAllBytes(path, Encoding.Default.GetBytes(message.Content));
                }
                else return UploadResult.FAILED_FILEEXISTS;
            }
            catch (MySqlException e)
            {
                MainWindow.PostToLog(e.Message);
                if (e.Message.Contains("Duplicate entry"))
                    return UploadResult.FAILED_ID_EXISTS;
                else
                    return UploadResult.FAILED_OTHER;
            }
            catch (Exception e)
            {
                MainWindow.PostToLog(e.GetType().ToString());
                MainWindow.PostToLog(e.StackTrace);
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