using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;
using System.Xml;
using DynamicDocsWPF.Model;
using MySql.Data.MySqlClient;
using RestService;
using RestService.Model.Database;
using WebServer;
using WebServerWPF.RestDots;
using WebServerWPF.RestDTOs;

namespace WebServerWPF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestServiceNew : IRestServiceNew
    {
        private const string DocPath = "./Docs/";
        private const string ProcessPath = "./Processes/";
        private static readonly DatabaseHelper Database = new DatabaseHelper();

        public ReplyGetPermissionLevel GetPermissionLevel(User user)
        {
            if (IsAuthorized(user) == AuthorizationResult.AUTHORIZED)
            {
                return new ReplyGetPermissionLevel()
                {
                    PermissionLevel = Database.GetUserByMail(user.Email).PermissionLevel
                };
            }

            return new ReplyGetPermissionLevel()
            {
                PermissionLevel = -1
            };
        } 

        public AuthorizationResult IsAuthorized(User user)
        {
            var dbUser = Database.GetUserByMail(user.Email);
            
            if (dbUser == null) return AuthorizationResult.INVALID_LOGIN;
            
            return HashHelper.CheckHash(user.Password, dbUser.Password) ? AuthorizationResult.AUTHORIZED : AuthorizationResult.INVALID_LOGIN;
        }

        public User GetAuthUser()
        {
            var httpContext = HttpContext.Current;

            var authHeader = httpContext.Request.Headers["Authorization"];

            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var encoding = Encoding.GetEncoding("iso-8859-1");
                var usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                var separatorIndex = usernamePassword.IndexOf(':');

                var username = usernamePassword.Substring(0, separatorIndex);
                var password = usernamePassword.Substring(separatorIndex + 1);

                return new User(username, password);
            }
            else
            {
                return null;
            }
        }

        private AuthorizationResult IsPermitted(User user, int permissionLevel)
        {
            if (IsAuthorized(user) != AuthorizationResult.AUTHORIZED) return AuthorizationResult.INVALID_LOGIN;
            
            var dbUser = Database.GetUserByMail(user.Email);
            
            if (dbUser == null) return AuthorizationResult.NO_PERMISSION;
            if (dbUser.PermissionLevel < permissionLevel) return AuthorizationResult.NO_PERMISSION;

            return AuthorizationResult.PERMITTED;
        }

        public ReplyGetProcessTemplate GetProcessTemplate(RequestGetProcessTemplate request)
        {
            try
            {
                if (IsPermitted(GetAuthUser(), 2) == AuthorizationResult.PERMITTED)
                {
                    var template = Database.GetProcessTemplateById(request.Id);
                    var text = File.ReadAllText(template.FilePath);
                    var reply = new ReplyGetProcessTemplate()
                    {
                        Id = template.Id,
                        Description = template.Description,
                        Text = text
                    };

                    return reply;
                }
            }
            catch (Exception e)
            {
                PrintException(e);
            }

            return null;
        }

        public ReplyGetDocTemplate GetDocTemplate(RequestGetDocTemplate request)
        {
            try
            {
                if (IsPermitted(GetAuthUser(), 2) == AuthorizationResult.PERMITTED)
                {
                    var template = Database.GetDocTemplateById(request.Id);
                    var reply = new ReplyGetDocTemplate()
                    {
                        Id = template.Id,
                        Content = Encoding.Default.GetString(File.ReadAllBytes(template.FilePath))
                    };

                    return reply;
                }
            }
            catch (Exception e)
            {
                PrintException(e);
            }

            return null;
        }



        public ReplyGetProcessTemplateList GetProcessTemplateList()
        {
            try
            {
                if (IsPermitted(GetAuthUser(), 2) == AuthorizationResult.PERMITTED)
                {
                    var reply = new ReplyGetProcessTemplateList()
                    {
                        ProcessTemplates = Database.GetProcessTemplates()
                    };

                    return reply;
                }
            }
            catch (Exception e)
            {
                PrintException(e);
            }

            return null;
        }

        public ReplyGetDocTemplateList GetDocTemplateList()
        {
            try
            {
                if (IsPermitted(GetAuthUser(), 2) == AuthorizationResult.PERMITTED)
                {
                    var reply = new ReplyGetDocTemplateList()
                    {
                        DocTemplates = Database.GetDocTemplates()
                    };

                    return reply;
                }
            }
            catch (Exception e)
            {
                PrintException(e);
            }

            return null;
        }

        public ReplyGetAuthenticationResult CheckAuthentication()
        {
            return new ReplyGetAuthenticationResult()
            {
                AuthorizationResult = IsAuthorized(GetAuthUser())
            };
        }

        public ReplyGetEntryList GetEntryList(RequestGetEntryList request)
        {
            try
            {
                if (IsPermitted(GetAuthUser(), 2) == AuthorizationResult.PERMITTED)
                {
                    var reply = new ReplyGetEntryList()
                    {
                        Entries = Database.GetEntry(request.InstanceId)
                    };

                    return reply;
                }
            }
            catch (Exception e)
            {
                PrintException(e);
            }

            return null;
        }

        public ReplyPostProcessTemplate PostProcessTemplate(RequestPostProcessTemplate request)
        {
            var reply = new ReplyPostProcessTemplate();
            try
            {
                var auth = IsPermitted(GetAuthUser(),2);
                switch (auth)
                {
                    case AuthorizationResult.PERMITTED:
                    {
                        var path = $"{ProcessPath}{request.Id}.xml";

                        if (!File.Exists(path) || request.ForceOverWrite)
                        {
                            var dir = Path.GetDirectoryName(path);
                            if (!Directory.Exists(dir))
                                Directory.CreateDirectory(dir);
                            
                            MainWindow.PostToLog("Received Processfile...");
                            var process = XmlHelper.ReadXMLFromString(request.Text);
                            var processTemplate = new ProcessTemplate
                            {
                                Id = process.Name,
                                Description = process.Description,
                                FilePath = path
                            };
                            MainWindow.PostToLog("Registering in Database...");
                            Database.AddProcessTemplate(processTemplate);
                            MainWindow.PostToLog("Done!");

                            File.WriteAllText(path, request.Text);
                        }
                        else
                        {
                            reply.UploadResult = UploadResult.FAILED_FILEEXISTS;
                        }

                        break;
                    }
                    case AuthorizationResult.AUTHORIZED:
                        reply.UploadResult = UploadResult.NO_PERMISSION;
                        break;
                    default:
                        reply.UploadResult = UploadResult.INVALID_LOGIN;
                        break;
                }         
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Duplicate entry"))
                {
                    reply.UploadResult = UploadResult.FAILED_ID_EXISTS;
                }
                PrintException(e);
            }
            catch (XmlException e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FAILED_FILE_OR_TYPE_INVALID;
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FAILED_OTHER;
            }

            reply.UploadResult = UploadResult.SUCCESS;
            return reply;
        }

        public ReplyPostDocTemplate PostDocTemplate(RequestPostDocTemplate request)
        {
            var reply = new ReplyPostDocTemplate();
            try
            {
                var auth = IsPermitted(GetAuthUser(),2);
                switch (auth)
                {
                    case AuthorizationResult.PERMITTED:
                    {
                        var path = $"{DocPath}{request.Id}.docx";

                        if (!File.Exists(path) || request.ForceOverWrite)
                        {
                            var dir = Path.GetDirectoryName(path);
                            if (!Directory.Exists(dir))
                                Directory.CreateDirectory(dir);
                            
                            MainWindow.PostToLog("Received Document...");
                            var docTemplate = new DocTemplate()
                            {
                                Id = request.Id,
                                FilePath = path
                            };
                            MainWindow.PostToLog("Registering in Database...");
                            Database.AddDocTemplate(docTemplate);
                            MainWindow.PostToLog("Done!");
                                 
                            File.WriteAllBytes(path, Encoding.Default.GetBytes(request.Content));
                        }
                        else
                        {
                            reply.UploadResult = UploadResult.FAILED_FILEEXISTS;
                        }

                        break;
                    }
                    case AuthorizationResult.AUTHORIZED:
                        reply.UploadResult = UploadResult.NO_PERMISSION;
                        break;
                    default:
                        reply.UploadResult = UploadResult.INVALID_LOGIN;
                        break;
                }         
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Duplicate entry"))
                {
                    reply.UploadResult = UploadResult.FAILED_ID_EXISTS;
                }
                PrintException(e);
            }
            catch (XmlException e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FAILED_FILE_OR_TYPE_INVALID;
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FAILED_OTHER;
            }

            reply.UploadResult = UploadResult.SUCCESS;
            return reply;
        }

        public ReplyPostProcessUpdate PostProcessUpdate(RequestPostProcessUpdate request)
        {
            var reply = new ReplyPostProcessUpdate();
            try
            {
                var auth = IsPermitted(GetAuthUser(),2);
                switch (auth)
                {
                    case AuthorizationResult.PERMITTED:
                        MainWindow.PostToLog("Received a process update.");
                        if (request.Declined)
                        {
                            MainWindow.PostToLog("The process was declined.");
                            Database.DeclineRunningProcess(request.Id);
                        }
                        else
                        {
                            MainWindow.PostToLog("The process was approved.");
                            Database.ApproveRunningProcess(request.Id);
                        }
                        break;
                    case AuthorizationResult.NO_PERMISSION: 
                        reply.UploadResult = UploadResult.NO_PERMISSION;                            
                        break;                      
                    case AuthorizationResult.INVALID_LOGIN:
                        reply.UploadResult = UploadResult.INVALID_LOGIN;
                        break;
                }
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Duplicate entry"))
                {
                    reply.UploadResult = UploadResult.FAILED_ID_EXISTS;
                }
                PrintException(e);
            }
            catch (XmlException e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FAILED_FILE_OR_TYPE_INVALID;
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FAILED_OTHER;
            }

            reply.UploadResult = UploadResult.SUCCESS;
            return reply;
        }

        public ReplyPostProcessInstance PostProcessInstance(RequestPostProcessInstance request)
        {
            var reply = new ReplyPostProcessInstance();
            try
            {
                var auth = IsPermitted(GetAuthUser(),2);
                switch (auth)
                {
                    case AuthorizationResult.PERMITTED:
                        MainWindow.PostToLog("Received request to create a new Process Instance.");
                        MainWindow.PostToLog("Registering in Database...");
                        Database.AddRunningProcess(request.RunningProcess);
                        MainWindow.PostToLog("Done");
                        break;
                    case AuthorizationResult.NO_PERMISSION: 
                        reply.UploadResult = UploadResult.NO_PERMISSION;                            
                        break;                      
                    case AuthorizationResult.INVALID_LOGIN:
                        reply.UploadResult = UploadResult.INVALID_LOGIN;
                        break;
                }
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Duplicate entry"))
                {
                    reply.UploadResult = UploadResult.FAILED_ID_EXISTS;
                }
                PrintException(e);
            }
            catch (XmlException e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FAILED_FILE_OR_TYPE_INVALID;
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FAILED_OTHER;
            }

            reply.UploadResult = UploadResult.SUCCESS;
            return reply;
        }

        public ReplyPostEntry PostEntry(RequestPostEntry request)
        {
            var reply = new ReplyPostEntry();
            try
            {
                var auth = IsPermitted(GetAuthUser(),2);
                switch (auth)
                {
                    case AuthorizationResult.PERMITTED:
                        MainWindow.PostToLog("Received request to add a new Entry.");
                        var entry = request.Entry;
                        MainWindow.PostToLog("Registering in Database...");
                        Database.AddEntry(entry);
                        MainWindow.PostToLog("Done!");
                        break;
                    case AuthorizationResult.NO_PERMISSION: 
                        reply.UploadResult = UploadResult.NO_PERMISSION;                            
                        break;                      
                    case AuthorizationResult.INVALID_LOGIN:
                        reply.UploadResult = UploadResult.INVALID_LOGIN;
                        break;
                }
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Duplicate entry"))
                {
                    reply.UploadResult = UploadResult.FAILED_ID_EXISTS;
                }
                PrintException(e);
            }
            catch (XmlException e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FAILED_FILE_OR_TYPE_INVALID;
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FAILED_OTHER;
            }

            reply.UploadResult = UploadResult.SUCCESS;
            return reply;
        }
        
        public ReplyPostUser Register(RequestPostUser request)
        {
            var reply = new ReplyPostUser();
            try
            {
                var auth = IsPermitted(GetAuthUser(),2);
                switch (auth)
                {
                    case AuthorizationResult.PERMITTED:
                        MainWindow.PostToLog("Received request to register a new user.");
                        MainWindow.PostToLog("Registering in Database...");          
                        Database.AddUser(request.User);
                        MainWindow.PostToLog("Done!");
                        break;
                    case AuthorizationResult.NO_PERMISSION: 
                        reply.UploadResult = UploadResult.NO_PERMISSION;                            
                        break;                      
                    case AuthorizationResult.INVALID_LOGIN:
                        reply.UploadResult = UploadResult.INVALID_LOGIN;
                        break;
                }
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Duplicate entry"))
                {
                    MainWindow.PostToLog("User already exists.");
                    reply.UploadResult = UploadResult.USER_EXISTS;
                }
                PrintException(e);
            }
            catch (XmlException e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FAILED_FILE_OR_TYPE_INVALID;
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FAILED_OTHER;
            }

            reply.UploadResult = UploadResult.SUCCESS;
            return reply;
        }

        private static void PrintException(Exception e)
        {
            MainWindow.PostToLog("\n\n");
            MainWindow.PostToLog("========================================");
            MainWindow.PostToLog(e.GetType().ToString());
            MainWindow.PostToLog(e.Message);
            MainWindow.PostToLog(e.StackTrace);
            MainWindow.PostToLog("========================================\n\n");
        }
    }
}