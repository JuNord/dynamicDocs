using MySql.Data.MySqlClient;

namespace DynamicDocsWPF.HelperClasses
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

        
        
        
        
        
    }
}