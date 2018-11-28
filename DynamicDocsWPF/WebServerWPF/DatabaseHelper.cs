using System.Collections.Generic;
using MySql.Data.MySqlClient;
using RestService;
using WebServerWPF.Model;

namespace WebServer
{
    public class DatabaseHelper
    {
        private const string MyConnectionString = "SERVER=localhost;" +              
                                 "DATABASE=processmanagement;" + 
                                 "UID=root;"                     
            //"PASSWORD=;"                  
            ;

        private MySqlConnection connection;
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

            var reader = cmd.ExecuteReader();

            while (reader.NextResult())
            {
                templates.Add(new ProcessTemplate()
                {
                    Process_ID = reader.GetString(0),
                    FilePath = reader.GetString(1),
                    Description = reader.GetString(2)
                });
            }
            reader.Close();
            return templates;
        }

        public void AddProcessTemplate(ProcessTemplate template)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO ProcessTemplate VALUES (\"{template.Process_ID}\", \"{template.FilePath}\", \"{template.Description}\");";
            cmd.ExecuteNonQuery();
        }
        
        //Dat wat Julius jemacht hat
        
        //SELECT * FROM RUNNING PROCESS
        public List<RunningProcess> GetRunningProcesses()
        {
            var runningprocesses = new List<RunningProcess>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM RUNNINGPROCESS;";

            var reader = cmd.ExecuteReader();

            while (reader.NextResult())
            {
                runningprocesses.Add(new RunningProcess()
                {
                    CurrentProcess_ID = int.Parse(reader.GetString(0)),
                    ProcessTemplate_ID = reader.GetString(1),
                    Owner_ID = reader.GetString(2),
                    CurrentStep = int.Parse(reader.GetString(3)),
                    Declined = bool.Parse(reader.GetString(4))    
                });
            }

            reader.Close();
            return runningprocesses;
        }
        
        public RunningProcess GetRunningProcessById(int processId)
        {
            RunningProcess runningprocess = null;
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM RUNNINGPROCESS WHERE currentprocess_id = {processId};";

            var reader = cmd.ExecuteReader();

            if (reader.NextResult())
            {
                runningprocess = new RunningProcess()
                {
                    CurrentProcess_ID = int.Parse(reader.GetString(0)),
                    ProcessTemplate_ID = reader.GetString(1),
                    Owner_ID = reader.GetString(2),
                    CurrentStep = int.Parse(reader.GetString(3)),
                    Declined = bool.Parse(reader.GetString(4))
                };
            }

            reader.Close();
            return runningprocess;
        }
        
        //INSERT INTO RUNNINGPROCESS
        public void AddRunningProcess(RunningProcess runningProcess)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO RunningProcess (PROCESSTEMPLATE_ID,OWNER_ID,CURRENTSTEP,DECLINED) VALUES (" +
                              $"\"{runningProcess.ProcessTemplate_ID}\", " +
                              $"\"{runningProcess.Owner_ID}\", " +
                              $"{runningProcess.CurrentStep}, " +
                              $"{runningProcess.Declined});";
            cmd.ExecuteNonQuery();
        }

