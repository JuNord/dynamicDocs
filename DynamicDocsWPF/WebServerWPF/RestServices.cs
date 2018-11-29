using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;
using DynamicDocsWPF.Model;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RestService;
using RestService.Model.Database;
using WebServer;

namespace WebServerWPF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestService : IRestService
    {
        private const string TemplatePath = "./Templates/";
        private const string ProcessPath = "./Processes/";
        private static readonly DatabaseHelper Database = new DatabaseHelper();

        public AuthorizationResult CheckAuth(User user)
        {
            return IsAuthorized(user);
        }

        public int GetPermissionLevel(User user)
        {
            if (IsAuthorized(user) == AuthorizationResult.AUTHORIZED)
            {
                return Database.GetUserByMail(user.Email).PermissionLevel;
            }
            else return -1;
        }

        public FileMessage GetFile(DataMessage message)
        {
            FileMessage fileMessage = null;
            if (message.DataType == DataType.FileRequest)
            {
                var request = JsonConvert.DeserializeObject<FileRequest>(message.Content);
                switch (message.DataType)
                {
                    case DataType.ProcessTemplate:
                        var processTemplate = Database.GetProcessTemplateById(request.Id);
                        
                        fileMessage = new FileMessage()
                        {
                            ID = request.Id,                 
                            FileType = request.FileType,
                            Content = File.ReadAllText(processTemplate.FilePath)
                        };
                        break;
                    case DataType.DocTemplate:
                        var docTemplate = Database.GetDocTemplateById(request.Id);
                        
                        fileMessage = new FileMessage()
                        {
                            ID = request.Id,                 
                            FileType = request.FileType,
                            Content = File.ReadAllText(docTemplate.FilePath)
                        };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return fileMessage;
        }

        private AuthorizationResult IsAuthorized(User user)
        {
            var dbUser = Database.GetUserByMail(user.Email);
            if (dbUser != null)
            {
                if (HashHelper.CheckHash(user.Password_Hash, dbUser.Password_Hash))
                {
                    return AuthorizationResult.AUTHORIZED;
                }
            }
            return AuthorizationResult.INVALID_LOGIN;
        }

        private AuthorizationResult IsPermitted(User user, int permissionLevel)
        {
            if (IsAuthorized(user) == AuthorizationResult.AUTHORIZED)
            {
                var dbUser = Database.GetUserByMail(user.Email);
                if (dbUser != null)
                {
                    if (HashHelper.CheckHash(user.Password_Hash, dbUser.Password_Hash))
                    {
                        return AuthorizationResult.PERMITTED;
                    }
                }

                return AuthorizationResult.NO_PERMISSION;
            }
            return AuthorizationResult.INVALID_LOGIN;
        }
        
        public UploadResult PostData(DataMessage message)
        {
            try
            {
                AuthorizationResult auth = AuthorizationResult.INVALID_LOGIN;
                if(message.DataType != DataType.UserAccount) auth = IsAuthorized(message.User);
                
                if (auth == AuthorizationResult.AUTHORIZED || message.DataType == DataType.UserAccount)
                {
                    switch (message.DataType)
                    {
                        case DataType.ProcessUpdate:
                            if (IsPermitted(message.User, 1) == AuthorizationResult.PERMITTED)
                            {
                                MainWindow.PostToLog("Received a process update.");
                                var update = JsonConvert.DeserializeObject<ProcessUpdate>(message.Content);
                                if (update.Declined)
                                {
                                    MainWindow.PostToLog("The process was declined.");
                                    Database.DeclineRunningProcess(update.ID);
                                }
                                else
                                {
                                    MainWindow.PostToLog("The process was approved.");
                                    Database.ApproveRunningProcess(update.ID);
                                }
                            }
                            else return UploadResult.NO_PERMISSION;

                            break;
                        case DataType.ProcessInstance:
                            if (IsPermitted(message.User, 1) == AuthorizationResult.PERMITTED)
                            {
                                MainWindow.PostToLog("Received request to create a new Process Instance.");
                                var instance = JsonConvert.DeserializeObject<RunningProcess>(message.Content);
                                MainWindow.PostToLog("Registering in Database...");
                                Database.AddRunningProcess(instance);
                                MainWindow.PostToLog("Done");
                            }
                            else return UploadResult.NO_PERMISSION;

                            break;
                        case DataType.UserAccount:    
                            MainWindow.PostToLog("Received request to register a new user.");
                            MainWindow.PostToLog("Registering in Database...");
                            if (Database.GetUserByMail(message.User.Email) == null)
                            {
                                Database.AddUser(message.User);
                            }
                            else
                            {
                                MainWindow.PostToLog("User already exists.");
                                return UploadResult.USER_EXISTS;
                            }
                            MainWindow.PostToLog("Done!");                
                            break;
                        case DataType.Entry:
                            if (IsPermitted(message.User, 1) == AuthorizationResult.PERMITTED)
                            {
                                MainWindow.PostToLog("Received request to register a new user.");
                                var entry = JsonConvert.DeserializeObject<Entry>(message.Content);
                                MainWindow.PostToLog("Registering in Database...");
                                Database.AddEntry(entry);
                                MainWindow.PostToLog("Done!");
                            }
                            else return UploadResult.NO_PERMISSION;

                            break;
                        default:
                            MainWindow.PostToLog("UNKNOWN");
                            break;
                    }
                }
                else
                {
                    return UploadResult.INVALID_LOGIN;
                }
            }
            catch (MySqlException e)
            {
                var result = UploadResult.FAILED_OTHER;
                MainWindow.PostToLog("\n\n");
                MainWindow.PostToLog("========================================");
                MainWindow.PostToLog(e.Message);
                if (e.Message.Contains("Duplicate entry"))
                {
                    result = UploadResult.FAILED_ID_EXISTS;
                }
                else
                {
                    MainWindow.PostToLog(e.GetType().ToString());
                    MainWindow.PostToLog(e.StackTrace);
                }

                MainWindow.PostToLog("========================================\n\n");
                return result;
            }
            catch (XmlException e)
            {
                MainWindow.PostToLog("\n\n");
                MainWindow.PostToLog("========================================");
                MainWindow.PostToLog(e.GetType().ToString());
                MainWindow.PostToLog(e.StackTrace);
                MainWindow.PostToLog(e.Message);
                MainWindow.PostToLog("========================================\n\n");

                return UploadResult.FAILED_FILE_OR_TYPE_INVALID;
            }
            catch (Exception e)
            {
                MainWindow.PostToLog("\n\n");
                MainWindow.PostToLog("========================================");
                MainWindow.PostToLog(e.GetType().ToString());
                MainWindow.PostToLog(e.StackTrace);
                MainWindow.PostToLog(e.Message);
                MainWindow.PostToLog("========================================\n\n");
                return UploadResult.FAILED_OTHER;
            }

            return UploadResult.SUCCESS;
        }

        public UploadResult PostFile(FileMessage message)
        {
            try
            {
                var auth = IsAuthorized(message.User);
                if (auth == AuthorizationResult.AUTHORIZED)
                {
                    if (IsPermitted(message.User, 2) == AuthorizationResult.PERMITTED)
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
                                    MainWindow.PostToLog("Received Processfile...");
                                    var process = XmlHelper.ReadXMLFromString(message.Content);
                                    var processTemplate = new ProcessTemplate
                                    {
                                        Id = process.Name,
                                        Description = process.Description,
                                        FilePath = path
                                    };
                                    MainWindow.PostToLog("Registering in Database...");
                                    Database.AddProcessTemplate(processTemplate);
                                    MainWindow.PostToLog("Done!");
                                    break;
                                case FileType.DocTemplate:
                                    MainWindow.PostToLog("Received Doctemplate...");
                                    var docTemplate = new DocTemplate
                                    {
                                        Id = message.ID,
                                        FilePath = path
                                    };
                                    MainWindow.PostToLog("Registering in Database...");
                                    Database.AddDocTemplate(docTemplate);
                                    MainWindow.PostToLog("Done!");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            File.WriteAllBytes(path, Encoding.Default.GetBytes(message.Content));
                        }
                        else
                        {
                            return UploadResult.FAILED_FILEEXISTS;
                        }
                    }
                    else return UploadResult.NO_PERMISSION;
                }
                else return UploadResult.INVALID_LOGIN;
            }
            catch (MySqlException e)
            {
                var result = UploadResult.FAILED_OTHER;
                MainWindow.PostToLog("\n\n");
                MainWindow.PostToLog("========================================");
                MainWindow.PostToLog(e.Message);
                if (e.Message.Contains("Duplicate entry"))
                {
                    result = UploadResult.FAILED_ID_EXISTS;
                }
                else
                {
                    MainWindow.PostToLog(e.GetType().ToString());
                    MainWindow.PostToLog(e.StackTrace);
                }

                MainWindow.PostToLog("========================================\n\n");
                return result;
            }
            catch (XmlException e)
            {
                MainWindow.PostToLog("\n\n");
                MainWindow.PostToLog("========================================");
                MainWindow.PostToLog(e.GetType().ToString());
                MainWindow.PostToLog(e.StackTrace);
                MainWindow.PostToLog(e.Message);
                MainWindow.PostToLog("========================================\n\n");

                return UploadResult.FAILED_FILE_OR_TYPE_INVALID;
            }
            catch (Exception e)
            {
                MainWindow.PostToLog("\n\n");
                MainWindow.PostToLog("========================================");
                MainWindow.PostToLog(e.GetType().ToString());
                MainWindow.PostToLog(e.StackTrace);
                MainWindow.PostToLog(e.Message);
                MainWindow.PostToLog("========================================\n\n");
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

        public DataMessage GetDataList(DataMessage message)
        {
            var outputMessage = new DataMessage()
            {
                DataType = message.DataType,

            };
            switch (message.DataType)
            {
                case DataType.Entry:
                    break;
                case DataType.ProcessInstance:
                    break;
                case DataType.ProcessUpdate:
                    break;
                case DataType.ProcessTemplate:
                    outputMessage.Content = JsonConvert.SerializeObject(Database.GetProcessTemplates());
                    break;
                case DataType.UserAccount:
                    break;
                case DataType.AuthorizationResult:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message.DataType), message.DataType, null);
            }

            return outputMessage;
        }

        private static string GetFiletypeDirectory(FileMessage message)
        {
            switch (message.FileType)
            {
                case FileType.ProcessTemplate: return $"{ProcessPath}{message.FileName}";
                case FileType.DocTemplate: return $"{TemplatePath}{message.FileName}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool FileIsValid(FileMessage message)
        {
            return true;
        }
    }
}