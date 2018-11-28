using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using WebServerWPF.Model;

namespace WebServerWPF
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
            cmd.CommandText = $"INSERT INTO ProcessTemplate VALUES (\"{template.Process_ID}\", \"{template.FilePath}\", \"{template.Description}\");";
            Console.WriteLine("STATEMENT:"+cmd.CommandText);
            cmd.ExecuteNonQuery();
        }
    }
}