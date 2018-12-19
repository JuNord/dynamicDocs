using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RestService;
using RestService.Model;
using RestService.Model.Database;
using RestService.RestDTOs;
using WebServer;

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

        public ReplyGetPermissionLevel GetPermissionLevel(string message)
        {
            var request = JsonConvert.DeserializeObject<RequestGetPermissionLevel>(message);
            if (IsPermitted(GetAuthUser(), 3) == AuthorizationResult.Permitted ||
                IsAuthorized(GetAuthUser()) == AuthorizationResult.Authorized &&
                GetAuthUser().Email.Equals(request.Username))
                return new ReplyGetPermissionLevel
                {
                    PermissionLevel = Database.GetUserByMail(request.Username).PermissionLevel
                };

            return new ReplyGetPermissionLevel
            {
                PermissionLevel = -1
            };
        }

        public ReplyGetProcessInstance GetProcessInstance(string message)
        {
            var request = JsonConvert.DeserializeObject<RequestGetProcessInstance>(message);

            try
            {
                if (IsPermitted(GetAuthUser(), 1) == AuthorizationResult.Permitted)
                {
                    var reply = new ReplyGetProcessInstance
                    {
                        ProcessInstance = Database.GetInstanceById(request.Id)
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

        public ReplyGetProcessTemplate GetProcessTemplate(string message)
        {
            var request = JsonConvert.DeserializeObject<RequestGetProcessTemplate>(message);

            try
            {
                if (IsPermitted(GetAuthUser(), 1) == AuthorizationResult.Permitted)
                {
                    var template = Database.GetProcessTemplateById(request.Id);
                    var text = File.ReadAllText(template.FilePath);
                    var reply = new ReplyGetProcessTemplate
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

        public ReplyGetDocTemplate GetDocTemplate(string message)
        {
            var request = JsonConvert.DeserializeObject<RequestGetDocTemplate>(message);

            try
            {
                if (IsPermitted(GetAuthUser(), 1) == AuthorizationResult.Permitted)
                {
                    var template = Database.GetDocTemplateById(request.Id);
                    var reply = new ReplyGetDocTemplate
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
                if (IsPermitted(GetAuthUser(), 1) == AuthorizationResult.Permitted)
                {
                    var reply = new ReplyGetProcessTemplateList
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
                if (IsPermitted(GetAuthUser(), 1) == AuthorizationResult.Permitted)
                {
                    var reply = new ReplyGetDocTemplateList
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

        public ReplyGetRoles GetRoleList()
        {
            try
            {
                if (IsPermitted(GetAuthUser(), 3) == AuthorizationResult.Permitted)
                {
                    var reply = new ReplyGetRoles
                    {
                        Roles = Database.GetRoles()
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

        public ReplyGetProcessInstanceList GetProcessInstanceList()
        {
            try
            {
                if (IsPermitted(GetAuthUser(), 1) == AuthorizationResult.Permitted)
                {
                    var reply = new ReplyGetProcessInstanceList
                    {
                        ProcessInstances = Database.GetProcessInstances(GetAuthUser())
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

        public ReplyGetArchivedInstanceList GetArchivedInstanceList()
        {
            try
            {
                if (IsPermitted(GetAuthUser(), 1) == AuthorizationResult.Permitted)
                {
                    var reply = new ReplyGetArchivedInstanceList
                    {
                        Instances = Database.GetArchivePermissions(GetAuthUser())
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

        public ReplyGetResponsibilityList GetResponsibilityList()
        {
            try
            {
                if (IsPermitted(GetAuthUser(), 1) == AuthorizationResult.Permitted)
                {
                    var reply = new ReplyGetResponsibilityList
                    {
                        Responsibilities = Database.GetPending(GetAuthUser())
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

        public ReplyGetUserList GetUserList()
        {
            try
            {
                if (IsPermitted(GetAuthUser(), 3) == AuthorizationResult.Permitted)
                {
                    var users = Database.GetUsers();
                    var safeList = new List<User>();

                    foreach (var user in users)
                        safeList.Add(new User
                        {
                            Email = user.Email,
                            PermissionLevel = user.PermissionLevel
                        });

                    var reply = new ReplyGetUserList
                    {
                        Users = safeList
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
            return new ReplyGetAuthenticationResult
            {
                AuthorizationResult = IsAuthorized(GetAuthUser())
            };
        }

        public ReplyGetEntryList GetEntryList(string message)
        {
            var request = JsonConvert.DeserializeObject<RequestGetEntryList>(message);

            try
            {
                if (IsPermitted(GetAuthUser(), 1) == AuthorizationResult.Permitted)
                {
                    var reply = new ReplyGetEntryList
                    {
                        Entries = Database.GetEntries(request.InstanceId)
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
                var auth = IsPermitted(GetAuthUser(), 2);
                switch (auth)
                {
                    case AuthorizationResult.Permitted:
                    {
                        var path = $"{ProcessPath}{request.Id}.xml";

                        if (!File.Exists(path) || request.ForceOverWrite)
                        {
                            var dir = Path.GetDirectoryName(path);
                            if (!Directory.Exists(dir))
                                Directory.CreateDirectory(dir);

                            MainWindow.PostToLog("Received Processfile...");
                            var process = XmlHelper.ReadXmlFromString(request.Text);
                            var processTemplate = new ProcessTemplate
                            {
                                Id = process.Name,
                                Description = process.Description,
                                FilePath = path
                            };
                            try
                            {
                                MainWindow.PostToLog("Registering in Database...");
                                Database.AddProcessTemplate(processTemplate);
                                MainWindow.PostToLog("Done!");
                            }
                            catch (MySqlException e)
                            {
                                if (e.Message.Contains("Duplicate entry"))
                                    reply.UploadResult = UploadResult.FailedIdExists;

                                PrintException(e);
                            }
                            catch (XmlFormatException xmle)
                            {
                                switch (xmle.State)
                                {
                                    case XmlState.Missingattribute:
                                        MainWindow.PostToLog(
                                            $"Einem der \"{xmle.TagName}\"-Tags scheint ein Attribut zu fehlen. Bitte kontaktieren Sie einen Administrator.");
                                        break;
                                    case XmlState.Missingparenttag:
                                        MainWindow.PostToLog(
                                            $"Einer der \"{xmle.TagName}\"-Tags befindet sich nicht in seinem Parenttag. Bitte kontaktieren Sie einen Administrator.");
                                        break;
                                    case XmlState.Invalid:
                                        MainWindow.PostToLog(
                                            "Die geladene XML-Datei weist einen nicht eindeutigen Fehler auf. Fehlen Klammern, Tags oder Anführungszeichen? Bitte prüfen Sie ihre Datei.");
                                        break;
                                }
                            }
                            catch (ArgumentOutOfRangeException e3)
                            {
                                MainWindow.PostToLog($"Die vom Server bezogene XML-Datei beinhaltet einen unbekannten Tag \"{e3.ActualValue}\".");
                            }

                            File.WriteAllText(path, request.Text);
                        }
                        else
                        {
                            reply.UploadResult = UploadResult.FailedFileexists;
                        }

                        break;
                    }
                    case AuthorizationResult.Authorized:
                        reply.UploadResult = UploadResult.NoPermission;
                        break;
                    default:
                        reply.UploadResult = UploadResult.InvalidLogin;
                        break;
                }
            }

            catch (XmlException e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedFileOrTypeInvalid;
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedOther;
            }

            reply.UploadResult = UploadResult.Success;
            return reply;
        }

        public ReplyPermissionChange ChangePermission(RequestPermissionChange request)
        {
            var reply = new ReplyPermissionChange();
            try
            {
                var auth = IsPermitted(GetAuthUser(), 3);
                switch (auth)
                {
                    case AuthorizationResult.Permitted:
                        Database.UpdateUserPermission(request);
                        Database.AddRole(new Role()
                        {
                            Mail = request.Email,
                            RoleId = request.Role
                        });
                        break;
                    case AuthorizationResult.NoPermission:
                        reply.UploadResult = UploadResult.NoPermission;
                        break;
                    case AuthorizationResult.InvalidLogin:
                        reply.UploadResult = UploadResult.InvalidLogin;
                        break;
                }
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Duplicate entry")) reply.UploadResult = UploadResult.FailedIdExists;

                PrintException(e);
            }
            catch (XmlException e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedFileOrTypeInvalid;
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedOther;
            }

            reply.UploadResult = UploadResult.Success;
            return reply;
        }

        public ReplyPostDocTemplate PostDocTemplate(RequestPostDocTemplate request)
        {
            var reply = new ReplyPostDocTemplate();
            try
            {
                var auth = IsPermitted(GetAuthUser(), 2);
                switch (auth)
                {
                    case AuthorizationResult.Permitted:
                    {
                        var path = $"{DocPath}{request.Id}.docx";

                        if (!File.Exists(path) || request.ForceOverWrite)
                        {
                            var dir = Path.GetDirectoryName(path);
                            if (dir != null)
                                if (!Directory.Exists(dir))
                                    Directory.CreateDirectory(dir);

                            MainWindow.PostToLog("Received Document...");
                            var docTemplate = new DocTemplate
                            {
                                Id = request.Id,
                                FilePath = path
                            };
                            MainWindow.PostToLog("Registering in Database...");
                            if (Database.GetDocTemplateById(docTemplate.Id) == null)
                            {
                                File.WriteAllBytes(path, Encoding.Default.GetBytes(request.Content));
                                Database.AddDocTemplate(docTemplate);
                                
                                reply.UploadResult = UploadResult.Success;
                            }
                            else if (Database.GetDocTemplateById(docTemplate.Id) != null && request.ForceOverWrite)
                            {
                                File.WriteAllBytes(path, Encoding.Default.GetBytes(request.Content));
                                
                                reply.UploadResult = UploadResult.Success;
                            }
                            else
                            {
                                reply.UploadResult = UploadResult.FailedFileexists;
                            }
                                
                            MainWindow.PostToLog("Done!");
                        }
                        else
                        {
                            reply.UploadResult = UploadResult.FailedFileexists;
                        }

                        break;
                    }
                    case AuthorizationResult.Authorized:
                        reply.UploadResult = UploadResult.NoPermission;
                        break;
                    default:
                        reply.UploadResult = UploadResult.InvalidLogin;
                        break;
                }
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Duplicate entry")) reply.UploadResult = UploadResult.FailedIdExists;

                PrintException(e);
            }
            catch (XmlException e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedFileOrTypeInvalid;
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedOther;
            }

            
            return reply;
        }

        public ReplyPostProcessUpdate PostProcessUpdate(RequestPostProcessUpdate request)
        {
            var reply = new ReplyPostProcessUpdate();
            try
            {
                var auth = IsPermitted(GetAuthUser(), 1);
                switch (auth)
                {
                    case AuthorizationResult.Permitted:
                        var instance = Database.GetInstanceById(request.Id);
                        var pending = Database.GetPending(GetAuthUser());
                        var entries = Database.GetEntries(request.Id);

                        if (instance.Archived || (instance.OwnerId.Equals(GetAuthUser().Email) && !request.Declined) || (!instance.OwnerId.Equals(GetAuthUser().Email) && !pending.Any(inst => inst.InstanceId == request.Id)))
                        {
                            reply.UploadResult = UploadResult.NoPermission;
                        }
                        else
                        {
                            MainWindow.PostToLog("Received a process update.");
                            if (request.Declined)
                            {
                                MainWindow.PostToLog("The process was declined.");
                                Database.DeclineProcessInstance(request.Id);
                            }
                            else
                            {
                                MainWindow.PostToLog("The process was approved.");
                                Database.ApproveProcessInstance(request.Id);
                            }

                            if (request.Locks)
                                Database.LockProcessInstance(request.Id);
                            
                            reply.UploadResult = UploadResult.Success;
                        }
                        break;
                    case AuthorizationResult.NoPermission:
                        reply.UploadResult = UploadResult.NoPermission;
                        break;
                    case AuthorizationResult.InvalidLogin:
                        reply.UploadResult = UploadResult.InvalidLogin;
                        break;
                }
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Duplicate entry")) reply.UploadResult = UploadResult.FailedIdExists;

                PrintException(e);
            }
            catch (XmlException e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedFileOrTypeInvalid;
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedOther;
            }
            return reply;
        }

        public ReplyPostEntryUpdate PostEntryUpdate(RequestPostEntryUpdate requestPost)
        {
            var reply = new ReplyPostEntryUpdate();
            try
            {
                var auth = IsPermitted(GetAuthUser(), 1);
                switch (auth)
                {
                    case AuthorizationResult.Permitted:
                        var instance = Database.GetInstanceById(requestPost.Entry.InstanceId);
                        if (instance.Archived || !instance.OwnerId.Equals(GetAuthUser().Email) || instance.CurrentStepIndex > 1)
                        {
                            reply.UploadResult = UploadResult.NoPermission;
                        }
                        else
                        {
                            MainWindow.PostToLog("Received an entry update.");
                            if (instance != null)
                                if (!instance.Locked)
                                    Database.UpdateEntry(requestPost.Entry);
                        }
                        reply.UploadResult = UploadResult.Success;
                        break;
                    case AuthorizationResult.NoPermission:
                        reply.UploadResult = UploadResult.NoPermission;
                        break;
                    case AuthorizationResult.InvalidLogin:
                        reply.UploadResult = UploadResult.InvalidLogin;
                        break;
                }

            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Duplicate entry")) reply.UploadResult = UploadResult.FailedIdExists;

                PrintException(e);
            }
            catch (XmlException e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedFileOrTypeInvalid;
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedOther;
            }
            return reply;
        }

        public ReplyPostProcessInstance PostProcessInstance(RequestPostProcessInstance request)
        {
            var reply = new ReplyPostProcessInstance();
            try
            {
                var auth = IsPermitted(GetAuthUser(), 1);
                switch (auth)
                {
                    case AuthorizationResult.Permitted:
                        MainWindow.PostToLog("Received request to create a new Process Instance.");
                        MainWindow.PostToLog("Registering in Database...");
                        reply.InstanceId = (int) Database.InsertProcessInstance(request.ProcessInstance);
                        MainWindow.PostToLog("Done");
                        break;
                    case AuthorizationResult.NoPermission:
                        reply.UploadResult = UploadResult.NoPermission;
                        break;
                    case AuthorizationResult.InvalidLogin:
                        reply.UploadResult = UploadResult.InvalidLogin;
                        break;
                }

                reply.UploadResult = UploadResult.Success;
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Duplicate entry")) reply.UploadResult = UploadResult.FailedIdExists;

                reply.UploadResult = UploadResult.FailedOther;
                PrintException(e);
            }
            catch (XmlException e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedFileOrTypeInvalid;
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedOther;
            }

            return reply;
        }

        public ReplyPostEntry PostEntry(RequestPostEntry request)
        {
            var reply = new ReplyPostEntry();
            try
            {
                var auth = IsPermitted(GetAuthUser(), 1);
                switch (auth)
                {
                    case AuthorizationResult.Permitted:
                        var instance = Database.GetInstanceById(request.Entry.InstanceId);
                        var pending = Database.GetPending(GetAuthUser());
                        if (instance.Archived ||
                            (instance.OwnerId.Equals(GetAuthUser().Email) && instance.CurrentStepIndex > 1) ||
                            (!instance.OwnerId.Equals(GetAuthUser().Email) &&
                             !pending.Any(inst => inst.InstanceId == request.Entry.InstanceId)))
                        {
                            reply.UploadResult = UploadResult.NoPermission;
                        }
                        else
                        {
                            MainWindow.PostToLog("Received request to add a new Entry.");
                            var entry = request.Entry;
                            MainWindow.PostToLog("Registering in Database...");
                            Database.AddEntry(entry);
                            MainWindow.PostToLog("Done!");
                        }

                        break;
                    case AuthorizationResult.NoPermission:
                        reply.UploadResult = UploadResult.NoPermission;
                        break;
                    case AuthorizationResult.InvalidLogin:
                        reply.UploadResult = UploadResult.InvalidLogin;
                        break;
                }
                reply.UploadResult = UploadResult.Success;
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Duplicate entry"))
                    reply.UploadResult = UploadResult.FailedIdExists;
                else if (e.Message.Contains("foreign key constraint fails"))
                    reply.UploadResult = UploadResult.MissingLink;

                PrintException(e);
            }
            catch (XmlException e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedFileOrTypeInvalid;
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedOther;
            }
            return reply;
        }

        public ReplyPostUser Register(RequestPostUser request)
        {
            var reply = new ReplyPostUser();
            try
            {
               
                MainWindow.PostToLog("Received request to register a new user.");
                MainWindow.PostToLog("Registering in Database...");
                Database.AddUser(new User(request.Email, HashHelper.Hash(request.Password)));
                MainWindow.PostToLog("Done!");
                
                reply.UploadResult = UploadResult.Success;
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Duplicate entry"))
                {
                    MainWindow.PostToLog("User already exists.");
                    reply.UploadResult = UploadResult.UserExists;
                }

                PrintException(e);
            }
            catch (Exception e)
            {
                PrintException(e);
                reply.UploadResult = UploadResult.FailedOther;
            }
            return reply;
        }

        private static AuthorizationResult IsAuthorized(User user)
        {
            try
            {
                var dbUser = Database.GetUserByMail(user.Email);

                if (dbUser == null) return AuthorizationResult.InvalidLogin;

                return HashHelper.CheckHash(user.Password, dbUser.Password)
                    ? AuthorizationResult.Authorized
                    : AuthorizationResult.InvalidLogin;
            }
            catch (Exception)
            {
                return AuthorizationResult.InvalidFormat;
            }
        }

        private static User GetAuthUser()
        {
            var authHeader = WebOperationContext.Current?.IncomingRequest.Headers["Authorization"];

            if (authHeader?.StartsWith("Basic")??false)
            {
                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var encoding = Encoding.GetEncoding("iso-8859-1");
                var usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                var separatorIndex = usernamePassword.IndexOf(':');

                var username = usernamePassword.Substring(0, separatorIndex);
                var password = usernamePassword.Substring(separatorIndex + 1);

                return new User(username, password);
            }

            return null;
        }

        private static AuthorizationResult IsPermitted(User user, int permissionLevel)
        {
            if (IsAuthorized(user) != AuthorizationResult.Authorized) return AuthorizationResult.InvalidLogin;

            var dbUser = Database.GetUserByMail(user.Email);

            if (dbUser == null) return AuthorizationResult.NoPermission;
            if (dbUser.PermissionLevel < permissionLevel) return AuthorizationResult.NoPermission;

            return AuthorizationResult.Permitted;
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