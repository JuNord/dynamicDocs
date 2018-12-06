using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using RestService;
using RestService.Model.Database;
using RestService.Model.Process;
using WebServerWPF.RestDTOs;

namespace WebServerWPF
{
    public class DatabaseHelper
    {
        private const string MyConnectionString = "SERVER=localhost;" +
                                                  "DATABASE=processmanagement;" +
                                                  "UID=root;"
            //"PASSWORD=;"                  
        ;

        private readonly MySqlConnection connection;

        /// <summary>
        /// Provides project specific methods to access the database
        /// </summary>
        public DatabaseHelper()
        {
            connection = new MySqlConnection(MyConnectionString);
            connection.Open();
        }

        /// <summary>
        /// Returns a list of process template objects including a template ID, description and local path to the template file
        /// </summary>
        /// <returns></returns>
        public List<ProcessTemplate> GetProcessTemplates()
        {
            var templates = new List<ProcessTemplate>();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM PROCESSTEMPLATE;";

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    templates.Add(new ProcessTemplate
                    {
                        Id = reader.GetString(0),
                        FilePath = reader.GetString(1),
                        Description = reader.GetString(2)
                    });
            }

            return templates;
        }

        /// <summary>
        /// Returns the process object according to the id, including a description text and a path
        /// </summary>
        /// <param name="id">The id (e.g. vacation) of the template</param>
        /// <returns></returns>
        public ProcessTemplate GetProcessTemplateById(string id)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText =
                $"SELECT * FROM ProcessTemplate WHERE id = \"{id}\";";
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                    return new ProcessTemplate
                    {
                        Id = reader.GetString(0),
                        FilePath = reader.GetString(1),
                        Description = reader.GetString(2)
                    };
            }

            return null;
        }

        /// <summary>
        /// Allows to register a new processtemplate in the database to track its path and return it on request
        /// </summary>
        /// <param name="template"></param>
        public void AddProcessTemplate(ProcessTemplate template)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO ProcessTemplate VALUES (\"{template.Id}\", \"{template.FilePath}\", \"{template.Description}\");";
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<ProcessInstance> GetProcessInstances(User user)
        {
            var processInstances = new List<ProcessInstance>();
            var cmd = connection.CreateCommand();
            cmd.CommandText =$"SELECT * FROM processinstance WHERE owner_id = \"{user.Email}\";";

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    processInstances.Add(new ProcessInstance
                    {
                        Id = int.Parse(reader.GetString(0)),
                        TemplateId = reader.GetString(1),
                        OwnerId = reader.GetString(2),
                        CurrentStep = reader.GetInt32(3),
                        Declined = reader.GetBoolean(4),
                        Archived = reader.GetBoolean(5),
                        Locked = reader.GetBoolean(6),
                        Created = reader.GetMySqlDateTime(7).GetDateTime().ToShortDateString(),
                        Changed = reader.GetMySqlDateTime(8).GetDateTime().ToShortDateString(),
                        Subject = reader.GetString(9)
                    });
            }

            return processInstances;
        }
      
        public ProcessInstance GetProcessInstanceById(int instanceId)
        {
            ProcessInstance processinstance = null;
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM processinstance WHERE id = {instanceId};";

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                    processinstance = new ProcessInstance
                    {
                        Id = int.Parse(reader.GetString(0)),
                        TemplateId = reader.GetString(1),
                        OwnerId = reader.GetString(2),
                        CurrentStep = reader.GetInt32(3),
                        Declined = reader.GetBoolean(4),
                        Archived = reader.GetBoolean(5),
                        Locked = reader.GetBoolean(6),
                        Created = reader.GetMySqlDateTime(7).GetDateTime().ToShortDateString(),
                        Changed = reader.GetMySqlDateTime(8).GetDateTime().ToShortDateString(),
                        Subject = reader.GetString(9)
                    };
            }

            return processinstance;
        }

        //INSERT INTO processinstance
        public long AddProcessInstance(ProcessInstance processInstance)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText =
                "INSERT INTO processinstance (TEMPLATE_ID,OWNER_ID,CURRENTSTEP,DECLINED,ARCHIVED,LOCKED,CREATED,CHANGED,SUBJECT) VALUES (" +
                $"\"{processInstance.TemplateId}\", " +
                $"\"{processInstance.OwnerId}\", " +
                $"{processInstance.CurrentStep}, " +
                $"{processInstance.Declined}," +
                $"{processInstance.Archived}," +
                $"{processInstance.Locked}," +
                $"\"{DateTime.Parse(processInstance.Created):yyyy-MM-dd}\"," +
                $"\"{DateTime.Parse(processInstance.Created):yyyy-MM-dd}\"," +
                $"\"{processInstance.Subject}\");";
            cmd.ExecuteNonQuery();

            int instanceId = (int) cmd.LastInsertedId;
            
            var processTemplate = GetProcessTemplateById(processInstance.TemplateId);
            var processObject = XmlHelper.ReadXmlFromString(File.ReadAllText(processTemplate.FilePath));
            
            IncrementProcessInstance(instanceId);
            PushToNextUser(instanceId, processObject, GetProcessInstanceById(instanceId));
            
            return instanceId;  
        }

        public void LockProcessInstance(int id)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"UPDATE processinstance SET locked = true WHERE id = {id};";
            cmd.ExecuteNonQuery();
        }

        public void DeclineProcessInstance(int id)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"UPDATE processinstance SET declined = true WHERE id = {id};";
            cmd.ExecuteNonQuery();
            ArchiveProcessinstance(id);
        }   

        public void ApproveProcessInstance(int id)
        {
            var processInstance = GetProcessInstanceById(id);

            if (processInstance != null)
            {
                var template = GetProcessTemplateById(processInstance.TemplateId);
                if (template != null)
                {
                    var processObject = XmlHelper.ReadXmlFromPath(template.FilePath);
                    
                    IncrementProcessInstance(id);
                    if (processInstance.CurrentStep + 1 >= processObject.ProcessStepCount)
                    {
                        ArchiveProcessinstance(id);
                    }
                    else
                    {
                        PushToNextUser(id, processObject, processInstance);   
                    }
                }
            }
        }

        private void PushToNextUser(int id, ProcessObject processObject, ProcessInstance processinstance)
        {
            User nextResponsibleUser = null;
            var entries = GetEntries(id);
            var processStep = processObject.GetStepAtIndex(processinstance.CurrentStep);
            RemoveAllPendings(id);
            if (processStep != null)
            {
                if (IsPlaceHolder(processStep.Target))
                {
                    var mail = GetStringValueFromEntryList(entries,
                        processStep.Target.Substring(1, processStep.Target.Length - 2));
                    nextResponsibleUser = GetUserByMail(mail);
                }
                else
                {
                    nextResponsibleUser =
                        GetUserByRole(processStep.Target.ToLower()) ?? GetUserByMail(processStep.Target);
                }
            }
            
            if (nextResponsibleUser != null)
            {   
                SetNewResponsibleUser(id, nextResponsibleUser);
            }
            
            //TODO: HANDLE MISSING NEXT USER
        }

        private string GetStringValueFromEntryList(List<Entry> entries, string fieldName)
        {
            return entries.First(entry => entry.FieldName.Equals(fieldName)).Data;
        }

        private bool IsPlaceHolder(string value)
        {
            try
            {
                var linkRegex = new Regex("^\\[(.*?)\\]$");
                return linkRegex.IsMatch(value);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<PendingInstance> GetResponsibilities(User user)
        {
            var responsibilities = new List<PendingInstance>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM pendinginstance WHERE responsible_User_Id = \"{user.Email}\";";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    responsibilities.Add(new PendingInstance()
                    {
                        InstanceId = reader.GetInt32(0),
                        ResponsibleUserId = reader.GetString(1)
                    });
                }
            }

            return responsibilities;
        }
        
        private void RemoveAllPendings(int id)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM pendinginstance WHERE instance_id = {id};";
            cmd.ExecuteNonQuery();
        }
        
        private void SetNewResponsibleUser(int id, User user)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO pendinginstance VALUES({id},\"{user.Email}\");";
            cmd.ExecuteNonQuery();
        }
        
        private void IncrementProcessInstance(int id)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"UPDATE processinstance SET CurrentStep = CurrentStep + 1 WHERE id = {id};";
            cmd.ExecuteNonQuery();         
        }

        private void ArchiveProcessinstance(int id)
        {
            var processinstance = GetProcessInstanceById(id);
            if (processinstance != null)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"UPDATE processinstance SET archived = true WHERE id = {id};" +
                                  $"INSERT INTO ArchivePermission VALUES({id}, \"{processinstance.OwnerId}\");";
                cmd.ExecuteNonQuery();
                RemoveAllPendings(id);
            }
        }

        //SELECT * FROM USER
        public List<User> GetUsers()
        {
            var users = new List<User>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM USER;";

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    users.Add(new User(reader.GetString(0), reader.GetString(1), int.Parse(reader.GetString(2))));
            }

            return users;
        }

        public User GetUserByRole(string role)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM user WHERE role = \"{role}\";";

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new User()
                    {
                        Email = reader.GetString(0),
                        Password = reader.GetString(1),
                        PermissionLevel = reader.GetInt32(2)
                    };
                }
                return null;
            }
        }
        
        public User GetUserByMail(string email)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM user WHERE email = \"{email}\";";

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new User()
                    {
                        Email = reader.GetString(0),
                        Password = reader.GetString(1),
                        PermissionLevel = reader.GetInt32(2)
                    };
                }
                return null;
            }
        }

        //INSERT INTO USER
        public void AddUser(User user)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO User VALUES (\"{user.Email}\",\"{user.Password}\",0 , \"\");";

            cmd.ExecuteNonQuery();
        }
        
        public void UpdateUserPermission(RequestPermissionChange request)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"UPDATE User SET permissionlevel = {request.PermissionLevel} WHERE email = \"{request.Email}\";";

            cmd.ExecuteNonQuery();
        }

        //SELECT * FROM ROLES
        public List<Roles> GetRoles()
        {
            var roles = new List<Roles>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM ROLES;";

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    roles.Add(new Roles
                    {
                        Id = reader.GetString(0),
                        User_ID = reader.GetString(1)
                    });
            }

            return roles;
        }

        //INSERT INTO ROLES
        public void AddRoles(Roles roles)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO Roles VALUES (\"{roles.Id}\",\"{roles.User_ID}\");";
            cmd.ExecuteNonQuery();
        }
        
        //SELECT * FROM DOCTEMPLATES
        public List<DocTemplate> GetDocTemplates()
        {
            var doctemplates = new List<DocTemplate>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM DocTemplate;";

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    doctemplates.Add(new DocTemplate
                    {
                        Id = reader.GetString(0),
                        FilePath = reader.GetString(1)
                    });
            }

            return doctemplates;
        }
        
        public DocTemplate GetDocTemplateById(string id)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText =
                $"SELECT * FROM DocTemplate WHERE id = \"{id}\";";
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                    return new DocTemplate()
                    {
                        Id = reader.GetString(0),
                        FilePath = reader.GetString(1)
                    };
            }

            return null;
        }


        //INSERT INTO DOCTEMPLATES
        public void AddDocTemplate(DocTemplate docTemplate)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO DocTemplate VALUES (\"{docTemplate.Id}\"," +
                              $"\"{docTemplate.FilePath}\");";
            cmd.ExecuteNonQuery();
        }

        //SELECT * FROM ARCHIVEPERMISSION
        public List<ArchivePermission> GetArchivePermisson()
        {
            var archivepermission = new List<ArchivePermission>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM archivepermission;";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    archivepermission.Add(new ArchivePermission
                    {
                        ArchivedProcess_ID = int.Parse(reader.GetString(0)),
                        AuthorizedUser_ID = reader.GetString(1)
                    });
            }

            return archivepermission;
        }

        //INSERT INTO ARCHIVEPERMISSION
        public void AddArchivePermission(ArchivePermission archivePermission)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO ArchivePermission VALUES(" +
                              $"{archivePermission.ArchivedProcess_ID}," +
                              $"\"{archivePermission.AuthorizedUser_ID}\");";
            cmd.ExecuteNonQuery();
        }

        //SELECT * FROM ENTRY
        public List<Entry> GetEntries(int instanceId)
        {
            var entries = new List<Entry>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM entry WHERE process_id = {instanceId};";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    entries.Add(new Entry
                    {
                        EntryId = int.Parse(reader.GetString(0)),
                        InstanceId = int.Parse(reader.GetString(1)),
                        FieldName = reader.GetString(2),
                        Data = reader.GetString(3)
                    });
            }

            return entries;
        }

        //INSERT INTO Entry
        public void AddEntry(Entry entry)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Entry (PROCESS_ID,FIELDNAME,DATA) VALUES (" +
                              $"{entry.InstanceId}," +
                              $"\"{entry.FieldName}\"," +
                              $"\"{entry.Data}\");";
            cmd.ExecuteNonQuery();
        }
        
        public void UpdateEntry(Entry entry)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Entry SET " +
                              $"data =\"{entry.Data}\" " +
                              $"WHERE id = {entry.EntryId};";
            cmd.ExecuteNonQuery();
        }
    }
}