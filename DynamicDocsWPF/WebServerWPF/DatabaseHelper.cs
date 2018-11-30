using System.Collections.Generic;
using MySql.Data.MySqlClient;
using RestService;
using RestService.Model.Database;

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

        public DatabaseHelper()
        {
            connection = new MySqlConnection(MyConnectionString);
            connection.Open();
        }

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

        public void AddProcessTemplate(ProcessTemplate template)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO ProcessTemplate VALUES (\"{template.Id}\", \"{template.FilePath}\", \"{template.Description}\");";
            cmd.ExecuteNonQuery();
        }

        //Dat wat Julius jemacht hat

        //SELECT * FROM RUNNING PROCESS
        public List<RunningProcess> GetRunningProcesses()
        {
            var runningprocesses = new List<RunningProcess>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM RUNNINGPROCESS;";

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    runningprocesses.Add(new RunningProcess
                    {
                        Id = int.Parse(reader.GetString(0)),
                        Template_ID = reader.GetString(1),
                        Owner_ID = reader.GetString(2),
                        CurrentStep = reader.GetInt32(3),
                        Declined = reader.GetBoolean(4),
                        Archived = reader.GetBoolean(5)
                    });
            }

            return runningprocesses;
        }

        public RunningProcess GetRunningProcessById(int processId)
        {
            RunningProcess runningprocess = null;
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM RUNNINGPROCESS WHERE id = {processId};";

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                    runningprocess = new RunningProcess
                    {
                        Id = int.Parse(reader.GetString(0)),
                        Template_ID = reader.GetString(1),
                        Owner_ID = reader.GetString(2),
                        CurrentStep = reader.GetInt32(3),
                        Declined = reader.GetBoolean(4),
                        Archived = reader.GetBoolean(5)
                    };
            }

            return runningprocess;
        }

        //INSERT INTO RUNNINGPROCESS
        public void AddRunningProcess(RunningProcess runningProcess)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText =
                "INSERT INTO RunningProcess (TEMPLATE_ID,OWNER_ID,CURRENTSTEP,DECLINED,ARCHIVED) VALUES (" +
                $"\"{runningProcess.Template_ID}\", " +
                $"\"{runningProcess.Owner_ID}\", " +
                $"{runningProcess.CurrentStep}, " +
                $"{runningProcess.Declined}," +
                $"{runningProcess.Archived});";
            cmd.ExecuteNonQuery();
        }

        public void DeclineRunningProcess(int id)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"UPDATE RunningProcess SET declined = true WHERE id = {id};";
            cmd.ExecuteNonQuery();
            ArchiveRunningProcess(id);
        }

        public void ApproveRunningProcess(int id)
        {
            var runningProcess = GetRunningProcessById(id);

            if (runningProcess != null)
            {
                var template = GetProcessTemplateById(runningProcess.Template_ID);
                if (template != null)
                {
                    var process = XmlHelper.ReadXMLFromPath(template.FilePath);
                    if (runningProcess.CurrentStep + 1 > process.ProcessStepCount)
                        ArchiveRunningProcess(id);
                    else
                        IncrementRunningProcess(id);
                }
            }
        }

        private void IncrementRunningProcess(int id)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"UPDATE RunningProcess SET CurrentStep = CurrentStep + 1 WHERE id = {id};";
            cmd.ExecuteNonQuery();
        }

        private void ArchiveRunningProcess(int id)
        {
            var runningProcess = GetRunningProcessById(id);
            if (runningProcess != null)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"UPDATE runningProcess SET archived = true WHERE id = {id};" +
                                  $"INSERT INTO ArchivePermission VALUES({id}, \"{runningProcess.Owner_ID}\");";
                cmd.ExecuteNonQuery();
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
            cmd.CommandText = $"INSERT INTO User VALUES (\"{user.Email}\",\"{user.Password}\",0);";

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
        public List<Entry> GetEntry(int processId)
        {
            var entries = new List<Entry>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM entry WHERE process_id = {processId};";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    entries.Add(new Entry
                    {
                        Entry_ID = int.Parse(reader.GetString(0)),
                        Process_ID = int.Parse(reader.GetString(1)),
                        FieldName = reader.GetString(2),
                        DataType = reader.GetString(3),
                        Data = reader.GetString(4),
                        PermissionLevel = int.Parse(reader.GetString(5))
                    });
            }

            return entries;
        }

        //INSERT INTO Entry
        public void AddEntry(Entry entry)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Entry (PROCESS_ID,FIELDNAME,DATATYPE,DATA,PERMISSIONLEVEL) VALUES (" +
                              $"{entry.Process_ID}," +
                              $"\"{entry.FieldName}\"," +
                              $"\"{entry.DataType}\"," +
                              $"\"{entry.Data}\"," +
                              $"{entry.PermissionLevel});";
            cmd.ExecuteNonQuery();
        }
    }
}