using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using RestService;
using RestService.Model.Database;
using RestService.Model.Process;
using RestService.RestDTOs;

namespace WebServerWPF
{
    public class DatabaseHelper
    {
        private const string MyConnectionString = "SERVER=localhost;" +
                                                  "DATABASE=processmanagement;" +
                                                  "UID=root;"
            //"PASSWORD=;"                  
            ;

        private readonly MySqlConnection _connection;

        /// <summary>
        ///  Provides project specific methods to access the database
        /// </summary>
        public DatabaseHelper()
        {
            _connection = new MySqlConnection(MyConnectionString);
            _connection.Open();
        }

        /// <summary>
        /// Executes a given text on the database and returns the command for further result processing.
        /// </summary>
        /// <param name="cmdText">The statement to be executed.</param>
        /// <returns>The command that has been executed.</returns>
        private MySqlCommand Execute(string cmdText)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = cmdText;
            cmd.ExecuteNonQuery();
            return cmd;
        }

        /// <summary>
        /// Executes a given text on the database and returns a reader to process the selection rows.
        /// </summary>
        /// <param name="cmdText">The statement to be executed.</param>
        /// <returns>A reader of the executed command.</returns>
        private MySqlDataReader Read(string cmdText)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = cmdText;
            return cmd.ExecuteReader();
        }

        /// <summary>
        /// Returns a list of process template objects including a template ID, description and local path to the template file
        /// </summary>
        /// <returns></returns>
        public List<ProcessTemplate> GetProcessTemplates()
        {
            var templates = new List<ProcessTemplate>();

            using (var reader = Read("SELECT * FROM PROCESSTEMPLATE;"))
                while (reader.Read())
                    templates.Add(new ProcessTemplate
                    {
                        Id = reader.GetString(0),
                        FilePath = reader.GetString(1),
                        Description = reader.GetString(2)
                    });

            return templates;
        }
        
        public void RemoveProcessTemplate(string id)
        {
            Execute($"DELETE FROM ProcessTemplate WHERE id = \"{id}\";");
        }

        /// <summary>
        ///  Returns the process object according to the id, including a description text and a path
        /// </summary>
        /// <param name="id">The id (e.g. vacation) of the template</param>
        /// <returns></returns>
        public ProcessTemplate GetProcessTemplateById(string id)
        {
            using (var reader = Read($"SELECT * FROM ProcessTemplate WHERE id = \"{id}\";"))
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
            Execute(
                $"INSERT INTO ProcessTemplate VALUES (\"{template.Id}\", \"{template.FilePath}\", \"{template.Description}\");");
        }

        /// <summary>
        /// Returns a list of all ProcessInstances belonging to a user
        /// </summary>
        /// <param name="user">The user the instances should belong to.</param>
        /// <returns>A list of processInstances</returns>
        public List<ProcessInstance> GetProcessInstances(User user)
        {
            var processInstances = new List<ProcessInstance>();

            using (var reader = Read($"SELECT * FROM processinstance WHERE owner_id = \"{user.Email}\";"))
            {
                while (reader.Read())
                    processInstances.Add(new ProcessInstance
                    {
                        Id = int.Parse(reader.GetString(0)),
                        TemplateId = reader.GetString(1),
                        OwnerId = reader.GetString(2),
                        CurrentStepIndex = reader.GetInt32(3),
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

        /// <summary>
        /// Returns a specific processInstance according to its database Id.
        /// </summary>
        /// <param name="instanceId">The instanceId to search for.</param>
        /// <returns>The processInstance belonging to given InstanceId</returns>
        public ProcessInstance GetProcessInstanceById(int instanceId)
        {
            ProcessInstance processinstance = null;

            using (var reader = Read($"SELECT * FROM processinstance WHERE id = {instanceId};"))
            {
                if (reader.Read())
                    processinstance = new ProcessInstance
                    {
                        Id = int.Parse(reader.GetString(0)),
                        TemplateId = reader.GetString(1),
                        OwnerId = reader.GetString(2),
                        CurrentStepIndex = reader.GetInt32(3),
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

        /// <summary>
        /// Adds a processInstance to the database.
        /// </summary>
        /// <param name="processInstance">The ProcessInstance to be inserted.</param>
        /// <returns></returns>
        public long InsertProcessInstance(ProcessInstance processInstance)
        {
            var instanceId = (int) Execute(
                "INSERT INTO processinstance (TEMPLATE_ID,OWNER_ID,CURRENTSTEP,DECLINED,ARCHIVED,LOCKED,CREATED,CHANGED,SUBJECT) VALUES (" +
                $"\"{processInstance.TemplateId}\", " +
                $"\"{processInstance.OwnerId}\", " +
                $"{processInstance.CurrentStepIndex}, " +
                $"{processInstance.Declined}," +
                $"{processInstance.Archived}," +
                $"{processInstance.Locked}," +
                $"\"{DateTime.Parse(processInstance.Created):yyyy-MM-dd}\"," +
                $"\"{DateTime.Parse(processInstance.Created):yyyy-MM-dd}\"," +
                $"\"{processInstance.Subject}\");").LastInsertedId;

            var processTemplate = GetProcessTemplateById(processInstance.TemplateId);
            var processObject = XmlHelper.ReadXmlFromString(File.ReadAllText(processTemplate.FilePath));

            IncrementProcessInstance(instanceId);
            PushToNextUser(instanceId, processObject, GetProcessInstanceById(instanceId));

            return instanceId;
        }

        public void LockProcessInstance(int id)
        {
            Execute($"UPDATE processinstance SET locked = true WHERE id = {id};");
        }

        public void DeclineProcessInstance(int id)
        {
            Execute($"UPDATE processinstance SET declined = true WHERE id = {id};");
            ArchiveProcessInstance(id);
        }

        public void ApproveProcessInstance(int id)
        {
            var instance = GetProcessInstanceById(id);
            var template = GetProcessTemplateById(instance?.TemplateId);

            if (template == null) return;
            var processObject = XmlHelper.ReadXmlFromPath(template.FilePath);

            IncrementProcessInstance(id);
            instance = GetProcessInstanceById(id);

            if (instance.CurrentStepIndex >= processObject.StepCount)
                ArchiveProcessInstance(id);
            else
                PushToNextUser(id, processObject, instance);
        }

        private void PushToNextUser(int id, ProcessObject processObject, ProcessInstance processinstance)
        {
            var regex = new Regex("^([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5})$");
            var processStep = processObject.GetStepAtIndex(processinstance.CurrentStepIndex);

            if (processStep == null) return;

            /*if (IsPlaceHolder(processStep.Target))
            {
                var fieldName = GetStringValueFromEntryList(entries,
                    processStep.Target.Substring(1, processStep.Target.Length - 2));

                var value = GetStringValueFromEntryList(entries, fieldName);
                    
                SetNewResponsibleUser(id, GetUserByMail(value).Email);
            }
            else*/
            if (regex.IsMatch(processStep.Target))
                SetNewResponsibleUser(id, processStep.Target);
            else
                SetNewResponsibleUser(id, "", processStep.Target);
        }

        private static string GetStringValueFromEntryList(IEnumerable<Entry> entries, string fieldName)
        {
            return entries.FirstOrDefault(entry => entry.FieldName.Equals(fieldName))?.Data;
        }

        private static bool IsPlaceHolder(string value)
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
            var roles = GetRolesByMail(user.Email);

            MySqlCommand cmd;

            foreach (var role in roles)
                using (var reader = Read($"SELECT * FROM pendinginstance WHERE role = \"{role.Role}\";"))
                {
                    while (reader.Read())
                        responsibilities.Add(new PendingInstance
                        {
                            InstanceId = reader.GetInt32(0),
                            ResponsibleUserId = reader.GetString(1)
                        });
                }

            using (var reader = Read($"SELECT * FROM pendinginstance WHERE mail = \"{user.Email}\";"))
            {
                while (reader.Read())
                    responsibilities.Add(new PendingInstance
                    {
                        InstanceId = reader.GetInt32(0),
                        ResponsibleUserId = reader.GetString(1)
                    });
            }

            return responsibilities;
        }

        private void RemoveAllPending(int id)
        {
            Execute($"DELETE FROM pendinginstance WHERE instance_id = {id};");
        }

        private void SetNewResponsibleUser(int id, string mail, string role = "")
        {
            RemoveAllPending(id);
            Execute($"INSERT INTO pendinginstance VALUES({id},\"{mail}\",\"{role}\");");
        }

        private void IncrementProcessInstance(int id)
        {
            Execute($"UPDATE processinstance SET CurrentStep = CurrentStep + 1 WHERE id = {id};");
        }

        private void ArchiveProcessInstance(int id)
        {
            var processInstance = GetProcessInstanceById(id);
            if (processInstance == null) return;

            RemoveAllPending(id);
            Execute($"UPDATE processinstance SET archived = true WHERE id = {id};");
            Execute($"INSERT INTO ArchivePermission VALUES({id}, \"{processInstance.OwnerId}\");");
        }

        //SELECT * FROM USER
        public IEnumerable<User> GetUsers()
        {
            var users = new List<User>();

            using (var reader = Read("SELECT * FROM USER;"))
            {
                while (reader.Read())
                    users.Add(new User(reader.GetString(0), reader.GetString(1), reader.GetInt32(2)));
            }

            return users;
        }

        public User GetUserByRole(string role)
        {
            using (var reader = Read($"SELECT * FROM user WHERE role = \"{role}\";"))
            {
                if (reader.Read())
                    return new User
                    {
                        Email = reader.GetString(0),
                        Password = reader.GetString(1),
                        PermissionLevel = reader.GetInt32(2)
                    };
            }

            return null;
        }

        public User GetUserByMail(string email)
        {
            using (var reader = Read($"SELECT * FROM user WHERE email = \"{email}\";"))
            {
                if (!reader.Read()) return null;

                return new User
                {
                    Email = reader.GetString(0),
                    Password = reader.GetString(1),
                    PermissionLevel = reader.GetInt32(2)
                };
            }
        }

        //INSERT INTO USER
        public void AddUser(User user)
        {
            Execute($"INSERT INTO User VALUES (\"{user.Email}\",\"{user.Password}\",0);");
        }

        public void UpdateUserPermission(RequestPermissionChange request)
        {
            Execute($"UPDATE User SET permissionlevel = {request.PermissionLevel} WHERE email = \"{request.Email}\";");
        }

        public List<Roles> GetRolesByMail(string mail)
        {
            var roles = new List<Roles>();

            using (var reader = Read($"SELECT * FROM roles WHERE mail = \"{mail}\";"))
            {
                while (reader.Read())
                    roles.Add(new Roles
                    {
                        Role = reader.GetString(0),
                        Mail = reader.GetString(1)
                    });
            }

            return roles;
        }

        //SELECT * FROM ROLES
        public List<Roles> GetRoles()
        {
            var roles = new List<Roles>();

            using (var reader = Read("SELECT * FROM ROLES;"))
            {
                while (reader.Read())
                    roles.Add(new Roles
                    {
                        Role = reader.GetString(0),
                        Mail = reader.GetString(1)
                    });
            }

            return roles;
        }

        //INSERT INTO ROLES
        public void AddRoles(Roles roles)
        {
            Execute($"INSERT INTO Roles VALUES (\"{roles.Role}\",\"{roles.Mail}\");");
        }
        //SELECT * FROM DOCTEMPLATES
        public List<DocTemplate> GetDocTemplates()
        {
            var doctemplates = new List<DocTemplate>();

            using (var reader = Read("SELECT * FROM DocTemplate;"))
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
            using (var reader = Read($"SELECT * FROM DocTemplate WHERE id = \"{id}\";"))
            {
                if (reader.Read())
                    return new DocTemplate
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
            Execute($"INSERT INTO DocTemplate VALUES (\"{docTemplate.Id}\",\"{docTemplate.FilePath}\");");
        }
        
        public void RemoveDocTemplate(string id)
        {
            Execute($"DELETE FROM DocTemplate WHERE id = \"{id}\";");
        }


        //SELECT * FROM ARCHIVEPERMISSION
        public List<ArchivePermission> GetArchivePermission()
        {
            var archivePermission = new List<ArchivePermission>();

            using (var reader = Read("SELECT * FROM archivepermission;"))
            {
                while (reader.Read())
                    archivePermission.Add(new ArchivePermission
                    {
                        ArchivedProcessId = int.Parse(reader.GetString(0)),
                        AuthorizedUserId = reader.GetString(1)
                    });
            }

            return archivePermission;
        }

        //INSERT INTO ARCHIVEPERMISSION
        public void AddArchivePermission(ArchivePermission archivePermission)
        {
            Execute("INSERT INTO ArchivePermission VALUES(" +
                    $"{archivePermission.ArchivedProcessId}," +
                    $"\"{archivePermission.AuthorizedUserId}\"" +
                    ");");
        }

        //SELECT * FROM ENTRY
        public List<Entry> GetEntries(int instanceId)
        {
            var entries = new List<Entry>();

            using (var reader = Read($"SELECT * FROM entry WHERE process_id = {instanceId};"))
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
            Execute(
                "INSERT INTO Entry (PROCESS_ID,FIELDNAME,DATA) VALUES (" +
                $"{entry.InstanceId},\"{entry.FieldName}\",\"{entry.Data}\"" +
                ");");
        }

        public void UpdateEntry(Entry entry)
        {
            Execute($"UPDATE Entry SET data =\"{entry.Data}\" WHERE id = {entry.EntryId};");
        }
    }
}