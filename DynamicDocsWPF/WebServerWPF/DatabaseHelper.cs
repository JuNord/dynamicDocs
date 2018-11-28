using System.Collections.Generic;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using WebServer.Model;

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

            return templates;
        }

        public void AddProcessTemplate(ProcessTemplate template)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO ProcessTemplate VALUES ({template.Process_ID}, {template.FilePath}, {template.Description});";
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
                    Owner_ID = int.Parse(reader.GetString(2)),
                    CurrentStep = int.Parse(reader.GetString(3)),
                    Declined = bool.Parse(reader.GetString(4))    
                });
            }

            return runningprocesses;
        }
        
        //INSERT INTO RUNNINGPROCESS
        public void AddRunningProcess(RunningProcess runningProcess)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO RunningProcess VALUES ({runningProcess.CurrentProcess_ID}, " +
                              $"\"{runningProcess.ProcessTemplate_ID}\", " +
                              $"{runningProcess.Owner_ID}, " +
                              $"{runningProcess.CurrentStep}, " +
                              $"{runningProcess.Declined});";
            cmd.ExecuteNonQuery();
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
                users.Add(new User()
                {
                    User_ID = int.Parse(reader.GetString(0)),
                    Email = reader.GetString(1),
                    Password_Hash = reader.GetString(2),
                    PermissionLevel = int.Parse(reader.GetString(3))
                    
                });
            }

            return users;
        }
        
        //INSERT INTO USER
        public void AddUser(User user)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO User VALUES (\"{user.Password_Hash}\","+
                              $"\"{user.Email}\"," +
                              $"\"{user.Password_Hash}\","+
                              $"{user.PermissionLevel}," +
       
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
                    User_ID = int.Parse(reader.GetString(1))
                });
                
            }

            return roles;
        }
        
        //INSERT INTO ROLES
        public void AddRoles(Roles roles)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO Roles VALUES (\"{roles.Role_ID}\"," +
                              $"{roles.User_ID});";
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

            return doctemplates;
        }
        //INSERT INTO DOCTEMPLATES
        public void AddDocTemplates(DocTemplate docTemplate)
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
                    Owner_ID = int.Parse(reader.GetString(2)),
                    CurrentStep = int.Parse(reader.GetString(3)),
                    Declined = bool.Parse(reader.GetString(4))    
                });
            }

            return archivedprocesses;
        }
        
        //INSERT INTO ARCHIVEDPROCESS
        public void AddArchivedProcess(ArchivedProcess archivedProcess)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO ArchivedProcess VALUES ({archivedProcess.CurrentProcess_ID}," +
                              $"{archivedProcess.ProcessTemplate_ID}," +
                              $"{archivedProcess.Owner_ID}," +
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
                    AuthorizedUser_ID = int.Parse(reader.GetString(1))
                });
            }

            return archivepermission;
        }
        
        //INSERT INTO ARCHIVEPERMISSION
        public void AddArchivePermission(ArchivePermission archivePermission)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO ArchivePermission VALUES({archivePermission.ArchivedProcess_ID}," +
                              $"{archivePermission.AuthorizedUser_ID});";
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

            return entries;

        }
        
        //INSERT INTO Entry
        public void AddEntry(Entry entry)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO Entry VALUES ({entry.Entry_ID}," +
                              $"{entry.Process_ID}," +
                              $"\"{entry.FieldName}\"," +
                              $"\"{entry.DataType}\"," +
                              $"\"{entry.Data}\"," +
                              $"{entry.PermissionLevel});";
            cmd.ExecuteNonQuery();
        }
    }
}