        public void DeclineRunningProcess(int id)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"UPDATE RunningProcess SET declined = true WHERE currentprocess_id = {id};";
            cmd.ExecuteNonQuery();
            ArchiveRunningProcess(id);
        }
        
        public void ApproveRunningProcess(int id)
        {
            var runningProcess = GetRunningProcessById(id);

            if (runningProcess != null)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"SELECT filepath FROM ProcessTemplate WHERE process_id = {runningProcess.ProcessTemplate_ID};";
                var reader = cmd.ExecuteReader();
                if (reader.NextResult())
                {
                    var path = reader.GetString(0);
                    var process = XmlHelper.ReadXMLFromPath(path);
                    if (runningProcess.CurrentStep + 1 > process.ProcessStepCount)
                    {
                        ArchiveRunningProcess(id);
                    }
                    else
                    {
                        IncrementRunningProcess(id);
                    }
                }
                reader.Close();
            }
        }
        
        private void IncrementRunningProcess(int id)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"UPDATE RunningProcess SET CurrentStep = CurrentStep + 1;";
            cmd.ExecuteNonQuery();
        }
        
        private void ArchiveRunningProcess(int processId)
        {
            var runningProcess = GetRunningProcessById(processId);
            if (runningProcess != null)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText =
                    $"INSERT INTO ArchivedProcess (PROCESSTEMPLATE_ID,OWNER_ID,CURRENTSTEP,DECLINED) VALUES (" +
                    $"\"{runningProcess.ProcessTemplate_ID}\", " +
                    $"\"{runningProcess.Owner_ID}\", " +
                    $"{runningProcess.CurrentStep}, " +
                    $"{runningProcess.Declined});";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"DELETE FROM RunningProcess WHERE currentprocess_id = {processId}";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"SELECT currentprocess_id FROM ArchivedProcess WHERE " +
                                  $"processtemplate_id = {runningProcess.ProcessTemplate_ID}" +
                                  $"Owner_ID = \"{runningProcess.Owner_ID}\", " +
                                  $"CurrentStep = {runningProcess.CurrentStep}, " +
                                  $"Declined = {runningProcess.Declined});";
                var reader = cmd.ExecuteReader();

                if (reader.NextResult())
                {
                    var id = reader.GetInt32(0);
                    cmd.CommandText = $"INSERT INTO ArchivePermission VALUES({id}, {runningProcess.Owner_ID});";
                    cmd.ExecuteNonQuery();
                }

                reader.Close();
            }
        }
        
        //SELECT * FROM USER
        public List<User> GetUsers()
        {
            var users = new List<User>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM USER;";

            var reader = cmd.ExecuteReader();

            while (reader.NextResult())
            {
                users.Add(new User(reader.GetString(0), reader.GetString(1), int.Parse(reader.GetString(2))));
            }
            
            reader.Close();
            return users;
        }
        
        //INSERT INTO USER
        public void AddUser(User user)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO User VALUES (\"{user.Email}\",\"{user.Password_Hash}\",0);";
       
            cmd.ExecuteNonQuery();
        }
        
        //SELECT * FROM ROLES
        public List<Roles> GetRoles()
        {
            var roles=new List<Roles>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM ROLES;";

            var reader = cmd.ExecuteReader();

            while (reader.NextResult())
            {
                roles.Add(new Roles()
                {
                    Role_ID = reader.GetString(0), 
                    User_ID = reader.GetString(1)
                });
                
            }
            reader.Close();
            return roles;
        }
        
        //INSERT INTO ROLES
        public void AddRoles(Roles roles)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO Roles VALUES (\"{roles.Role_ID}\",\"{roles.User_ID}\");";
            cmd.ExecuteNonQuery();
        }
        
        //SELECT * FROM DOCTEMPLATES
        public List<DocTemplate> GetDocTemplates()
        {
            var doctemplates = new List<DocTemplate>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM DocTemplate;";

            var reader = cmd.ExecuteReader();
            while (reader.NextResult())
            {
                doctemplates.Add(new DocTemplate()
                {
                    DocTemplate_ID = reader.GetString(0),
                    FilePath = reader.GetString(1)
                });
            }
            
            reader.Close();
            return doctemplates;
        }
        //INSERT INTO DOCTEMPLATES
        public void AddDocTemplate(DocTemplate docTemplate)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO DocTemplate VALUES (\"{docTemplate.DocTemplate_ID}\"," +
                              $"\"{docTemplate.FilePath}\");";
            cmd.ExecuteNonQuery();
        }
        
        //SELECT * FROM ARCHIVEDPROCESS
        public List<ArchivedProcess> GetArchivedProcesses()
        {
            var archivedprocesses=new List<ArchivedProcess>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM ArchivedProcess;";

            var reader = cmd.ExecuteReader();
            while (reader.NextResult())
            {
                archivedprocesses.Add(new ArchivedProcess()
                {
                    CurrentProcess_ID = int.Parse(reader.GetString(0)),
                    ProcessTemplate_ID = reader.GetString(1),
                    Owner_ID = reader.GetString(2),
                    CurrentStep = int.Parse(reader.GetString(3)),
                    Declined = bool.Parse(reader.GetString(4))    
                });
            }
            reader.Close();
            return archivedprocesses;
        }
        
        //INSERT INTO ARCHIVEDPROCESS
        public void AddArchivedProcess(ArchivedProcess archivedProcess)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO ArchivedProcess VALUES ({archivedProcess.CurrentProcess_ID}," +
                              $"{archivedProcess.ProcessTemplate_ID}," +
                              $"\"{archivedProcess.Owner_ID}\"," +
                              $"{archivedProcess.CurrentStep}," +
                              $"{archivedProcess.Declined});";
            cmd.ExecuteNonQuery();
        }
        
        //SELECT * FROM ARCHIVEPERMISSION
        public List<ArchivePermission> GetArchivePermisson()
        {
            var archivepermission=new List<ArchivePermission>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM archivepermission;";
            var reader = cmd.ExecuteReader();

            while (reader.NextResult())
            {
                archivepermission.Add(new ArchivePermission()
                {
                    ArchivedProcess_ID = int.Parse(reader.GetString(0)),
                    AuthorizedUser_ID = reader.GetString(1)
                });
            }
            reader.Close();
            return archivepermission;
        }
        
        //INSERT INTO ARCHIVEPERMISSION
        public void AddArchivePermission(ArchivePermission archivePermission)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO ArchivePermission VALUES({archivePermission.ArchivedProcess_ID}," +
                              $"\"{archivePermission.AuthorizedUser_ID}\");";
            cmd.ExecuteNonQuery();
        }
        
        //SELECT * FROM ENTRY
        public List<Entry> GetEntry()
        {
            var entries=new List<Entry>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM entry;";
            var reader = cmd.ExecuteReader();

            while (reader.NextResult())
            {
                entries.Add(new Entry()
                {
                    Entry_ID = int.Parse(reader.GetString(0)),
                    Process_ID = int.Parse(reader.GetString(1)),
                    FieldName = reader.GetString(2),
                    DataType = reader.GetString(3),
                    Data = reader.GetString(4),
                    PermissionLevel = int.Parse(reader.GetString(5))
                });
            }
            reader.Close();
            return entries;

        }
        
        //INSERT INTO Entry
        public void AddEntry(Entry entry)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO Entry (PROCESS_ID,FIELDNAME,DATATYPE,DATA,PERMISSIONLEVEL) VALUES (" +
                              $"{entry.Process_ID}," +
                              $"\"{entry.FieldName}\"," +
                              $"\"{entry.DataType}\"," +
                              $"\"{entry.Data}\"," +
                              $"{entry.PermissionLevel});";
            cmd.ExecuteNonQuery();
        }
        
        
    }
}